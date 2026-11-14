-- 03 - Datos de seguridad: perfiles y administrador - 575_AV

USE [ConectAR_DB];
GO

SET NOCOUNT ON;
GO

-- T04 - Arbol de perfiles (Composite)

MERGE INTO Permiso AS destino
USING (VALUES
    (N'Admin',                N'Compuesto', 1),
    (N'Ventas',               N'Compuesto', 1),
    (N'Caja',                 N'Compuesto', 1),
    (N'Deposito',             N'Compuesto', 1),
    (N'AdministradorCompras', N'Compuesto', 1),
    (N'Propietario',          N'Compuesto', 1)
) AS origen (Nombre, Tipo, EsRol)
    ON destino.Nombre_Permiso = origen.Nombre
WHEN NOT MATCHED THEN
    INSERT (Nombre_Permiso, Tipo, Rol) VALUES (origen.Nombre, origen.Tipo, origen.EsRol);
GO

MERGE INTO Permiso AS destino
USING (VALUES
    (N'CompletarCarrito'),
    (N'GenerarOrdenPago'),
    (N'RegistrarCliente'),
    (N'ValidarPedidoYCobrar'),
    (N'EntregarArticulos'),
    (N'GestionArticulos'),
    (N'GestionUsuarios'),
    (N'GestionPerfiles'),
    (N'GestionBackup'),
    (N'GestionBitacora'),
    (N'RegistrarProveedor'),
    (N'ImportarListaPrecios'),
    (N'CompararPrecios'),
    (N'GenerarOrdenCompra'),
    (N'AutorizarOrdenCompra'),
    (N'RegistrarRecepcion'),
    (N'ReporteCompras'),
    (N'SerializarOrdenCompra')
) AS origen (Nombre)
    ON destino.Nombre_Permiso = origen.Nombre
WHEN NOT MATCHED THEN
    INSERT (Nombre_Permiso, Tipo, Rol) VALUES (origen.Nombre, N'Simple', 0);
GO

-- Asignacion de cada permiso a su familia
UPDATE Permiso SET Nombre_PermisoPadre = N'Admin'
WHERE Nombre_Permiso IN (N'GestionUsuarios', N'GestionPerfiles', N'GestionBackup', N'GestionBitacora');

UPDATE Permiso SET Nombre_PermisoPadre = N'Ventas'
WHERE Nombre_Permiso IN (N'CompletarCarrito', N'GenerarOrdenPago');

UPDATE Permiso SET Nombre_PermisoPadre = N'Caja'
WHERE Nombre_Permiso IN (N'ValidarPedidoYCobrar', N'RegistrarCliente');

UPDATE Permiso SET Nombre_PermisoPadre = N'Deposito'
WHERE Nombre_Permiso IN (N'EntregarArticulos', N'GestionArticulos', N'RegistrarRecepcion');

UPDATE Permiso SET Nombre_PermisoPadre = N'AdministradorCompras'
WHERE Nombre_Permiso IN (N'RegistrarProveedor', N'ImportarListaPrecios', N'CompararPrecios', N'GenerarOrdenCompra',
                         N'ReporteCompras', N'SerializarOrdenCompra');

UPDATE Permiso SET Nombre_PermisoPadre = N'Propietario'
WHERE Nombre_Permiso IN (N'AutorizarOrdenCompra');
GO

UPDATE Permiso SET Nombre_PermisoPadre = N'Admin'
WHERE Nombre_Permiso IN (N'Ventas', N'Caja', N'Deposito', N'AdministradorCompras', N'Propietario');
GO

-- Limpieza del mapa de casos de uso anterior.
DECLARE @obsoletos TABLE (Nombre NVARCHAR(100));
INSERT INTO @obsoletos (Nombre) VALUES
    (N'LlenarCarrito'), (N'SeleccionarProducto'), (N'GenerarFactura'),
    (N'GenerarReserva'), (N'RealizarCobro'), (N'GestionProductos'),
    (N'Vendedor'), (N'Cajero'), (N'EncargadoDeposito');

UPDATE Permiso SET Nombre_PermisoPadre = NULL
WHERE Nombre_PermisoPadre IN (SELECT Nombre FROM @obsoletos);

DELETE FROM Permiso
WHERE Nombre_Permiso IN (SELECT Nombre FROM @obsoletos)
  AND NOT EXISTS (SELECT 1 FROM Usuarios u WHERE u.Rol = Permiso.Nombre_Permiso);
GO

-- T02 / T03 / T08 - Usuario administrador inicial
IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Usuario = 'admin')
BEGIN
    SET IDENTITY_INSERT Usuarios ON;

    INSERT INTO Usuarios
        (Codigo, DNI, Nombre, Apellido, Usuario, Rol, [Contraseña],
         Direccion, Telefono, Email, Activo, Bloqueado, DVH)
    VALUES
        (1, 40000000, 'Agustin', 'Villaverde', 'admin', N'Admin',
         '60fe74406e7f353ed979f350f2fbb6a2e8690a5fa7d1b0c32983d1d8b3f95f67',
         'Sin especificar', '1100000000', 'admin@conectar.com.ar', 1, 0,
         N'6103c1653f1b1422d0f3a3955c4fd5c03229f20da7ead00be0e70f80de605a25');

    SET IDENTITY_INSERT Usuarios OFF;
END
GO

-- DVV de la tabla Usuarios
IF EXISTS (SELECT 1 FROM DigitoVertical WHERE Tabla = N'Usuarios')
    UPDATE DigitoVertical
    SET DVV = N'1e2d3b7a678a2833cd8bfb51d1c981c78cf8253d41c60294c103f564f772d9e4'
    WHERE Tabla = N'Usuarios';
ELSE
    INSERT INTO DigitoVertical (Tabla, DVV)
    VALUES (N'Usuarios', N'1e2d3b7a678a2833cd8bfb51d1c981c78cf8253d41c60294c103f564f772d9e4');
GO

PRINT '03 - Perfiles y usuario administrador cargados.';
GO
