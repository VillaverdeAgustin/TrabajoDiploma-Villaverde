-- =============================================================================
-- ConectAR S.R.L. - Trabajo de Diploma - Villaverde, Agustin (575_AV)
-- 01 - Esquema transversal (T01 a T08)
--
-- Crea la base ConectAR_DB y las tablas de los servicios transversales:
-- seguridad (Usuarios, Permiso), bitacora de eventos, digito verificador,
-- idiomas e historial de usuario.
--
-- Las tablas del dominio de negocio (RFN1 Ventas / RFN2 Compras) NO se
-- definen aqui: van en 05_esquema_negocio_575_AV.sql.
--
-- Orden de ejecucion: 01 -> 02 -> 03 -> 04
-- =============================================================================

IF DB_ID('ConectAR_DB') IS NULL
    CREATE DATABASE [ConectAR_DB];
GO

USE [ConectAR_DB];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- -----------------------------------------------------------------------------
-- T04 - Gestion de perfiles (patron Composite)
--   Tipo: 'Simple' = hoja (permiso) | 'Compuesto' = familia
--   Rol : 1 = la familia es un rol asignable a un usuario
-- La jerarquia se resuelve con una FK reflexiva sobre Nombre_PermisoPadre,
-- por eso un permiso pertenece a una sola familia.
-- -----------------------------------------------------------------------------
IF OBJECT_ID('dbo.Permiso', 'U') IS NULL
CREATE TABLE [dbo].[Permiso](
    [Nombre_Permiso]      [nvarchar](100) COLLATE Modern_Spanish_CI_AS NOT NULL,
    [Tipo]                [nvarchar](20)  COLLATE Modern_Spanish_CI_AS NULL,
    [Rol]                 [bit]           NOT NULL CONSTRAINT [DF_Permiso_Rol] DEFAULT (0),
    [Nombre_PermisoPadre] [nvarchar](100) COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_Permiso] PRIMARY KEY CLUSTERED ([Nombre_Permiso] ASC)
) ON [PRIMARY];
GO

-- -----------------------------------------------------------------------------
-- T02 - Usuarios. Un usuario es un servicio, no una entidad de negocio:
-- no participa del DER de negocio.
--   Contrasena : SHA-256 irreversible (T03)
--   DVH        : digito verificador horizontal de la fila (T08)
-- -----------------------------------------------------------------------------
IF OBJECT_ID('dbo.Usuarios', 'U') IS NULL
CREATE TABLE [dbo].[Usuarios](
    [Codigo]     [int] IDENTITY(1,1)     NOT NULL,
    [DNI]        [int]                   NOT NULL,
    [Nombre]     [varchar](50)  COLLATE Modern_Spanish_CI_AS NOT NULL,
    [Apellido]   [varchar](50)  COLLATE Modern_Spanish_CI_AS NOT NULL,
    [Usuario]    [varchar](50)  COLLATE Modern_Spanish_CI_AS NOT NULL,
    [Rol]        [nvarchar](100) COLLATE Modern_Spanish_CI_AS NULL,
    [Contraseña] [varchar](65)  COLLATE Modern_Spanish_CI_AS NULL,
    [Direccion]  [varchar](50)  COLLATE Modern_Spanish_CI_AS NULL,
    [Telefono]   [varchar](50)  COLLATE Modern_Spanish_CI_AS NULL,
    [Email]      [varchar](50)  COLLATE Modern_Spanish_CI_AS NULL,
    [Activo]     [bit]                   NOT NULL,
    [Bloqueado]  [bit]                   NOT NULL,
    [DVH]        [nvarchar](65) COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_Usuarios] PRIMARY KEY CLUSTERED ([Codigo] ASC),
    CONSTRAINT [UQ_Usuarios_Usuario] UNIQUE ([Usuario])
) ON [PRIMARY];
GO

-- -----------------------------------------------------------------------------
-- T06 - Bitacora de eventos, escrita por el logger centralizado de Servicios.
-- Accion referencia al enum Entidad_BE.TipoAccion.
-- -----------------------------------------------------------------------------
IF OBJECT_ID('dbo.Bitacora', 'U') IS NULL
CREATE TABLE [dbo].[Bitacora](
    [Registro] [int] IDENTITY(1,1) NOT NULL,
    [Usuario]  [nvarchar](50) COLLATE Modern_Spanish_CI_AS NOT NULL,
    [Accion]   [int]          NOT NULL,
    [Fecha]    [datetime]     NULL,
    CONSTRAINT [PK_Bitacora] PRIMARY KEY CLUSTERED ([Registro] ASC)
) ON [PRIMARY];
GO

-- -----------------------------------------------------------------------------
-- T08 - Digito verificador vertical: una fila por tabla controlada.
-- -----------------------------------------------------------------------------
IF OBJECT_ID('dbo.DigitoVertical', 'U') IS NULL
CREATE TABLE [dbo].[DigitoVertical](
    [Tabla] [nvarchar](50) COLLATE Modern_Spanish_CI_AS NOT NULL,
    [DVV]   [nvarchar](65) COLLATE Modern_Spanish_CI_AS NOT NULL,
    CONSTRAINT [PK_DigitoVertical] PRIMARY KEY CLUSTERED ([Tabla] ASC)
) ON [PRIMARY];
GO

-- -----------------------------------------------------------------------------
-- T05 - Cambio de idioma (patron Observer). Las traducciones viven en la BD,
-- no en archivos .resx: agregar un idioma es insertar filas, sin tocar codigo.
-- -----------------------------------------------------------------------------
IF OBJECT_ID('dbo.Idioma', 'U') IS NULL
CREATE TABLE [dbo].[Idioma](
    [Id]     [int] IDENTITY(1,1) NOT NULL,
    [Codigo] [nvarchar](5)  COLLATE Modern_Spanish_CI_AS NOT NULL,
    [Nombre] [nvarchar](50) COLLATE Modern_Spanish_CI_AS NOT NULL,
    CONSTRAINT [PK_Idioma] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_Idioma_Codigo] UNIQUE ([Codigo])
) ON [PRIMARY];
GO

IF OBJECT_ID('dbo.Traduccion', 'U') IS NULL
CREATE TABLE [dbo].[Traduccion](
    [IdIdioma] [int]           NOT NULL,
    [Clave]    [nvarchar](100) COLLATE Modern_Spanish_CI_AS NOT NULL,
    [Texto]    [nvarchar](300) COLLATE Modern_Spanish_CI_AS NOT NULL,
    CONSTRAINT [PK_Traduccion] PRIMARY KEY CLUSTERED ([IdIdioma] ASC, [Clave] ASC),
    CONSTRAINT [FK_Traduccion_Idioma] FOREIGN KEY([IdIdioma]) REFERENCES [dbo].[Idioma]([Id])
) ON [PRIMARY];
GO

-- -----------------------------------------------------------------------------
-- Historial de usuario: tabla espejo del ABM de Usuarios, con el estado de la
-- fila y quien la modifico.
--
-- El ORDEN de las columnas es significativo: Acceso_DAL/MP_HistorialUsuario.cs
-- mapea por indice posicional (MapearFila, dr[0]..dr[14]). No reordenar.
-- -----------------------------------------------------------------------------
IF OBJECT_ID('dbo.HistorialUsuario', 'U') IS NULL
CREATE TABLE [dbo].[HistorialUsuario](
    [IdHistorial]        [int] IDENTITY(1,1) NOT NULL,  --  0
    [UsuarioId]          [int]           NOT NULL,      --  1
    [DNI]                [nvarchar](50)  NULL,          --  2
    [Nombre]             [nvarchar](100) NULL,          --  3
    [Apellido]           [nvarchar](100) NULL,          --  4
    [Usuario]            [nvarchar](50)  NULL,          --  5
    [Rol]                [nvarchar](100) NULL,          --  6
    [Direccion]          [nvarchar](200) NULL,          --  7
    [Telefono]           [nvarchar](50)  NULL,          --  8
    [Email]              [nvarchar](100) NULL,          --  9
    [Activo]             [bit]           NOT NULL,      -- 10
    [Bloqueado]          [bit]           NOT NULL,      -- 11
    [Accion]             [int]           NOT NULL,      -- 12
    [UsuarioResponsable] [nvarchar](50)  NULL,          -- 13
    [Fecha]              [datetime]      NOT NULL,      -- 14
    CONSTRAINT [PK_HistorialUsuario] PRIMARY KEY CLUSTERED ([IdHistorial] ASC)
) ON [PRIMARY];
GO

-- -----------------------------------------------------------------------------
-- Integridad referencial
-- -----------------------------------------------------------------------------
IF OBJECT_ID('dbo.FK_Permiso_PermisoPadre', 'F') IS NULL
    ALTER TABLE [dbo].[Permiso] WITH CHECK ADD CONSTRAINT [FK_Permiso_PermisoPadre]
        FOREIGN KEY([Nombre_PermisoPadre]) REFERENCES [dbo].[Permiso]([Nombre_Permiso]);
GO

IF OBJECT_ID('dbo.FK_Usuarios_Permiso', 'F') IS NULL
    ALTER TABLE [dbo].[Usuarios] WITH CHECK ADD CONSTRAINT [FK_Usuarios_Permiso]
        FOREIGN KEY([Rol]) REFERENCES [dbo].[Permiso]([Nombre_Permiso]);
GO

IF OBJECT_ID('dbo.FK_HistorialUsuario_Usuarios', 'F') IS NULL
    ALTER TABLE [dbo].[HistorialUsuario] WITH CHECK ADD CONSTRAINT [FK_HistorialUsuario_Usuarios]
        FOREIGN KEY([UsuarioId]) REFERENCES [dbo].[Usuarios]([Codigo]);
GO

PRINT '01 - Esquema transversal creado sobre ConectAR_DB.';
GO
