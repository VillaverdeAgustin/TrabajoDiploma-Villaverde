# Evidencia de pruebas — T07 Backup y Restore

Pruebas ejecutadas el 06/11/2026 sobre una base instalada desde cero (todos los scripts, 01 a 11). La prueba
es automática (`PruebaT07`): abre las pantallas reales de la aplicación (frmBackup_575_AV,
frmRestore_575_AV, frmMenu y frmLogin), presiona los botones, contesta los mensajes y verifica la base, msdb
y los archivos generados.

- **Resultado:** 17 casos (24 verificaciones), todos correctos.
- **Instancia:** SQL Server Express (`.\SQLEXPRESS`); carpeta de backups de la instancia
  `C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\Backup`.
- **Capturas:** `capturas/frmBackup_575_AV.png`, `capturas/frmRestore_575_AV.png`,
  `capturas/frmRestore_575_AV_confirmacion.png` y `capturas/frmRecalcular_575_AV_restaurar.png`.
- **Regresión:** `PruebaTraducciones` da OK en los 29 formularios (incluidos los dos nuevos y frmRecalcular).
  La auditoría de claves da 763 claves en ES y EN, sin faltantes, duplicadas ni sin uso. Los escenarios de
  T08 (`t08.sh`, base limpia) dan los mismos resultados que antes; ahora "Restaurar copia" aparece habilitado.

## Cómo funciona

| Paso | Dónde | Detalle |
|---|---|---|
| Carpeta predeterminada | `SP_RutaBackups` | `SERVERPROPERTY('InstanceDefaultBackupPath')`. El archivo lo escribe el servicio de SQL Server, por eso no se ofrece Documentos |
| Backup | `SP_GenerarBackup` | `BACKUP DATABASE ... TO DISK = @ruta WITH CHECKSUM, INIT`, archivo `ConectAR_DB_aaaammdd_hhmmss.bak` |
| Verificación | `SP_VerificarBackup` | `RESTORE VERIFYONLY FROM DISK = @ruta` (valida los checksums si el backup los tiene) |
| Lista de backups | `SP_ListarBackups` | `msdb.dbo.backupset` + `backupmediafamily`: backups completos a disco de ConectAR_DB, con fecha, tamaño y ruta |
| Validar el archivo | `MP_Backup_575_AV.LeerCabecera` (contra master) | `RESTORE HEADERONLY`: tiene que ser un backup y de ConectAR_DB |
| Restore | `MP_Backup_575_AV.Restaurar` (contra master) | `SINGLE_USER WITH ROLLBACK IMMEDIATE` → `RESTORE DATABASE ... WITH REPLACE` → `MULTI_USER` |
| Después del restore | `BackupBLL_575_AV.Restaurar` | Recalcula DVH y DVV de todas las tablas, registra `RestoreRealizado` (72), verifica la integridad y cierra la sesión (`Logout`) |

**Excepción a "toda persistencia por stored procedure".** La lectura de la cabecera y el restore no pueden
ir por un SP de ConectAR_DB: no se puede reemplazar una base desde adentro de ella. Se ejecutan en la DAL
como comandos parametrizados (`@ruta`) contra master; el nombre de la base es una constante, nunca un dato
del usuario. El backup, la verificación y la lista sí van por SP (script 02). `BACKUP` y `RESTORE
VERIFYONLY` no admiten transacción, por eso `AccesoDatos` suma `EjecutarSinTransaccion`.

**Bitácora.** `BackupGenerado` (71) se registra después del backup, así que no queda dentro del archivo:
si se restaura ese mismo backup, ese registro desaparece con el resto de los datos posteriores.
`RestoreRealizado` (72) se registra después del restore, en la base restaurada, y queda.

**Desde dónde se restaura.** Desde el menú Admin → Restaurar copia (permiso GestionBackup), o desde la
pantalla de integridad del login (T08) con el botón "Restaurar copia". En los dos casos, después del
restore la sesión queda cerrada y se vuelve al login.

## Casos

| # | Caso | Datos | Resultado esperado | Resultado obtenido |
|---|---|---|---|---|
| T07-1 | Carpeta predeterminada | Pantalla recién abierta | La carpeta de backups de la instancia (SERVERPROPERTY('InstanceDefaultBackupPath')) | Correcto: C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\Backup |
| T07-2 | Generar backup | Carpeta predeterminada, usuario admin | Archivo ConectAR_DB_aaaammdd_hhmmss.bak en esa carpeta, verificado con RESTORE VERIFYONLY, registrado en msdb y en la bitácora (71) | Correcto: Se generó la copia ConectAR_DB_20261106_234052.bak y se verificó correctamente. — ConectAR_DB_20261106_234052.bak · 6,84 MB · Correcta (RESTORE VERIFYONLY): el archivo se puede restaurar · msdb: sí · bitácora 71: +1 |
| T07-3 | Cabecera del archivo generado | RESTORE HEADERONLY leído por la prueba | Es un backup de ConectAR_DB con checksum | Correcto: ConectAR_DB\|1\|06/11/2026 23:40:52 |
| T07-4 | Error: carpeta inexistente | Z:\no\existe | Mensaje de error sin detalles técnicos y sin registro en la bitácora | Correcto: No se pudo generar la copia: verifique que la carpeta exista y que el servicio de SQL Server pueda escribir en ella. |
| T07-5 | Error: ruta que no es completa | backups (ruta relativa) | Pide una ruta completa del servidor | Correcto: La carpeta no es válida: escriba una ruta completa del servidor, por ejemplo C:\Backups. |
| T07-6 | Backups registrados | msdb (backupset + backupmediafamily) | Lista con fecha, tamaño y ruta; al elegir uno se completa el archivo a restaurar | Correcto: 4 backups; elegido ConectAR_DB_20261106_234052.bak |
| T07-7 | Cancelar la confirmación | Responder No a "se reemplazan todos los datos actuales" | No se toca la base | Correcto: Restauración cancelada: no se modificó la base. — teléfono 11 0000-0707 |
| T07-8 | Restaurar y verificar que vuelven los datos | Backup del paso T07-2, después de cambiar el teléfono de Cablenet y dar de alta un proveedor | Vuelven el teléfono original y la cantidad de proveedores; se registra RestoreRealizado (72) en la base restaurada | Correcto: teléfono 11 4201-5560 (original 11 4201-5560), proveedores de prueba 0 · bitácora 72: 1 |
| T07-9 | Integridad después del restore | VerificarIntegridadTotal sobre todas las tablas | Ninguna tabla con error | Correcto: OK |
| T07-10 | Cierre de sesión | Después del restore | La sesión queda cerrada y el menú se cierra para volver a entrar | Correcto: sesión abierta: no · menú abierto: no · último evento: Logout |
| T07-11 | Error: archivo inexistente | C:\no_existe_575_AV\ConectAR_DB_20260101_000000.bak | No se pudo leer el archivo; la base no se toca | Correcto: No se pudo leer el archivo: no existe, el servicio de SQL Server no tiene acceso o no es una copia de seguridad. |
| T07-12 | Error: archivo que no es un backup | Archivo de texto con extensión .bak | No se pudo leer el archivo | Correcto: No se pudo leer el archivo: no existe, el servicio de SQL Server no tiene acceso o no es una copia de seguridad. |
| T07-13 | Error: backup de otra base | Backup de OtraBase_575_AV (base creada y borrada por la prueba) | Rechaza el archivo indicando de qué base es | Correcto: El archivo es una copia de la base OtraBase_575_AV, no de ConectAR_DB. |
| T07-14 | Error: sin elegir archivo | Archivo vacío | Pide elegir una copia | Correcto: Elija una copia de la lista o un archivo con Examinar. |
| T07-15 | El restore recalcula los DV | Backup tomado con Articulo inconsistente (DVH viejo); se restaura ese backup | Después del restore los DVH y DVV se recalculan: integridad OK aunque el backup tenía el error | Correcto: antes del backup: Articulo · después del restore: OK · descripción: Patchera 24 bocas Cat 6 de 19 pulgadas (editado fuera del sistema) |
| T07-16 | Restaurar desde la pantalla de integridad | Login de admin con Articulo alterado: Restaurar copia → backup del paso T07-2 | El botón está habilitado; se restaura, se recalculan los DV, la sesión queda cerrada y se vuelve al login (no entra al menú) | Correcto: botón habilitado: sí · menú: no · sesión: cerrada · integridad: OK · descripción: Patchera 24 bocas Cat 6 de 19 pulgadas |
| T07-17 | Solo Admin ve las opciones | Menú de cada usuario de prueba | Copia de seguridad y Restaurar copia habilitadas solo para admin (permiso GestionBackup) | Correcto: admin: habilitadas · vendedor: deshabilitadas · cajero: deshabilitadas · deposito: deshabilitadas · compras: deshabilitadas · propietario: deshabilitadas |

## Observaciones

- Un backup guarda también los stored procedures de ese momento: restaurar un backup tomado con una
  versión anterior de los scripts trae de vuelta esos SP. Los backups de esta entrega se tomaron con la
  versión actual.
- El restore usa las rutas de archivos de datos guardadas en el backup, así que está pensado para la misma
  instancia. Restaurar en otra PC con otras carpetas requeriría `WITH MOVE`, que queda fuera de alcance.
- El archivo que no es un backup (T07-12) y el inexistente (T07-11) dan el mismo mensaje: SQL Server no
  distingue, sin exponer detalles técnicos, entre un archivo ilegible y uno que no existe.
