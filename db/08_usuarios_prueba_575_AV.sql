-- 08 - Usuarios de prueba - 575_AV

USE [ConectAR_DB];
GO

SET NOCOUNT ON;
GO

-- Un usuario por familia: Ventas, Caja y Deposito (clave en SHA-256)
SET IDENTITY_INSERT Usuarios ON;

IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Usuario = 'vendedor')
    INSERT INTO Usuarios
        (Codigo, DNI, Nombre, Apellido, Usuario, Rol, [Contraseña],
         Direccion, Telefono, Email, Activo, Bloqueado, DVH)
    VALUES
        (2, 35111222, 'Martina', 'Sosa', 'vendedor', N'Ventas',
         'b42df123efc703204d49a871bb3d7a7c9cf78fafd54418f04d549c9a9c35ce9e',
         'Av Espora 1450', '1142381001', 'vendedor@conectar.com.ar', 1, 0,
         N'a81c3ecc0f6a51a3fab8ab2aab9c53f8569505aea6d1ea60e886b0ff07b670e9');

IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Usuario = 'cajero')
    INSERT INTO Usuarios
        (Codigo, DNI, Nombre, Apellido, Usuario, Rol, [Contraseña],
         Direccion, Telefono, Email, Activo, Bloqueado, DVH)
    VALUES
        (3, 33222333, 'Lucas', 'Benitez', 'cajero', N'Caja',
         '865edff273bba32d02e2c4feb4b9a8e4be73c9eb3cbf322db86eb3e007b13659',
         'Roca 870', '1142381002', 'cajero@conectar.com.ar', 1, 0,
         N'8c27bb50301e9e804e760031c04a18a7d094536f55b5b45f9dff885c56d4b3d9');

IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Usuario = 'deposito')
    INSERT INTO Usuarios
        (Codigo, DNI, Nombre, Apellido, Usuario, Rol, [Contraseña],
         Direccion, Telefono, Email, Activo, Bloqueado, DVH)
    VALUES
        (4, 31333444, 'Carla', 'Ruiz', 'deposito', N'Deposito',
         '6d49fe39faf6cdcab286985a898bb228bfda1f1945723c85b0d257854958b420',
         'Monteagudo 215', '1142381003', 'deposito@conectar.com.ar', 1, 0,
         N'00067733cef34a126546e54dd33827b05572bd8c71ae3afa1e160bb928ff9c28');

SET IDENTITY_INSERT Usuarios OFF;
GO

-- DVV de Usuarios con admin y los tres usuarios de prueba
IF (SELECT COUNT(*) FROM Usuarios) = 4
    UPDATE DigitoVertical
    SET DVV = N'37c7f76e0ea03cf5ab711b24a30a8d31cac417b326224d99f5b271365d88cad3'
    WHERE Tabla = N'Usuarios';
ELSE
    PRINT 'Usuarios tiene filas propias: el DVV se recalcula desde el login del administrador.';
GO

PRINT '08 - Usuarios de prueba cargados.';
GO
