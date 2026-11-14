# Contrato diagrama ↔ código — RFN2 Gestión de Compras

**Para la Entrega 2.** El RFN2 se diagrama en la Entrega 1 y se programa después. Este archivo fija los nombres de clases, métodos, SP, tablas y formularios del RFN2. El código de la Entrega 2 los respeta sin renombrar nada, y los diagramas de Enterprise Architect se generan después a partir del código. Regenerado el 25/09/2026 a partir de los mismos diagramas (versión sin reservas).

Rigen las mismas reglas que en el RFN1 (`ESPEC_RFN1_Ventas_575_AV.md`, sección 5): cada SP escribe una sola tabla, toda escritura sigue con `PersistirDigito` y `RegistrarBitacora`, y cada formulario registra su apertura.

## Valores nuevos de `TipoAccion`

Continúan la numeración existente: el RFN1 llega hasta 45 (`DesencriptacionCorreo`) y T08 usa 46 (`RecalculoDigitoVerificador`) y 47 (`SistemaNoDisponible`). Corregido el 06/10/2026: antes empezaba en 46 y chocaba con T08.

| Valor | Nº |
|---|---|
| `GestionProveedoresAbierta` | 48 |
| `AltaProveedor` | 49 |
| `ModificacionProveedor` | 50 |
| `ImportarListaAbierta` | 51 |
| `ImportacionListaPrecios` | 52 |
| `CompararPreciosAbierta` | 53 |
| `AltaOrdenCompra` | 54 |
| `AltaDetalleOrdenCompra` | 55 |
| `OrdenesCompraAbierta` | 56 |
| `AutorizacionesAbierta` | 57 |
| `AutorizacionOrdenCompra` | 58 |
| `RechazoOrdenCompra` | 59 |
| `RecepcionAbierta` | 60 |
| `RecepcionMercaderia` | 61 |
| `RecepcionParcial` | 62 |
| `ImportacionListaRevertida` | 63 |
| `EmisionOrdenCompra` | 64 |

Ya existentes que usa el RFN2: `MovimientoStock`, `AltaBobina`.

## Stored procedures nuevos

`SP_ActualizarEstadoOrdenCompra`, `SP_ActualizarProveedor`, `SP_ActualizarRecibidoOrdenCompra`, `SP_ActualizarTotalOrdenCompra`, `SP_AgregarDetalleOrdenCompra`, `SP_AgregarDetalleRecepcion`, `SP_AgregarItemListaPrecio`, `SP_BuscarOrdenCompraBorrador`, `SP_BuscarPrecioVigente`, `SP_BuscarProveedorPorCuit`, `SP_CompararPreciosArticulo`, `SP_CrearListaPrecio`, `SP_CrearOrdenCompra`, `SP_CrearProveedor`, `SP_CrearRecepcion`, `SP_DesactivarListaPrecio`, `SP_EmitirOrdenCompra`, `SP_ExtDetalleOrdenCompra`, `SP_ExtDetalleRecepcion`, `SP_ExtItemListaPrecio`, `SP_ExtListaPrecio`, `SP_ExtOrdenCompraPorEstado`, `SP_ExtProveedor`, `SP_ExtRecepcion`, `SP_ProximoNumeroOrdenCompra`, `SP_RegistrarAutorizacionOrdenCompra`

Cada uno escribe una sola tabla. `SP_ExtListaPrecio`, `SP_ExtItemListaPrecio`, `SP_ExtRecepcion` y `SP_ExtDetalleRecepcion` son de solo lectura y los usa T08 para verificar y recalcular el dígito.

## Clases que ya existen y suman métodos

- `ArticuloBLL_575_AV`: `ListarParaReponer() : List<ArticuloBE_575_AV>`.
- `ItemListaPrecioBE_575_AV`: `MultiploDe(unidad_venta : string) : decimal` (estático).
- `GestorDeExistencias_575_AV.Reponer` y `BobinaBLL_575_AV.CrearBobina` **ya están programados** y son los que usa la recepción.

## Métodos de las clases nuevas

En notación de diagrama: `Nombre(parametro : Tipo) : Retorno`. Sacados del código del commit `6d868bb` (08/10/2026); los privados auxiliares de formato se omiten.

**`ProveedorBLL_575_AV`**

```
+ ListarProveedores() : List<ProveedorBE_575_AV>
+ BuscarPorCuit(cuit : string) : ProveedorBE_575_AV
+ CrearProveedor(proveedor : ProveedorBE_575_AV, usuario : string) : int
+ ModificarProveedor(proveedor : ProveedorBE_575_AV, usuario : string)
+ VerificarIntegridad() : bool
+ RecalcularDV()
- PersistirDigito(proveedor : ProveedorBE_575_AV)
- Validar(proveedor : ProveedorBE_575_AV)
- ValidarCuit(cuit : string) : bool
- ValidarCorreo(correo : string)
```

**`ListaPrecioBLL_575_AV`**

```
+ Previsualizar(ruta : string, id_proveedor : int) : ResumenImportacion_575_AV
+ Importar(lista : ListaPrecioBE_575_AV, items : List<ItemListaPrecioBE_575_AV>, usuario : string) : ResumenImportacion_575_AV
- Revertir(lista : ListaPrecioBE_575_AV, usuario : string)
+ CompararPrecios(id_articulo : int, moneda : string) : List<ComparacionPrecio_575_AV>
+ ObtenerPrecioVigente(id_proveedor : int, id_articulo : int) : ItemListaPrecioBE_575_AV
+ VerificarIntegridad() : bool
+ RecalcularDV()
- PersistirDigito(entidad : IVerificable, tabla : string)
- PersistirDVH(entidad : IVerificable, tabla : string)
```

**`OrdenCompraBLL_575_AV`**

```
+ AgregarArticuloABorrador(id_proveedor : int, detalle : DetalleOrdenCompraBE_575_AV, usuario : string) : OrdenCompraBE_575_AV
+ ObtenerBorrador(id_proveedor : int) : OrdenCompraBE_575_AV
+ GenerarOrdenCompra(orden : OrdenCompraBE_575_AV, detalles : List<DetalleOrdenCompraBE_575_AV>, usuario : string) : OrdenCompraBE_575_AV
+ ListarPendientes() : List<OrdenCompraBE_575_AV>
+ ListarParaRecepcion() : List<OrdenCompraBE_575_AV>
+ ObtenerDetalle(id_orden_compra : int) : List<DetalleOrdenCompraBE_575_AV>
- ConDetalle(ordenes : List<OrdenCompraBE_575_AV>) : List<OrdenCompraBE_575_AV>
+ Autorizar(orden : OrdenCompraBE_575_AV, usuario : string)
+ Rechazar(orden : OrdenCompraBE_575_AV, motivo : string, usuario : string)
- Pendiente(orden : OrdenCompraBE_575_AV) : OrdenCompraBE_575_AV
- Validar(orden : OrdenCompraBE_575_AV)
- AjustarAMultiplo(detalle : DetalleOrdenCompraBE_575_AV)
- Releer(id_orden_compra : int) : OrdenCompraBE_575_AV
+ VerificarIntegridad() : bool
+ RecalcularDV()
- PersistirDigito(entidad : IVerificable, tabla : string)
```

**`RecepcionBLL_575_AV`**

```
+ RegistrarRecepcion(orden : OrdenCompraBE_575_AV, recepcion : RecepcionBE_575_AV, detalles : List<DetalleRecepcionBE_575_AV>, usuario : string) : RecepcionBE_575_AV
- Validar(orden : OrdenCompraBE_575_AV, detalles : List<DetalleRecepcionBE_575_AV>)
- Diferencias(renglones : List<DetalleOrdenCompraBE_575_AV>, recibidos : List<DetalleRecepcionBE_575_AV>) : string
- CrearBobinas(id_proveedor : int, id_recepcion : int, recibido : DetalleRecepcionBE_575_AV, usuario : string)
+ VerificarIntegridad() : bool
+ RecalcularDV()
- PersistirDigito(entidad : IVerificable, tabla : string)
- PersistirDVH(entidad : IVerificable, tabla : string)
```

**`MP_Proveedor_575_AV`**

```
+ ListarProveedores() : List<ProveedorBE_575_AV>
+ BuscarPorCuit(cuit : string) : ProveedorBE_575_AV
+ CrearProveedor(proveedor : ProveedorBE_575_AV) : int
+ ActualizarProveedor(proveedor : ProveedorBE_575_AV)
+ ActualizarDVH(id_proveedor : int, dvh : string)
```

**`MP_ListaPrecio_575_AV`**

```
+ CrearLista(lista : ListaPrecioBE_575_AV) : int
+ DesactivarLista(id_lista_precio : int)
+ AgregarItem(item : ItemListaPrecioBE_575_AV) : int
+ ListarPreciosPorArticulo(id_articulo : int, moneda : string) : List<ComparacionPrecio_575_AV>
+ BuscarPrecioVigente(id_proveedor : int, id_articulo : int) : ItemListaPrecioBE_575_AV
+ ListarListas() : List<ListaPrecioBE_575_AV>
+ ListarItems() : List<ItemListaPrecioBE_575_AV>
+ ActualizarDVH(id_lista_precio : int, dvh : string)
+ ActualizarDVHItem(id_item_lista : int, dvh : string)
```

**`MP_OrdenCompra_575_AV`**

```
+ ProximoNumero() : string
+ BuscarBorrador(id_proveedor : int) : OrdenCompraBE_575_AV
+ CrearOrdenCompra(orden : OrdenCompraBE_575_AV) : int
+ AgregarDetalle(detalle : DetalleOrdenCompraBE_575_AV) : int
+ ActualizarTotal(id_orden_compra : int, total : decimal)
+ Emitir(id_orden_compra : int, nro_orden_compra : string, total : decimal)
+ ListarPorEstado(estados : EstadoOrdenCompra_575_AV[]) : List<OrdenCompraBE_575_AV>
+ ListarDetalle(id_orden_compra : int) : List<DetalleOrdenCompraBE_575_AV>
+ ListarDetalles() : List<DetalleOrdenCompraBE_575_AV>
+ RegistrarAutorizacion(orden : OrdenCompraBE_575_AV)
+ ActualizarCantidadRecibida(id_detalle_orden_compra : int, cantidad : decimal)
+ ActualizarEstado(id_orden_compra : int, estado : EstadoOrdenCompra_575_AV)
+ ActualizarDVH(id_orden_compra : int, dvh : string)
+ ActualizarDVHDetalle(id_detalle_orden_compra : int, dvh : string)
```

**`MP_Recepcion_575_AV`**

```
+ CrearRecepcion(recepcion : RecepcionBE_575_AV) : int
+ AgregarDetalle(detalle : DetalleRecepcionBE_575_AV) : int
+ ListarRecepciones() : List<RecepcionBE_575_AV>
+ ListarDetalles() : List<DetalleRecepcionBE_575_AV>
+ ActualizarDVH(id_recepcion : int, dvh : string)
+ ActualizarDVHDetalle(id_detalle_recepcion : int, dvh : string)
```

**`LectorListaPrecios_575_AV`**

```
+ Leer(ruta : string) : List<ItemListaPrecioBE_575_AV>
- LeerXlsx(ruta : string) : List<Fila>
- LeerCsv(ruta : string) : List<Fila>
- Mapear(filas : List<Fila>, desdeXlsx : bool) : List<ItemListaPrecioBE_575_AV>
```

## Tablas nuevas

Todas con `dvh` y su fila en `DigitoVertical`.

| Tabla | Columnas |
|---|---|
| `Proveedor` | id_proveedor, razon_social, cuit (único), condicion_iva, rubro, telefono, correo_electronico (cifrado AES-256), direccion, activo |
| `ListaPrecio` | id_lista_precio, id_proveedor, archivo, fecha_vigencia, moneda (ARS/USD), fecha_importacion, usuario_importacion, activo |
| `ItemListaPrecio` | id_item_lista, id_lista_precio, id_articulo (vacío si el artículo es nuevo), codigo_proveedor, descripcion, precio_lista, unidad_venta, plazo_entrega |
| `OrdenCompra` | id_orden_compra, nro_orden_compra (único cuando tiene valor), id_proveedor, fecha_emision, usuario_emisor, estado varchar(25), total, fecha_autorizacion, usuario_autorizante, observaciones, activo |
| `DetalleOrdenCompra` | id_detalle_orden_compra, id_orden_compra, id_articulo, cantidad, id_unidad_medida, precio_unitario, subtotal, cantidad_recibida |
| `Recepcion` | id_recepcion, id_orden_compra, remito_proveedor, fecha_recepcion, usuario_deposito, diferencias |
| `DetalleRecepcion` | id_detalle_recepcion, id_recepcion, id_articulo, cantidad_recibida, id_unidad_medida |

## Estados de la orden de compra (`EstadoOrdenCompra_575_AV`)

`Borrador` → `PendienteAutorizacion` → `Autorizada` o `Rechazada` → `RecibidaParcial` → `Recibida`. El nombre `PendienteAutorizacion` sale de las especificaciones de CUN-010 y CUN-011.

## Reglas de negocio fijadas al programar (08/10/2026)

- **Lista vencida:** más de 30 días desde la fecha de vigencia (constante `DIAS_VIGENCIA`).
- **Comparación de precios:** se filtra por moneda (ARS/USD), sin conversión.
- **Importación fallida:** la lista queda inactiva (`SP_DesactivarListaPrecio`, `TipoAccion` 63) y no entra en comparaciones ni en precio vigente.
- **Dígito vertical de los detalles:** el DVH se calcula por renglón y el DVV una sola vez al final de la operación.
- **Unidad de medida del renglón de la orden:** la del artículo. No hay conversión entre unidades (metro ↔ caja de 305 m).
- **Múltiplo de compra:** sale del "x N" de la unidad de venta del proveedor ("Caja x 305 m" da 305; sin "x N", 1). La misma regla define el tamaño de las bobinas en la recepción.
- **Limitación conocida:** cada SP tiene su propia transacción, igual que en el RFN1. Se valida todo antes de grabar; un error de la base a mitad de una operación lo detecta el control de integridad.

## Formularios

| Caso de uso | Formulario | Actor |
|---|---|---|
| CUN-007 Registrar Proveedor | `frmProveedor_575_AV` | Administrador de compras |
| CUN-008 Importar Lista de Precios | `frmImportarListaPrecios_575_AV` | Administrador de compras |
| CUN-009 Comparar Precios | `frmCompararPrecios_575_AV` | Administrador de compras |
| CUN-010 Generar Orden de Compra | `frmOrdenCompra_575_AV` | Administrador de compras |
| CUN-011 Autorizar Orden de Compra | `frmAutorizarOrdenCompra_575_AV` | Propietario |
| CUN-012 Registrar Recepción de Mercadería | `frmRecepcionMercaderia_575_AV` | Encargado de depósito |

## Permisos (`TipoPermiso`)

Valores nuevos, uno por caso de uso, con la misma fila en `03_datos_seguridad.sql`:

| Permiso | Familia |
|---|---|
| `RegistrarProveedor` | `AdministradorCompras` |
| `ImportarListaPrecios` | `AdministradorCompras` |
| `CompararPrecios` | `AdministradorCompras` |
| `GenerarOrdenCompra` | `AdministradorCompras` |
| `AutorizarOrdenCompra` | `Propietario` |
| `RegistrarRecepcion` | `Deposito` |

Las familias `AdministradorCompras` y `Propietario` ya existen en el script 03 y pasan a colgar de `Admin`, igual que `Ventas`, `Caja` y `Deposito`.

## Lectura de listas de precios

`LectorListaPrecios_575_AV` vive en Servicios y **no usa paquetes NuGet ni librerías de terceros**:

- `.csv`: lectura de texto con separador `;` o `,`.
- `.xlsx`: el archivo es un ZIP con XML adentro. Se abre con `System.IO.Compression.ZipArchive` y se leen `xl/sharedStrings.xml` y la primera hoja con `System.Xml`. Las dos son referencias del propio .NET Framework, no NuGet. Se toman los valores guardados de las celdas; no se evalúan fórmulas.
