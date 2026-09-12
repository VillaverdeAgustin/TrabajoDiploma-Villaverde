# Especificación técnica — RFN1 Gestión de Ventas

**Proyecto:** ConectAR — Trabajo de Diploma (T1-23-32), Villaverde Agustín
**Destino:** implementación de CUN-001 a CUN-006 y de los requisitos RFN1.1 a RFN1.9
**Fuente:** `TD-Proyecto_Villaverde_ConectAR.docx`, secciones G02.1 y N02.1

Este documento define **qué** hay que construir y con **qué nombres**. No reemplaza al
`CLAUDE.md` de la raíz del repo: las reglas de arquitectura, nomenclatura y requisitos
transversales están ahí y siguen vigentes.

---

## 0. Reglas que no se negocian

1. **Sufijo `575_AV` en toda clase, variable y formulario nuevos.** Todo lo que se cree
   para este requerimiento nace con el sufijo. No usar `883_SC` en ningún lado.
2. **Clases en singular.** `ProductoBE_575_AV`, nunca `ProductosBE_575_AV`.
3. **Sin ORM.** ADO.NET puro, toda persistencia por stored procedure, siempre con
   `SqlParameter`. Reutilizar `AccesoDatos` (`LeerTabla(sp, parametros)` /
   `Escribir(sp, parametros)`).
4. **La GUI nunca llama a la DAL.** GUI → BLL → DAL → SP.
5. **Dígito verificador en todas las tablas de negocio.** Cada BE implementa
   `IVerificable`; DVH por fila y DVV por tabla, ambos SHA-256, vía
   `VerificadorIntegridad` y `MP_VerificadorIntegridad`.
6. **Bitácora en todas las clases de negocio.** Toda alta, modificación y baja llama a
   `BitacoraBLL.RegistrarBitacora(usuario, TipoAccion)`. Si hace falta un valor nuevo en
   `TipoAccion`, se agrega ahí.
7. **El usuario no es negocio, es servicio.** Ninguna tabla de negocio lleva FK a
   `Usuarios`: se guarda el nombre de usuario como texto.
8. **Baja lógica**, nunca `DELETE` físico. Campo `Activo bit`.
9. **Los mensajes de error de la GUI no exponen detalles técnicos.** La excepción se
   registra; al usuario se le muestra un mensaje neutro.

### Lo que todo formulario nuevo debe hacer

```csharp
public partial class frmXxx_575_AV : Form, IObservadorIdioma
{
    public frmXxx_575_AV()
    {
        InitializeComponent();
        EstiloConectAR_575_AV.AplicarFormulario(this);
    }
    // Load:    gestorIdioma.Suscribir(this);
    // Closed:  gestorIdioma.Desuscribir(this);
    // ActualizarTextos(): retraduce título, etiquetas, botones y encabezados de grilla
}
```

Las traducciones van en la tabla `Traduccion` (nunca en `.resx`), con claves del estilo
`VTA_LBL_CLIENTE`, `VTA_BTN_CONFIRMAR`, `PROD_COL_SALDO_BOBINA`. Cada opción nueva de
menú necesita su permiso en el árbol de `Familia` / `PermisoSimple`.

---

## 1. Modelo de datos

Trece tablas. Todas llevan `DVH varchar(64)` y se registran en `DigitoVertical` para el DVV.

### Catálogos

| Tabla | Campos |
|---|---|
| `Categoria` | `IdCategoria` PK identity · `Nombre` varchar(50) · `Activo` bit · `DVH` |
| `Marca` | `IdMarca` PK identity · `Nombre` varchar(50) · `Activo` bit · `DVH` |
| `UnidadMedida` | `IdUnidadMedida` PK identity · `Nombre` varchar(30) · `Abreviatura` varchar(5) · `Fraccionable` bit · `DVH` |
| `MedioPago` | `IdMedioPago` PK identity · `Nombre` varchar(50) · `RequiereAutorizacion` bit · `Activo` bit · `DVH` |

`UnidadMedida.Fraccionable` es la que habilita el circuito de bobina: "Unidad" va en
falso, "Metro" en verdadero.

### Producto y bobina — el núcleo del dominio

```
Producto
  IdProducto          int PK identity
  Codigo              varchar(25)  UNIQUE
  Descripcion         varchar(200)
  IdCategoria         int FK Categoria
  IdMarca             int FK Marca
  IdUnidadMedida      int FK UnidadMedida
  PrecioUnitario      decimal(12,2)
  StockActual         decimal(12,2)   -- decimal, no int: admite fracciones
  PuntoReposicion     decimal(12,2)
  Activo              bit
  DVH                 varchar(64)

Bobina
  IdBobina            int PK identity
  IdProducto          int FK Producto
  Identificador       varchar(25)     -- etiqueta física de la bobina o caja
  MedidaInicial       decimal(12,2)   -- p. ej. 305.00
  SaldoActual         decimal(12,2)
  FechaApertura       datetime
  Estado              varchar(15)     -- Cerrada / Abierta / Agotada
  DVH                 varchar(64)
```

**`StockActual` es `decimal`, no `int`.** Es el punto que el `CLAUDE.md` marca como
particularidad del rubro: no alcanza un contador entero. Un artículo fraccionable tiene
N bobinas y su stock es la suma de los saldos.

### Venta

```
OrdenVenta                          OrdenVentaDetalle
  IdOrdenVenta    int PK              IdOrdenVentaDetalle int PK
  NumeroOrden     varchar(15) UNIQUE  IdOrdenVenta        int FK
  IdCliente       int FK NULL         IdProducto          int FK
  UsuarioVendedor varchar(50)         Cantidad            decimal(12,2)
  NombreCliente   varchar(100)        IdUnidadMedida      int FK
  FechaHora       datetime            PrecioUnitario      decimal(12,2)
  Estado          varchar(15)         Subtotal            decimal(12,2)
  Total           decimal(12,2)       IdBobina            int FK NULL
  Activo          bit                 DVH                 varchar(64)
  DVH             varchar(64)
```

`IdCliente` es nullable porque el carrito lo arma el Vendedor antes de que el Cajero
identifique al cliente (CUN-001 pide solo el nombre). `Estado` toma `Abierta`,
`Facturada`, `Reservada` o `Anulada`. `IdBobina` en el detalle es nullable: solo se
completa cuando el artículo se fraccionó de una bobina concreta.

### Cliente, cobro, comprobante y reserva

```
Cliente                             Cobro
  IdCliente          int PK           IdCobro         int PK
  RazonSocial        varchar(100)     IdOrdenVenta    int FK
  Nombre             varchar(50)      IdMedioPago     int FK
  Apellido           varchar(50)      Monto           decimal(12,2)
  DNI                varchar(15)      FechaHora       datetime
  CUIT               varchar(13)      CodigoAutorizacion varchar(30)
  CondicionIVA       varchar(30)      Estado          varchar(15)
  Direccion          varchar(150)     DVH             varchar(64)
  Telefono           varchar(30)
  CorreoElectronico  varchar(256)    <-- AES-256
  FechaActualizacion datetime        Comprobante
  Activo             bit              IdComprobante     int PK
  DVH                varchar(64)      NumeroComprobante varchar(20) UNIQUE
                                      TipoComprobante   varchar(5)
Reserva                               IdOrdenVenta      int FK
  IdReserva            int PK         IdCliente         int FK
  IdCliente            int FK         FechaEmision      datetime
  IdOrdenVenta         int FK         Neto              decimal(12,2)
  FechaGeneracion      datetime       IVA               decimal(12,2)
  FechaEstimadaEntrega datetime       Total             decimal(12,2)
  Estado               varchar(15)    DVH               varchar(64)
  Activo               bit
  DVH                  varchar(64)   ReservaDetalle
                                       IdReservaDetalle int PK
                                       IdReserva        int FK
                                       IdProducto       int FK
                                       Cantidad         decimal(12,2)
                                       IdUnidadMedida   int FK
                                       DVH              varchar(64)
```

**`Cliente.CorreoElectronico` es el campo del requisito T03.2** — encriptación reversible
AES-256 sobre un campo de una tabla de negocio, con botón de desencriptar protegido por
clave. Se guarda cifrado y se descifra solo al mostrarlo.

**Ese servicio hoy no existe.** `Servicios/Encriptador.cs` solo tiene `EncriptarIrrev`
(SHA-256). Hay que crear `EncriptadorReversible_575_AV` con `Encriptar(texto)` y
`Desencriptar(cifrado)` en AES-256, con la clave fuera del código fuente.

---

## 2. Stored procedures

Se respeta la convención ya existente en `db/02_procedimientos.sql`
(`SP_Ext…`, `SP_Crear…`, `SP_Actualizar…`, `SP_Elim…`, `SP_Buscar…`).

| Grupo | Procedimientos |
|---|---|
| Catálogos | `SP_ExtCategoria`, `SP_ExtMarca`, `SP_ExtUnidadMedida`, `SP_ExtMedioPago` |
| Producto | `SP_ExtProducto`, `SP_BuscarProducto` (por descripción, categoría, marca o código), `SP_CrearProducto`, `SP_ActualizarProducto`, `SP_ElimProducto`, `SP_ActualizarStock` |
| Bobina | `SP_ExtBobinaPorProducto`, `SP_CrearBobina`, `SP_ActualizarSaldoBobina` |
| Cliente | `SP_ExtCliente`, `SP_BuscarCliente` (por DNI, CUIT o razón social), `SP_CrearCliente`, `SP_ActualizarCliente`, `SP_ElimCliente` |
| Orden de venta | `SP_ExtOrdenVenta`, `SP_ExtOrdenVentaAbiertas`, `SP_ExtOrdenVentaDetalle`, `SP_CrearOrdenVenta`, `SP_AgregarDetalleOrdenVenta`, `SP_QuitarDetalleOrdenVenta`, `SP_ActualizarEstadoOrdenVenta` |
| Cobro | `SP_RegistrarCobro`, `SP_ExtCobroPorOrden` |
| Comprobante | `SP_CrearComprobante`, `SP_ExtComprobante`, `SP_ProximoNumeroComprobante` |
| Reserva | `SP_CrearReserva`, `SP_AgregarDetalleReserva`, `SP_ExtReserva`, `SP_ActualizarEstadoReserva` |

`SP_CrearOrdenVenta`, `SP_RegistrarCobro` y `SP_CrearComprobante` son transaccionales:
el descuento de stock, la actualización del saldo de bobina y la emisión del comprobante
tienen que confirmarse o revertirse en bloque.

---

## 3. Clases por capa

### Entidad_BE — todas implementan `IVerificable`

`ClienteBE_575_AV` · `ProductoBE_575_AV` · `BobinaBE_575_AV` · `CategoriaBE_575_AV` ·
`MarcaBE_575_AV` · `UnidadMedidaBE_575_AV` · `MedioPagoBE_575_AV` ·
`OrdenVentaBE_575_AV` · `OrdenVentaDetalleBE_575_AV` · `CobroBE_575_AV` ·
`ComprobanteBE_575_AV` · `ReservaBE_575_AV` · `ReservaDetalleBE_575_AV`

Enumeraciones: `CondicionIVA_575_AV`, `EstadoOrdenVenta_575_AV`, `EstadoBobina_575_AV`,
`EstadoReserva_575_AV`.

### Acceso_DAL — un mapeador por agregado

`MP_Cliente_575_AV` · `MP_Producto_575_AV` · `MP_Bobina_575_AV` ·
`MP_OrdenVenta_575_AV` · `MP_Cobro_575_AV` · `MP_Comprobante_575_AV` ·
`MP_Reserva_575_AV` · `MP_Catalogo_575_AV` (los cuatro catálogos juntos)

Mismo patrón que `MP_Bitacora`: `AccesoDatos` para ejecutar, `SqlParameter` para todo
parámetro, `DBNull.Value` para los criterios no informados, y proyección fila por fila
sobre la BE.

### Negocio_BLL

| Clase | Responsabilidad |
|---|---|
| `ClienteBLL_575_AV` | Alta, modificación y baja lógica. Cifra y descifra el correo. Valida CUIT y evita duplicados por CUIT o DNI. |
| `ProductoBLL_575_AV` | Consulta y búsqueda. `ValidarDisponibilidad(producto, cantidad)` resuelve RFN1.2 contra stock y saldos de bobina. |
| `BobinaBLL_575_AV` | `Fraccionar(producto, metros)` (RFN1.8): elige la bobina abierta con saldo suficiente, o abre una nueva; descuenta y cierra la bobina al agotarse. |
| `VentaBLL_575_AV` | Arma y persiste el carrito: crear orden, agregar y quitar detalle, recalcular total, confirmar. |
| `FacturacionBLL_575_AV` | Emite el comprobante, descuenta stock y dispara el fraccionamiento (RFN1.6, RFN1.7). |
| `CobroBLL_575_AV` | Registra el cobro y resuelve la autorización del medio de pago (RFN1.5). |
| `ReservaBLL_575_AV` | Genera la reserva y calcula la fecha estimada de entrega (RFN1.9). |

Toda operación que escriba llama a `BitacoraBLL.RegistrarBitacora` y recalcula el DVH
por `VerificadorIntegridad`.

### Servicios

`EncriptadorReversible_575_AV` — AES-256 para el correo del cliente (T03.2). **Es lo
único que falta en esta capa.** El resto (`Bitacora`, `VerificadorIntegridad`,
`SessionManager`, `GestorDeIdioma`) se reutiliza como está.

### Presentacion — un formulario por caso de uso

| Formulario | CU | Qué hace |
|---|---|---|
| `frmLlenarCarrito_575_AV` | CUN-001 | Grilla del carrito, botón "Mas" que abre CUN-002, campo nombre del cliente, botón "Confirmar". Al confirmar muestra "Se creó la orden Nro. xxx a nombre de xxx". |
| `frmSeleccionarProducto_575_AV` | CUN-002 | Búsqueda por nombre, marca o código; grilla de resultados **resaltando los que no tienen stock**; cantidad; "Agregar al Carrito" y "Finalizar". |
| `frmRegistrarCliente_575_AV` | CUN-003 | Alta de cliente. Se abre desde CUN-004 cuando la búsqueda no encuentra a nadie. |
| `frmRealizarVenta_575_AV` | CUN-004 | Lista de carritos abiertos, resumen del carrito seleccionado, búsqueda de cliente, botones "Cobrar" y "Generar Reserva". |
| `frmRealizarCobro_575_AV` | CUN-005 | Medios de pago, monto total, datos del pago, resultado de la autorización. |
| `frmGenerarReserva_575_AV` | CUN-006 | Productos del carrito, botón "Reservar", comprobante con ID y fecha estimada. |

Los ítems de menú `MENU_LLENAR_CARRITO`, `MENU_REALIZAR_VENTA`, `MENU_CLIENTE` y
`MENU_PRODUCTOS` **ya existen traducidos en `frmMenu`** pero no abren nada. Hay que
engancharlos, con su permiso correspondiente.

---

## 4. Orden de implementación sugerido

1. **Catálogos y producto.** Tablas, SP, BE, DAL, BLL y ABM de producto. Sin esto no hay
   nada que vender. Incluye `Bobina` y el fraccionamiento.
2. **Cliente.** Junto con `EncriptadorReversible_575_AV`, que cierra T03.2.
3. **Carrito.** CUN-002 primero (es punto de extensión de CUN-001), después CUN-001.
4. **Facturación y cobro.** CUN-005 antes que CUN-004, que lo invoca.
5. **Reserva.** CUN-006.

Cada fase deja el proyecto compilando y el menú usable. Datos de prueba en un
`db/05_datos_negocio.sql` nuevo, con el DVH ya calculado o recalculable desde
*Recalcular Dígitos* (CUS-012).

---

## 5. Inconsistencias del documento detectadas al armar esto

No las corrijo acá porque son trabajo sobre el `.docx`, pero conviene resolverlas antes
de la entrega:

1. **CUN-004, escenario principal, paso 9:** dice "El sistema emite una factura **por
   duplicado**". El docente pidió expresamente "emite la factura", sin ese agregado. La
   corrección se aplicó en G02 pero no en la especificación del caso de uso.
2. **CUN-004, postcondiciones:** "El sistema actualiza el stock". Misma regla de no
   nombrar al sistema como sujeto que ya se aplicó en G02.
3. **CUN-003** no menciona el DNI ni el punto de extensión, pero la corrección del
   docente en G02 dice que el alta de cliente entra por DNI y va como *extend*. La
   especificación del CU debería reflejarlo.
4. **CUN-005** valida el pago "con la entidad bancaria". No hay integración bancaria
   posible en el alcance: hay que simular la autorización y dejarlo dicho.
5. **CUN-006** tiene como precondición "haber realizado el pago", pero en CUN-004 la
   reserva se dispara desde el paso 8, *antes* del cobro. Una de las dos está mal.
6. Las secciones **GUI de CUN-005 y CUN-006** están vacías y sin marcador `[Pendiente]`;
   se completan recién cuando existan los formularios.
