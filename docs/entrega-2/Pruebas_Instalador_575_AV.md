# Evidencia de pruebas — Instalador (Entrega 2)

Pruebas del instalador `ConectAR_575_AV.msi` (Advanced Installer, versión 2.0.0) ejecutadas el
05/11/2026 en la PC de desarrollo: Windows 11, SQL Server Express (`.\SQLEXPRESS`), cuenta
`LEGIONARY\aguss`, del grupo Administradores.

- **Cómo se corrieron:** las pruebas por usuario se hicieron en modo silencioso con un token sin
  administrador, por ejemplo:

  ```bat
  runas /trustlevel:0x20000 "msiexec /i ConectAR_575_AV.msi MSIINSTALLPERUSER=1 /qn /l*v instalar.log"
  ```

  El recorrido por máquina, con doble clic y todas las pantallas del asistente, quedó en las capturas
  de `capturas/instalador/`.
- **Evidencia:** cada caso cita las líneas del log de Windows Installer (`/l*v`) que escribe la acción
  del instalador (`ConectAR_575_AV: ...`), más lo que se comprobó en la base, en el registro y al
  abrir el programa.
- **Builds posteriores:** los .msi regenerados después (commits `dc6762c`, `3fbcd93` y el de A02/A03)
  usan el mismo proyecto y las mismas acciones. Solo cambian el programa y los scripts que se empaquetan.

| Captura | Pantalla |
|---|---|
| `01_bienvenida.png` | Bienvenida |
| `02_carpetainstalacion.png` | Carpeta de instalación e "Instalar para: Todos los usuarios / Solo para mí" |
| `03_SQL_instalador.png` | Diálogo "Base de datos": instancia, "Probar conexión" y "Continuar sin crear la base" |
| `04_Instalar.png` | Listo para instalar |
| `04b_progreso.png` | Barra de progreso mientras corren los scripts |
| `05_Instalacion_completada.png` | Instalación completada |
| `06_login.png` | El programa abre directo en el login |

## Casos

| # | Caso | Pasos | Resultado esperado | Resultado obtenido | Evidencia |
|---|---|---|---|---|---|
| a | PC sin la base | Se borran ConectAR_DB y `%ProgramData%\ConectAR_575_AV`. Se instala por usuario sin administrador y, aparte, por máquina con doble clic. Se abre el programa | Se crea ConectAR_DB con los scripts 01 a 11 en orden, con la cuenta de quien instala (no SYSTEM). Se escribe `conexion.cfg` con permiso de modificación para Usuarios. El programa abre en el login | Correcto. La base queda con 29 tablas, 6 usuarios y las traducciones cargadas. `conexion.cfg` contiene `instancia=.\SQLEXPRESS` + CRLF, en UTF-8 sin BOM, con `BUILTIN\Usuarios:(OI)(CI)(M)`. Se crean los accesos directos en el escritorio y el menú Inicio. La ventana que abre es "Login" | Log: `cuenta de ejecucion: LEGIONARY\aguss` · `conexion.cfg guardado en C:\ProgramData\ConectAR_575_AV\conexion.cfg con instancia=.\SQLEXPRESS` · `ejecutando 01_esquema.sql (1 de 11)` … `ejecutando 11_datos_prueba_compras_575_AV.sql (11 de 11)` · `ConectAR_DB creada en .\SQLEXPRESS` · `La instalación finalizó satisfactoriamente`. Capturas 01 a 06 |
| b | Base existente | Con la base del caso (a) y un login hecho (1 registro en la bitácora), se vuelve a instalar | No corre ningún script y los datos se conservan | Correcto. Antes y después: `tablas=29 bitacora=1 max_registro=1 articulos=8 creada=2026-11-05 02:28:14` | Log: `ConectAR_DB ya existe en .\SQLEXPRESS: no se ejecuta ningun script` · `La instalación finalizó satisfactoriamente` |
| c | Instancia inválida | Instancia `NOEXISTE\INSTANCIA`; la prueba de conexión falla y se marca "Continuar sin crear la base". Se abre el programa | La instalación termina, el cfg guarda la instancia elegida y el programa abre frmConexion_575_AV para elegir la instancia y crear la base | Correcto. `conexion.cfg`: `instancia=NOEXISTE\INSTANCIA`. La ventana que abre es "Conexión a la base de datos" | Log: `instancia elegida: 'NOEXISTE\INSTANCIA' … sin base: True` · `se continua sin crear la base: el programa la crea en el primer inicio` · `La instalación finalizó satisfactoriamente` |
| d | Usuario sin administrador (por usuario) | 1) Token sin administrador (`runas /trustlevel:0x20000`) con `MSIINSTALLPERUSER=1`. 2) Prueba con interfaz desde `runas`. 3) "Solo para mí" con la cuenta normal, sin elevar | Se instala en `%LocalAppData%\Programs\ConectAR_575_AV` sin pedir UAC | 1) Correcto: instalado por usuario (contexto 2), base creada, abre en el login. 2) La cuenta pertenece a Administradores, así que Windows Installer la trata como administrador: propone por máquina y pide UAC. Por eso se agregó "Instalar para: Solo para mí" en la pantalla de carpeta. 3) Correcto: por usuario, sin UAC, con accesos directos del usuario, y abre en el login | 1) `APPDIR = C:\Users\aguss\AppData\Local\Programs\ConectAR_575_AV\` · `MSIINSTALLPERUSER = 1` · `La instalación finalizó satisfactoriamente`. 2) `conectar_d.log`: `MSI_LUA: Setting AdminUser property to 1 because this is the client …` · `Modifying ALLUSERS property. Its current value is '2'. Its new value: '1'`. 3) `Deleting ALLUSERS property` · `instalado: {1AF4469E-…} contexto=2 (2 = por usuario)` · `ventana: Login`. Captura 02 (opción "Solo para mí") |
| e | Desinstalar | Desinstalar la instalación del caso (a) | Se quita el programa; ConectAR_DB sigue intacta | Correcto. Antes y después: `tablas=29 bitacora=0 articulos=8 creada=2026-11-05 02:28:14`. La carpeta del programa ya no existe y `conexion.cfg` se conserva | Log: `La eliminación se ha completado satisfactoriamente` · `ls: cannot access '…\Programs\ConectAR_575_AV': No such file or directory` |
| f | Reinstalar encima | Instalar un .msi nuevo sobre una instalación existente | Actualiza (actualización mayor) sin duplicar la entrada en Programas | Correcto, después de una corrección. En el primer intento la tabla Upgrade (atributo 257) dejaba afuera a la misma versión 2.0.0 y quedaban dos entradas. Con el atributo 769 se reemplaza la anterior y queda una sola entrada | Primer intento: `{1F420014-…}` y `{7A09ADE6-…}` instalados a la vez. Después de corregir: `OLDPRODUCTS = {1F420014-…};{7A09ADE6-…}` y queda solo `{00E25091-…} contexto=2` |
| — | Falla de un script | Se genera un .msi de prueba cuyo script 07 termina con `SELECT * FROM TablaQueNoExiste_575_AV;`. Se instala con la base inexistente | Se borra la base a medio crear, se avisa que el programa la va a crear al iniciar y la instalación termina igual | Correcto. ConectAR_DB no existe después de la instalación, el instalador termina con código 0 y el programa abre frmConexion_575_AV | Log: `ejecutando 07_datos_prueba_575_AV.sql (7 de 11)` · `fallo la creacion de la base: El nombre de objeto 'TablaQueNoExiste_575_AV' no es válido.` · `se borro la base a medio crear` · `La instalación finalizó satisfactoriamente`. Aviso: "No se pudo crear la base de datos ConectAR_DB y se deshizo lo creado. La instalación continúa: ConectAR va a crear la base de datos la primera vez que se inicie." |
| 7 | Nada se escribe en la carpeta de instalación | grep sobre todo el código buscando escrituras a disco y rutas armadas a partir de la carpeta del programa | Ninguna escritura en la carpeta del programa en tiempo de ejecución | Correcto. La única escritura es `conexion.cfg`, en `%ProgramData%\ConectAR_575_AV`. La carpeta del programa solo se lee: los scripts `db\` que usa `InstaladorBaseDatos_575_AV` | `ConfiguracionConexion_575_AV.cs:60: Directory.CreateDirectory(carpeta);` · `ConfiguracionConexion_575_AV.cs:64: File.WriteAllText(RutaArchivo, …)` (RutaArchivo = CommonApplicationData\ConectAR_575_AV\conexion.cfg) · `InstaladorBaseDatos_575_AV.cs:25: … Assembly.Location), CARPETA_SCRIPTS` (lectura) · `LectorListaPrecios_575_AV.cs:55: File.OpenRead(ruta)` (lectura) |

## Notas

- Los .msi generados con la versión de prueba de Advanced Installer muestran un aviso de evaluación
  al instalar (aparece en el log: "Este paquete ha sido creado con una versión de prueba de Advanced
  Installer"). El .msi no vence.
- Desde la Entrega 2 la app suma dos funciones que escriben archivos: los XML de A03, en la ubicación
  que elige el usuario (por defecto, Documentos). Ninguna escribe en la carpeta del programa, así que
  el punto 7 se sigue cumpliendo.
