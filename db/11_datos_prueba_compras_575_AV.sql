-- 11 - Datos de prueba de compras - 575_AV

USE [ConectAR_DB];
GO

SET NOCOUNT ON;
GO

-- Proveedores (correo cifrado con AES-256)
IF NOT EXISTS (SELECT 1 FROM [dbo].[Proveedor])
BEGIN
    SET IDENTITY_INSERT [dbo].[Proveedor] ON;

    INSERT INTO [dbo].[Proveedor] ([id_proveedor], [razon_social], [cuit], [condicion_iva], [rubro],
        [telefono], [correo_electronico], [direccion], [activo], [dvh])
    VALUES
    (1, N'Cablenet Distribuidora S.A.', N'30-70845123-8', N'ResponsableInscripto', N'Cableado estructurado', N'11 4201-5560', N'ZRlybUhW69K/4VsrCNPYOli8R0PmGolD805ZzZ1WU+nWQx6o1LyYxQzvK7MA6fD3', N'Av. Mitre 2350, Avellaneda', 1, N'4b11a819bb9c6b13ce74138b35f708556cd010bb4ae1893795592817b75e1a28'),
    (2, N'FibraSur S.R.L.', N'30-71562948-4', N'ResponsableInscripto', N'Fibra optica y conectividad', N'221 482-3310', N'vviFY7wfcVuXsZfKIoeI0aiUZLKJAJgBaS33HwLMnyGz2tXSJG45JJvtdSmQkO/X', N'Calle 12 845, La Plata', 1, N'3ad474035902c9ef40328275c6ffd4e5e863457a5356aa77e94bb43395264076'),
    (3, N'Redes Integrales de Hernan Ortiz', N'20-28456713-8', N'Monotributista', N'Racks y canalizaciones', N'11 3987-4412', N'nGdqUfWkopOrLvLbMz3bZLWu7wPnAtLvquNb6KH/EaZ54apyWm5L1cd9v7F585xx', N'Av. Espora 980, Burzaco', 1, N'66fe5caf2b4350b5961ca2205a2e9fdc8a06803994562610039b758369a2628c');

    SET IDENTITY_INSERT [dbo].[Proveedor] OFF;

    UPDATE [dbo].[DigitoVertical]
    SET [DVV] = N'c96b0b8836276acc936f662c055560331975ca3f95164128cdcc636326855182'
    WHERE [Tabla] = N'Proveedor';
END
GO

-- Un usuario por rol de compras: AdministradorCompras y Propietario (clave en SHA-256)
SET IDENTITY_INSERT Usuarios ON;

IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Usuario = 'compras')
    INSERT INTO Usuarios
        (Codigo, DNI, Nombre, Apellido, Usuario, Rol, [Contraseña],
         Direccion, Telefono, Email, Activo, Bloqueado, DVH)
    VALUES
        (5, 29444555, 'Hernan', 'Molina', 'compras', N'AdministradorCompras',
         '90270432db3aa05e17ed5eeaa9c5c76df71e39f299287ce3079bdd435f545129',
         'Pringles 610', '1142381004', 'compras@conectar.com.ar', 1, 0,
         N'6cb38b1b4d03bd64dcb02dcc0907ff8fdfcbe9216156e1e8f3862e4b2498291a');

IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Usuario = 'propietario')
    INSERT INTO Usuarios
        (Codigo, DNI, Nombre, Apellido, Usuario, Rol, [Contraseña],
         Direccion, Telefono, Email, Activo, Bloqueado, DVH)
    VALUES
        (6, 25666777, 'Graciela', 'Paz', 'propietario', N'Propietario',
         'd464ea04fcf96c30047728a031fb3edc75fea8c9b636cc6d3bce61bec6dd30a9',
         'Av Espora 1450', '1142381005', 'propietario@conectar.com.ar', 1, 0,
         N'58a07d398086e20a2c97b63d5784dadf09d6dc1a949372a89644140a4870531c');

SET IDENTITY_INSERT Usuarios OFF;
GO

-- DVV de Usuarios con admin, los tres usuarios del 08 y los dos de compras
IF (SELECT COUNT(*) FROM Usuarios) = 6
    UPDATE DigitoVertical
    SET DVV = N'94a9f56851bd67c6d881b465c198d1987ab7d202874f8fc63525dc2e2ec5034e'
    WHERE Tabla = N'Usuarios';
ELSE
    PRINT 'Usuarios tiene filas propias: el DVV se recalcula desde el login del administrador.';
GO

PRINT '11 - Datos de prueba de compras cargados.';
GO
