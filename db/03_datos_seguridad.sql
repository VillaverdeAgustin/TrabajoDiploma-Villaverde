-- =============================================================================
-- ConectAR S.R.L. - Trabajo de Diploma - Villaverde, Agustin (575_AV)
-- 03 - Datos iniciales de seguridad: arbol de perfiles y usuario administrador
--
-- Idempotente: se puede reejecutar sin duplicar filas.
-- =============================================================================

USE [ConectAR_DB];
GO

SET NOCOUNT ON;
GO

-- -----------------------------------------------------------------------------
-- T04 - Arbol de perfiles (Composite)
--
--   Familias / roles (Tipo='Compuesto', Rol=1) -> raices del TreeView
--   Permisos         (Tipo='Simple',    Rol=0) -> hojas
--
-- Los permisos simples se corresponden uno a uno con el enum
-- Entidad_BE.TipoPermiso: agregar un valor alli obliga a insertarlo aca.
--
-- La jerarquia usa una FK reflexiva (Nombre_PermisoPadre), de modo que un
-- permiso cuelga de una sola familia.
-- -----------------------------------------------------------------------------

-- Roles del negocio, segun G04 - Roles del sistema
MERGE INTO Permiso AS destino
USING (VALUES
    (N'Admin',                N'Compuesto', 1),
    (N'Vendedor',             N'Compuesto', 1),
    (N'Cajero',               N'Compuesto', 1),
    (N'EncargadoDeposito',    N'Compuesto', 1),
    -- Roles de RFN2 (Compras). Quedan creados para completar el modelo de
    -- roles; sus permisos se agregan junto con los casos de uso CUN-007..013.
    (N'AdministradorCompras', N'Compuesto', 1),
    (N'Propietario',          N'Compuesto', 1)
) AS origen (Nombre, Tipo, EsRol)
    ON destino.Nombre_Permiso = origen.Nombre
WHEN NOT MATCHED THEN
    INSERT (Nombre_Permiso, Tipo, Rol) VALUES (origen.Nombre, origen.Tipo, origen.EsRol);
GO

-- Permisos simples
MERGE INTO Permiso AS destino
USING (VALUES
    (N'LlenarCarrito'),
    (N'SeleccionarProducto'),
    (N'GenerarFactura'),
    (N'GenerarReserva'),
    (N'RealizarCobro'),
    (N'RegistrarCliente'),
    (N'GestionUsuarios'),
    (N'GestionPerfiles'),
    (N'GestionProductos'),
    (N'GestionBackup'),
    (N'GestionBitacora')
) AS origen (Nombre)
    ON destino.Nombre_Permiso = origen.Nombre
WHEN NOT MATCHED THEN
    INSERT (Nombre_Permiso, Tipo, Rol) VALUES (origen.Nombre, N'Simple', 0);
GO

-- Asignacion de cada permiso a su familia
UPDATE Permiso SET Nombre_PermisoPadre = N'Admin'
WHERE Nombre_Permiso IN (N'GestionUsuarios', N'GestionPerfiles', N'GestionBackup', N'GestionBitacora');

UPDATE Permiso SET Nombre_PermisoPadre = N'Vendedor'
WHERE Nombre_Permiso IN (N'LlenarCarrito', N'SeleccionarProducto', N'GenerarReserva');

UPDATE Permiso SET Nombre_PermisoPadre = N'Cajero'
WHERE Nombre_Permiso IN (N'RegistrarCliente', N'GenerarFactura', N'RealizarCobro');

UPDATE Permiso SET Nombre_PermisoPadre = N'EncargadoDeposito'
WHERE Nombre_Permiso IN (N'GestionProductos');
GO

-- -----------------------------------------------------------------------------
-- T02 / T03 / T08 - Usuario administrador inicial
--
--   usuario : admin
--   clave   : Admin1234
--
-- La clave se guarda como SHA-256 irreversible (Servicios.Encriptador).
-- CAMBIAR LA CLAVE EN EL PRIMER INICIO DE SESION.
--
-- El DVH es el SHA-256 de UsuarioBE.ObtenerCamposDV(), es decir de:
--   Codigo|DNI|Nombre|Apellido|Usuario|Rol|Contrasena|Direccion|Telefono|Email|Activo|Bloqueado
-- con Activo/Bloqueado renderizados como 'True'/'False' (string.Join sobre bool).
-- El DVV de Usuarios es el SHA-256 de la lista de DVH unidos por '|'; con una
-- sola fila equivale al SHA-256 del unico DVH.
--
-- Si se cambian los datos de esta fila hay que recalcular ambos digitos, o
-- correr "Recalcular Digitos" (CUS-012) desde la aplicacion.
-- -----------------------------------------------------------------------------
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
