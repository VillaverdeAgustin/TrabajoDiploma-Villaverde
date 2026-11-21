# ConectAR — Gestor de Ventas y Compras

Trabajo de Diploma (T1-23-32) — UAI, Facultad de Tecnología Informática
**Alumno:** Villaverde, Agustín — Legajo B00033339-T1
**Comisión:** 3°BN — Turno noche — Sede Centro — Cursada 2026
**Docentes:** Ing. Pereyra, Jorge Agustín — Ing. Silvestro, Agustín

Sistema de gestión para **ConectAR S.R.L.**, distribuidora de materiales para
cableado estructurado y conectividad. Windows Forms + C# + SQL Server, en capas,
con acceso por ADO.NET y stored procedures.

Única excepción a "toda persistencia por stored procedure": el restore de T07. No se puede reemplazar
una base desde adentro de ella, así que `MP_Backup_575_AV` lee la cabecera del .bak y ejecuta
`RESTORE DATABASE` como comandos parametrizados contra master (ver `docs/entrega-2/Pruebas_T07_575_AV.md`).

---

## Estructura

```
Entidad_BE/      entidades
Acceso_DAL/      acceso a datos (ADO.NET + stored procedures)
Negocio_BLL/     lógica de negocio
Servicios/       transversales: encriptación, logger, DV, idioma, sesión
Presentacion/    formularios Windows Forms
db/              scripts de base de datos e instalar_base.bat
instalador/      generador del setup (Advanced Installer), acciones del instalador y diccionario
docs/            capturas y material por entrega
```

## Requisitos

**PC donde se instala el programa**

- Windows 10 u 11 de 64 bits.
- .NET Framework 4.7.2 o posterior (viene con Windows 10 1803 en adelante). Si falta,
  el instalador avisa y no continúa.
- SQL Server 2019 o posterior (la edición Express alcanza), en la misma PC, con
  autenticación de Windows. Si la PC no tiene ninguna instancia, el instalador instala
  **SQL Server Express 2025** (instancia `SQLEXPRESS`), que viene incluido en el setup; eso sí
  necesita una cuenta de administrador.
- Con SQL Server ya instalado no hace falta ser administrador: sin permisos se instala solo para
  el usuario.

**PC de desarrollo**, además:

- Visual Studio 2022 o posterior, con la carga de trabajo de desarrollo de escritorio .NET
  (aporta MSBuild y el compilador `csc` de Roslyn).
- `ildasm.exe` del SDK de .NET Framework 4.8 (`Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools`,
  lo instala Visual Studio) e `ilasm.exe` de .NET Framework (`%WINDIR%\Microsoft.NET\Framework64\v4.0.30319`).
- Advanced Installer **Professional** 24.1. Se usa en **modo de prueba**, activo desde el 30/10/2026
  y vigente hasta el **29/11/2026** aproximadamente. Los `.msi` generados no vencen; vencido el
  período de prueba solo deja de poder regenerarse. Los paquetes generados con la prueba muestran un
  aviso de Advanced Installer al instalar.
- `sqlcmd`, solo para la instalación alternativa de la base con `instalar_base.bat`.

## Instalación

Ejecutar `ConectAR_575_AV_Setup.exe` (paquete de **64 bits**, versión 3.0.0, unos 724 MB porque
incluye SQL Server Express 2025) y seguir el asistente:

0. **SQL Server Express**. Antes del asistente se busca alguna instancia de SQL Server en la PC
   (`HKLM\SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL`, vista de 64 bits). Si no
   hay ninguna, se instala SQL Server Express 2025 en silencio, con la instancia `SQLEXPRESS`,
   autenticación de Windows y la cuenta que instala como administradora del motor. Si ya hay una,
   este paso se saltea.

1. **Carpeta e "Instalar para"**. *Todos los usuarios* instala en `C:\Program Files\ConectAR_575_AV`
   y pide confirmación de administrador (UAC). *Solo para mí* instala en
   `%LocalAppData%\Programs\ConectAR_575_AV`, con accesos directos solo para ese usuario, y **no
   pide UAC**. Se propone *Todos los usuarios* si la cuenta es administradora y *Solo para mí* si no.
2. **Base de datos**. Lista las instancias locales de SQL Server (`localhost` para la
   predeterminada, `.\NOMBRE` para las demás) y propone la ya configurada, `.\SQLEXPRESS` o la
   primera; se puede escribir otra. **Probar conexión** usa
   autenticación de Windows. Si la prueba falla, se puede marcar **Continuar sin crear la base**.
3. **Instalar**. Durante la instalación, con la cuenta de Windows de quien instala (no SYSTEM):
   - se guarda la instancia en `%ProgramData%\ConectAR_575_AV\conexion.cfg`, con permiso de
     modificación para Usuarios (lo lee la capa de acceso a datos; el `App.config` no tiene conexión);
   - si `ConectAR_DB` no existe, se ejecutan los scripts `db\01` a `db\13` contra `master`, en orden
     y respetando `GO`, con la misma clase que usa el programa (`InstaladorBaseDatos_575_AV`). El
     avance se ve en la barra de progreso. Si un script falla, se borra la base a medio crear, se
     avisa y la instalación termina igual;
   - si `ConectAR_DB` ya existe (reinstalación o actualización desde la Entrega 2), **se actualiza
     sin perder los datos**: primero se hace un backup completo con los SP del T07
     (`ConectAR_DB_antes_de_actualizar_<fecha>.bak`, en la carpeta Backup de la instancia); después
     se ejecutan los scripts sin los de datos de prueba (`07`, `08` y `11`), incluido
     `13_actualizacion_E3_575_AV.sql`, que pasa las órdenes de compra a los estados sin autorización
     y quita el rol `AdministradorCompras`; por último se recalculan el DVH y el DVV de todas las
     tablas con `VerificadorIntegridadBLL` del programa instalado (el mismo recálculo que el restore)
     y se verifica la integridad. Si algo falla, se restaura el backup y la base queda como estaba.

Con la base creada, el programa abre directo en el login. Si no se creó (instancia inválida,
*Continuar sin crear la base* o falla de un script), la primera vez se abre **Conexión a la base de
datos**, que permite elegir la instancia, probarla y crear la base. El administrador también la abre
desde **Admin → Conexión a la base de datos**.

Instalar otra compilación encima la reemplaza (actualización mayor, mismo `UpgradeCode`): no quedan
dos entradas. Al desinstalar se quitan el programa y los accesos directos; **la base de datos no se
borra**.

Instalación sin interfaz: `msiexec /i ConectAR_575_AV.msi /qn` instala para todos los usuarios y
necesita una consola de administrador. Sin permisos de administrador hay que pedir el modo por usuario:

```
msiexec /i ConectAR_575_AV.msi MSIINSTALLPERUSER=1 ALLUSERS="" /qn
```

Opcionales: `CONECTAR_INSTANCIA=.\SQLEXPRESS` (si no se indica, se usa la ya configurada o la primera
instancia local) y `CONECTAR_SIN_BASE=1`.

## Usuarios de prueba

| Usuario | Clave | Rol (familia) | Pantallas |
|---|---|---|---|
| `admin` | `Admin1234` | Admin | Todas: Admin (usuarios, historial, perfiles, copia de seguridad y restauración, bitácora, bitácora de cambios, conexión a la base), Ventas, Caja, Depósito, Compras, Gestión de artículos y Reportes |
| `vendedor` | `Ventas2026` | Ventas | Ventas: Carrito de Compras y Orden de Pago |
| `cajero` | `Caja2026` | Caja | Caja: Validar Pedido y Cobrar (con alta de cliente por DNI) |
| `deposito` | `Deposito2026` | Deposito | Depósito: Entregar Artículos; Compras: Recepción de mercadería; Gestión: Artículos |
| `propietario` | `Propietario2026` | Propietario | Compras: Proveedores, Importar lista de precios, Comparar precios, Órdenes de compra y Serialización de órdenes; Reportes: Reporte de compras |

Todos pueden cambiar su clave y el idioma de la interfaz.

## Datos de prueba

- **Artículo sin stock:** `CER-DSX-600` (certificador). En el carrito aparece en rojo
  y al agregarlo muestra "Sin disponibilidad".
- **Artículos debajo del punto de reposición:** `JCK-RJ45-C6` (18 u, reposición 50)
  y `RCK-12U-MUR` (3 u, reposición 4).
- **Bobina abierta:** `BOB-UTP6-001` de `CAB-UTP6-EXT` (cable por metro), con 120 m
  de saldo sobre 305 m. El artículo tiene además dos bobinas cerradas.
- **Clientes**, para buscarlos por DNI en la caja:

  | DNI | Cliente | Condición IVA | CUIT | Comprobante |
  |---|---|---|---|---|
  | `27845192` | Redes del Sur S.R.L. (Marcela Ferreyra) | Responsable inscripto | `30-71455842-7` | A |
  | `30112458` | Diego Quinteros | Consumidor final | — | B |

  Un DNI que no existe ofrece dar de alta al cliente desde la caja.

## Cómo probar "Ver correo"

En *Caja → Validar Pedido y Cobrar*, buscar un cliente por DNI: el correo se ve
oculto. *Ver correo* pide la **clave del usuario que tiene la sesión iniciada**
(por ejemplo `Caja2026` con `cajero`, o `Admin1234` con `admin`). Con la clave
correcta muestra el correo descifrado (`compras@redesdelsur.com.ar` o
`diego.quinteros@gmail.com`) y lo registra en la bitácora; con otra clave no lo
descifra.

## Cómo probar el control de integridad (T08)

Alterar una fila sin pasar por el sistema, por ejemplo:

```sql
UPDATE ConectAR_DB.dbo.Articulo SET precio_unitario = precio_unitario + 1 WHERE codigo = 'PAT-24-C6';
```

El control corre después de validar usuario y clave:

- **admin**: ve la pantalla de integridad con las tablas afectadas (`Articulo`) y
  tres botones: *Recalcular* (recalcula los dígitos, lo registra en la bitácora,
  vuelve a verificar y entra al menú), *Restaurar backup* (deshabilitado hasta que
  se implemente el módulo de Backup y Restore, T07) y *Salir* (cierra la sesión y
  el programa).
- **Cualquier otro usuario** (por ejemplo `vendedor`): ve "Sistema no disponible.
  Contacte al administrador.", queda registrado en la bitácora, se cierra la
  sesión y sale del programa.

## Clave AES

El correo de clientes y proveedores se guarda cifrado con AES-256 usando la
constante `CLAVE_AES` de `Servicios/EncriptadorReversible_575_AV.cs`. **Si se
cambia esa clave, los correos ya cargados no se pueden volver a leer.**

---

## Nomenclatura

Las clases y formularios del **negocio** llevan sufijo `575_AV` (tres últimos
dígitos del DNI + iniciales), según la regla de la cátedra. Los servicios
transversales conservan los nombres del trabajo práctico ya entregado.

## Ramas

`main` y la rama de la entrega en curso. Cada rama `entrega-N` se crea al
comenzar esa entrega, no antes: hoy existen `main` y `entrega-1`.

Se trabaja en la rama de la entrega en curso; al aprobarse se mergea a `main`
y se etiqueta `entrega-N-aprobada`. **Las ramas no se borran**: el docente las
revisa.
