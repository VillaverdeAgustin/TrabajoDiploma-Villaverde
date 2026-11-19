-- 13 - Actualizacion de una base de la Entrega 2 a la Entrega 3 - 575_AV
-- En una base nueva no cambia nada: cada paso busca lo que dejo la version anterior.
-- El instalador lo corre despues de respaldar la base; despues recalcula DVH y DVV de todas las tablas.

USE [ConectAR_DB];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
SET NOCOUNT ON;
GO

-- RFN2 sin autorizacion: las ordenes pendientes o autorizadas quedan emitidas y las rechazadas vuelven a borrador
IF EXISTS (SELECT 1 FROM sys.check_constraints
           WHERE name = N'CK_OrdenCompra_estado' AND definition LIKE N'%Autorizada%')
    ALTER TABLE [dbo].[OrdenCompra] DROP CONSTRAINT [CK_OrdenCompra_estado];
GO

UPDATE [dbo].[OrdenCompra] SET [estado] = 'Emitida'
WHERE [estado] IN ('PendienteAutorizacion', 'Autorizada');

UPDATE [dbo].[OrdenCompra] SET [estado] = 'Borrador'
WHERE [estado] = 'Rechazada';
GO

IF OBJECT_ID('dbo.CK_OrdenCompra_estado', 'C') IS NULL
    ALTER TABLE [dbo].[OrdenCompra] WITH CHECK ADD CONSTRAINT [CK_OrdenCompra_estado] CHECK ([estado] IN
        ('Borrador', 'Emitida', 'RecibidaParcial', 'Recibida'));
GO

IF COL_LENGTH('dbo.OrdenCompra', 'fecha_autorizacion') IS NOT NULL
    ALTER TABLE [dbo].[OrdenCompra] DROP COLUMN [fecha_autorizacion];

IF COL_LENGTH('dbo.OrdenCompra', 'usuario_autorizante') IS NOT NULL
    ALTER TABLE [dbo].[OrdenCompra] DROP COLUMN [usuario_autorizante];
GO

IF OBJECT_ID('dbo.SP_RegistrarAutorizacionOrdenCompra', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_RegistrarAutorizacionOrdenCompra];
GO

-- T04: el Propietario hace las compras. Los usuarios del rol AdministradorCompras pasan a Propietario
-- (el usuario de prueba 'compras' queda inactivo) y se quitan el rol y el permiso de autorizar.
UPDATE [dbo].[Usuarios] SET [Activo] = 0
WHERE [Usuario] = 'compras' AND [Rol] = N'AdministradorCompras';

UPDATE [dbo].[Usuarios] SET [Rol] = N'Propietario'
WHERE [Rol] = N'AdministradorCompras';

UPDATE [dbo].[Permiso] SET [Nombre_PermisoPadre] = N'Propietario'
WHERE [Nombre_PermisoPadre] = N'AdministradorCompras';

UPDATE [dbo].[Permiso] SET [Nombre_PermisoPadre] = NULL
WHERE [Nombre_PermisoPadre] = N'AutorizarOrdenCompra';

DELETE FROM [dbo].[Permiso]
WHERE [Nombre_Permiso] IN (N'AutorizarOrdenCompra', N'AdministradorCompras');
GO

PRINT '13 - Base actualizada a la Entrega 3.';
GO
