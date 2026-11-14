-- 01 - Esquema transversal - 575_AV

IF DB_ID('ConectAR_DB') IS NULL
    CREATE DATABASE [ConectAR_DB];
GO

USE [ConectAR_DB];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- T04 - Gestion de perfiles (patron Composite)
IF OBJECT_ID('dbo.Permiso', 'U') IS NULL
CREATE TABLE [dbo].[Permiso](
    [Nombre_Permiso]      [nvarchar](100) COLLATE Modern_Spanish_CI_AS NOT NULL,
    [Tipo]                [nvarchar](20)  COLLATE Modern_Spanish_CI_AS NULL,
    [Rol]                 [bit]           NOT NULL CONSTRAINT [DF_Permiso_Rol] DEFAULT (0),
    [Nombre_PermisoPadre] [nvarchar](100) COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_Permiso] PRIMARY KEY CLUSTERED ([Nombre_Permiso] ASC)
) ON [PRIMARY];
GO

-- T02 - Usuarios. Un usuario es un servicio, no una entidad de negocio:
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

-- T06 - Bitacora de eventos, escrita por el logger centralizado de Servicios.
IF OBJECT_ID('dbo.Bitacora', 'U') IS NULL
CREATE TABLE [dbo].[Bitacora](
    [Registro] [int] IDENTITY(1,1) NOT NULL,
    [Usuario]  [nvarchar](50) COLLATE Modern_Spanish_CI_AS NOT NULL,
    [Accion]   [int]          NOT NULL,
    [Fecha]    [datetime]     NULL,
    CONSTRAINT [PK_Bitacora] PRIMARY KEY CLUSTERED ([Registro] ASC)
) ON [PRIMARY];
GO

-- T08 - Digito verificador vertical: una fila por tabla controlada.
IF OBJECT_ID('dbo.DigitoVertical', 'U') IS NULL
CREATE TABLE [dbo].[DigitoVertical](
    [Tabla] [nvarchar](50) COLLATE Modern_Spanish_CI_AS NOT NULL,
    [DVV]   [nvarchar](65) COLLATE Modern_Spanish_CI_AS NOT NULL,
    CONSTRAINT [PK_DigitoVertical] PRIMARY KEY CLUSTERED ([Tabla] ASC)
) ON [PRIMARY];
GO

-- T05 - Cambio de idioma (patron Observer). Las traducciones viven en la BD,
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

-- Historial de usuario: tabla espejo del ABM de Usuarios, con el estado de la
IF OBJECT_ID('dbo.HistorialUsuario', 'U') IS NULL
CREATE TABLE [dbo].[HistorialUsuario](
    [IdHistorial]        [int] IDENTITY(1,1) NOT NULL,
    [UsuarioId]          [int]           NOT NULL,
    [DNI]                [nvarchar](50)  NULL,
    [Nombre]             [nvarchar](100) NULL,
    [Apellido]           [nvarchar](100) NULL,
    [Usuario]            [nvarchar](50)  NULL,
    [Rol]                [nvarchar](100) NULL,
    [Direccion]          [nvarchar](200) NULL,
    [Telefono]           [nvarchar](50)  NULL,
    [Email]              [nvarchar](100) NULL,
    [Activo]             [bit]           NOT NULL,
    [Bloqueado]          [bit]           NOT NULL,
    [Accion]             [int]           NOT NULL,
    [UsuarioResponsable] [nvarchar](50)  NULL,
    [Fecha]              [datetime]      NOT NULL,
    CONSTRAINT [PK_HistorialUsuario] PRIMARY KEY CLUSTERED ([IdHistorial] ASC)
) ON [PRIMARY];
GO

-- Integridad referencial
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
