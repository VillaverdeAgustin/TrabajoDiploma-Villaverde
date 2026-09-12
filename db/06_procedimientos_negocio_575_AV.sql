-- =============================================================================
-- ConectAR S.R.L. - Trabajo de Diploma - Villaverde, Agustin (575_AV)
-- 06 - Stored procedures de negocio, fase 1: catalogos, producto y bobina
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

-- Persiste el DVH de una fila cualquiera de negocio. La BLL lo llama despues
-- del alta o la modificacion, una vez que conoce el Id generado.
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
        N'UPDATE ' + QUOTENAME(@tabla) + N' SET DVH = @dvh WHERE ' + QUOTENAME(@clave) + N' = @id';
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
    SELECT IdCategoria, Nombre, Activo, DVH
    FROM Categoria
    ORDER BY IdCategoria;
END
GO

IF OBJECT_ID('dbo.SP_ExtMarca', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtMarca];
GO
CREATE PROCEDURE [dbo].[SP_ExtMarca]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdMarca, Nombre, Activo, DVH
    FROM Marca
    ORDER BY IdMarca;
END
GO

IF OBJECT_ID('dbo.SP_ExtUnidadMedida', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtUnidadMedida];
GO
CREATE PROCEDURE [dbo].[SP_ExtUnidadMedida]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdUnidadMedida, Nombre, Abreviatura, Fraccionable, DVH
    FROM UnidadMedida
    ORDER BY IdUnidadMedida;
END
GO

IF OBJECT_ID('dbo.SP_ExtMedioPago', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtMedioPago];
GO
CREATE PROCEDURE [dbo].[SP_ExtMedioPago]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdMedioPago, Nombre, RequiereAutorizacion, Activo, DVH
    FROM MedioPago
    ORDER BY IdMedioPago;
END
GO

-- Alta de marca: la usa el ABM de producto cuando la marca no existe todavia.
IF OBJECT_ID('dbo.SP_CrearMarca', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_CrearMarca];
GO
CREATE PROCEDURE [dbo].[SP_CrearMarca]
    @Nombre VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Marca (Nombre, Activo, DVH) VALUES (@Nombre, 1, NULL);
    SELECT SCOPE_IDENTITY();
END
GO

-- =============================================================================
-- Producto
-- =============================================================================

-- Listado completo. El DVV se calcula sobre la secuencia ordenada por PK,
-- asi que el ORDER BY tiene que coincidir con el OrderBy de la BLL.
IF OBJECT_ID('dbo.SP_ExtProducto', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtProducto];
GO
CREATE PROCEDURE [dbo].[SP_ExtProducto]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdProducto, Codigo, Descripcion, IdCategoria, IdMarca, IdUnidadMedida,
           PrecioUnitario, StockActual, PuntoReposicion, Activo, DVH
    FROM Producto
    ORDER BY IdProducto;
END
GO

-- RFN1.1 - Consultar articulos por descripcion, categoria, marca o codigo.
-- Los criterios no informados llegan NULL desde la DAL y no filtran.
IF OBJECT_ID('dbo.SP_BuscarProducto', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_BuscarProducto];
GO
CREATE PROCEDURE [dbo].[SP_BuscarProducto]
(
    @Descripcion VARCHAR(200) = NULL,
    @IdCategoria INT          = NULL,
    @IdMarca     INT          = NULL,
    @Codigo      VARCHAR(25)  = NULL,
    @SoloActivos BIT          = 1
)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdProducto, Codigo, Descripcion, IdCategoria, IdMarca, IdUnidadMedida,
           PrecioUnitario, StockActual, PuntoReposicion, Activo, DVH
    FROM Producto
    WHERE (@Descripcion IS NULL OR Descripcion LIKE '%' + @Descripcion + '%')
      AND (@IdCategoria IS NULL OR IdCategoria = @IdCategoria)
      AND (@IdMarca     IS NULL OR IdMarca     = @IdMarca)
      AND (@Codigo      IS NULL OR Codigo      = @Codigo)
      AND (@SoloActivos = 0     OR Activo      = 1)
    ORDER BY IdProducto;
END
GO

-- Devuelve el IdProducto generado: la BLL lo necesita para calcular el DVH
-- y persistirlo enseguida con SP_ActualizarDVHNegocio.
IF OBJECT_ID('dbo.SP_CrearProducto', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_CrearProducto];
GO
CREATE PROCEDURE [dbo].[SP_CrearProducto]
(
    @Codigo          VARCHAR(25),
    @Descripcion     VARCHAR(200),
    @IdCategoria     INT,
    @IdMarca         INT,
    @IdUnidadMedida  INT,
    @PrecioUnitario  DECIMAL(12,2),
    @StockActual     DECIMAL(12,2),
    @PuntoReposicion DECIMAL(12,2)
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Producto
        (Codigo, Descripcion, IdCategoria, IdMarca, IdUnidadMedida,
         PrecioUnitario, StockActual, PuntoReposicion, Activo, DVH)
    VALUES
        (@Codigo, @Descripcion, @IdCategoria, @IdMarca, @IdUnidadMedida,
         @PrecioUnitario, @StockActual, @PuntoReposicion, 1, NULL);

    SELECT SCOPE_IDENTITY();
END
GO

IF OBJECT_ID('dbo.SP_ActualizarProducto', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ActualizarProducto];
GO
CREATE PROCEDURE [dbo].[SP_ActualizarProducto]
(
    @IdProducto      INT,
    @Codigo          VARCHAR(25),
    @Descripcion     VARCHAR(200),
    @IdCategoria     INT,
    @IdMarca         INT,
    @IdUnidadMedida  INT,
    @PrecioUnitario  DECIMAL(12,2),
    @StockActual     DECIMAL(12,2),
    @PuntoReposicion DECIMAL(12,2),
    @Activo          BIT
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Producto
    SET Codigo          = @Codigo,
        Descripcion     = @Descripcion,
        IdCategoria     = @IdCategoria,
        IdMarca         = @IdMarca,
        IdUnidadMedida  = @IdUnidadMedida,
        PrecioUnitario  = @PrecioUnitario,
        StockActual     = @StockActual,
        PuntoReposicion = @PuntoReposicion,
        Activo          = @Activo
    WHERE IdProducto = @IdProducto;
END
GO

-- Baja logica: nunca DELETE fisico.
IF OBJECT_ID('dbo.SP_ElimProducto', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ElimProducto];
GO
CREATE PROCEDURE [dbo].[SP_ElimProducto]
    @IdProducto INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Producto SET Activo = 0 WHERE IdProducto = @IdProducto;
END
GO

-- RFN1.6 - Movimiento de stock. @Cantidad es un delta con signo: negativo
-- descuenta una venta, positivo repone. No deja el stock por debajo de cero.
IF OBJECT_ID('dbo.SP_ActualizarStock', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ActualizarStock];
GO
CREATE PROCEDURE [dbo].[SP_ActualizarStock]
(
    @IdProducto INT,
    @Cantidad   DECIMAL(12,2)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Producto WHERE IdProducto = @IdProducto)
    BEGIN
        RAISERROR('Producto inexistente.', 16, 1);
        RETURN;
    END

    IF (SELECT StockActual FROM Producto WHERE IdProducto = @IdProducto) + @Cantidad < 0
    BEGIN
        RAISERROR('El movimiento deja el stock en negativo.', 16, 1);
        RETURN;
    END

    UPDATE Producto
    SET StockActual = StockActual + @Cantidad
    WHERE IdProducto = @IdProducto;
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
    SELECT IdBobina, IdProducto, Identificador, MedidaInicial, SaldoActual,
           FechaApertura, Estado, DVH
    FROM Bobina
    ORDER BY IdBobina;
END
GO

-- Las bobinas de un producto, primero las abiertas y de menor saldo: asi el
-- fraccionamiento agota la bobina ya abierta antes de abrir una nueva.
IF OBJECT_ID('dbo.SP_ExtBobinaPorProducto', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtBobinaPorProducto];
GO
CREATE PROCEDURE [dbo].[SP_ExtBobinaPorProducto]
    @IdProducto INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdBobina, IdProducto, Identificador, MedidaInicial, SaldoActual,
           FechaApertura, Estado, DVH
    FROM Bobina
    WHERE IdProducto = @IdProducto
      AND Estado <> 'Agotada'
    ORDER BY CASE Estado WHEN 'Abierta' THEN 0 ELSE 1 END, SaldoActual, IdBobina;
END
GO

IF OBJECT_ID('dbo.SP_CrearBobina', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_CrearBobina];
GO
CREATE PROCEDURE [dbo].[SP_CrearBobina]
(
    @IdProducto    INT,
    @Identificador VARCHAR(25),
    @MedidaInicial DECIMAL(12,2),
    @SaldoActual   DECIMAL(12,2),
    @Estado        VARCHAR(15),
    @FechaApertura DATETIME = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Bobina
        (IdProducto, Identificador, MedidaInicial, SaldoActual, FechaApertura, Estado, DVH)
    VALUES
        (@IdProducto, @Identificador, @MedidaInicial, @SaldoActual, @FechaApertura, @Estado, NULL);

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
    @IdBobina      INT,
    @SaldoActual   DECIMAL(12,2),
    @Estado        VARCHAR(15),
    @FechaApertura DATETIME = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Bobina WHERE IdBobina = @IdBobina)
    BEGIN
        RAISERROR('Bobina inexistente.', 16, 1);
        RETURN;
    END

    IF @SaldoActual < 0 OR @SaldoActual > (SELECT MedidaInicial FROM Bobina WHERE IdBobina = @IdBobina)
    BEGIN
        RAISERROR('El saldo queda fuera del rango de la bobina.', 16, 1);
        RETURN;
    END

    UPDATE Bobina
    SET SaldoActual   = @SaldoActual,
        Estado        = @Estado,
        FechaApertura = ISNULL(@FechaApertura, FechaApertura)
    WHERE IdBobina = @IdBobina;
END
GO

PRINT '06 - Stored procedures de negocio fase 1 creados.';
GO
