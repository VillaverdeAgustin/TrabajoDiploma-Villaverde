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

### Generar el instalador

```
powershell -ExecutionPolicy Bypass -File instalador\generar_instalador.ps1
```

Deja `ConectAR_575_AV_Setup.exe` en `instalador\salida\`: un único archivo con el `.msi` y SQL
Server Express 2025 adentro. El instalador de SQL Server Express (`SQLEXPR_x64_ENU.exe`, 714 MB) no
se versiona: el script lo toma de `T.DIPLOMA\_descargas\SQLServerExpress2025\` o de la ruta que se
pase con `-SqlExpress`. Todo lo demás sale de archivos versionados; el `.aip` se vuelve a crear en
cada corrida:

1. Compila la solución en Release y las acciones del instalador (`compilar_acciones.ps1`): una DLL
   x64 en C# (`instalador\acciones\`) que compila los mismos archivos de la DAL que el programa.
   Windows Installer llama funciones nativas, así que `ildasm`/`ilasm` le agregan las exportaciones.
2. Crea un proyecto Professional y le aplica `ConectAR_575_AV.aic` (producto *ConectAR 575_AV*,
   versión 3.0.0, `UpgradeCode` fijo, archivos con las DLL de PDFsharp-MigraDoc, su licencia y la
   carpeta `Ayuda`, accesos directos, condición de .NET 4.7.2 y el prerequisito de SQL Server
   Express 2025).
3. Por automatización COM: tipo de instalación *por máquina si es administrador, por usuario si no*,
   build de un solo `.exe` y el diálogo **Base de datos**, encadenado después de la pantalla de
   carpeta.
4. Agrega `BaseDatosDlg_575_AV.xml`, lo que ni la línea de comandos ni COM permiten configurar:
   controles y eventos del diálogo, la opción *Instalar para* en la pantalla de carpeta, las acciones
   personalizadas y sus secuencias, y el diccionario `ConectAR_575_AV_es.ail` (corrige "Browse" y el
   título de la ventana de interfaz básica).
5. Ajusta la actualización mayor para que otra compilación de la misma versión reemplace a la
   instalada (cada compilación tiene un `ProductCode` nuevo), hace que el setup extraiga el `.msi` en
   `%ProgramData%` (desde `AppData\Roaming` la instalación por usuario falla con el error 2203) y
   compila.

Al compilar aparecen avisos `AI_ICE05` y `AI_ICE07`, que son **esperables**: advierten que algunas
acciones y propiedades solo existen en la interfaz (listar instancias, carpetas de *Instalar para*,
la instancia elegida y la casilla *Continuar sin crear la base*). En una instalación sin interfaz,
`PrepararBaseDatos_575_AV` calcula la instancia por su cuenta y el modo se pide con
`MSIINSTALLPERUSER=1`, como se indica arriba. La compilación termina bien.

### Alternativa: instalar la base con el script

Desde una consola, en la carpeta `db`:

```
.\instalar_base.bat /limpio
```

- Ejecuta los scripts `01` a `13` en orden y se detiene ante el primer error.
- `/limpio` borra `ConectAR_DB` antes de instalar; pide escribir `SI` para confirmar.
- Sin `/limpio`, instala sobre la base existente.
- Por defecto usa la instancia `localhost\SQLEXPRESS`. Si es otra, se pasa como
  parámetro: `.\instalar_base.bat MIPC\SQLEXPRESS /limpio`.
- Al terminar muestra `Base instalada` o el script que falló.

Después, al abrir el programa, la pantalla de conexión encuentra la base ya creada y
solo hay que guardar la instancia.

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
