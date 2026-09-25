-- =============================================================================
-- ConectAR S.R.L. - Trabajo de Diploma - Villaverde, Agustin (575_AV)
-- 06 - Stored procedures de negocio, fase 1: catalogos, articulo y bobina
--
-- Misma convencion que 02: DROP + CREATE (idempotente y portable a versiones
-- anteriores a SQL Server 2016 SP1), SET NOCOUNT ON y parametros tipados.
-- Las firmas estan atadas a los mapeadores Acceso_DAL/MP_*_575_AV.cs: si se
-- cambia un nombre de parametro aca, hay que cambiarlo tambien alli.
--
-- Orden de ejecucion: 01 -> 02 -> 03 -> 04 -> 05 -> 06
-- =============================================================================

USE [ConectAR_DB];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================================================
-- Digito verificador de las tablas de negocio
-- =============================================================================

-- Persiste el dvh de una fila cualquiera de negocio. La BLL lo llama despues
-- del alta o la modificacion, una vez que conoce el id generado.
-- El nombre de tabla se valida contra sys.tables y la columna clave se deduce
-- de la PK, para no dejar una via de SQL Injection abierta.
IF OBJECT_ID('dbo.SP_ActualizarDVHNegocio', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ActualizarDVHNegocio];
GO
CREATE PROCEDURE [dbo].[SP_ActualizarDVHNegocio]
(
    @tabla NVARCHAR(50),
    @id    INT,
    @dvh   NVARCHAR(65)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = @tabla)
    BEGIN
        RAISERROR('Tabla inexistente.', 16, 1);
        RETURN;
    END

    DECLARE @clave NVARCHAR(128) = (
        SELECT TOP 1 c.name
        FROM sys.indexes i
             JOIN sys.index_columns ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id
             JOIN sys.columns c        ON c.object_id  = i.object_id AND c.column_id = ic.column_id
        WHERE i.object_id = OBJECT_ID(@tabla) AND i.is_primary_key = 1
    );

    IF @clave IS NULL
    BEGIN
        RAISERROR('La tabla no tiene clave primaria simple.', 16, 1);
        RETURN;
    END

    DECLARE @Query NVARCHAR(MAX) =
        N'UPDATE ' + QUOTENAME(@tabla) + N' SET dvh = @dvh WHERE ' + QUOTENAME(@clave) + N' = @id';
    EXEC sp_executesql @Query, N'@dvh NVARCHAR(65), @id INT', @dvh, @id;
END
GO

-- =============================================================================
-- Catalogos
-- =============================================================================

IF OBJECT_ID('dbo.SP_ExtCategoria', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtCategoria];
GO
CREATE PROCEDURE [dbo].[SP_ExtCategoria]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_categoria, nombre, activo, dvh
    FROM Categoria
    ORDER BY id_categoria;
END
GO

IF OBJECT_ID('dbo.SP_ExtMarca', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtMarca];
GO
CREATE PROCEDURE [dbo].[SP_ExtMarca]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_marca, nombre, activo, dvh
    FROM Marca
    ORDER BY id_marca;
END
GO

IF OBJECT_ID('dbo.SP_ExtUnidadMedida', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtUnidadMedida];
GO
CREATE PROCEDURE [dbo].[SP_ExtUnidadMedida]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_unidad_medida, nombre, abreviatura, fraccionable, dvh
    FROM UnidadMedida
    ORDER BY id_unidad_medida;
END
GO

IF OBJECT_ID('dbo.SP_ExtMedioPago', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtMedioPago];
GO
CREATE PROCEDURE [dbo].[SP_ExtMedioPago]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_medio_pago, nombre, requiere_autorizacion, activo, dvh
    FROM MedioPago
    ORDER BY id_medio_pago;
END
GO

-- Alta de marca: la usa el ABM de articulo cuando la marca no existe todavia.
IF OBJECT_ID('dbo.SP_CrearMarca', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_CrearMarca];
GO
CREATE PROCEDURE [dbo].[SP_CrearMarca]
    @nombre VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Marca (nombre, activo, dvh) VALUES (@nombre, 1, NULL);
    SELECT SCOPE_IDENTITY();
END
GO

-- =============================================================================
-- Articulo
-- =============================================================================

-- Listado completo. El DVV se calcula sobre la secuencia ordenada por PK, asi
-- que el ORDER BY tiene que coincidir con el OrderBy de la BLL.
IF OBJECT_ID('dbo.SP_ExtArticulo', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtArticulo];
GO
CREATE PROCEDURE [dbo].[SP_ExtArticulo]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_articulo, codigo, descripcion, id_categoria, id_marca, id_unidad_medida,
           precio_unitario, cantidad_disponible, punto_reposicion, deposito_ubicacion,
           estado_articulo, activo, dvh
    FROM Articulo
    ORDER BY id_articulo;
END
GO

-- RFN1.1 - Consultar articulos por descripcion, categoria, marca o codigo.
-- Los criterios no informados llegan NULL desde la DAL y no filtran.
IF OBJECT_ID('dbo.SP_BuscarArticulo', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_BuscarArticulo];
GO
CREATE PROCEDURE [dbo].[SP_BuscarArticulo]
(
    @descripcion  VARCHAR(200) = NULL,
    @id_categoria INT          = NULL,
    @id_marca     INT          = NULL,
    @codigo       VARCHAR(25)  = NULL,
    @solo_activos BIT          = 1
)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_articulo, codigo, descripcion, id_categoria, id_marca, id_unidad_medida,
           precio_unitario, cantidad_disponible, punto_reposicion, deposito_ubicacion,
           estado_articulo, activo, dvh
    FROM Articulo
    WHERE (@descripcion  IS NULL OR descripcion LIKE '%' + @descripcion + '%')
      AND (@id_categoria IS NULL OR id_categoria = @id_categoria)
      AND (@id_marca     IS NULL OR id_marca     = @id_marca)
      AND (@codigo       IS NULL OR codigo       = @codigo)
      AND (@solo_activos = 0     OR activo       = 1)
    ORDER BY id_articulo;
END
GO

-- Devuelve el id_articulo generado: la BLL lo necesita para calcular el dvh y
-- persistirlo enseguida con SP_ActualizarDVHNegocio.
IF OBJECT_ID('dbo.SP_CrearArticulo', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_CrearArticulo];
GO
CREATE PROCEDURE [dbo].[SP_CrearArticulo]
(
    @codigo              VARCHAR(25),
    @descripcion         VARCHAR(200),
    @id_categoria        INT,
    @id_marca            INT,
    @id_unidad_medida    INT,
    @precio_unitario     DECIMAL(12,2),
    @cantidad_disponible DECIMAL(12,2),
    @punto_reposicion    DECIMAL(12,2),
    @deposito_ubicacion  VARCHAR(30) = NULL,
    @estado_articulo     VARCHAR(15) = 'Disponible'
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Articulo
        (codigo, descripcion, id_categoria, id_marca, id_unidad_medida,
         precio_unitario, cantidad_disponible, punto_reposicion,
         deposito_ubicacion, estado_articulo, activo, dvh)
    VALUES
        (@codigo, @descripcion, @id_categoria, @id_marca, @id_unidad_medida,
         @precio_unitario, @cantidad_disponible, @punto_reposicion,
         @deposito_ubicacion, @estado_articulo, 1, NULL);

    SELECT SCOPE_IDENTITY();
END
GO

IF OBJECT_ID('dbo.SP_ActualizarArticulo', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ActualizarArticulo];
GO
CREATE PROCEDURE [dbo].[SP_ActualizarArticulo]
(
    @id_articulo         INT,
    @codigo              VARCHAR(25),
    @descripcion         VARCHAR(200),
    @id_categoria        INT,
    @id_marca            INT,
    @id_unidad_medida    INT,
    @precio_unitario     DECIMAL(12,2),
    @cantidad_disponible DECIMAL(12,2),
    @punto_reposicion    DECIMAL(12,2),
    @deposito_ubicacion  VARCHAR(30),
    @estado_articulo     VARCHAR(15),
    @activo              BIT
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Articulo
    SET codigo              = @codigo,
        descripcion         = @descripcion,
        id_categoria        = @id_categoria,
        id_marca            = @id_marca,
        id_unidad_medida    = @id_unidad_medida,
        precio_unitario     = @precio_unitario,
        cantidad_disponible = @cantidad_disponible,
        punto_reposicion    = @punto_reposicion,
        deposito_ubicacion  = @deposito_ubicacion,
        estado_articulo     = @estado_articulo,
        activo              = @activo
    WHERE id_articulo = @id_articulo;
END
GO

-- Baja logica: nunca DELETE fisico.
IF OBJECT_ID('dbo.SP_ElimArticulo', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ElimArticulo];
GO
CREATE PROCEDURE [dbo].[SP_ElimArticulo]
    @id_articulo INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Articulo SET activo = 0 WHERE id_articulo = @id_articulo;
END
GO

-- RFN1.6 - Movimiento de existencias. @cantidad es un delta con signo:
-- negativo descuenta una venta, positivo repone. No deja la cantidad en
-- negativo. Cuando llega a cero el articulo pasa a SinStock, y vuelve a
-- Disponible al reponerse, salvo que este Bloqueado por una reserva.
IF OBJECT_ID('dbo.SP_ActualizarStock', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ActualizarStock];
GO
CREATE PROCEDURE [dbo].[SP_ActualizarStock]
(
    @id_articulo INT,
    @cantidad    DECIMAL(12,2)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Articulo WHERE id_articulo = @id_articulo)
    BEGIN
        RAISERROR('Articulo inexistente.', 16, 1);
        RETURN;
    END

    IF (SELECT cantidad_disponible FROM Articulo WHERE id_articulo = @id_articulo) + @cantidad < 0
    BEGIN
        RAISERROR('El movimiento deja las existencias en negativo.', 16, 1);
        RETURN;
    END

    UPDATE Articulo
    SET cantidad_disponible = cantidad_disponible + @cantidad,
        estado_articulo = CASE
            WHEN estado_articulo = 'Bloqueado' THEN 'Bloqueado'
            WHEN cantidad_disponible + @cantidad = 0 THEN 'SinStock'
            ELSE 'Disponible'
        END
    WHERE id_articulo = @id_articulo;
END
GO

-- Bloqueo y liberacion del articulo. El bloqueo lo dispara la reserva de un
-- faltante y se mantiene hasta que la compra se concrete.
IF OBJECT_ID('dbo.SP_ActualizarEstadoArticulo', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ActualizarEstadoArticulo];
GO
CREATE PROCEDURE [dbo].[SP_ActualizarEstadoArticulo]
(
    @id_articulo     INT,
    @estado_articulo VARCHAR(15)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @estado_articulo NOT IN ('Disponible', 'Bloqueado', 'SinStock')
    BEGIN
        RAISERROR('Estado de articulo invalido.', 16, 1);
        RETURN;
    END

    UPDATE Articulo SET estado_articulo = @estado_articulo WHERE id_articulo = @id_articulo;
END
GO

-- =============================================================================
-- Bobina
-- =============================================================================

IF OBJECT_ID('dbo.SP_ExtBobina', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtBobina];
GO
CREATE PROCEDURE [dbo].[SP_ExtBobina]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_bobina, id_articulo, identificador, medida_inicial, saldo_bobina,
           fecha_apertura, estado, dvh
    FROM Bobina
    ORDER BY id_bobina;
END
GO

-- Las bobinas de un articulo, primero las abiertas y de menor saldo: asi el
-- fraccionamiento agota la bobina ya abierta antes de abrir una nueva.
IF OBJECT_ID('dbo.SP_ExtBobinaPorArticulo', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtBobinaPorArticulo];
GO
CREATE PROCEDURE [dbo].[SP_ExtBobinaPorArticulo]
    @id_articulo INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_bobina, id_articulo, identificador, medida_inicial, saldo_bobina,
           fecha_apertura, estado, dvh
    FROM Bobina
    WHERE id_articulo = @id_articulo
      AND estado <> 'Agotada'
    ORDER BY CASE estado WHEN 'Abierta' THEN 0 ELSE 1 END, saldo_bobina, id_bobina;
END
GO

IF OBJECT_ID('dbo.SP_CrearBobina', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_CrearBobina];
GO
CREATE PROCEDURE [dbo].[SP_CrearBobina]
(
    @id_articulo    INT,
    @identificador  VARCHAR(25),
    @medida_inicial DECIMAL(12,2),
    @saldo_bobina   DECIMAL(12,2),
    @estado         VARCHAR(15),
    @fecha_apertura DATETIME = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Bobina
        (id_articulo, identificador, medida_inicial, saldo_bobina, fecha_apertura, estado, dvh)
    VALUES
        (@id_articulo, @identificador, @medida_inicial, @saldo_bobina, @fecha_apertura, @estado, NULL);

    SELECT SCOPE_IDENTITY();
END
GO

-- RFN1.8 - Fraccionamiento. La BLL decide el nuevo saldo y el estado; aca solo
-- se persiste, validando que el saldo no quede fuera de rango.
IF OBJECT_ID('dbo.SP_ActualizarSaldoBobina', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ActualizarSaldoBobina];
GO
CREATE PROCEDURE [dbo].[SP_ActualizarSaldoBobina]
(
    @id_bobina      INT,
    @saldo_bobina   DECIMAL(12,2),
    @estado         VARCHAR(15),
    @fecha_apertura DATETIME = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Bobina WHERE id_bobina = @id_bobina)
    BEGIN
        RAISERROR('Bobina inexistente.', 16, 1);
        RETURN;
    END

    IF @saldo_bobina < 0 OR @saldo_bobina > (SELECT medida_inicial FROM Bobina WHERE id_bobina = @id_bobina)
    BEGIN
        RAISERROR('El saldo queda fuera del rango de la bobina.', 16, 1);
        RETURN;
    END

    UPDATE Bobina
    SET saldo_bobina   = @saldo_bobina,
        estado         = @estado,
        fecha_apertura = ISNULL(@fecha_apertura, fecha_apertura)
    WHERE id_bobina = @id_bobina;
END
GO

-- =============================================================================
-- Cliente
--
-- En la Entrega 1 el cliente solo se consulta y se da de alta: el alta entra
-- por DNI desde la caja, como extend de CUN-004. No hay SP_ActualizarCliente ni
-- SP_ElimCliente todavia; el ABM completo queda fuera del alcance.
-- =============================================================================

IF OBJECT_ID('dbo.SP_ExtCliente', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtCliente];
GO
CREATE PROCEDURE [dbo].[SP_ExtCliente]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_cliente, razon_social, nombre, apellido, dni, cuit, condicion_iva,
           direccion, telefono, correo_electronico, fecha_actualizacion, activo, dvh
    FROM Cliente
    ORDER BY id_cliente;
END
GO

-- Entrada del circuito de caja: se busca al cliente por documento.
IF OBJECT_ID('dbo.SP_BuscarClientePorDni', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_BuscarClientePorDni];
GO
CREATE PROCEDURE [dbo].[SP_BuscarClientePorDni]
    @dni VARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_cliente, razon_social, nombre, apellido, dni, cuit, condicion_iva,
           direccion, telefono, correo_electronico, fecha_actualizacion, activo, dvh
    FROM Cliente
    WHERE dni = @dni AND activo = 1;
END
GO

-- Busqueda general por documento, CUIT o razon social. Los criterios no
-- informados llegan NULL desde la DAL y no filtran.
IF OBJECT_ID('dbo.SP_BuscarCliente', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_BuscarCliente];
GO
CREATE PROCEDURE [dbo].[SP_BuscarCliente]
(
    @dni          VARCHAR(15)  = NULL,
    @cuit         VARCHAR(13)  = NULL,
    @razon_social VARCHAR(100) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_cliente, razon_social, nombre, apellido, dni, cuit, condicion_iva,
           direccion, telefono, correo_electronico, fecha_actualizacion, activo, dvh
    FROM Cliente
    WHERE (@dni          IS NULL OR dni  = @dni)
      AND (@cuit         IS NULL OR cuit = @cuit)
      AND (@razon_social IS NULL OR razon_social LIKE '%' + @razon_social + '%')
    ORDER BY id_cliente;
END
GO

-- Devuelve el id_cliente generado: la BLL lo necesita para calcular el dvh.
-- El correo llega ya cifrado desde la BLL (T03.2).
IF OBJECT_ID('dbo.SP_CrearCliente', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_CrearCliente];
GO
CREATE PROCEDURE [dbo].[SP_CrearCliente]
(
    @razon_social        VARCHAR(100) = NULL,
    @nombre              VARCHAR(50),
    @apellido            VARCHAR(50),
    @dni                 VARCHAR(15),
    @cuit                VARCHAR(13)  = NULL,
    @condicion_iva       VARCHAR(30),
    @direccion           VARCHAR(150) = NULL,
    @telefono            VARCHAR(30)  = NULL,
    @correo_electronico  VARCHAR(256) = NULL,
    @fecha_actualizacion DATETIME
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Cliente
        (razon_social, nombre, apellido, dni, cuit, condicion_iva,
         direccion, telefono, correo_electronico, fecha_actualizacion, activo, dvh)
    VALUES
        (@razon_social, @nombre, @apellido, @dni, @cuit, @condicion_iva,
         @direccion, @telefono, @correo_electronico, @fecha_actualizacion, 1, NULL);

    SELECT SCOPE_IDENTITY();
END
GO

-- =============================================================================
-- Carrito y detalle (CUN-001)
--
-- Cada procedimiento escribe una sola tabla, como exige la regla de negocio:
-- cuando una operacion toca dos tablas, las encadena la BLL, que recalcula el
-- dvh de cada fila y el DVV de cada tabla por separado.
-- =============================================================================

IF OBJECT_ID('dbo.SP_ExtCarrito', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtCarrito];
GO
CREATE PROCEDURE [dbo].[SP_ExtCarrito]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_carrito, nro_carrito, usuario_vendedor, nombre_cliente,
           fecha_apertura, estado, precio_total, activo, dvh
    FROM Carrito
    ORDER BY id_carrito;
END
GO

-- Los carritos que el cajero puede tomar: armados por el vendedor y todavia
-- sin facturar.
IF OBJECT_ID('dbo.SP_ExtCarritoAbiertos', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtCarritoAbiertos];
GO
CREATE PROCEDURE [dbo].[SP_ExtCarritoAbiertos]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_carrito, nro_carrito, usuario_vendedor, nombre_cliente,
           fecha_apertura, estado, precio_total, activo, dvh
    FROM Carrito
    WHERE activo = 1 AND estado IN ('Abierto', 'Confirmado')
    ORDER BY fecha_apertura;
END
GO

IF OBJECT_ID('dbo.SP_ExtDetalleCarrito', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtDetalleCarrito];
GO
CREATE PROCEDURE [dbo].[SP_ExtDetalleCarrito]
    @id_carrito INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_detalle_carrito, id_carrito, id_articulo, cantidad, id_unidad_medida,
           precio_unitario, subtotal, id_bobina, dvh
    FROM DetalleCarrito
    WHERE (@id_carrito IS NULL OR id_carrito = @id_carrito)
    ORDER BY id_detalle_carrito;
END
GO

-- Numero de carrito siguiente, con el formato CAR-000000.
IF OBJECT_ID('dbo.SP_ProximoNumeroCarrito', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ProximoNumeroCarrito];
GO
CREATE PROCEDURE [dbo].[SP_ProximoNumeroCarrito]
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @siguiente INT = ISNULL(
        (SELECT MAX(TRY_CONVERT(INT, RIGHT(nro_carrito, 6))) FROM Carrito), 0) + 1;
    SELECT 'CAR-' + RIGHT('000000' + CONVERT(VARCHAR(6), @siguiente), 6);
END
GO

IF OBJECT_ID('dbo.SP_CrearCarrito', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_CrearCarrito];
GO
CREATE PROCEDURE [dbo].[SP_CrearCarrito]
(
    @nro_carrito      VARCHAR(15),
    @usuario_vendedor VARCHAR(50),
    @nombre_cliente   VARCHAR(100) = NULL,
    @fecha_apertura   DATETIME
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Carrito
        (nro_carrito, usuario_vendedor, nombre_cliente, fecha_apertura,
         estado, precio_total, activo, dvh)
    VALUES
        (@nro_carrito, @usuario_vendedor, @nombre_cliente, @fecha_apertura,
         'Abierto', 0, 1, NULL);

    SELECT SCOPE_IDENTITY();
END
GO

IF OBJECT_ID('dbo.SP_AgregarDetalleCarrito', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_AgregarDetalleCarrito];
GO
CREATE PROCEDURE [dbo].[SP_AgregarDetalleCarrito]
(
    @id_carrito       INT,
    @id_articulo      INT,
    @cantidad         DECIMAL(12,2),
    @id_unidad_medida INT,
    @precio_unitario  DECIMAL(12,2),
    @subtotal         DECIMAL(12,2),
    @id_bobina        INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO DetalleCarrito
        (id_carrito, id_articulo, cantidad, id_unidad_medida,
         precio_unitario, subtotal, id_bobina, dvh)
    VALUES
        (@id_carrito, @id_articulo, @cantidad, @id_unidad_medida,
         @precio_unitario, @subtotal, @id_bobina, NULL);

    SELECT SCOPE_IDENTITY();
END
GO

-- El renglon se borra fisicamente: es un borrador de mostrador, no un
-- documento. La baja logica aplica al carrito, que se anula.
IF OBJECT_ID('dbo.SP_QuitarDetalleCarrito', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_QuitarDetalleCarrito];
GO
CREATE PROCEDURE [dbo].[SP_QuitarDetalleCarrito]
    @id_detalle_carrito INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM DetalleCarrito WHERE id_detalle_carrito = @id_detalle_carrito;
END
GO

IF OBJECT_ID('dbo.SP_ActualizarTotalCarrito', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ActualizarTotalCarrito];
GO
CREATE PROCEDURE [dbo].[SP_ActualizarTotalCarrito]
(
    @id_carrito   INT,
    @precio_total DECIMAL(12,2)
)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Carrito SET precio_total = @precio_total WHERE id_carrito = @id_carrito;
END
GO

IF OBJECT_ID('dbo.SP_ActualizarEstadoCarrito', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ActualizarEstadoCarrito];
GO
CREATE PROCEDURE [dbo].[SP_ActualizarEstadoCarrito]
(
    @id_carrito INT,
    @estado     VARCHAR(15)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @estado NOT IN ('Abierto', 'Confirmado', 'Facturado', 'Anulado')
    BEGIN
        RAISERROR('Estado de carrito invalido.', 16, 1);
        RETURN;
    END

    UPDATE Carrito SET estado = @estado WHERE id_carrito = @id_carrito;
END
GO

-- Cierra el carrito: fija el total y lo pasa a Confirmado en una sola
-- escritura sobre Carrito. El total se recalcula desde el detalle aca, para
-- que no dependa de lo que traiga la pantalla.
IF OBJECT_ID('dbo.SP_ConfirmarCarrito', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ConfirmarCarrito];
GO
CREATE PROCEDURE [dbo].[SP_ConfirmarCarrito]
(
    @id_carrito     INT,
    @nombre_cliente VARCHAR(100) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM DetalleCarrito WHERE id_carrito = @id_carrito)
    BEGIN
        RAISERROR('El carrito no tiene articulos.', 16, 1);
        RETURN;
    END

    UPDATE Carrito
    SET precio_total   = (SELECT SUM(subtotal) FROM DetalleCarrito WHERE id_carrito = @id_carrito),
        estado         = 'Confirmado',
        nombre_cliente = ISNULL(@nombre_cliente, nombre_cliente)
    WHERE id_carrito = @id_carrito AND estado = 'Abierto';

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('El carrito no esta abierto.', 16, 1);
        RETURN;
    END
END
GO

-- =============================================================================
-- Reserva del faltante (CUN-001)
-- =============================================================================

IF OBJECT_ID('dbo.SP_CrearReserva', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_CrearReserva];
GO
CREATE PROCEDURE [dbo].[SP_CrearReserva]
(
    @id_articulo    INT,
    @id_cliente     INT = NULL,
    @nombre_cliente VARCHAR(100) = NULL,
    @cantidad       DECIMAL(12,2),
    @fecha_reserva  DATETIME,
    @plazo_entrega  INT
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Reserva
        (id_articulo, id_cliente, nombre_cliente, cantidad,
         fecha_reserva, plazo_entrega, estado, dvh)
    VALUES
        (@id_articulo, @id_cliente, @nombre_cliente, @cantidad,
         @fecha_reserva, @plazo_entrega, 'Pendiente', NULL);

    SELECT SCOPE_IDENTITY();
END
GO

IF OBJECT_ID('dbo.SP_ExtReservaPorArticulo', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtReservaPorArticulo];
GO
CREATE PROCEDURE [dbo].[SP_ExtReservaPorArticulo]
    @id_articulo INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_reserva, id_articulo, id_cliente, nombre_cliente, cantidad,
           fecha_reserva, plazo_entrega, estado, dvh
    FROM Reserva
    WHERE (@id_articulo IS NULL OR id_articulo = @id_articulo)
    ORDER BY id_reserva;
END
GO

IF OBJECT_ID('dbo.SP_ActualizarEstadoReserva', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ActualizarEstadoReserva];
GO
CREATE PROCEDURE [dbo].[SP_ActualizarEstadoReserva]
(
    @id_reserva INT,
    @estado     VARCHAR(15)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @estado NOT IN ('Pendiente', 'EnCompra', 'Cumplida', 'Anulada')
    BEGIN
        RAISERROR('Estado de reserva invalido.', 16, 1);
        RETURN;
    END

    UPDATE Reserva SET estado = @estado WHERE id_reserva = @id_reserva;
END
GO

-- =============================================================================
-- Orden de pago (CUN-002)
-- =============================================================================

-- Numero de orden siguiente, con el formato OP-000000.
IF OBJECT_ID('dbo.SP_ProximoNumeroOrdenPago', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ProximoNumeroOrdenPago];
GO
CREATE PROCEDURE [dbo].[SP_ProximoNumeroOrdenPago]
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @siguiente INT = ISNULL(
        (SELECT MAX(TRY_CONVERT(INT, RIGHT(nro_orden_pago, 6))) FROM OrdenPago), 0) + 1;
    SELECT 'OP-' + RIGHT('000000' + CONVERT(VARCHAR(6), @siguiente), 6);
END
GO

IF OBJECT_ID('dbo.SP_CrearOrdenPago', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_CrearOrdenPago];
GO
CREATE PROCEDURE [dbo].[SP_CrearOrdenPago]
(
    @nro_orden_pago VARCHAR(15),
    @id_carrito     INT,
    @nombre_cliente VARCHAR(100) = NULL,
    @fecha_emision  DATETIME,
    @precio_total   DECIMAL(12,2)
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO OrdenPago
        (nro_orden_pago, id_carrito, nombre_cliente, fecha_emision,
         precio_total, estado, activo, dvh)
    VALUES
        (@nro_orden_pago, @id_carrito, @nombre_cliente, @fecha_emision,
         @precio_total, 'Pendiente', 1, NULL);

    SELECT SCOPE_IDENTITY();
END
GO

-- Las ordenes que esperan cobro en la caja.
IF OBJECT_ID('dbo.SP_ExtOrdenPagoPendientes', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtOrdenPagoPendientes];
GO
CREATE PROCEDURE [dbo].[SP_ExtOrdenPagoPendientes]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_orden_pago, nro_orden_pago, id_carrito, nombre_cliente,
           fecha_emision, precio_total, estado, activo, dvh
    FROM OrdenPago
    WHERE activo = 1 AND estado = 'Pendiente'
    ORDER BY fecha_emision;
END
GO

-- Con el numero busca una orden; sin el, devuelve todas. La segunda forma la
-- usa el control de integridad, que necesita la tabla completa ordenada por
-- clave primaria.
IF OBJECT_ID('dbo.SP_BuscarOrdenPago', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_BuscarOrdenPago];
GO
CREATE PROCEDURE [dbo].[SP_BuscarOrdenPago]
    @nro_orden_pago VARCHAR(15) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_orden_pago, nro_orden_pago, id_carrito, nombre_cliente,
           fecha_emision, precio_total, estado, activo, dvh
    FROM OrdenPago
    WHERE (@nro_orden_pago IS NULL OR nro_orden_pago = @nro_orden_pago)
    ORDER BY id_orden_pago;
END
GO

IF OBJECT_ID('dbo.SP_ActualizarEstadoOrdenPago', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ActualizarEstadoOrdenPago];
GO
CREATE PROCEDURE [dbo].[SP_ActualizarEstadoOrdenPago]
(
    @id_orden_pago INT,
    @estado        VARCHAR(15)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @estado NOT IN ('Pendiente', 'Pagada', 'Anulada')
    BEGIN
        RAISERROR('Estado de orden de pago invalido.', 16, 1);
        RETURN;
    END

    UPDATE OrdenPago SET estado = @estado WHERE id_orden_pago = @id_orden_pago;
END
GO

-- =============================================================================
-- Comprobante, detalle y pago (CUN-004)
-- =============================================================================

-- Numero de comprobante siguiente, por tipo, con el formato A-0001-00000000.
IF OBJECT_ID('dbo.SP_ProximoNumeroComprobante', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ProximoNumeroComprobante];
GO
CREATE PROCEDURE [dbo].[SP_ProximoNumeroComprobante]
    @tipo_comprobante VARCHAR(5)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @siguiente INT = ISNULL(
        (SELECT MAX(TRY_CONVERT(INT, RIGHT(nro_comprobante, 8)))
         FROM Comprobante WHERE tipo_comprobante = @tipo_comprobante), 0) + 1;

    SELECT @tipo_comprobante + '-0001-' +
           RIGHT('00000000' + CONVERT(VARCHAR(8), @siguiente), 8);
END
GO

IF OBJECT_ID('dbo.SP_CrearComprobante', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_CrearComprobante];
GO
CREATE PROCEDURE [dbo].[SP_CrearComprobante]
(
    @nro_comprobante  VARCHAR(20),
    @tipo_comprobante VARCHAR(5),
    @id_orden_pago    INT,
    @id_cliente       INT,
    @fecha_emision    DATETIME,
    @neto             DECIMAL(12,2),
    @iva              DECIMAL(12,2),
    @precio_total     DECIMAL(12,2)
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Comprobante
        (nro_comprobante, tipo_comprobante, id_orden_pago, id_cliente,
         fecha_emision, neto, iva, precio_total, estado, dvh)
    VALUES
        (@nro_comprobante, @tipo_comprobante, @id_orden_pago, @id_cliente,
         @fecha_emision, @neto, @iva, @precio_total, 'Emitido', NULL);

    SELECT SCOPE_IDENTITY();
END
GO

IF OBJECT_ID('dbo.SP_AgregarDetalleComprobante', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_AgregarDetalleComprobante];
GO
CREATE PROCEDURE [dbo].[SP_AgregarDetalleComprobante]
(
    @id_comprobante   INT,
    @id_articulo      INT,
    @cantidad         DECIMAL(12,2),
    @id_unidad_medida INT,
    @precio_unitario  DECIMAL(12,2)
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO DetalleComprobante
        (id_comprobante, id_articulo, cantidad, id_unidad_medida, precio_unitario, dvh)
    VALUES
        (@id_comprobante, @id_articulo, @cantidad, @id_unidad_medida, @precio_unitario, NULL);

    SELECT SCOPE_IDENTITY();
END
GO

-- Con el numero busca un comprobante; sin el, devuelve todos. La segunda forma
-- la usa el control de integridad.
IF OBJECT_ID('dbo.SP_BuscarComprobante', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_BuscarComprobante];
GO
CREATE PROCEDURE [dbo].[SP_BuscarComprobante]
    @nro_comprobante VARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_comprobante, nro_comprobante, tipo_comprobante, id_orden_pago,
           id_cliente, fecha_emision, neto, iva, precio_total, estado, dvh
    FROM Comprobante
    WHERE (@nro_comprobante IS NULL OR nro_comprobante = @nro_comprobante)
    ORDER BY id_comprobante;
END
GO

IF OBJECT_ID('dbo.SP_ExtDetalleComprobante', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtDetalleComprobante];
GO
CREATE PROCEDURE [dbo].[SP_ExtDetalleComprobante]
    @id_comprobante INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_detalle_comprobante, id_comprobante, id_articulo, cantidad,
           id_unidad_medida, precio_unitario, dvh
    FROM DetalleComprobante
    WHERE (@id_comprobante IS NULL OR id_comprobante = @id_comprobante)
    ORDER BY id_detalle_comprobante;
END
GO

IF OBJECT_ID('dbo.SP_ActualizarEstadoComprobante', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ActualizarEstadoComprobante];
GO
CREATE PROCEDURE [dbo].[SP_ActualizarEstadoComprobante]
(
    @id_comprobante INT,
    @estado         VARCHAR(15)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @estado NOT IN ('Emitido', 'Entregado')
    BEGIN
        RAISERROR('Estado de comprobante invalido.', 16, 1);
        RETURN;
    END

    UPDATE Comprobante SET estado = @estado WHERE id_comprobante = @id_comprobante;
END
GO

IF OBJECT_ID('dbo.SP_RegistrarPago', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_RegistrarPago];
GO
CREATE PROCEDURE [dbo].[SP_RegistrarPago]
(
    @id_comprobante      INT,
    @id_medio_pago       INT,
    @monto               DECIMAL(12,2),
    @fecha_hora          DATETIME,
    @codigo_autorizacion VARCHAR(30) = NULL,
    @estado              VARCHAR(15)
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Pago
        (id_comprobante, id_medio_pago, monto, fecha_hora,
         codigo_autorizacion, estado, dvh)
    VALUES
        (@id_comprobante, @id_medio_pago, @monto, @fecha_hora,
         @codigo_autorizacion, @estado, NULL);

    SELECT SCOPE_IDENTITY();
END
GO

IF OBJECT_ID('dbo.SP_ExtPago', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtPago];
GO
CREATE PROCEDURE [dbo].[SP_ExtPago]
    @id_comprobante INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_pago, id_comprobante, id_medio_pago, monto, fecha_hora,
           codigo_autorizacion, estado, dvh
    FROM Pago
    WHERE (@id_comprobante IS NULL OR id_comprobante = @id_comprobante)
    ORDER BY id_pago;
END
GO

-- =============================================================================
-- Entrega en deposito (CUN-005)
--
-- No mueve existencias: el stock ya bajo al emitirse el comprobante.
-- =============================================================================

IF OBJECT_ID('dbo.SP_CrearEntrega', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_CrearEntrega];
GO
CREATE PROCEDURE [dbo].[SP_CrearEntrega]
(
    @id_comprobante   INT,
    @usuario_deposito VARCHAR(50),
    @fecha_entrega    DATETIME,
    @estado           VARCHAR(15)
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Entrega
        (id_comprobante, usuario_deposito, fecha_entrega, estado, dvh)
    VALUES
        (@id_comprobante, @usuario_deposito, @fecha_entrega, @estado, NULL);

    SELECT SCOPE_IDENTITY();
END
GO

IF OBJECT_ID('dbo.SP_AgregarDetalleEntrega', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_AgregarDetalleEntrega];
GO
CREATE PROCEDURE [dbo].[SP_AgregarDetalleEntrega]
(
    @id_entrega         INT,
    @id_articulo        INT,
    @cantidad_entregada DECIMAL(12,2),
    @id_unidad_medida   INT
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO DetalleEntrega
        (id_entrega, id_articulo, cantidad_entregada, id_unidad_medida, dvh)
    VALUES
        (@id_entrega, @id_articulo, @cantidad_entregada, @id_unidad_medida, NULL);

    SELECT SCOPE_IDENTITY();
END
GO

-- Con el comprobante devuelve su entrega; sin el, devuelve todas. La segunda
-- forma la usa el control de integridad.
IF OBJECT_ID('dbo.SP_ExtEntregaPorComprobante', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtEntregaPorComprobante];
GO
CREATE PROCEDURE [dbo].[SP_ExtEntregaPorComprobante]
    @id_comprobante INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_entrega, id_comprobante, usuario_deposito, fecha_entrega, estado, dvh
    FROM Entrega
    WHERE (@id_comprobante IS NULL OR id_comprobante = @id_comprobante)
    ORDER BY id_entrega;
END
GO

IF OBJECT_ID('dbo.SP_ExtDetalleEntrega', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtDetalleEntrega];
GO
CREATE PROCEDURE [dbo].[SP_ExtDetalleEntrega]
    @id_entrega INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_detalle_entrega, id_entrega, id_articulo, cantidad_entregada,
           id_unidad_medida, dvh
    FROM DetalleEntrega
    WHERE (@id_entrega IS NULL OR id_entrega = @id_entrega)
    ORDER BY id_detalle_entrega;
END
GO

PRINT '06 - Stored procedures de negocio fase 1 creados.';
GO
