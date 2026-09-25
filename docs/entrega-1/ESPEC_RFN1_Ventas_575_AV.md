# Especificación técnica — RFN1 Gestión de Venta por mostrador

**Proyecto:** ConectAR — Trabajo de Diploma (T1-23-32), Villaverde Agustín
**Destino:** implementación de CUN-001 a CUN-005 y de los requisitos RFN1.1 a RFN1.9
**Fuente:** `TD-Proyecto_Villaverde_ConectAR.docx`, secciones G02.1 y N02.1
**Versión:** 3.1 — 25/09/2026. Contrato diagrama ↔ código (sección 5); alcance de cliente acotado a la Entrega 1.

> **Qué cambió respecto de la versión 1.** El mapa de casos de uso se rehízo: desapareció
> "Seleccionar Producto", apareció "Generar Orden de Pago", se fusionaron facturación y
> cobro, y la entrega en depósito pasó a ser caso de uso propio. Además: la factura pasó
> a ser **comprobante no válido como factura**, la reserva **bloquea el artículo** y
> dispara el movimiento del RFN2, y las existencias se manejan con **Singleton**.
> El término del dominio es **Artículo**, no Producto.

> **Qué cambió respecto de la versión 2.** La cátedra exige **equilibrio total entre
> diagramas y código**: la bitácora (T06) y el dígito verificador (T08) tienen que verse
> en los diagramas de cada caso de uso con los mismos nombres que en el código. Los
> diagramas N02.1.2.x.b/c/d se rehicieron así y la **sección 5** fija el contrato que
> el código tiene que respetar. Se cambiaron además tres cosas del modelo: **cada SP
> escribe una sola tabla** (sección 2), `Entrega.id_comprobante` deja de ser único
> (entregas parciales) y se agrega `AutorizadorPago_575_AV`.

---

## 0. Reglas que no se negocian

1. **Sufijo `575_AV` en toda clase, variable y formulario nuevos.** No usar `883_SC`.
2. **Clases en singular.** `ArticuloBE_575_AV`, nunca `ArticulosBE_575_AV`.
3. **Sin ORM.** ADO.NET puro, toda persistencia por stored procedure, siempre con
   `SqlParameter`. Reutilizar `AccesoDatos` (`LeerTabla` / `Escribir`).
4. **La GUI nunca llama a la DAL.** GUI → BLL → DAL → SP.
5. **Dígito verificador en todas las tablas de negocio.** Cada BE implementa
   `IVerificable`; DVH por fila y DVV por tabla, SHA-256.
6. **Bitácora en todas las clases de negocio.** Toda alta, modificación y baja llama a
   `BitacoraBLL.RegistrarBitacora(usuario, TipoAccion)`.
7. **El usuario no es negocio, es servicio.** Ninguna tabla de negocio lleva FK a
   `Usuarios`: se guarda el nombre de usuario como texto.
8. **Baja lógica**, nunca `DELETE` físico. Campo `activo bit`.
9. **Atributos y columnas en minúscula**, con guión bajo. Los mensajes de error de la
   GUI no exponen detalles técnicos.

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

Traducciones en la tabla `Traduccion`, nunca en `.resx`, con claves del estilo
`VTA_LBL_CLIENTE`, `VTA_BTN_CONFIRMAR`, `ART_COL_SALDO`. Cada opción nueva de menú
necesita su permiso en el árbol de `Familia` / `PermisoSimple`. Los tres permisos de
este requerimiento son **Ventas**, **Caja** y **Depósito**.

---

## 1. Modelo de datos

Dieciséis tablas. Todas llevan `dvh varchar(64)` y se registran en `DigitoVertical`.

### Catálogos

| Tabla | Campos |
|---|---|
| `Categoria` | `id_categoria` PK identity · `nombre` varchar(50) · `activo` bit · `dvh` |
| `Marca` | `id_marca` PK identity · `nombre` varchar(50) · `activo` bit · `dvh` |
| `UnidadMedida` | `id_unidad_medida` PK identity · `nombre` varchar(30) · `abreviatura` varchar(5) · `fraccionable` bit · `dvh` |
| `MedioPago` | `id_medio_pago` PK identity · `nombre` varchar(50) · `requiere_autorizacion` bit · `activo` bit · `dvh` |

`UnidadMedida.fraccionable` habilita el circuito de bobina: "Unidad" en falso, "Metro"
en verdadero.

### Artículo y bobina — el núcleo del dominio

```
Articulo
  id_articulo         int PK identity
  codigo              varchar(25)  UNIQUE
  descripcion         varchar(200)
  id_categoria        int FK Categoria
  id_marca            int FK Marca
  id_unidad_medida    int FK UnidadMedida
  precio_unitario     decimal(12,2)
  cantidad_disponible decimal(12,2)   -- decimal, no int: admite fracciones
  punto_reposicion    decimal(12,2)
  deposito_ubicacion  varchar(30)
  estado_articulo     varchar(15)     -- Disponible / Bloqueado / Sin stock
  activo              bit
  dvh                 varchar(64)

Bobina
  id_bobina           int PK identity
  id_articulo         int FK Articulo
  identificador       varchar(25)
  medida_inicial      decimal(12,2)   -- p. ej. 305.00
  saldo_bobina        decimal(12,2)
  fecha_apertura      datetime
  estado              varchar(15)     -- Cerrada / Abierta / Agotada
  dvh                 varchar(64)
```

**`cantidad_disponible` es `decimal`, no `int`.** Es la particularidad del rubro: no
alcanza un contador entero. **`estado_articulo`** es nuevo y sostiene el bloqueo que
pidió el docente: cuando se reserva un artículo faltante, queda bloqueado hasta que la
compra se concrete.

### Carrito, reserva y orden de pago

```
Carrito                             DetalleCarrito
  id_carrito       int PK             id_detalle_carrito int PK
  nro_carrito      varchar(15) UNIQUE id_carrito         int FK
  usuario_vendedor varchar(50)        id_articulo        int FK
  nombre_cliente   varchar(100)       cantidad           decimal(12,2)
  fecha_apertura   datetime           id_unidad_medida   int FK
  estado           varchar(15)        precio_unitario    decimal(12,2)
  precio_total     decimal(12,2)      subtotal           decimal(12,2)
  activo           bit                id_bobina          int FK NULL
  dvh              varchar(64)        dvh                varchar(64)

Reserva                             OrdenPago
  id_reserva       int PK             id_orden_pago    int PK
  id_articulo      int FK             nro_orden_pago   varchar(15) UNIQUE
  id_cliente       int FK NULL        id_carrito       int FK
  nombre_cliente   varchar(100)       nombre_cliente   varchar(100)
  cantidad         decimal(12,2)      fecha_emision    datetime
  fecha_reserva    datetime           precio_total     decimal(12,2)
  plazo_entrega    int                estado           varchar(15)
  estado           varchar(15)        activo           bit
  dvh              varchar(64)        dvh              varchar(64)
```

`Carrito.estado` toma `Abierto`, `Confirmado`, `Facturado` o `Anulado`.
`OrdenPago.estado` toma `Pendiente`, `Pagada` o `Anulada`.

### Cliente, cobro, comprobante y entrega

```
Cliente                             Pago
  id_cliente          int PK          id_pago         int PK
  razon_social        varchar(100)    id_comprobante  int FK
  nombre              varchar(50)     id_medio_pago   int FK
  apellido            varchar(50)     monto           decimal(12,2)
  dni                 varchar(15)     fecha_hora      datetime
  cuit                varchar(13)     codigo_autorizacion varchar(30)
  condicion_iva       varchar(30)     estado          varchar(15)
  direccion           varchar(150)    dvh             varchar(64)
  telefono            varchar(30)
  correo_electronico  varchar(256)   <-- AES-256
  fecha_actualizacion datetime       Comprobante
  activo              bit             id_comprobante    int PK
  dvh                 varchar(64)     nro_comprobante   varchar(20) UNIQUE
                                      tipo_comprobante  varchar(5)
Entrega                               id_orden_pago     int FK
  id_entrega       int PK             id_cliente        int FK
  id_comprobante   int FK             fecha_emision     datetime
  usuario_deposito varchar(50)        neto              decimal(12,2)
  fecha_entrega    datetime           iva               decimal(12,2)
  estado           varchar(15)        precio_total      decimal(12,2)
  dvh              varchar(64)        estado            varchar(15)
                                      dvh               varchar(64)

DetalleComprobante                  DetalleEntrega
  id_detalle_comprobante int PK       id_detalle_entrega int PK
  id_comprobante   int FK             id_entrega         int FK
  id_articulo      int FK             id_articulo        int FK
  cantidad         decimal(12,2)      cantidad_entregada decimal(12,2)
  id_unidad_medida int FK             id_unidad_medida   int FK
  precio_unitario  decimal(12,2)      dvh                varchar(64)
  dvh              varchar(64)
```

**`Cliente.correo_electronico` es el campo del requisito T03.2** — AES-256 reversible,
con botón de desencriptar protegido por clave. **Ese servicio no existe todavía**:
`Servicios/Encriptador.cs` solo tiene `EncriptarIrrev` (SHA-256). Hay que crear
`EncriptadorReversible_575_AV`. Según G05.3, el mismo tratamiento va sobre
`Proveedor.correo_electronico` cuando se implemente el RFN2.

**`Comprobante.estado`** toma `Emitido` o `Entregado`. Es lo que impide que un
comprobante se use dos veces para retirar mercadería.

---

## 2. Stored procedures

Convención existente de `db/02_procedimientos.sql`: `SP_Ext…`, `SP_Crear…`,
`SP_Actualizar…`, `SP_Elim…`, `SP_Buscar…`.

| Grupo | Procedimientos |
|---|---|
| Catálogos | `SP_ExtCategoria`, `SP_ExtMarca`, `SP_ExtUnidadMedida`, `SP_ExtMedioPago` |
| Artículo | `SP_ExtArticulo`, `SP_BuscarArticulo` (descripción, categoría, marca o código), `SP_CrearArticulo`, `SP_ActualizarArticulo`, `SP_ElimArticulo`, `SP_ActualizarStock`, `SP_ActualizarEstadoArticulo` |
| Bobina | `SP_ExtBobinaPorArticulo`, `SP_CrearBobina`, `SP_ActualizarSaldoBobina` |
| Cliente | `SP_ExtCliente`, `SP_BuscarClientePorDni`, `SP_BuscarCliente`, `SP_CrearCliente` |
| Carrito | `SP_ExtCarrito`, `SP_ExtCarritoAbiertos`, `SP_ExtDetalleCarrito`, `SP_ProximoNumeroCarrito`, `SP_CrearCarrito`, `SP_AgregarDetalleCarrito`, `SP_QuitarDetalleCarrito`, `SP_ActualizarTotalCarrito`, `SP_ActualizarEstadoCarrito` |
| Reserva | `SP_CrearReserva`, `SP_ExtReservaPorArticulo`, `SP_ActualizarEstadoReserva` |
| Orden de pago | `SP_ProximoNumeroOrdenPago`, `SP_CrearOrdenPago`, `SP_ExtOrdenPagoPendientes`, `SP_BuscarOrdenPago`, `SP_ActualizarEstadoOrdenPago` |
| Comprobante y pago | `SP_ProximoNumeroComprobante`, `SP_CrearComprobante`, `SP_AgregarDetalleComprobante`, `SP_BuscarComprobante`, `SP_ExtDetalleComprobante`, `SP_RegistrarPago`, `SP_ActualizarEstadoComprobante` |
| Entrega | `SP_CrearEntrega`, `SP_AgregarDetalleEntrega`, `SP_ExtEntregaPorComprobante` |

**Cada SP escribe una sola tabla.** Reemplaza a la regla de la versión 2, que pedía
SP transaccionales que tocaban varias tablas (`SP_CrearReserva` bloqueaba el artículo,
`SP_CrearComprobante` descontaba stock, `SP_CrearEntrega` marcaba el comprobante). Eso
rompía dos cosas:

- **El dígito verificador.** Un SP que modifica una fila de otra tabla deja su `dvh`
  viejo, y la tabla queda como corrupta en el próximo control de integridad.
- **El Singleton.** Si un SP descuenta stock o bloquea, `GestorDeExistencias_575_AV`
  deja de ser el único que lo hace.

Por eso: el stock y el bloqueo pasan **siempre** por el gestor, y cada cambio de estado
en otra tabla es una llamada propia de la BLL, seguida de `PersistirDigito` sobre esa
fila. Los diagramas de secuencia muestran exactamente ese orden.

---

## 3. Clases por capa

### Entidad_BE — todas implementan `IVerificable`

`ClienteBE_575_AV` · `ArticuloBE_575_AV` · `BobinaBE_575_AV` · `CategoriaBE_575_AV` ·
`MarcaBE_575_AV` · `UnidadMedidaBE_575_AV` · `MedioPagoBE_575_AV` ·
`CarritoBE_575_AV` · `DetalleCarritoBE_575_AV` · `ReservaBE_575_AV` ·
`OrdenPagoBE_575_AV` · `ComprobanteBE_575_AV` · `DetalleComprobanteBE_575_AV` ·
`PagoBE_575_AV` · `EntregaBE_575_AV` · `DetalleEntregaBE_575_AV`

`ResultadoPago_575_AV` (`aprobado` bool, `codigo_autorizacion` string, `motivo` string) no
se persiste y por eso no implementa `IVerificable`.

Enumeraciones: `CondicionIva_575_AV`, `EstadoArticulo_575_AV`, `EstadoCarrito_575_AV`,
`EstadoOrdenPago_575_AV`, `EstadoComprobante_575_AV`, `EstadoReserva_575_AV`,
`EstadoBobina_575_AV`.

### Acceso_DAL — un mapeador por agregado

`MP_Cliente_575_AV` · `MP_Articulo_575_AV` · `MP_Bobina_575_AV` · `MP_Carrito_575_AV` ·
`MP_Reserva_575_AV` · `MP_OrdenPago_575_AV` · `MP_Comprobante_575_AV` ·
`MP_Entrega_575_AV` · `MP_Catalogo_575_AV`

Mismo patrón que `MP_Bitacora`: `AccesoDatos` para ejecutar, `SqlParameter` para todo
parámetro, `DBNull.Value` para los criterios no informados, proyección fila por fila
sobre la BE.

### Negocio_BLL

| Clase | Responsabilidad |
|---|---|
| `ClienteBLL_575_AV` | Alta desde la caja (CUN-003) y búsqueda por DNI, que es la entrada del circuito de caja. Cifra el correo al guardarlo y lo descifra solo con el botón protegido por clave (T03.2). Valida CUIT y evita duplicados por CUIT o DNI. El ABM completo queda fuera de la Entrega 1. |
| `ArticuloBLL_575_AV` | Consulta y búsqueda con filtro por categoría (RFN1.1). `ValidarDisponibilidad` resuelve RFN1.2 contra `cantidad_disponible` y los saldos de bobina. |
| `BobinaBLL_575_AV` | `Fraccionar(articulo, medida)` resuelve RFN1.8: elige la bobina abierta con saldo suficiente o abre una nueva, descuenta y la cierra al agotarse. |
| `CarritoBLL_575_AV` | Crea el carrito, agrega y quita renglones, recalcula el total y lo confirma (CUN-001). |
| `ReservaBLL_575_AV` | Registra la reserva del faltante, **bloquea el artículo** y **emite el movimiento de reposición hacia el RFN2**. |
| `OrdenPagoBLL_575_AV` | Emite la orden de pago a partir del carrito confirmado (CUN-002, RFN1.4). |
| `VentaBLL_575_AV` | Valida el pedido contra la orden de pago, registra el cobro, descuenta stock, dispara el fraccionamiento y emite el comprobante (CUN-004; RFN1.5, RFN1.6, RFN1.7). |
| `EntregaBLL_575_AV` | Valida el comprobante, registra la entrega y lo marca como entregado (CUN-005, RFN1.9). |

Toda operación que escriba llama a `BitacoraBLL.RegistrarBitacora` y recalcula el DVH
por `VerificadorIntegridad`.

### Servicios

| Clase | Para qué |
|---|---|
| `EncriptadorReversible_575_AV` | AES-256 sobre el correo del cliente (T03.2). Clave fuera del código fuente. **No existe todavía.** |
| `AutorizadorPago_575_AV` | Pide la autorización del pago a la entidad bancaria. En el prototipo **la simula**: devuelve un `ResultadoPago_575_AV` (`aprobado`, `codigo_autorizacion`, `motivo`) aprobado o rechazado según los datos de prueba. Es la clase que aparece en el DS de CUN-004. |
| `GestorDeExistencias_575_AV` | **Singleton**, como pidió el docente. Punto único de verdad sobre la disponibilidad y el bloqueo de artículos: nadie descuenta stock ni bloquea por fuera de él. Evita que dos cajas vendan el mismo saldo de bobina. |

El resto (`Bitacora`, `VerificadorIntegridad`, `SessionManager`, `GestorDeIdioma`) se
reutiliza como está.

### Presentacion — un formulario por caso de uso

| Formulario | CU | Qué hace |
|---|---|---|
| `frmCarritoCompras_575_AV` | CUN-001 | Buscador con **filtro por categoría**, grilla de resultados resaltando los sin stock, grilla del carrito, cantidad, "Agregar", "Quitar" y "Confirmar carrito". Ante faltante, ofrece reservar. **No se llama "Llenar carrito".** |
| `frmOrdenPago_575_AV` | CUN-002 | Detalle del carrito confirmado, nombre del cliente, botón "Emitir". Muestra "Se generó la orden de pago Nro. xxx". |
| `frmRegistrarCliente_575_AV` | CUN-003 | Alta de cliente. Se abre desde CUN-004 cuando el DNI no existe. |
| `frmValidarCobrar_575_AV` | CUN-004 | Ingreso de DNI, detalle de la orden de pago, medios de pago, autorización, emisión del comprobante. |
| `frmEntregaDeposito_575_AV` | CUN-005 | Ingreso del número de comprobante, validación, artículos a entregar con su ubicación, "Registrar entrega". Contempla entrega parcial. |

Los ítems `MENU_CLIENTE`, `MENU_PRODUCTOS`, `MENU_LLENAR_CARRITO` y
`MENU_REALIZAR_VENTA` ya existen traducidos en `frmMenu` pero no abren nada. Hay que
engancharlos y **renombrar las claves de traducción** que hablan de "llenar carrito".

---

## 4. Orden de implementación

1. **Catálogos y artículo.** Tablas, SP, BE, DAL, BLL y ABM de artículo. Incluye
   `Bobina`, el fraccionamiento y el `GestorDeExistencias_575_AV`.
2. **Cliente**, junto con `EncriptadorReversible_575_AV`, que cierra T03.2.
3. **Carrito y reserva** (CUN-001), con el bloqueo de artículo.
4. **Orden de pago** (CUN-002).
5. **Validar y cobrar** (CUN-004), que arrastra CUN-003.
6. **Entrega** (CUN-005).

Cada fase deja el proyecto compilando y el menú usable. Datos de prueba en un
`db/05_datos_negocio.sql` nuevo, con el DVH recalculable desde *Recalcular Dígitos*
(CUS-012).

**El docente va a probar sobre el requerimiento**, así que el circuito completo tiene
que correr de punta a punta: buscar un artículo fraccionable, armar el carrito,
emitir la orden de pago, cobrarla, emitir el comprobante y entregarlo en depósito.

---

## 5. Contrato diagrama ↔ código

La cátedra controla que diagramas y código digan **exactamente lo mismo**. Los diagramas
de `T.DIPLOMA\Diagramas\N02.1.2.x.b/c/d_*_575_AV.puml` son el contrato:

- **Mismos nombres**: clases, métodos, parámetros, SP y valores de `TipoAccion`.
- **Mismo orden de llamadas** en cada escritura.
- **Si hace falta cambiar un nombre o agregar un método, no lo decidas solo.** Frená y
  avisale a Agustín, porque hay que corregir el diagrama y volver a dibujarlo en
  Enterprise Architect.

### 5.1 Patrón de toda escritura (ya es el de la Fase 1)

Es el que usan hoy `ArticuloBLL_575_AV` y `BobinaBLL_575_AV`, y es el que dibujan los DS:

```csharp
articulo.id_articulo = mpArticulo.CrearArticulo(articulo);   // 1. escribe (un SP, una tabla)
PersistirDigito(articulo);                                   // 2. T08
bitacora.RegistrarBitacora(usuario, TipoAccion.AltaArticulo); // 3. T06

private void PersistirDigito(ArticuloBE_575_AV articulo)
{
    articulo.dvh = VerificadorIntegridad.CalcularDVH(articulo);   // SHA-256 de ObtenerCamposDV()
    mpArticulo.ActualizarDVH(articulo.id_articulo, articulo.dvh); // SP_ActualizarDVHNegocio
    verificador.ActualizarDVV(TABLA);                             // VerificadorIntegridadBLL
}
```

- Cada BLL de negocio tiene `VerificarIntegridad()`, `RecalcularDV()` y `PersistirDigito`
  privado.
- Las BLL que escriben varias tablas (`VentaBLL_575_AV`, `EntregaBLL_575_AV`) usan
  `PersistirDigito(entidad : IVerificable, tabla : string)`.
- Si el valor de la fila lo calcula el SP (un total, un estado por defecto), la BLL
  **relee la fila** antes de calcular el `dvh`, como hace hoy `MoverCantidad`.

**Un `ActualizarDVH` por tabla.** Cuando un mapeador atiende más de una tabla, cada una
tiene su método. Todos llaman a `SP_ActualizarDVHNegocio` con su `@tabla`:

| Mapeador | Métodos | Tabla |
|---|---|---|
| `MP_Carrito_575_AV` | `ActualizarDVH` / `ActualizarDVHDetalle` | `Carrito` / `DetalleCarrito` |
| `MP_Comprobante_575_AV` | `ActualizarDVH` / `ActualizarDVHDetalle` / `ActualizarDVHPago` | `Comprobante` / `DetalleComprobante` / `Pago` |
| `MP_Entrega_575_AV` | `ActualizarDVH` / `ActualizarDVHDetalle` | `Entrega` / `DetalleEntrega` |

**Toda tabla nueva se da de alta en `DigitoVertical`** en el script de esquema, con un
DVV inicial.

### 5.2 Bitácora al abrir cada formulario

Como ya hacen `frmBitacora` y la gestión de artículos, cada formulario del circuito
registra su apertura. Lo hace la GUI, llamando a `BitacoraBLL` con el usuario de
`SessionManager.GetInstance.UsuarioActual().user`. `frmRegistrarCliente_575_AV` no
registra apertura, porque se abre desde la caja.

### 5.3 Valores nuevos de `TipoAccion`

Se agregan a continuación de `LiberacionArticulo = 26`, **con estos números**. La
bitácora guarda el entero, así que no se renumera nada que ya exista.

| Valor | Nº | Dónde se registra |
|---|---|---|
| `AltaCliente` | 27 | `ClienteBLL.CrearCliente` |
| `ModificacionCliente` | 28 | Reservado: ABM de cliente, fuera de la Entrega 1 |
| `BajaCliente` | 29 | Reservado: ABM de cliente, fuera de la Entrega 1 |
| `CarritoComprasAbierto` | 30 | `frmCarritoCompras_575_AV` al abrir |
| `AltaCarrito` | 31 | `CarritoBLL.CrearCarrito` |
| `AltaDetalleCarrito` | 32 | `CarritoBLL.AgregarDetalle` |
| `BajaDetalleCarrito` | 33 | `CarritoBLL.QuitarDetalle` |
| `ConfirmacionCarrito` | 34 | `CarritoBLL.ConfirmarCarrito` |
| `AltaReserva` | 35 | `ReservaBLL.RegistrarReserva` |
| `OrdenPagoAbierta` | 36 | `frmOrdenPago_575_AV` al abrir |
| `EmisionOrdenPago` | 37 | `OrdenPagoBLL.GenerarOrdenPago` |
| `CajaAbierta` | 38 | `frmValidarCobrar_575_AV` al abrir |
| `CobroRegistrado` | 39 | `VentaBLL.Cobrar`, pago acreditado |
| `CobroRechazado` | 40 | `VentaBLL.Cobrar`, pago rechazado |
| `EmisionComprobante` | 41 | `VentaBLL.Cobrar`, al emitir el comprobante |
| `DepositoAbierto` | 42 | `frmEntregaDeposito_575_AV` al abrir |
| `EntregaRegistrada` | 43 | `EntregaBLL.RegistrarEntrega`, entrega total |
| `EntregaParcial` | 44 | `EntregaBLL.RegistrarEntrega`, entrega parcial |
| `DesencriptacionCorreo` | 45 | `ClienteBLL.DesencriptarCorreo`, al mostrar el correo descifrado (T03.2) |

Los que ya existen y aparecen en los DS: `MovimientoStock` (22),
`FraccionamientoBobina` (24) y `BloqueoArticulo` (25). Si `frmBitacora` o el filtro de
eventos traducen las acciones, agregar las claves de traducción de los valores nuevos.

### 5.4 Métodos públicos, exactamente como en los diagramas

Están en notación de diagrama: `Nombre(parametro : Tipo) : Retorno` equivale en C# a
`Retorno Nombre(Tipo parametro)`. Las clases de la Fase 1 (`ArticuloBLL_575_AV`,
`BobinaBLL_575_AV`, `MP_Articulo_575_AV`, `MP_Bobina_575_AV`, `MP_Catalogo_575_AV`,
`GestorDeExistencias_575_AV`) ya coinciden con los diagramas. Lo único que se les
agrega es lo del punto 5.5.

**`ClienteBLL_575_AV`**

```
+ BuscarPorDni(dni : string) : ClienteBE_575_AV
+ CrearCliente(cliente : ClienteBE_575_AV, usuario : string) : int
+ DesencriptarCorreo(cliente : ClienteBE_575_AV, clave : string, usuario : string) : string
+ VerificarIntegridad() : bool
+ RecalcularDV()
- Validar(cliente : ClienteBE_575_AV)
- ValidarCuit(cuit : string) : bool
- PersistirDigito(cliente : ClienteBE_575_AV)
```

**T03.2 — botón de desencriptar protegido por clave.** En `frmValidarCobrar_575_AV`, los
datos del cliente muestran el correo cifrado y un botón *Ver correo*. Al presionarlo se
pide la contraseña del usuario en sesión. `DesencriptarCorreo` compara
`Encriptador.EncriptarIrrev(clave)` con la del usuario de `SessionManager`: si no
coincide, lanza una excepción y no descifra; si coincide, devuelve
`EncriptadorReversible_575_AV.Desencriptar(cliente.correo_electronico)` y registra
`TipoAccion.DesencriptacionCorreo`. No reutiliza `UsuarioBLL.VerifUsuario`, que bloquea
al usuario y cambia la clave según el caso.

**`CarritoBLL_575_AV`**

```
+ CrearCarrito(usuario : string) : CarritoBE_575_AV
+ AgregarDetalle(carrito : CarritoBE_575_AV, detalle : DetalleCarritoBE_575_AV, usuario : string) : CarritoBE_575_AV
+ QuitarDetalle(carrito : CarritoBE_575_AV, detalle : DetalleCarritoBE_575_AV, usuario : string) : CarritoBE_575_AV
+ ConfirmarCarrito(carrito : CarritoBE_575_AV, usuario : string) : string
+ ObtenerDetalle(id_carrito : int) : List<DetalleCarritoBE_575_AV>
+ VerificarIntegridad() : bool
+ RecalcularDV()
- Validar(carrito : CarritoBE_575_AV)
- PersistirDigito(carrito : CarritoBE_575_AV)
- PersistirDigito(detalle : DetalleCarritoBE_575_AV)
```

**`ReservaBLL_575_AV`**

```
+ RegistrarReserva(reserva : ReservaBE_575_AV, usuario : string) : ReservaBE_575_AV
+ VerificarIntegridad() : bool
+ RecalcularDV()
- PersistirDigito(reserva : ReservaBE_575_AV)
```

**`OrdenPagoBLL_575_AV`**

```
+ GenerarOrdenPago(carrito : CarritoBE_575_AV, nombre_cliente : string, usuario : string) : OrdenPagoBE_575_AV
+ BuscarOrdenPago(nro_orden_pago : string) : OrdenPagoBE_575_AV
+ VerificarIntegridad() : bool
+ RecalcularDV()
- Validar(carrito : CarritoBE_575_AV)
- PersistirDigito(orden : OrdenPagoBE_575_AV)
```

**`VentaBLL_575_AV`**

```
+ Cobrar(orden : OrdenPagoBE_575_AV, cliente : ClienteBE_575_AV, detalles : List<DetalleCarritoBE_575_AV>, medio_pago : MedioPagoBE_575_AV, datos_pago : string, usuario : string) : ComprobanteBE_575_AV
+ VerificarIntegridad() : bool
+ RecalcularDV()
- PersistirDigito(entidad : IVerificable, tabla : string)
```

**`EntregaBLL_575_AV`**

```
+ ValidarComprobante(nro_comprobante : string) : List<DetalleComprobanteBE_575_AV>
+ RegistrarEntrega(comprobante : ComprobanteBE_575_AV, detalles : List<DetalleEntregaBE_575_AV>, usuario : string) : EntregaBE_575_AV
+ VerificarIntegridad() : bool
+ RecalcularDV()
- PersistirDigito(entidad : IVerificable, tabla : string)
```

**`MP_Cliente_575_AV`**

```
+ BuscarPorDni(dni : string) : ClienteBE_575_AV
+ BuscarPorCuit(cuit : string) : ClienteBE_575_AV
+ CrearCliente(cliente : ClienteBE_575_AV) : int
+ ActualizarDVH(id_cliente : int, dvh : string)
```

**`MP_Carrito_575_AV`**

```
+ ProximoNumero() : string
+ CrearCarrito(carrito : CarritoBE_575_AV) : int
+ AgregarDetalle(detalle : DetalleCarritoBE_575_AV) : int
+ QuitarDetalle(id_detalle_carrito : int)
+ ActualizarTotal(id_carrito : int, precio_total : decimal)
+ ActualizarEstado(id_carrito : int, estado : EstadoCarrito_575_AV)
+ ListarDetalle(id_carrito : int) : List<DetalleCarritoBE_575_AV>
+ ActualizarDVH(id_carrito : int, dvh : string)
+ ActualizarDVHDetalle(id_detalle_carrito : int, dvh : string)
```

**`MP_Reserva_575_AV`**

```
+ CrearReserva(reserva : ReservaBE_575_AV) : int
+ ActualizarDVH(id_reserva : int, dvh : string)
```

**`MP_OrdenPago_575_AV`**

```
+ ProximoNumero() : string
+ CrearOrdenPago(orden : OrdenPagoBE_575_AV) : int
+ BuscarPorNumero(nro_orden_pago : string) : OrdenPagoBE_575_AV
+ ActualizarEstado(id_orden_pago : int, estado : EstadoOrdenPago_575_AV)
+ ActualizarDVH(id_orden_pago : int, dvh : string)
```

**`MP_Comprobante_575_AV`**

```
+ ProximoNumero() : string
+ CrearComprobante(comprobante : ComprobanteBE_575_AV) : int
+ AgregarDetalle(detalle : DetalleComprobanteBE_575_AV) : int
+ RegistrarPago(pago : PagoBE_575_AV) : int
+ BuscarPorNumero(nro_comprobante : string) : ComprobanteBE_575_AV
+ ListarDetalle(id_comprobante : int) : List<DetalleComprobanteBE_575_AV>
+ ActualizarEstado(id_comprobante : int, estado : EstadoComprobante_575_AV)
+ ActualizarDVH(id_comprobante : int, dvh : string)
+ ActualizarDVHDetalle(id_detalle_comprobante : int, dvh : string)
+ ActualizarDVHPago(id_pago : int, dvh : string)
```

**`MP_Entrega_575_AV`**

```
+ CrearEntrega(entrega : EntregaBE_575_AV) : int
+ AgregarDetalle(detalle : DetalleEntregaBE_575_AV) : int
+ ActualizarDVH(id_entrega : int, dvh : string)
+ ActualizarDVHDetalle(id_detalle_entrega : int, dvh : string)
```

**`EncriptadorReversible_575_AV`**

```
+ {static} Encriptar(texto : string) : string
+ {static} Desencriptar(cifrado : string) : string
```

**`AutorizadorPago_575_AV`**

```
+ Autorizar(medio_pago : MedioPagoBE_575_AV, datos_pago : string, monto : decimal) : ResultadoPago_575_AV
```

### 5.5 Integridad de **todas** las tablas al iniciar sesión — corregir ya

Hoy `frmLogin` llama a `VerificadorIntegridadBLL.VerificarIntegridad()` y a
`RecalcularDV()`, y los dos tienen **"Usuarios" escrito fijo en el código**. Resultado:
si alguien modifica a mano una fila de `Articulo` o de `Bobina`, el sistema no lo
detecta. `ArticuloBLL_575_AV.VerificarIntegridad()` existe, pero nadie la llama. La
cátedra pide dígito verificador **sobre todas las tablas**.

Qué hacer, sin romper lo que anda:

1. En `VerificadorIntegridadBLL` agregar
   `VerificarIntegridadTotal() : List<string>`. Devuelve los nombres de las tablas que
   no pasan el control; si la lista está vacía, todo está bien. Recorre `Usuarios` (con
   el método actual) y cada BLL de negocio: `Categoria`, `Marca`, `UnidadMedida` y
   `MedioPago` (por `ArticuloBLL`, que atiende los catálogos), `Articulo`, `Bobina`, y
   las tablas de las fases siguientes a medida que existan.
2. Agregar `RecalcularDVTotal()`, con el mismo recorrido, llamando a `RecalcularDV()` de
   cada BLL.
3. **Instanciar las BLL de negocio adentro de esos métodos**, como hoy se hace con
   `UsuarioBLL`, y **no como campos de la clase**. `ArticuloBLL_575_AV` ya tiene un campo
   `VerificadorIntegridadBLL`; si este tuviera a su vez un campo `ArticuloBLL_575_AV`,
   construir cualquiera de los dos entraría en recursión infinita.
4. En `frmLogin`, usar los métodos nuevos. El mensaje de error nombra la tabla afectada,
   sin detalles técnicos.
5. Los catálogos todavía no tienen `VerificarIntegridad()` ni `RecalcularDV()`. Agregarlos
   en `ArticuloBLL_575_AV`, junto con los `ActualizarDVH` que les falten en
   `MP_Catalogo_575_AV`.

Esto cambia el diagrama de secuencia del Login (T02.4.4), que hay que actualizar en el
documento. **Hacerlo antes de la Fase 2**: afecta a todas las tablas que vienen.

---

## 6. Lo que queda fuera de esta especificación

- **El aviso por umbral de reposición con patrón Observer y su dashboard** pertenece al
  RFN2 y se documenta en la Entrega 2. Conviene que `GestorDeExistencias_575_AV` ya
  exponga el evento, aunque todavía no haya quien lo escuche.
- **El ABM completo de cliente.** En la Entrega 1 el cliente solo se da de alta desde la
  caja (CUN-003) y se busca por DNI. No se programan modificación ni baja, ni sus SP
  (`SP_ActualizarCliente`, `SP_ElimCliente`), y la opción *Cliente* del menú queda
  oculta. `ModificacionCliente` (28) y `BajaCliente` (29) quedan reservados en
  `TipoAccion` para no mover la numeración.
- **La bitácora de cambios con triggers** (T06.B) se documenta en la Entrega 2 y se
  programa en la Entrega 3.
- **El comprobante no se integra con ARCA.** Se emite como *comprobante no válido como
  factura*: nada de CAE, ni de servicios web del organismo.
