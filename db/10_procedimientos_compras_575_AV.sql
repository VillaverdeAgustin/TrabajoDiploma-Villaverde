-- 10 - Stored procedures de compras - 575_AV

USE [ConectAR_DB];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- Proveedor (CUN-007)
IF OBJECT_ID('dbo.SP_ExtProveedor', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtProveedor];
GO
CREATE PROCEDURE [dbo].[SP_ExtProveedor]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_proveedor, razon_social, cuit, condicion_iva, rubro, telefono,
           correo_electronico, direccion, activo, dvh
    FROM Proveedor
    ORDER BY id_proveedor;
END
GO

IF OBJECT_ID('dbo.SP_BuscarProveedorPorCuit', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_BuscarProveedorPorCuit];
GO
CREATE PROCEDURE [dbo].[SP_BuscarProveedorPorCuit]
    @cuit VARCHAR(13)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_proveedor, razon_social, cuit, condicion_iva, rubro, telefono,
           correo_electronico, direccion, activo, dvh
    FROM Proveedor
    WHERE cuit = @cuit;
END
GO

IF OBJECT_ID('dbo.SP_CrearProveedor', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_CrearProveedor];
GO
CREATE PROCEDURE [dbo].[SP_CrearProveedor]
(
    @razon_social       VARCHAR(150),
    @cuit               VARCHAR(13),
    @condicion_iva      VARCHAR(30),
    @rubro              VARCHAR(60)  = NULL,
    @telefono           VARCHAR(30)  = NULL,
    @correo_electronico VARCHAR(256) = NULL,
    @direccion          VARCHAR(150) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Proveedor WHERE cuit = @cuit)
    BEGIN
        RAISERROR('CUIT duplicado.', 16, 1);
        RETURN;
    END

    INSERT INTO Proveedor
        (razon_social, cuit, condicion_iva, rubro, telefono, correo_electronico, direccion, activo, dvh)
    VALUES
        (@razon_social, @cuit, @condicion_iva, @rubro, @telefono, @correo_electronico, @direccion, 1, NULL);

    SELECT SCOPE_IDENTITY();
END
GO

IF OBJECT_ID('dbo.SP_ActualizarProveedor', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ActualizarProveedor];
GO
CREATE PROCEDURE [dbo].[SP_ActualizarProveedor]
(
    @id_proveedor       INT,
    @razon_social       VARCHAR(150),
    @cuit               VARCHAR(13),
    @condicion_iva      VARCHAR(30),
    @rubro              VARCHAR(60)  = NULL,
    @telefono           VARCHAR(30)  = NULL,
    @correo_electronico VARCHAR(256) = NULL,
    @direccion          VARCHAR(150) = NULL,
    @activo             BIT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Proveedor WHERE cuit = @cuit AND id_proveedor <> @id_proveedor)
    BEGIN
        RAISERROR('CUIT duplicado.', 16, 1);
        RETURN;
    END

    UPDATE Proveedor
    SET razon_social = @razon_social, cuit = @cuit, condicion_iva = @condicion_iva,
        rubro = @rubro, telefono = @telefono, correo_electronico = @correo_electronico,
        direccion = @direccion, activo = @activo
    WHERE id_proveedor = @id_proveedor;
END
GO

-- Lista de precios (CUN-008 y CUN-009)
IF OBJECT_ID('dbo.SP_CrearListaPrecio', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_CrearListaPrecio];
GO
CREATE PROCEDURE [dbo].[SP_CrearListaPrecio]
(
    @id_proveedor        INT,
    @archivo             VARCHAR(260),
    @fecha_vigencia      DATE,
    @moneda              VARCHAR(3),
    @fecha_importacion   DATETIME,
    @usuario_importacion VARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO ListaPrecio
        (id_proveedor, archivo, fecha_vigencia, moneda, fecha_importacion, usuario_importacion, activo, dvh)
    VALUES
        (@id_proveedor, @archivo, @fecha_vigencia, @moneda, @fecha_importacion, @usuario_importacion, 1, NULL);

    SELECT SCOPE_IDENTITY();
END
GO

-- Deja sin efecto una lista cuya importacion no se completo
IF OBJECT_ID('dbo.SP_DesactivarListaPrecio', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_DesactivarListaPrecio];
GO
CREATE PROCEDURE [dbo].[SP_DesactivarListaPrecio]
    @id_lista_precio INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE ListaPrecio SET activo = 0 WHERE id_lista_precio = @id_lista_precio;
END
GO

IF OBJECT_ID('dbo.SP_AgregarItemListaPrecio', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_AgregarItemListaPrecio];
GO
CREATE PROCEDURE [dbo].[SP_AgregarItemListaPrecio]
(
    @id_lista_precio  INT,
    @id_articulo      INT           = NULL,
    @codigo_proveedor VARCHAR(25),
    @descripcion      VARCHAR(200)  = NULL,
    @precio_lista     DECIMAL(12,2),
    @unidad_venta     VARCHAR(30)   = NULL,
    @plazo_entrega    INT           = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO ItemListaPrecio
        (id_lista_precio, id_articulo, codigo_proveedor, descripcion, precio_lista,
         unidad_venta, plazo_entrega, dvh)
    VALUES
        (@id_lista_precio, @id_articulo, @codigo_proveedor, @descripcion, @precio_lista,
         @unidad_venta, @plazo_entrega, NULL);

    SELECT SCOPE_IDENTITY();
END
GO

IF OBJECT_ID('dbo.SP_ExtListaPrecio', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtListaPrecio];
GO
CREATE PROCEDURE [dbo].[SP_ExtListaPrecio]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_lista_precio, id_proveedor, archivo, fecha_vigencia, moneda,
           fecha_importacion, usuario_importacion, activo, dvh
    FROM ListaPrecio
    ORDER BY id_lista_precio;
END
GO

IF OBJECT_ID('dbo.SP_ExtItemListaPrecio', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtItemListaPrecio];
GO
CREATE PROCEDURE [dbo].[SP_ExtItemListaPrecio]
    @id_lista_precio INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT i.id_item_lista, i.id_lista_precio, i.id_articulo, i.codigo_proveedor, i.descripcion,
           i.precio_lista, i.unidad_venta, i.plazo_entrega, i.dvh
    FROM ItemListaPrecio i
    WHERE (@id_lista_precio IS NULL OR i.id_lista_precio = @id_lista_precio)
    ORDER BY i.id_item_lista;
END
GO

-- Precio vigente: la lista activa mas reciente del proveedor que incluye el articulo
IF OBJECT_ID('dbo.SP_BuscarPrecioVigente', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_BuscarPrecioVigente];
GO
CREATE PROCEDURE [dbo].[SP_BuscarPrecioVigente]
(
    @id_proveedor INT,
    @id_articulo  INT
)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 i.id_item_lista, i.id_lista_precio, i.id_articulo, i.codigo_proveedor, i.descripcion,
           i.precio_lista, i.unidad_venta, i.plazo_entrega, i.dvh
    FROM ItemListaPrecio i
         JOIN ListaPrecio l ON l.id_lista_precio = i.id_lista_precio
    WHERE l.id_proveedor = @id_proveedor
      AND i.id_articulo = @id_articulo
      AND l.activo = 1
      AND l.fecha_vigencia <= CAST(GETDATE() AS DATE)
    ORDER BY l.fecha_vigencia DESC, l.id_lista_precio DESC, i.id_item_lista DESC;
END
GO

-- Un precio por proveedor: el de su lista activa mas reciente en esa moneda que incluye el articulo
IF OBJECT_ID('dbo.SP_CompararPreciosArticulo', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_CompararPreciosArticulo];
GO
CREATE PROCEDURE [dbo].[SP_CompararPreciosArticulo]
(
    @id_articulo INT,
    @moneda      VARCHAR(3)
)
AS
BEGIN
    SET NOCOUNT ON;
    WITH vigentes AS (
        SELECT p.id_proveedor, p.razon_social, l.id_lista_precio, l.fecha_vigencia, l.moneda,
               i.id_item_lista, i.codigo_proveedor, i.descripcion, i.precio_lista,
               i.unidad_venta, i.plazo_entrega,
               ROW_NUMBER() OVER (PARTITION BY p.id_proveedor
                                  ORDER BY l.fecha_vigencia DESC, l.id_lista_precio DESC,
                                           i.id_item_lista DESC) AS orden
        FROM ItemListaPrecio i
             JOIN ListaPrecio l ON l.id_lista_precio = i.id_lista_precio
             JOIN Proveedor p   ON p.id_proveedor = l.id_proveedor
        WHERE i.id_articulo = @id_articulo
          AND l.moneda = @moneda
          AND l.activo = 1
          AND p.activo = 1
          AND l.fecha_vigencia <= CAST(GETDATE() AS DATE)
    )
    SELECT id_proveedor, razon_social, id_lista_precio, fecha_vigencia, moneda, id_item_lista,
           codigo_proveedor, descripcion, precio_lista, unidad_venta, plazo_entrega
    FROM vigentes
    WHERE orden = 1
    ORDER BY precio_lista, razon_social;
END
GO

-- Orden de compra (CUN-009 a CUN-011)
IF OBJECT_ID('dbo.SP_ProximoNumeroOrdenCompra', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ProximoNumeroOrdenCompra];
GO
CREATE PROCEDURE [dbo].[SP_ProximoNumeroOrdenCompra]
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @siguiente INT = ISNULL(
        (SELECT MAX(TRY_CONVERT(INT, RIGHT(nro_orden_compra, 6))) FROM OrdenCompra
         WHERE nro_orden_compra IS NOT NULL), 0) + 1;
    SELECT 'OC-' + RIGHT('000000' + CONVERT(VARCHAR(6), @siguiente), 6);
END
GO

IF OBJECT_ID('dbo.SP_BuscarOrdenCompraBorrador', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_BuscarOrdenCompraBorrador];
GO
CREATE PROCEDURE [dbo].[SP_BuscarOrdenCompraBorrador]
    @id_proveedor INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 o.id_orden_compra, o.nro_orden_compra, o.id_proveedor, o.fecha_emision, o.usuario_emisor,
           o.estado, o.total, o.fecha_autorizacion, o.usuario_autorizante, o.observaciones,
           o.activo, o.dvh, p.razon_social
    FROM OrdenCompra o
         JOIN Proveedor p ON p.id_proveedor = o.id_proveedor
    WHERE o.id_proveedor = @id_proveedor AND o.estado = 'Borrador' AND o.activo = 1
    ORDER BY o.id_orden_compra;
END
GO

IF OBJECT_ID('dbo.SP_CrearOrdenCompra', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_CrearOrdenCompra];
GO
CREATE PROCEDURE [dbo].[SP_CrearOrdenCompra]
(
    @id_proveedor   INT,
    @usuario_emisor VARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO OrdenCompra
        (nro_orden_compra, id_proveedor, fecha_emision, usuario_emisor, estado, total,
         fecha_autorizacion, usuario_autorizante, observaciones, activo, dvh)
    VALUES
        (NULL, @id_proveedor, NULL, @usuario_emisor, 'Borrador', 0, NULL, NULL, NULL, 1, NULL);

    SELECT SCOPE_IDENTITY();
END
GO

IF OBJECT_ID('dbo.SP_AgregarDetalleOrdenCompra', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_AgregarDetalleOrdenCompra];
GO
CREATE PROCEDURE [dbo].[SP_AgregarDetalleOrdenCompra]
(
    @id_orden_compra  INT,
    @id_articulo      INT,
    @cantidad         DECIMAL(12,2),
    @id_unidad_medida INT,
    @precio_unitario  DECIMAL(12,2),
    @subtotal         DECIMAL(14,2)
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO DetalleOrdenCompra
        (id_orden_compra, id_articulo, cantidad, id_unidad_medida, precio_unitario, subtotal,
         cantidad_recibida, dvh)
    VALUES
        (@id_orden_compra, @id_articulo, @cantidad, @id_unidad_medida, @precio_unitario, @subtotal,
         0, NULL);

    SELECT SCOPE_IDENTITY();
END
GO

IF OBJECT_ID('dbo.SP_ActualizarTotalOrdenCompra', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ActualizarTotalOrdenCompra];
GO
CREATE PROCEDURE [dbo].[SP_ActualizarTotalOrdenCompra]
(
    @id_orden_compra INT,
    @total           DECIMAL(14,2)
)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE OrdenCompra SET total = @total WHERE id_orden_compra = @id_orden_compra;
END
GO

IF OBJECT_ID('dbo.SP_EmitirOrdenCompra', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_EmitirOrdenCompra];
GO
CREATE PROCEDURE [dbo].[SP_EmitirOrdenCompra]
(
    @id_orden_compra  INT,
    @nro_orden_compra VARCHAR(15),
    @total            DECIMAL(14,2)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE OrdenCompra
    SET nro_orden_compra = @nro_orden_compra, total = @total,
        estado = 'PendienteAutorizacion', fecha_emision = GETDATE()
    WHERE id_orden_compra = @id_orden_compra AND estado = 'Borrador';

    IF @@ROWCOUNT = 0
        RAISERROR('La orden de compra no esta en borrador.', 16, 1);
END
GO

-- Estados separados por coma; sin estados devuelve todas
IF OBJECT_ID('dbo.SP_ExtOrdenCompraPorEstado', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtOrdenCompraPorEstado];
GO
CREATE PROCEDURE [dbo].[SP_ExtOrdenCompraPorEstado]
    @estados VARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT o.id_orden_compra, o.nro_orden_compra, o.id_proveedor, o.fecha_emision, o.usuario_emisor,
           o.estado, o.total, o.fecha_autorizacion, o.usuario_autorizante, o.observaciones,
           o.activo, o.dvh, p.razon_social
    FROM OrdenCompra o
         JOIN Proveedor p ON p.id_proveedor = o.id_proveedor
    WHERE @estados IS NULL
       OR o.estado IN (SELECT LTRIM(RTRIM(value)) FROM STRING_SPLIT(@estados, ','))
    ORDER BY o.id_orden_compra;
END
GO

IF OBJECT_ID('dbo.SP_ExtDetalleOrdenCompra', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtDetalleOrdenCompra];
GO
CREATE PROCEDURE [dbo].[SP_ExtDetalleOrdenCompra]
    @id_orden_compra INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_detalle_orden_compra, id_orden_compra, id_articulo, cantidad, id_unidad_medida,
           precio_unitario, subtotal, cantidad_recibida, dvh
    FROM DetalleOrdenCompra
    WHERE (@id_orden_compra IS NULL OR id_orden_compra = @id_orden_compra)
    ORDER BY id_detalle_orden_compra;
END
GO

IF OBJECT_ID('dbo.SP_RegistrarAutorizacionOrdenCompra', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_RegistrarAutorizacionOrdenCompra];
GO
CREATE PROCEDURE [dbo].[SP_RegistrarAutorizacionOrdenCompra]
(
    @id_orden_compra     INT,
    @estado              VARCHAR(25),
    @usuario_autorizante VARCHAR(50),
    @observaciones       VARCHAR(300) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @estado NOT IN ('Autorizada', 'Rechazada')
    BEGIN
        RAISERROR('Estado de autorizacion invalido.', 16, 1);
        RETURN;
    END

    UPDATE OrdenCompra
    SET estado = @estado, usuario_autorizante = @usuario_autorizante,
        fecha_autorizacion = GETDATE(), observaciones = @observaciones
    WHERE id_orden_compra = @id_orden_compra AND estado = 'PendienteAutorizacion';

    IF @@ROWCOUNT = 0
        RAISERROR('La orden de compra no esta pendiente de autorizacion.', 16, 1);
END
GO

-- Cantidad recibida acumulada de un renglon de la orden
IF OBJECT_ID('dbo.SP_ActualizarRecibidoOrdenCompra', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ActualizarRecibidoOrdenCompra];
GO
CREATE PROCEDURE [dbo].[SP_ActualizarRecibidoOrdenCompra]
(
    @id_detalle_orden_compra INT,
    @cantidad_recibida       DECIMAL(12,2)
)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE DetalleOrdenCompra
    SET cantidad_recibida = @cantidad_recibida
    WHERE id_detalle_orden_compra = @id_detalle_orden_compra;
END
GO

IF OBJECT_ID('dbo.SP_ActualizarEstadoOrdenCompra', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ActualizarEstadoOrdenCompra];
GO
CREATE PROCEDURE [dbo].[SP_ActualizarEstadoOrdenCompra]
(
    @id_orden_compra INT,
    @estado          VARCHAR(25)
)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE OrdenCompra SET estado = @estado WHERE id_orden_compra = @id_orden_compra;
END
GO

-- Recepcion de mercaderia (CUN-012)
IF OBJECT_ID('dbo.SP_CrearRecepcion', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_CrearRecepcion];
GO
CREATE PROCEDURE [dbo].[SP_CrearRecepcion]
(
    @id_orden_compra  INT,
    @remito_proveedor VARCHAR(30),
    @fecha_recepcion  DATETIME,
    @usuario_deposito VARCHAR(50),
    @diferencias      VARCHAR(300) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Recepcion
        (id_orden_compra, remito_proveedor, fecha_recepcion, usuario_deposito, diferencias, dvh)
    VALUES
        (@id_orden_compra, @remito_proveedor, @fecha_recepcion, @usuario_deposito, @diferencias, NULL);

    SELECT SCOPE_IDENTITY();
END
GO

IF OBJECT_ID('dbo.SP_AgregarDetalleRecepcion', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_AgregarDetalleRecepcion];
GO
CREATE PROCEDURE [dbo].[SP_AgregarDetalleRecepcion]
(
    @id_recepcion      INT,
    @id_articulo       INT,
    @cantidad_recibida DECIMAL(12,2),
    @id_unidad_medida  INT
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO DetalleRecepcion
        (id_recepcion, id_articulo, cantidad_recibida, id_unidad_medida, dvh)
    VALUES
        (@id_recepcion, @id_articulo, @cantidad_recibida, @id_unidad_medida, NULL);

    SELECT SCOPE_IDENTITY();
END
GO

IF OBJECT_ID('dbo.SP_ExtRecepcion', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtRecepcion];
GO
CREATE PROCEDURE [dbo].[SP_ExtRecepcion]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_recepcion, id_orden_compra, remito_proveedor, fecha_recepcion, usuario_deposito,
           diferencias, dvh
    FROM Recepcion
    ORDER BY id_recepcion;
END
GO

IF OBJECT_ID('dbo.SP_ExtDetalleRecepcion', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtDetalleRecepcion];
GO
CREATE PROCEDURE [dbo].[SP_ExtDetalleRecepcion]
    @id_recepcion INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_detalle_recepcion, id_recepcion, id_articulo, cantidad_recibida, id_unidad_medida, dvh
    FROM DetalleRecepcion
    WHERE (@id_recepcion IS NULL OR id_recepcion = @id_recepcion)
    ORDER BY id_detalle_recepcion;
END
GO

-- A02 Reporte de compras (solo lectura): una fila por orden emitida en el periodo, con lo pedido y lo recibido.
-- La moneda de cada renglon es la de la lista de precios de la que salio su precio: la ultima lista del
-- proveedor, vigente a la fecha de emision, que tiene el articulo a ese mismo precio. Un precio cargado a mano es ARS.
IF OBJECT_ID('dbo.SP_ReporteCompras', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ReporteCompras];
GO
CREATE PROCEDURE [dbo].[SP_ReporteCompras]
(
    @desde        DATE,
    @hasta        DATE,
    @id_proveedor INT         = NULL,
    @estado       VARCHAR(25) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT o.id_orden_compra, o.nro_orden_compra, o.id_proveedor, p.razon_social, o.fecha_emision,
           o.estado, o.total, r.renglones, r.cantidad_solicitada, r.cantidad_recibida, r.monto_ars, r.monto_usd
    FROM OrdenCompra o
         JOIN Proveedor p ON p.id_proveedor = o.id_proveedor
         CROSS APPLY (
             SELECT COUNT(*)                                     AS renglones,
                    ISNULL(SUM(d.cantidad), 0)                   AS cantidad_solicitada,
                    ISNULL(SUM(d.cantidad_recibida), 0)          AS cantidad_recibida,
                    ISNULL(SUM(CASE WHEN m.moneda = 'USD' THEN 0 ELSE d.subtotal END), 0) AS monto_ars,
                    ISNULL(SUM(CASE WHEN m.moneda = 'USD' THEN d.subtotal ELSE 0 END), 0) AS monto_usd
             FROM DetalleOrdenCompra d
                  OUTER APPLY (
                      SELECT TOP 1 l.moneda
                      FROM ItemListaPrecio i
                           JOIN ListaPrecio l ON l.id_lista_precio = i.id_lista_precio
                      WHERE l.id_proveedor = o.id_proveedor
                        AND i.id_articulo = d.id_articulo
                        AND i.precio_lista = d.precio_unitario
                        AND l.fecha_vigencia <= CAST(o.fecha_emision AS DATE)
                      ORDER BY l.fecha_vigencia DESC, l.id_lista_precio DESC, i.id_item_lista DESC
                  ) m
             WHERE d.id_orden_compra = o.id_orden_compra
         ) r
    WHERE o.fecha_emision IS NOT NULL
      AND o.activo = 1
      AND CAST(o.fecha_emision AS DATE) BETWEEN @desde AND @hasta
      AND (@id_proveedor IS NULL OR o.id_proveedor = @id_proveedor)
      AND (@estado IS NULL OR o.estado = @estado)
    ORDER BY p.razon_social, o.fecha_emision, o.id_orden_compra;
END
GO

PRINT '10 - Stored procedures de compras creados.';
GO
