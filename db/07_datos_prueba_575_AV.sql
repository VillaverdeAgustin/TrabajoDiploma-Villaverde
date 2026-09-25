-- =============================================================================
-- ConectAR S.R.L. - Trabajo de Diploma - Villaverde, Agustin (575_AV)
-- 07 - Datos de prueba del RFN1 (Gestion de Venta por mostrador)
--
-- Juego minimo para recorrer el circuito completo de venta:
--
--   * Ocho articulos, con su ubicacion de deposito.
--   * Dos fraccionables (cable por metro lineal) con bobinas de 305 m:
--     CAB-UTP6-EXT tiene tres, una de ellas ABIERTA con 120 m de saldo, para
--     que se vea el remanente; CAB-UTP5E-INT tiene dos cerradas.
--   * JCK-RJ45-C6 y RCK-12U-MUR quedan por debajo de su punto de reposicion.
--   * CER-DSX-600 queda SIN STOCK: es el que dispara el faltante y la reserva.
--   * Dos clientes, uno responsable inscripto con CUIT y otro consumidor
--     final, con el correo electronico cifrado en AES-256 (T03.2).
--
-- Los dvh y los DVV van precalculados, con el mismo metodo que los catalogos
-- de 05: son el SHA-256 de ObtenerCamposDV() de cada entidad. Asi una base
-- recien instalada pasa el control de integridad del login sin pedir
-- Recalcular Digitos.
--
-- Los correos cifrados dependen de la clave 'ClaveAES' de App.config: si se
-- cambia esa clave, estos valores dejan de poder descifrarse.
--
-- Idempotente: si ya hay articulos cargados, el script no hace nada.
--
-- Orden de ejecucion: 01 -> 02 -> 03 -> 04 -> 05 -> 06 -> 07
-- =============================================================================

USE [ConectAR_DB];
GO

SET NOCOUNT ON;
GO

IF EXISTS (SELECT 1 FROM [dbo].[Articulo])
BEGIN
    PRINT '07 - Ya hay articulos cargados: no se insertan datos de prueba.';
    SET NOEXEC ON;
END
GO

-- Articulo
SET IDENTITY_INSERT [dbo].[Articulo] ON;
INSERT INTO [dbo].[Articulo] ([id_articulo], [codigo], [descripcion], [id_categoria], [id_marca], [id_unidad_medida], [precio_unitario], [cantidad_disponible], [punto_reposicion], [deposito_ubicacion], [estado_articulo], [activo], [dvh])
VALUES
    (1, N'CAB-UTP6-EXT', N'Cable UTP Cat 6 exterior CMX', 1, 1, 2, 1480.00, 730.00, 200.00, N'Pasillo 1 - Est. A', N'Disponible', 1, N'bf7896086ed61f17890da93253d3931fa60c708093e5e5234d271e12dd4d70c5'),
    (2, N'CAB-UTP5E-INT', N'Cable UTP Cat 5e interior CM', 1, 3, 2, 890.00, 610.00, 200.00, N'Pasillo 1 - Est. B', N'Disponible', 1, N'89a3c602224a72848ee3caa2f6f2daba5ee15ca2fb4e7079f455c67ff95c30ef'),
    (3, N'PAT-24-C6', N'Patchera 24 bocas Cat 6 de 19 pulgadas', 3, 2, 1, 98500.00, 14.00, 5.00, N'Pasillo 2 - Est. A', N'Disponible', 1, N'adaea20f92b1a9a00d2741bdeaba0bc0b2268dadcefc1d00b4f10a3a94df9561'),
    (4, N'JCK-RJ45-C6', N'Jack RJ45 Cat 6 keystone', 3, 2, 1, 6250.00, 18.00, 50.00, N'Pasillo 2 - Est. B', N'Disponible', 1, N'26bd38f5aa5b331f62d4f1d8f9232336414e69049d5e4fd508d448187d1560c6'),
    (5, N'FIC-RJ45-C6', N'Ficha RJ45 Cat 6, bolsa por 100', 3, 8, 1, 41200.00, 26.00, 10.00, N'Pasillo 2 - Est. C', N'Disponible', 1, N'54a0bdabd9bd23b379b4d34a7e6bfad35872ebbd57b4f923ccf77177543e4991'),
    (6, N'RCK-12U-MUR', N'Rack mural 12U de 570 mm', 4, 5, 1, 315000.00, 3.00, 4.00, N'Deposito - Sector G', N'Disponible', 1, N'aff7ee461a4d76bde26f57090b4b4d663471c3c20ab8947aad376636c30f32b9'),
    (7, N'CAN-20X12', N'Canaleta PVC 20x12 mm por 2 m', 5, 8, 1, 3900.00, 240.00, 100.00, N'Pasillo 4 - Est. A', N'Disponible', 1, N'e7b60a02017168c3794432dab9a40d0ec5e75a837a0d302b0e60a3fb1ce2c8e5'),
    (8, N'CER-DSX-600', N'Certificador de cableado DSX-600', 7, 6, 1, 9850000.00, 0.00, 1.00, N'Deposito - Sector S', N'SinStock', 1, N'86e4a492e9f5e962c425127b59efb2ed8019b089808a4212b0425d6db8c27d3d');
SET IDENTITY_INSERT [dbo].[Articulo] OFF;
GO

-- Bobina
SET IDENTITY_INSERT [dbo].[Bobina] ON;
INSERT INTO [dbo].[Bobina] ([id_bobina], [id_articulo], [identificador], [medida_inicial], [saldo_bobina], [fecha_apertura], [estado], [dvh])
VALUES
    (1, 1, N'BOB-UTP6-001', 305.00, 120.00, '2026-09-24T22:02:01', N'Abierta', N'216fa995b0f6e87715f24967852f330a1e75c829927925ba1994dda09da5e379'),
    (2, 1, N'BOB-UTP6-002', 305.00, 305.00, NULL, N'Cerrada', N'e99620fad43bcee6c705eaf002fcbc5fe9562db0b318e5dc72430ada98ff612e'),
    (3, 1, N'BOB-UTP6-003', 305.00, 305.00, NULL, N'Cerrada', N'769c03078282b77975bcb67cb2676a9fd3f307556fcc518108d47716515e21a0'),
    (4, 2, N'BOB-UTP5E-001', 305.00, 305.00, NULL, N'Cerrada', N'22351c98b3795191535b58f875a8733f69e4eb752332b858d94cfa0001e12188'),
    (5, 2, N'BOB-UTP5E-002', 305.00, 305.00, NULL, N'Cerrada', N'c55f0e0f855c7fb9f03886d5f53e30d4fec8886d0b3190ddebec193bde9d4e05');
SET IDENTITY_INSERT [dbo].[Bobina] OFF;
GO

-- Cliente
SET IDENTITY_INSERT [dbo].[Cliente] ON;
INSERT INTO [dbo].[Cliente] ([id_cliente], [razon_social], [nombre], [apellido], [dni], [cuit], [condicion_iva], [direccion], [telefono], [correo_electronico], [fecha_actualizacion], [activo], [dvh])
VALUES
    (1, N'Redes del Sur S.R.L.', N'Marcela', N'Ferreyra', N'27845192', N'30-71455842-7', N'ResponsableInscripto', N'Av. Hipolito Yrigoyen 13250, Burzaco', N'11 4238-7712', N'Z3TAnxkInbz1WoszMUnUBGMV9Mi1hIfefE+EgYpVGQQULq3byA2ABwh1HEoMSL9i', '2026-09-24T22:02:02', 1, N'1712b19d6af0a26aa1062dcfa1c183702169ce6f3b1ff1e2f6def2af948f1a1a'),
    (2, NULL, N'Diego', N'Quinteros', N'30112458', NULL, N'ConsumidorFinal', N'Pringles 455, Adrogue', N'11 6644-2201', N'OX54YVrAlRG3CNIlT2QiwYJO1USswyIfJgct8buYGzEOCCtI8LQk2UbGpzQIUNi0', '2026-09-24T22:02:02', 1, N'22c9d4633902f83c4183eedddad7050b4ea69528ff46a360be37076147750344');
SET IDENTITY_INSERT [dbo].[Cliente] OFF;
GO

-- DVV de las tablas con datos de prueba
UPDATE [dbo].[DigitoVertical] SET [DVV] = CASE [Tabla]
    WHEN N'Articulo' THEN N'54c73430e616b654abf1cdd0cf68bd655fe1fae5c12a8c44758da15f62829b3d'
    WHEN N'Bobina' THEN N'6e4bfafef925bcef4ed7716d8db1d3eb76783cffe0893a7b636aaa0d16d4cf98'
    WHEN N'Cliente' THEN N'05d85025d24e8646f46cde9ccc9c231ad52d83f37dea398ee4c8e839537954bd'
    ELSE [DVV] END
WHERE [Tabla] IN (N'Articulo', N'Bobina', N'Cliente');
GO

SET NOEXEC OFF;
GO

PRINT '07 - Datos de prueba del RFN1 cargados.';
GO
