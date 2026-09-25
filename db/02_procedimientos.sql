-- =============================================================================
-- ConectAR S.R.L. - Trabajo de Diploma - Villaverde, Agustin (575_AV)
-- 02 - Stored procedures transversales
--
-- Toda persistencia pasa por stored procedures y consultas parametrizadas.
-- Las firmas de cada SP estan atadas a los parametros que arma la DAL
-- (Acceso_DAL/MP_*.cs, IdiomaDAL.cs, PerfilDAL.cs): si se cambia un nombre
-- de parametro aca, hay que cambiarlo tambien alli.
--
-- Se usa el patron DROP + CREATE en lugar de CREATE OR ALTER: es
-- idempotente igual, pero portable a versiones de SQL Server anteriores
-- a 2016 SP1, que es lo que necesita el instalador de D01.
--
-- Los tres SPs de nombre dinamico (SP_ExtTabla, SP_ExtraerDVH, SP_ExtraerDVV)
-- validan el nombre de tabla contra sys.tables y lo escapan con QUOTENAME
-- antes de concatenarlo, para no dejar una via de SQL Injection abierta.
-- =============================================================================

USE [ConectAR_DB];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================================================
-- Genericos
-- =============================================================================

IF OBJECT_ID('dbo.SP_ExtTabla', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtTabla];
GO
CREATE PROCEDURE [dbo].[SP_ExtTabla]
    @tabla NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = @tabla)
    BEGIN
        RAISERROR('Tabla inexistente.', 16, 1);
        RETURN;
    END

    DECLARE @Query NVARCHAR(MAX) = N'SELECT * FROM ' + QUOTENAME(@tabla);
    EXEC sp_executesql @Query;
END
GO

-- =============================================================================
-- T02 - Usuarios
-- =============================================================================

-- LoginResult (Entidad_BE/LoginResult.cs):
--   0 = usuario inexistente | 1 = OK | 2 = bloqueado
--   3 = contrasena incorrecta | 4 = inactivo
IF OBJECT_ID('dbo.SP_Login', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_Login];
GO
CREATE PROCEDURE [dbo].[SP_Login]
(
    @user   NVARCHAR(50),
    @pass   NVARCHAR(65),
    @Result INT OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Activo BIT, @Bloqueado BIT;

    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Usuario = @user)
    BEGIN
        SET @Result = 0;
        RETURN;
    END

    SELECT @Activo = Activo, @Bloqueado = Bloqueado
    FROM Usuarios WHERE Usuario = @user;

    IF @Activo = 0        SET @Result = 4;
    ELSE IF @Bloqueado = 1 SET @Result = 2;
    ELSE IF EXISTS (SELECT 1 FROM Usuarios WHERE Usuario = @user AND [Contraseña] = @pass)
                          SET @Result = 1;
    ELSE                  SET @Result = 3;
END
GO

IF OBJECT_ID('dbo.SP_ExtUser', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtUser];
GO
CREATE PROCEDURE [dbo].[SP_ExtUser]
    @user NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Usuarios WHERE Usuario = @user;
END
GO

-- Alta de usuario con registro en la tabla espejo.
-- Devuelve el Codigo generado: la BLL lo necesita para calcular el DVH
-- y persistirlo enseguida con SP_ActualizarUs.
IF OBJECT_ID('dbo.SP_CrearUsuario', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_CrearUsuario];
GO
CREATE PROCEDURE [dbo].[SP_CrearUsuario]
(
    @DNI                INT,
    @Nombre             VARCHAR(50),
    @Apellido           NVARCHAR(50),
    @Usuario            NVARCHAR(50),
    @Rol                NVARCHAR(50),
    @Contraseña         NVARCHAR(65),
    @Direccion          NVARCHAR(50),
    @Telefono           NVARCHAR(50),
    @Email              NVARCHAR(50),
    @Activo             BIT,
    @Bloqueado          BIT,
    @UsuarioResponsable NVARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Usuarios
        (DNI, Nombre, Apellido, Usuario, Rol, [Contraseña], Direccion, Telefono, Email, Activo, Bloqueado, DVH)
    VALUES
        (@DNI, @Nombre, @Apellido, @Usuario, @Rol, @Contraseña, @Direccion, @Telefono, @Email, @Activo, @Bloqueado, NULL);

    DECLARE @NuevoCodigo INT = SCOPE_IDENTITY();

    -- TipoAccion.AltaUsuario = 3
    INSERT INTO HistorialUsuario
        (UsuarioId, DNI, Nombre, Apellido, Usuario, Rol, Direccion, Telefono, Email,
         Activo, Bloqueado, Accion, UsuarioResponsable, Fecha)
    VALUES
        (@NuevoCodigo, CONVERT(NVARCHAR(50), @DNI), @Nombre, @Apellido, @Usuario, @Rol,
         @Direccion, @Telefono, @Email, @Activo, @Bloqueado, 3, @UsuarioResponsable, GETDATE());

    SELECT @NuevoCodigo;
END
GO

-- Actualizacion sin historial. La BLL la usa para persistir el DVH recalculado,
-- que no es un cambio de datos del usuario y no debe ensuciar la tabla espejo.
IF OBJECT_ID('dbo.SP_ActualizarUs', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ActualizarUs];
GO
CREATE PROCEDURE [dbo].[SP_ActualizarUs]
(
    @Codigo    INT,
    @DNI       NVARCHAR(50),
    @Nombre    NVARCHAR(100),
    @Apellido  NVARCHAR(100),
    @Usuario   NVARCHAR(50),
    @Rol       NVARCHAR(50),
    @Direccion NVARCHAR(200),
    @Telefono  NVARCHAR(15),
    @Email     NVARCHAR(100),
    @Activo    BIT,
    @Bloqueado BIT,
    @DVH       NVARCHAR(65)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Usuarios
    SET DNI       = @DNI,
        Nombre    = @Nombre,
        Apellido  = @Apellido,
        Usuario   = @Usuario,
        Rol       = @Rol,
        Direccion = @Direccion,
        Telefono  = @Telefono,
        Email     = @Email,
        Activo    = @Activo,
        Bloqueado = @Bloqueado,
        DVH       = @DVH
    WHERE Codigo = @Codigo;
END
GO

-- Modificacion de usuario dejando constancia en la tabla espejo (T06.B).
IF OBJECT_ID('dbo.SP_ActualizarUsuarioConHistorial', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ActualizarUsuarioConHistorial];
GO
CREATE PROCEDURE [dbo].[SP_ActualizarUsuarioConHistorial]
(
    @Codigo             INT,
    @DNI                NVARCHAR(50),
    @Nombre             NVARCHAR(100),
    @Apellido           NVARCHAR(100),
    @Usuario            NVARCHAR(50),
    @Rol                NVARCHAR(50),
    @Direccion          NVARCHAR(200),
    @Telefono           NVARCHAR(15),
    @Email              NVARCHAR(100),
    @Activo             BIT,
    @Bloqueado          BIT,
    @DVH                NVARCHAR(65),
    @UsuarioResponsable NVARCHAR(50),
    @Accion             INT
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Usuarios
    SET DNI       = @DNI,
        Nombre    = @Nombre,
        Apellido  = @Apellido,
        Usuario   = @Usuario,
        Rol       = @Rol,
        Direccion = @Direccion,
        Telefono  = @Telefono,
        Email     = @Email,
        Activo    = @Activo,
        Bloqueado = @Bloqueado,
        DVH       = @DVH
    WHERE Codigo = @Codigo;

    INSERT INTO HistorialUsuario
        (UsuarioId, DNI, Nombre, Apellido, Usuario, Rol, Direccion, Telefono, Email,
         Activo, Bloqueado, Accion, UsuarioResponsable, Fecha)
    VALUES
        (@Codigo, @DNI, @Nombre, @Apellido, @Usuario, @Rol, @Direccion, @Telefono, @Email,
         @Activo, @Bloqueado, @Accion, @UsuarioResponsable, GETDATE());
END
GO

-- Baja logica: el usuario nunca se borra fisicamente, se desactiva.
IF OBJECT_ID('dbo.SP_ElimUsuario', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ElimUsuario];
GO
CREATE PROCEDURE [dbo].[SP_ElimUsuario]
(
    @Usuario            VARCHAR(50),
    @DVH                NVARCHAR(65),
    @UsuarioResponsable NVARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Usuarios
    SET Activo = 0, DVH = @DVH
    WHERE Usuario = @Usuario;

    -- TipoAccion.BajaUsuario = 4
    INSERT INTO HistorialUsuario
        (UsuarioId, DNI, Nombre, Apellido, Usuario, Rol, Direccion, Telefono, Email,
         Activo, Bloqueado, Accion, UsuarioResponsable, Fecha)
    SELECT Codigo, CONVERT(NVARCHAR(50), DNI), Nombre, Apellido, Usuario, Rol,
           Direccion, Telefono, Email, Activo, Bloqueado, 4, @UsuarioResponsable, GETDATE()
    FROM Usuarios
    WHERE Usuario = @Usuario;
END
GO

IF OBJECT_ID('dbo.SP_CambiarPass', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_CambiarPass];
GO
CREATE PROCEDURE [dbo].[SP_CambiarPass]
(
    @Usuario VARCHAR(50),
    @pass    VARCHAR(65),
    @DVH     NVARCHAR(65)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Usuarios
    SET [Contraseña] = @pass, DVH = @DVH
    WHERE Usuario = @Usuario;
END
GO

-- Bloqueo por 3 intentos fallidos y desbloqueo manual (T02).
IF OBJECT_ID('dbo.SP_ActualizarBloqueado', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ActualizarBloqueado];
GO
CREATE PROCEDURE [dbo].[SP_ActualizarBloqueado]
(
    @Usuario   VARCHAR(50),
    @pass      VARCHAR(65),
    @Bloqueado BIT,
    @DVH       NVARCHAR(65)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Usuarios
    SET Bloqueado = @Bloqueado, [Contraseña] = @pass, DVH = @DVH
    WHERE Usuario = @Usuario;
END
GO

-- =============================================================================
-- T06.B - Historial de usuario
-- =============================================================================

IF OBJECT_ID('dbo.SP_ListarHistorialUsuario', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ListarHistorialUsuario];
GO
CREATE PROCEDURE [dbo].[SP_ListarHistorialUsuario]
(
    @accion     INT      = NULL,
    @fechaDesde DATETIME = NULL,
    @fechaHasta DATETIME = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT IdHistorial, UsuarioId, DNI, Nombre, Apellido, Usuario, Rol,
           Direccion, Telefono, Email, Activo, Bloqueado, Accion,
           UsuarioResponsable, Fecha
    FROM HistorialUsuario
    WHERE (@accion     IS NULL OR Accion = @accion)
      AND (@fechaDesde IS NULL OR Fecha >= @fechaDesde)
      AND (@fechaHasta IS NULL OR Fecha <= @fechaHasta)
    ORDER BY Fecha DESC;
END
GO

IF OBJECT_ID('dbo.SP_ExtHistorialUsuario', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtHistorialUsuario];
GO
CREATE PROCEDURE [dbo].[SP_ExtHistorialUsuario]
    @idHistorial INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT IdHistorial, UsuarioId, DNI, Nombre, Apellido, Usuario, Rol,
           Direccion, Telefono, Email, Activo, Bloqueado, Accion,
           UsuarioResponsable, Fecha
    FROM HistorialUsuario
    WHERE IdHistorial = @idHistorial;
END
GO

-- =============================================================================
-- T06 - Bitacora de eventos
-- =============================================================================

IF OBJECT_ID('dbo.SP_ExtBitacora', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtBitacora];
GO
CREATE PROCEDURE [dbo].[SP_ExtBitacora]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Registro, Usuario, Accion, Fecha
    FROM Bitacora
    ORDER BY Fecha DESC;
END
GO

IF OBJECT_ID('dbo.SP_RegistrarEvento', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_RegistrarEvento];
GO
CREATE PROCEDURE [dbo].[SP_RegistrarEvento]
(
    @usuario NVARCHAR(50),
    @accion  INT,
    @fecha   DATETIME
)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Bitacora (Usuario, Accion, Fecha)
    VALUES (@usuario, @accion, @fecha);
END
GO

IF OBJECT_ID('dbo.SP_BuscarBitacora', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_BuscarBitacora];
GO
CREATE PROCEDURE [dbo].[SP_BuscarBitacora]
(
    @Usuario    VARCHAR(50) = NULL,
    @Accion     INT         = NULL,
    @FechaDesde DATETIME    = NULL,
    @FechaHasta DATETIME    = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Registro, Usuario, Accion, Fecha
    FROM Bitacora
    WHERE (@Usuario    IS NULL OR Usuario LIKE '%' + @Usuario + '%')
      AND (@Accion     IS NULL OR Accion = @Accion)
      AND (@FechaDesde IS NULL OR Fecha >= @FechaDesde)
      AND (@FechaHasta IS NULL OR Fecha <= @FechaHasta)
    ORDER BY Fecha DESC;
END
GO

-- =============================================================================
-- T08 - Digito verificador
-- =============================================================================

IF OBJECT_ID('dbo.SP_ExtraerDVH', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtraerDVH];
GO
CREATE PROCEDURE [dbo].[SP_ExtraerDVH]
    @tabla NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = @tabla)
    BEGIN
        RAISERROR('Tabla inexistente.', 16, 1);
        RETURN;
    END

    -- El orden debe coincidir con el OrderBy de VerificadorIntegridadBLL: el
    -- DVV se calcula sobre la secuencia de DVH. Se ordena por la clave primaria
    -- de la tabla, que en Usuarios es Codigo y en las tablas de negocio es su
    -- propio IdXxx. Se deduce de sys en lugar de fijarla, para que sirva igual
    -- en las tablas transversales y en las de negocio.
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
        N'SELECT DVH FROM ' + QUOTENAME(@tabla) + N' ORDER BY ' + QUOTENAME(@clave);
    EXEC sp_executesql @Query;
END
GO

IF OBJECT_ID('dbo.SP_ExtraerDVV', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtraerDVV];
GO
CREATE PROCEDURE [dbo].[SP_ExtraerDVV]
(
    @tabla      NVARCHAR(50),
    @tablaVerif NVARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = @tabla)
    BEGIN
        RAISERROR('Tabla inexistente.', 16, 1);
        RETURN;
    END

    DECLARE @Query NVARCHAR(MAX) =
        N'SELECT DVV FROM ' + QUOTENAME(@tabla) + N' WHERE Tabla = @tablaVerif';
    EXEC sp_executesql @Query, N'@tablaVerif NVARCHAR(50)', @tablaVerif;
END
GO

IF OBJECT_ID('dbo.SP_ActualizarDVV', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ActualizarDVV];
GO
CREATE PROCEDURE [dbo].[SP_ActualizarDVV]
(
    @tabla NVARCHAR(50),
    @dvv   NVARCHAR(65)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM DigitoVertical WHERE Tabla = @tabla)
        UPDATE DigitoVertical SET DVV = @dvv WHERE Tabla = @tabla;
    ELSE
        INSERT INTO DigitoVertical (Tabla, DVV) VALUES (@tabla, @dvv);
END
GO

-- =============================================================================
-- T05 - Idiomas
-- =============================================================================

IF OBJECT_ID('dbo.SP_ExtIdiomas', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtIdiomas];
GO
CREATE PROCEDURE [dbo].[SP_ExtIdiomas]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Codigo, Nombre FROM Idioma ORDER BY Nombre;
END
GO

IF OBJECT_ID('dbo.SP_ExtTraducciones', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtTraducciones];
GO
CREATE PROCEDURE [dbo].[SP_ExtTraducciones]
    @IdIdioma INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Clave, Texto FROM Traduccion WHERE IdIdioma = @IdIdioma;
END
GO

-- Un idioma nuevo nace clonando el diccionario de un idioma base, para que
-- ninguna clave quede sin traducir mientras se lo completa.
IF OBJECT_ID('dbo.SP_CrearIdioma', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_CrearIdioma];
GO
CREATE PROCEDURE [dbo].[SP_CrearIdioma]
(
    @Codigo     NVARCHAR(5),
    @Nombre     NVARCHAR(50),
    @CodigoBase NVARCHAR(5)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IdBase INT = (SELECT Id FROM Idioma WHERE Codigo = @CodigoBase);
    DECLARE @IdNuevo INT;

    INSERT INTO Idioma (Codigo, Nombre) VALUES (@Codigo, @Nombre);
    SET @IdNuevo = SCOPE_IDENTITY();

    INSERT INTO Traduccion (IdIdioma, Clave, Texto)
    SELECT @IdNuevo, Clave, Texto
    FROM Traduccion
    WHERE IdIdioma = @IdBase;
END
GO

IF OBJECT_ID('dbo.SP_ActualizarTraduccion', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ActualizarTraduccion];
GO
CREATE PROCEDURE [dbo].[SP_ActualizarTraduccion]
(
    @IdIdioma INT,
    @Clave    NVARCHAR(100),
    @Texto    NVARCHAR(300)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Traduccion
    SET Texto = @Texto
    WHERE IdIdioma = @IdIdioma AND Clave = @Clave;
END
GO

PRINT '02 - Stored procedures transversales creados.';
GO
