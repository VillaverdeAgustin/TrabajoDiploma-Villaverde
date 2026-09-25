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

-- Familias / roles del negocio, segun G04 - Roles del sistema.
-- Sin acentos: el nombre viaja a la base y se compara como texto. El rotulo que
-- ve el usuario ("Deposito" -> "Depósito") sale de la tabla Traduccion.
MERGE INTO Permiso AS destino
USING (VALUES
    (N'Admin',                N'Compuesto', 1),
    (N'Ventas',               N'Compuesto', 1),
    (N'Caja',                 N'Compuesto', 1),
    (N'Deposito',             N'Compuesto', 1),
    -- Roles de RFN2 (Compras). Quedan creados para completar el modelo de
    -- roles; sus permisos se agregan junto con los casos de uso CUN-007..013.
    (N'AdministradorCompras', N'Compuesto', 1),
    (N'Propietario',          N'Compuesto', 1)
) AS origen (Nombre, Tipo, EsRol)
    ON destino.Nombre_Permiso = origen.Nombre
WHEN NOT MATCHED THEN
    INSERT (Nombre_Permiso, Tipo, Rol) VALUES (origen.Nombre, origen.Tipo, origen.EsRol);
GO

-- Permisos simples. Uno a uno con el enum Entidad_BE.TipoPermiso: agregar un
-- valor alli obliga a insertarlo aca con el mismo texto.
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
    (N'GestionBitacora')
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

-- GestionArticulos queda en Deposito, que es donde estaba GestionProductos en el
-- mapa anterior: el ABM de articulos es tarea de deposito.
UPDATE Permiso SET Nombre_PermisoPadre = N'Deposito'
WHERE Nombre_Permiso IN (N'EntregarArticulos', N'GestionArticulos');
GO

-- Las tres familias del circuito cuelgan de Admin, para que el administrador
-- pueda recorrerlo completo. Es el Composite haciendo su trabajo: Admin no
-- repite los permisos, contiene las familias y los hereda por recursion, y
-- cada permiso sigue colgando de una sola familia.
UPDATE Permiso SET Nombre_PermisoPadre = N'Admin'
WHERE Nombre_Permiso IN (N'Ventas', N'Caja', N'Deposito');
GO

-- -----------------------------------------------------------------------------
-- Limpieza del mapa de casos de uso anterior.
--
-- Solo corre sobre bases ya instaladas: en una base nueva no hay nada que
-- borrar. Se desenganchan los hijos antes de borrar el padre, porque la FK
-- reflexiva lo impide, y no se toca ningun rol que algun usuario tenga asignado.
-- -----------------------------------------------------------------------------
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
