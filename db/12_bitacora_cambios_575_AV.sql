-- 12 - Bitacora de cambios de articulos (T06.B) - 575_AV

USE [ConectAR_DB];
GO

SET NOCOUNT ON;
GO

IF COL_LENGTH('dbo.Articulo', 'usuario_modificacion') IS NULL
    ALTER TABLE [dbo].[Articulo] ADD [usuario_modificacion] [varchar](50) COLLATE Modern_Spanish_CI_AS NULL;
GO

IF OBJECT_ID('dbo.SEQ_ArticuloCambio_575_AV', 'SO') IS NULL
    CREATE SEQUENCE [dbo].[SEQ_ArticuloCambio_575_AV] AS int START WITH 1 INCREMENT BY 1;
GO

IF OBJECT_ID('dbo.ArticuloHistorial_575_AV', 'U') IS NULL
CREATE TABLE [dbo].[ArticuloHistorial_575_AV](
    [id_historial]         [int] IDENTITY(1,1) NOT NULL,
    [id_cambio]            [int]          NOT NULL,
    [id_articulo]          [int]          NOT NULL,
    [imagen]               [char](1)      NOT NULL,
    [operacion]            [varchar](15)  COLLATE Modern_Spanish_CI_AS NOT NULL,
    [codigo]               [varchar](25)  COLLATE Modern_Spanish_CI_AS NOT NULL,
    [descripcion]          [varchar](200) COLLATE Modern_Spanish_CI_AS NOT NULL,
    [id_categoria]         [int]          NOT NULL,
    [id_marca]             [int]          NOT NULL,
    [id_unidad_medida]     [int]          NOT NULL,
    [precio_unitario]      [decimal](12,2) NOT NULL,
    [cantidad_disponible]  [decimal](12,2) NOT NULL,
    [punto_reposicion]     [decimal](12,2) NOT NULL,
    [deposito_ubicacion]   [varchar](30)  COLLATE Modern_Spanish_CI_AS NULL,
    [estado_articulo]      [varchar](15)  COLLATE Modern_Spanish_CI_AS NOT NULL,
    [activo]               [bit]          NOT NULL,
    [usuario_modificacion] [varchar](50)  COLLATE Modern_Spanish_CI_AS NULL,
    [fecha]                [datetime]     NOT NULL CONSTRAINT [DF_ArticuloHistorial_fecha] DEFAULT (GETDATE()),
    [dvh]                  [varchar](64)  COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_ArticuloHistorial_575_AV] PRIMARY KEY CLUSTERED ([id_historial] ASC),
    CONSTRAINT [CK_ArticuloHistorial_imagen] CHECK ([imagen] IN ('A', 'P')),
    CONSTRAINT [CK_ArticuloHistorial_operacion]
        CHECK ([operacion] IN ('Alta', 'Modificacion', 'Baja', 'Restauracion'))
) ON [PRIMARY];
GO

IF OBJECT_ID('dbo.FK_ArticuloHistorial_Articulo', 'F') IS NULL
    ALTER TABLE [dbo].[ArticuloHistorial_575_AV] WITH CHECK ADD CONSTRAINT [FK_ArticuloHistorial_Articulo]
        FOREIGN KEY([id_articulo]) REFERENCES [dbo].[Articulo]([id_articulo]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ArticuloHistorial_articulo_fecha')
    CREATE INDEX [IX_ArticuloHistorial_articulo_fecha]
        ON [dbo].[ArticuloHistorial_575_AV] ([id_articulo], [fecha]);
GO

-- Triggers de Articulo
CREATE OR ALTER TRIGGER [dbo].[TR_Articulo_Insert_575_AV] ON [dbo].[Articulo]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO ArticuloHistorial_575_AV
        (id_cambio, id_articulo, imagen, operacion, codigo, descripcion, id_categoria,
         id_marca, id_unidad_medida, precio_unitario, cantidad_disponible, punto_reposicion,
         deposito_ubicacion, estado_articulo, activo, usuario_modificacion)
    SELECT NEXT VALUE FOR SEQ_ArticuloCambio_575_AV, i.id_articulo, 'P', 'Alta',
           i.codigo, i.descripcion, i.id_categoria, i.id_marca, i.id_unidad_medida,
           i.precio_unitario, i.cantidad_disponible, i.punto_reposicion,
           i.deposito_ubicacion, i.estado_articulo, i.activo, i.usuario_modificacion
    FROM inserted i;
END
GO

CREATE OR ALTER TRIGGER [dbo].[TR_Articulo_Update_575_AV] ON [dbo].[Articulo]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT (UPDATE(codigo) OR UPDATE(descripcion) OR UPDATE(id_categoria) OR UPDATE(id_marca)
            OR UPDATE(id_unidad_medida) OR UPDATE(precio_unitario) OR UPDATE(punto_reposicion)
            OR UPDATE(deposito_ubicacion) OR UPDATE(activo))
        RETURN;

    DECLARE @restauracion bit =
        CASE WHEN CAST(SESSION_CONTEXT(N'operacion_575_AV') AS varchar(15)) = 'Restauracion'
             THEN 1 ELSE 0 END;

    CREATE TABLE #cambios (id_articulo int, operacion varchar(15), id_cambio int);

    SELECT i.id_articulo,
           CASE WHEN @restauracion = 1 THEN 'Restauracion'
                WHEN d.activo = 1 AND i.activo = 0 THEN 'Baja'
                ELSE 'Modificacion' END AS operacion
    INTO #filas
    FROM inserted i
    JOIN deleted d ON d.id_articulo = i.id_articulo
    WHERE EXISTS (SELECT i.codigo, i.descripcion, i.id_categoria, i.id_marca, i.id_unidad_medida,
                         i.precio_unitario, i.punto_reposicion, i.deposito_ubicacion, i.activo
                  EXCEPT
                  SELECT d.codigo, d.descripcion, d.id_categoria, d.id_marca, d.id_unidad_medida,
                         d.precio_unitario, d.punto_reposicion, d.deposito_ubicacion, d.activo);

    INSERT INTO #cambios (id_articulo, operacion, id_cambio)
    SELECT id_articulo, operacion, NEXT VALUE FOR SEQ_ArticuloCambio_575_AV
    FROM #filas;

    INSERT INTO ArticuloHistorial_575_AV
        (id_cambio, id_articulo, imagen, operacion, codigo, descripcion, id_categoria,
         id_marca, id_unidad_medida, precio_unitario, cantidad_disponible, punto_reposicion,
         deposito_ubicacion, estado_articulo, activo, usuario_modificacion)
    SELECT c.id_cambio, d.id_articulo, 'A', c.operacion,
           d.codigo, d.descripcion, d.id_categoria, d.id_marca, d.id_unidad_medida,
           d.precio_unitario, d.cantidad_disponible, d.punto_reposicion,
           d.deposito_ubicacion, d.estado_articulo, d.activo, d.usuario_modificacion
    FROM deleted d JOIN #cambios c ON c.id_articulo = d.id_articulo
    UNION ALL
    SELECT c.id_cambio, i.id_articulo, 'P', c.operacion,
           i.codigo, i.descripcion, i.id_categoria, i.id_marca, i.id_unidad_medida,
           i.precio_unitario, i.cantidad_disponible, i.punto_reposicion,
           i.deposito_ubicacion, i.estado_articulo, i.activo, i.usuario_modificacion
    FROM inserted i JOIN #cambios c ON c.id_articulo = i.id_articulo;
END
GO

CREATE OR ALTER TRIGGER [dbo].[TR_Articulo_Delete_575_AV] ON [dbo].[Articulo]
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE a SET a.activo = 0
    FROM Articulo a JOIN deleted d ON d.id_articulo = a.id_articulo
    WHERE a.activo = 1;
END
GO

-- Alta, modificacion y baja de articulos
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
    @estado_articulo     VARCHAR(15) = 'Disponible',
    @usuario             VARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Articulo
        (codigo, descripcion, id_categoria, id_marca, id_unidad_medida,
         precio_unitario, cantidad_disponible, punto_reposicion,
         deposito_ubicacion, estado_articulo, activo, usuario_modificacion, dvh)
    VALUES
        (@codigo, @descripcion, @id_categoria, @id_marca, @id_unidad_medida,
         @precio_unitario, @cantidad_disponible, @punto_reposicion,
         @deposito_ubicacion, @estado_articulo, 1, @usuario, NULL);

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
    @activo              BIT,
    @usuario             VARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Articulo
    SET codigo               = @codigo,
        descripcion          = @descripcion,
        id_categoria         = @id_categoria,
        id_marca             = @id_marca,
        id_unidad_medida     = @id_unidad_medida,
        precio_unitario      = @precio_unitario,
        cantidad_disponible  = @cantidad_disponible,
        punto_reposicion     = @punto_reposicion,
        deposito_ubicacion   = @deposito_ubicacion,
        estado_articulo      = @estado_articulo,
        activo               = @activo,
        usuario_modificacion = @usuario
    WHERE id_articulo = @id_articulo;
END
GO

IF OBJECT_ID('dbo.SP_ElimArticulo', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ElimArticulo];
GO
CREATE PROCEDURE [dbo].[SP_ElimArticulo]
(
    @id_articulo INT,
    @usuario     VARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Articulo SET activo = 0, usuario_modificacion = @usuario WHERE id_articulo = @id_articulo;
END
GO

-- Bitacora de cambios
IF OBJECT_ID('dbo.SP_ExtArticuloHistorial_575_AV', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtArticuloHistorial_575_AV];
GO
CREATE PROCEDURE [dbo].[SP_ExtArticuloHistorial_575_AV]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_historial, id_cambio, id_articulo, imagen, operacion, codigo, descripcion, id_categoria,
           id_marca, id_unidad_medida, precio_unitario, cantidad_disponible, punto_reposicion,
           deposito_ubicacion, estado_articulo, activo, usuario_modificacion, fecha, dvh
    FROM ArticuloHistorial_575_AV
    ORDER BY id_historial;
END
GO

IF OBJECT_ID('dbo.SP_BuscarArticuloHistorial_575_AV', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_BuscarArticuloHistorial_575_AV];
GO
CREATE PROCEDURE [dbo].[SP_BuscarArticuloHistorial_575_AV]
(
    @id_articulo INT         = NULL,
    @operacion   VARCHAR(15) = NULL,
    @desde       DATE,
    @hasta       DATE
)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_historial, id_cambio, id_articulo, imagen, operacion, codigo, descripcion, id_categoria,
           id_marca, id_unidad_medida, precio_unitario, cantidad_disponible, punto_reposicion,
           deposito_ubicacion, estado_articulo, activo, usuario_modificacion, fecha, dvh
    FROM ArticuloHistorial_575_AV
    WHERE imagen = 'P'
      AND (@id_articulo IS NULL OR id_articulo = @id_articulo)
      AND (@operacion IS NULL OR operacion = @operacion)
      AND fecha >= @desde
      AND fecha < DATEADD(DAY, 1, CAST(@hasta AS DATETIME))
    ORDER BY fecha DESC, id_cambio DESC;
END
GO

IF OBJECT_ID('dbo.SP_ExtCambioArticulo_575_AV', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtCambioArticulo_575_AV];
GO
CREATE PROCEDURE [dbo].[SP_ExtCambioArticulo_575_AV]
    @id_cambio INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_historial, id_cambio, id_articulo, imagen, operacion, codigo, descripcion, id_categoria,
           id_marca, id_unidad_medida, precio_unitario, cantidad_disponible, punto_reposicion,
           deposito_ubicacion, estado_articulo, activo, usuario_modificacion, fecha, dvh
    FROM ArticuloHistorial_575_AV
    WHERE id_cambio = @id_cambio
    ORDER BY imagen;
END
GO

IF OBJECT_ID('dbo.SP_ExtArticuloHistorialSinDVH_575_AV', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ExtArticuloHistorialSinDVH_575_AV];
GO
CREATE PROCEDURE [dbo].[SP_ExtArticuloHistorialSinDVH_575_AV]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_historial, id_cambio, id_articulo, imagen, operacion, codigo, descripcion, id_categoria,
           id_marca, id_unidad_medida, precio_unitario, cantidad_disponible, punto_reposicion,
           deposito_ubicacion, estado_articulo, activo, usuario_modificacion, fecha, dvh
    FROM ArticuloHistorial_575_AV
    WHERE dvh IS NULL
    ORDER BY id_historial;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_RestaurarArticulo_575_AV]
(
    @id_historial INT,
    @usuario      VARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;
    EXEC sp_set_session_context @key = N'operacion_575_AV', @value = 'Restauracion';

    UPDATE a
    SET a.codigo               = h.codigo,
        a.descripcion          = h.descripcion,
        a.id_categoria         = h.id_categoria,
        a.id_marca             = h.id_marca,
        a.id_unidad_medida     = h.id_unidad_medida,
        a.precio_unitario      = h.precio_unitario,
        a.punto_reposicion     = h.punto_reposicion,
        a.deposito_ubicacion   = h.deposito_ubicacion,
        a.activo               = h.activo,
        a.usuario_modificacion = @usuario
    FROM Articulo a
    JOIN ArticuloHistorial_575_AV h ON h.id_articulo = a.id_articulo
    WHERE h.id_historial = @id_historial;

    EXEC sp_set_session_context @key = N'operacion_575_AV', @value = NULL;
END
GO

-- Digito vertical de la bitacora de cambios
MERGE INTO [dbo].[DigitoVertical] AS destino
USING (VALUES
    (N'ArticuloHistorial_575_AV', N'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855')
) AS origen ([Tabla], [DVV])
    ON destino.[Tabla] = origen.[Tabla]
WHEN NOT MATCHED THEN
    INSERT ([Tabla], [DVV]) VALUES (origen.[Tabla], origen.[DVV]);
GO

PRINT '12 - Bitacora de cambios de articulos creada.';
GO
