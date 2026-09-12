-- =============================================================================
-- ConectAR S.R.L. - Trabajo de Diploma - Villaverde, Agustin (575_AV)
-- 05 - Esquema de negocio, fase 1: catalogos, producto y bobina (RFN1)
--
-- Cubre los catalogos del dominio, el producto y el circuito de bobina, que
-- es la particularidad del rubro: los articulos fraccionables se venden por
-- metro lineal a partir de una caja o rollo, y hay que llevar el saldo
-- remanente de cada bobina abierta. Por eso StockActual es decimal y no int.
--
-- Toda tabla de negocio lleva DVH y se controla por DVV (T08), y toda alta,
-- modificacion y baja se registra en bitacora desde la BLL (T06).
--
-- El usuario es un servicio, no una entidad de negocio: ninguna tabla de aca
-- lleva FK a Usuarios.
--
-- Orden de ejecucion: 01 -> 02 -> 03 -> 04 -> 05 -> 06
-- =============================================================================

USE [ConectAR_DB];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- -----------------------------------------------------------------------------
-- Catalogos
-- -----------------------------------------------------------------------------
IF OBJECT_ID('dbo.Categoria', 'U') IS NULL
CREATE TABLE [dbo].[Categoria](
    [IdCategoria] [int] IDENTITY(1,1) NOT NULL,
    [Nombre]      [varchar](50)  COLLATE Modern_Spanish_CI_AS NOT NULL,
    [Activo]      [bit]          NOT NULL CONSTRAINT [DF_Categoria_Activo] DEFAULT (1),
    [DVH]         [nvarchar](65) COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_Categoria] PRIMARY KEY CLUSTERED ([IdCategoria] ASC),
    CONSTRAINT [UQ_Categoria_Nombre] UNIQUE ([Nombre])
) ON [PRIMARY];
GO

IF OBJECT_ID('dbo.Marca', 'U') IS NULL
CREATE TABLE [dbo].[Marca](
    [IdMarca] [int] IDENTITY(1,1) NOT NULL,
    [Nombre]  [varchar](50)  COLLATE Modern_Spanish_CI_AS NOT NULL,
    [Activo]  [bit]          NOT NULL CONSTRAINT [DF_Marca_Activo] DEFAULT (1),
    [DVH]     [nvarchar](65) COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_Marca] PRIMARY KEY CLUSTERED ([IdMarca] ASC),
    CONSTRAINT [UQ_Marca_Nombre] UNIQUE ([Nombre])
) ON [PRIMARY];
GO

-- Fraccionable habilita el circuito de bobina: "Unidad" va en 0, "Metro" en 1.
IF OBJECT_ID('dbo.UnidadMedida', 'U') IS NULL
CREATE TABLE [dbo].[UnidadMedida](
    [IdUnidadMedida] [int] IDENTITY(1,1) NOT NULL,
    [Nombre]         [varchar](30)  COLLATE Modern_Spanish_CI_AS NOT NULL,
    [Abreviatura]    [varchar](5)   COLLATE Modern_Spanish_CI_AS NOT NULL,
    [Fraccionable]   [bit]          NOT NULL CONSTRAINT [DF_UnidadMedida_Fraccionable] DEFAULT (0),
    [DVH]            [nvarchar](65) COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_UnidadMedida] PRIMARY KEY CLUSTERED ([IdUnidadMedida] ASC),
    CONSTRAINT [UQ_UnidadMedida_Nombre] UNIQUE ([Nombre])
) ON [PRIMARY];
GO

IF OBJECT_ID('dbo.MedioPago', 'U') IS NULL
CREATE TABLE [dbo].[MedioPago](
    [IdMedioPago]          [int] IDENTITY(1,1) NOT NULL,
    [Nombre]               [varchar](50)  COLLATE Modern_Spanish_CI_AS NOT NULL,
    [RequiereAutorizacion] [bit]          NOT NULL CONSTRAINT [DF_MedioPago_RequiereAut] DEFAULT (0),
    [Activo]               [bit]          NOT NULL CONSTRAINT [DF_MedioPago_Activo] DEFAULT (1),
    [DVH]                  [nvarchar](65) COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_MedioPago] PRIMARY KEY CLUSTERED ([IdMedioPago] ASC),
    CONSTRAINT [UQ_MedioPago_Nombre] UNIQUE ([Nombre])
) ON [PRIMARY];
GO

-- -----------------------------------------------------------------------------
-- Producto. StockActual y PuntoReposicion son decimal, no int: un articulo
-- fraccionable tiene N bobinas y su stock es la suma de los saldos.
-- -----------------------------------------------------------------------------
IF OBJECT_ID('dbo.Producto', 'U') IS NULL
CREATE TABLE [dbo].[Producto](
    [IdProducto]      [int] IDENTITY(1,1) NOT NULL,
    [Codigo]          [varchar](25)  COLLATE Modern_Spanish_CI_AS NOT NULL,
    [Descripcion]     [varchar](200) COLLATE Modern_Spanish_CI_AS NOT NULL,
    [IdCategoria]     [int]          NOT NULL,
    [IdMarca]         [int]          NOT NULL,
    [IdUnidadMedida]  [int]          NOT NULL,
    [PrecioUnitario]  [decimal](12,2) NOT NULL CONSTRAINT [DF_Producto_Precio] DEFAULT (0),
    [StockActual]     [decimal](12,2) NOT NULL CONSTRAINT [DF_Producto_Stock] DEFAULT (0),
    [PuntoReposicion] [decimal](12,2) NOT NULL CONSTRAINT [DF_Producto_PtoRep] DEFAULT (0),
    [Activo]          [bit]          NOT NULL CONSTRAINT [DF_Producto_Activo] DEFAULT (1),
    [DVH]             [nvarchar](65) COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_Producto] PRIMARY KEY CLUSTERED ([IdProducto] ASC),
    CONSTRAINT [UQ_Producto_Codigo] UNIQUE ([Codigo])
) ON [PRIMARY];
GO

-- -----------------------------------------------------------------------------
-- Bobina. Identificador es la etiqueta fisica de la bobina o caja.
-- Estado: Cerrada (sin abrir) / Abierta (con saldo) / Agotada (saldo 0).
-- -----------------------------------------------------------------------------
IF OBJECT_ID('dbo.Bobina', 'U') IS NULL
CREATE TABLE [dbo].[Bobina](
    [IdBobina]      [int] IDENTITY(1,1) NOT NULL,
    [IdProducto]    [int]          NOT NULL,
    [Identificador] [varchar](25)  COLLATE Modern_Spanish_CI_AS NOT NULL,
    [MedidaInicial] [decimal](12,2) NOT NULL,
    [SaldoActual]   [decimal](12,2) NOT NULL,
    [FechaApertura] [datetime]     NULL,
    [Estado]        [varchar](15)  COLLATE Modern_Spanish_CI_AS NOT NULL CONSTRAINT [DF_Bobina_Estado] DEFAULT ('Cerrada'),
    [DVH]           [nvarchar](65) COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_Bobina] PRIMARY KEY CLUSTERED ([IdBobina] ASC),
    CONSTRAINT [UQ_Bobina_Identificador] UNIQUE ([Identificador]),
    CONSTRAINT [CK_Bobina_Estado] CHECK ([Estado] IN ('Cerrada', 'Abierta', 'Agotada')),
    CONSTRAINT [CK_Bobina_Saldo] CHECK ([SaldoActual] >= 0 AND [SaldoActual] <= [MedidaInicial])
) ON [PRIMARY];
GO

-- -----------------------------------------------------------------------------
-- Integridad referencial
-- -----------------------------------------------------------------------------
IF OBJECT_ID('dbo.FK_Producto_Categoria', 'F') IS NULL
    ALTER TABLE [dbo].[Producto] WITH CHECK ADD CONSTRAINT [FK_Producto_Categoria]
        FOREIGN KEY([IdCategoria]) REFERENCES [dbo].[Categoria]([IdCategoria]);
GO

IF OBJECT_ID('dbo.FK_Producto_Marca', 'F') IS NULL
    ALTER TABLE [dbo].[Producto] WITH CHECK ADD CONSTRAINT [FK_Producto_Marca]
        FOREIGN KEY([IdMarca]) REFERENCES [dbo].[Marca]([IdMarca]);
GO

IF OBJECT_ID('dbo.FK_Producto_UnidadMedida', 'F') IS NULL
    ALTER TABLE [dbo].[Producto] WITH CHECK ADD CONSTRAINT [FK_Producto_UnidadMedida]
        FOREIGN KEY([IdUnidadMedida]) REFERENCES [dbo].[UnidadMedida]([IdUnidadMedida]);
GO

IF OBJECT_ID('dbo.FK_Bobina_Producto', 'F') IS NULL
    ALTER TABLE [dbo].[Bobina] WITH CHECK ADD CONSTRAINT [FK_Bobina_Producto]
        FOREIGN KEY([IdProducto]) REFERENCES [dbo].[Producto]([IdProducto]);
GO

-- -----------------------------------------------------------------------------
-- Datos minimos de catalogo. Idempotente: solo inserta lo que falta.
-- El DVH queda en NULL y se calcula desde Recalcular Digitos (CUS-012).
-- -----------------------------------------------------------------------------
INSERT INTO [dbo].[UnidadMedida] ([Nombre], [Abreviatura], [Fraccionable])
SELECT v.[Nombre], v.[Abreviatura], v.[Fraccionable]
FROM (VALUES
    ('Unidad',       'u',   CONVERT(BIT, 0)),
    ('Metro lineal', 'm',   CONVERT(BIT, 1)),
    ('Caja 305 m',   'cj',  CONVERT(BIT, 1)),
    ('Rollo',        'rll', CONVERT(BIT, 1))
) AS v([Nombre], [Abreviatura], [Fraccionable])
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[UnidadMedida] u WHERE u.[Nombre] = v.[Nombre]);
GO

INSERT INTO [dbo].[Categoria] ([Nombre])
SELECT v.[Nombre]
FROM (VALUES
    ('Cable de red'), ('Fibra optica'), ('Conectividad'),
    ('Racks y gabinetes'), ('Canalizacion'), ('Herramientas'), ('Instrumental')
) AS v([Nombre])
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Categoria] c WHERE c.[Nombre] = v.[Nombre]);
GO

INSERT INTO [dbo].[Marca] ([Nombre])
SELECT v.[Nombre]
FROM (VALUES
    ('Furukawa'), ('Panduit'), ('Belden'), ('Commscope'),
    ('Legrand'), ('Fluke Networks'), ('Nexxt Solutions'), ('Generico')
) AS v([Nombre])
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Marca] m WHERE m.[Nombre] = v.[Nombre]);
GO

INSERT INTO [dbo].[MedioPago] ([Nombre], [RequiereAutorizacion])
SELECT v.[Nombre], v.[RequiereAutorizacion]
FROM (VALUES
    ('Efectivo',        CONVERT(BIT, 0)),
    ('Transferencia',   CONVERT(BIT, 0)),
    ('Tarjeta debito',  CONVERT(BIT, 1)),
    ('Tarjeta credito', CONVERT(BIT, 1)),
    ('Cuenta corriente',CONVERT(BIT, 1))
) AS v([Nombre], [RequiereAutorizacion])
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[MedioPago] m WHERE m.[Nombre] = v.[Nombre]);
GO

PRINT '05 - Esquema de negocio fase 1 (catalogos, producto y bobina) creado.';
GO
