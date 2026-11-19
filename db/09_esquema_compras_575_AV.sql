-- 09 - Esquema de compras - 575_AV

USE [ConectAR_DB];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- Proveedor
IF OBJECT_ID('dbo.Proveedor', 'U') IS NULL
CREATE TABLE [dbo].[Proveedor](
    [id_proveedor]       [int] IDENTITY(1,1) NOT NULL,
    [razon_social]       [varchar](150) COLLATE Modern_Spanish_CI_AS NOT NULL,
    [cuit]               [varchar](13)  COLLATE Modern_Spanish_CI_AS NOT NULL,
    [condicion_iva]      [varchar](30)  COLLATE Modern_Spanish_CI_AS NOT NULL,
    [rubro]              [varchar](60)  COLLATE Modern_Spanish_CI_AS NULL,
    [telefono]           [varchar](30)  COLLATE Modern_Spanish_CI_AS NULL,
    [correo_electronico] [varchar](256) COLLATE Modern_Spanish_CI_AS NULL,
    [direccion]          [varchar](150) COLLATE Modern_Spanish_CI_AS NULL,
    [activo]             [bit]          NOT NULL CONSTRAINT [DF_Proveedor_activo] DEFAULT (1),
    [dvh]                [varchar](64)  COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_Proveedor] PRIMARY KEY CLUSTERED ([id_proveedor] ASC),
    CONSTRAINT [UQ_Proveedor_cuit] UNIQUE ([cuit])
) ON [PRIMARY];
GO

-- Lista de precios y sus renglones
IF OBJECT_ID('dbo.ListaPrecio', 'U') IS NULL
CREATE TABLE [dbo].[ListaPrecio](
    [id_lista_precio]     [int] IDENTITY(1,1) NOT NULL,
    [id_proveedor]        [int]          NOT NULL,
    [archivo]             [varchar](260) COLLATE Modern_Spanish_CI_AS NOT NULL,
    [fecha_vigencia]      [date]         NOT NULL,
    [moneda]              [varchar](3)   COLLATE Modern_Spanish_CI_AS NOT NULL
                          CONSTRAINT [DF_ListaPrecio_moneda] DEFAULT ('ARS'),
    [fecha_importacion]   [datetime]     NOT NULL,
    [usuario_importacion] [varchar](50)  COLLATE Modern_Spanish_CI_AS NOT NULL,
    [activo]              [bit]          NOT NULL CONSTRAINT [DF_ListaPrecio_activo] DEFAULT (1),
    [dvh]                 [varchar](64)  COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_ListaPrecio] PRIMARY KEY CLUSTERED ([id_lista_precio] ASC),
    CONSTRAINT [CK_ListaPrecio_moneda] CHECK ([moneda] IN ('ARS', 'USD'))
) ON [PRIMARY];
GO

IF OBJECT_ID('dbo.ItemListaPrecio', 'U') IS NULL
CREATE TABLE [dbo].[ItemListaPrecio](
    [id_item_lista]    [int] IDENTITY(1,1) NOT NULL,
    [id_lista_precio]  [int]           NOT NULL,
    [id_articulo]      [int]           NULL,
    [codigo_proveedor] [varchar](25)   COLLATE Modern_Spanish_CI_AS NOT NULL,
    [descripcion]      [varchar](200)  COLLATE Modern_Spanish_CI_AS NULL,
    [precio_lista]     [decimal](12,2) NOT NULL,
    [unidad_venta]     [varchar](30)   COLLATE Modern_Spanish_CI_AS NULL,
    [plazo_entrega]    [int]           NULL,
    [dvh]              [varchar](64)   COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_ItemListaPrecio] PRIMARY KEY CLUSTERED ([id_item_lista] ASC),
    CONSTRAINT [CK_ItemListaPrecio_precio] CHECK ([precio_lista] > 0),
    CONSTRAINT [CK_ItemListaPrecio_plazo] CHECK ([plazo_entrega] IS NULL OR [plazo_entrega] >= 0)
) ON [PRIMARY];
GO

-- Orden de compra y su detalle
IF OBJECT_ID('dbo.OrdenCompra', 'U') IS NULL
CREATE TABLE [dbo].[OrdenCompra](
    [id_orden_compra]     [int] IDENTITY(1,1) NOT NULL,
    [nro_orden_compra]    [varchar](15)   COLLATE Modern_Spanish_CI_AS NULL,
    [id_proveedor]        [int]           NOT NULL,
    [fecha_emision]       [datetime]      NULL,
    [usuario_emisor]      [varchar](50)   COLLATE Modern_Spanish_CI_AS NOT NULL,
    [estado]              [varchar](25)   COLLATE Modern_Spanish_CI_AS NOT NULL
                          CONSTRAINT [DF_OrdenCompra_estado] DEFAULT ('Borrador'),
    [total]               [decimal](14,2) NOT NULL CONSTRAINT [DF_OrdenCompra_total] DEFAULT (0),
    [observaciones]       [varchar](300)  COLLATE Modern_Spanish_CI_AS NULL,
    [activo]              [bit]           NOT NULL CONSTRAINT [DF_OrdenCompra_activo] DEFAULT (1),
    [dvh]                 [varchar](64)   COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_OrdenCompra] PRIMARY KEY CLUSTERED ([id_orden_compra] ASC),
    CONSTRAINT [CK_OrdenCompra_estado] CHECK ([estado] IN
        ('Borrador', 'Emitida', 'RecibidaParcial', 'Recibida'))
) ON [PRIMARY];
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_OrdenCompra_nro')
    CREATE UNIQUE NONCLUSTERED INDEX [UQ_OrdenCompra_nro]
        ON [dbo].[OrdenCompra]([nro_orden_compra]) WHERE [nro_orden_compra] IS NOT NULL;
GO

IF OBJECT_ID('dbo.DetalleOrdenCompra', 'U') IS NULL
CREATE TABLE [dbo].[DetalleOrdenCompra](
    [id_detalle_orden_compra] [int] IDENTITY(1,1) NOT NULL,
    [id_orden_compra]         [int]           NOT NULL,
    [id_articulo]             [int]           NOT NULL,
    [cantidad]                [decimal](12,2) NOT NULL,
    [id_unidad_medida]        [int]           NOT NULL,
    [precio_unitario]         [decimal](12,2) NOT NULL,
    [subtotal]                [decimal](14,2) NOT NULL,
    [cantidad_recibida]       [decimal](12,2) NOT NULL CONSTRAINT [DF_DetalleOrdenCompra_recibida] DEFAULT (0),
    [dvh]                     [varchar](64)   COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_DetalleOrdenCompra] PRIMARY KEY CLUSTERED ([id_detalle_orden_compra] ASC),
    CONSTRAINT [CK_DetalleOrdenCompra_cantidad] CHECK ([cantidad] > 0),
    CONSTRAINT [CK_DetalleOrdenCompra_recibida] CHECK ([cantidad_recibida] >= 0)
) ON [PRIMARY];
GO

-- Recepcion de mercaderia y su detalle
IF OBJECT_ID('dbo.Recepcion', 'U') IS NULL
CREATE TABLE [dbo].[Recepcion](
    [id_recepcion]     [int] IDENTITY(1,1) NOT NULL,
    [id_orden_compra]  [int]           NOT NULL,
    [remito_proveedor] [varchar](30)   COLLATE Modern_Spanish_CI_AS NOT NULL,
    [fecha_recepcion]  [datetime]      NOT NULL,
    [usuario_deposito] [varchar](50)   COLLATE Modern_Spanish_CI_AS NOT NULL,
    [diferencias]      [varchar](300)  COLLATE Modern_Spanish_CI_AS NULL,
    [dvh]              [varchar](64)   COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_Recepcion] PRIMARY KEY CLUSTERED ([id_recepcion] ASC)
) ON [PRIMARY];
GO

IF OBJECT_ID('dbo.DetalleRecepcion', 'U') IS NULL
CREATE TABLE [dbo].[DetalleRecepcion](
    [id_detalle_recepcion] [int] IDENTITY(1,1) NOT NULL,
    [id_recepcion]         [int]           NOT NULL,
    [id_articulo]          [int]           NOT NULL,
    [cantidad_recibida]    [decimal](12,2) NOT NULL,
    [id_unidad_medida]     [int]           NOT NULL,
    [dvh]                  [varchar](64)   COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_DetalleRecepcion] PRIMARY KEY CLUSTERED ([id_detalle_recepcion] ASC),
    CONSTRAINT [CK_DetalleRecepcion_cantidad] CHECK ([cantidad_recibida] > 0)
) ON [PRIMARY];
GO

-- Integridad referencial
IF OBJECT_ID('dbo.FK_ListaPrecio_Proveedor', 'F') IS NULL
    ALTER TABLE [dbo].[ListaPrecio] WITH CHECK ADD CONSTRAINT [FK_ListaPrecio_Proveedor]
        FOREIGN KEY([id_proveedor]) REFERENCES [dbo].[Proveedor]([id_proveedor]);
GO

IF OBJECT_ID('dbo.FK_ItemListaPrecio_ListaPrecio', 'F') IS NULL
    ALTER TABLE [dbo].[ItemListaPrecio] WITH CHECK ADD CONSTRAINT [FK_ItemListaPrecio_ListaPrecio]
        FOREIGN KEY([id_lista_precio]) REFERENCES [dbo].[ListaPrecio]([id_lista_precio]);
GO

IF OBJECT_ID('dbo.FK_ItemListaPrecio_Articulo', 'F') IS NULL
    ALTER TABLE [dbo].[ItemListaPrecio] WITH CHECK ADD CONSTRAINT [FK_ItemListaPrecio_Articulo]
        FOREIGN KEY([id_articulo]) REFERENCES [dbo].[Articulo]([id_articulo]);
GO

IF OBJECT_ID('dbo.FK_OrdenCompra_Proveedor', 'F') IS NULL
    ALTER TABLE [dbo].[OrdenCompra] WITH CHECK ADD CONSTRAINT [FK_OrdenCompra_Proveedor]
        FOREIGN KEY([id_proveedor]) REFERENCES [dbo].[Proveedor]([id_proveedor]);
GO

IF OBJECT_ID('dbo.FK_DetalleOrdenCompra_OrdenCompra', 'F') IS NULL
    ALTER TABLE [dbo].[DetalleOrdenCompra] WITH CHECK ADD CONSTRAINT [FK_DetalleOrdenCompra_OrdenCompra]
        FOREIGN KEY([id_orden_compra]) REFERENCES [dbo].[OrdenCompra]([id_orden_compra]);
GO

IF OBJECT_ID('dbo.FK_DetalleOrdenCompra_Articulo', 'F') IS NULL
    ALTER TABLE [dbo].[DetalleOrdenCompra] WITH CHECK ADD CONSTRAINT [FK_DetalleOrdenCompra_Articulo]
        FOREIGN KEY([id_articulo]) REFERENCES [dbo].[Articulo]([id_articulo]);
GO

IF OBJECT_ID('dbo.FK_DetalleOrdenCompra_UnidadMedida', 'F') IS NULL
    ALTER TABLE [dbo].[DetalleOrdenCompra] WITH CHECK ADD CONSTRAINT [FK_DetalleOrdenCompra_UnidadMedida]
        FOREIGN KEY([id_unidad_medida]) REFERENCES [dbo].[UnidadMedida]([id_unidad_medida]);
GO

IF OBJECT_ID('dbo.FK_Recepcion_OrdenCompra', 'F') IS NULL
    ALTER TABLE [dbo].[Recepcion] WITH CHECK ADD CONSTRAINT [FK_Recepcion_OrdenCompra]
        FOREIGN KEY([id_orden_compra]) REFERENCES [dbo].[OrdenCompra]([id_orden_compra]);
GO

IF OBJECT_ID('dbo.FK_DetalleRecepcion_Recepcion', 'F') IS NULL
    ALTER TABLE [dbo].[DetalleRecepcion] WITH CHECK ADD CONSTRAINT [FK_DetalleRecepcion_Recepcion]
        FOREIGN KEY([id_recepcion]) REFERENCES [dbo].[Recepcion]([id_recepcion]);
GO

IF OBJECT_ID('dbo.FK_DetalleRecepcion_Articulo', 'F') IS NULL
    ALTER TABLE [dbo].[DetalleRecepcion] WITH CHECK ADD CONSTRAINT [FK_DetalleRecepcion_Articulo]
        FOREIGN KEY([id_articulo]) REFERENCES [dbo].[Articulo]([id_articulo]);
GO

IF OBJECT_ID('dbo.FK_DetalleRecepcion_UnidadMedida', 'F') IS NULL
    ALTER TABLE [dbo].[DetalleRecepcion] WITH CHECK ADD CONSTRAINT [FK_DetalleRecepcion_UnidadMedida]
        FOREIGN KEY([id_unidad_medida]) REFERENCES [dbo].[UnidadMedida]([id_unidad_medida]);
GO

-- Digito vertical de las tablas de compras
MERGE INTO [dbo].[DigitoVertical] AS destino
USING (VALUES
    (N'Proveedor',           N'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855'),
    (N'ListaPrecio',         N'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855'),
    (N'ItemListaPrecio',     N'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855'),
    (N'OrdenCompra',         N'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855'),
    (N'DetalleOrdenCompra',  N'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855'),
    (N'Recepcion',           N'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855'),
    (N'DetalleRecepcion',    N'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855')
) AS origen ([Tabla], [DVV])
    ON destino.[Tabla] = origen.[Tabla]
WHEN NOT MATCHED THEN
    INSERT ([Tabla], [DVV]) VALUES (origen.[Tabla], origen.[DVV]);
GO

PRINT '09 - Esquema de compras creado.';
GO
