# Contrato diagrama ↔ código — RFN2 Gestión de Compras

**Para la Entrega 2.** El RFN2 se diagrama en la Entrega 1 y se programa después. Este archivo fija los nombres que usan los diagramas `N02.2.2.x.b/c/d_*_575_AV.puml`, para que el código de la Entrega 2 los respete sin renombrar nada. Generado el 24/09/2026 a partir de los mismos diagramas.

Rigen las mismas reglas que en el RFN1 (`ESPEC_RFN1_Ventas_575_AV.md`, sección 5): cada SP escribe una sola tabla, toda escritura sigue con `PersistirDigito` y `RegistrarBitacora`, y cada formulario registra su apertura.

## Valores nuevos de `TipoAccion`

Continúan la numeración del RFN1, que llega hasta 45 (`DesencriptacionCorreo`).

| Valor | Nº |
|---|---|
| `GestionProveedoresAbierta` | 46 |
| `AltaProveedor` | 47 |
| `ModificacionProveedor` | 48 |
| `ImportarListaAbierta` | 49 |
| `ImportacionListaPrecios` | 50 |
| `CompararPreciosAbierta` | 51 |
| `AltaOrdenCompra` | 52 |
| `AltaDetalleOrdenCompra` | 53 |
| `OrdenesCompraAbierta` | 54 |
| `AutorizacionesAbierta` | 55 |
| `AutorizacionOrdenCompra` | 56 |
| `RechazoOrdenCompra` | 57 |
| `RecepcionAbierta` | 58 |
| `LiberacionReserva` | 59 |
| `RecepcionMercaderia` | 60 |
| `RecepcionParcial` | 61 |

Ya existentes que usa el RFN2: `MovimientoStock`, `AltaBobina`, `LiberacionArticulo`.

## Stored procedures nuevos

`SP_ActualizarEstadoOrdenCompra`, `SP_ActualizarProveedor`, `SP_ActualizarRecibidoOrdenCompra`, `SP_ActualizarTotalOrdenCompra`, `SP_AgregarDetalleOrdenCompra`, `SP_AgregarDetalleRecepcion`, `SP_AgregarItemListaPrecio`, `SP_BuscarOrdenCompraBorrador`, `SP_BuscarPrecioVigente`, `SP_BuscarProveedorPorCuit`, `SP_CompararPreciosArticulo`, `SP_CrearListaPrecio`, `SP_CrearOrdenCompra`, `SP_CrearProveedor`, `SP_CrearRecepcion`, `SP_EmitirOrdenCompra`, `SP_ExtDetalleOrdenCompra`, `SP_ExtOrdenCompraPorEstado`, `SP_ExtProveedor`, `SP_ProximoNumeroOrdenCompra`, `SP_RegistrarAutorizacionOrdenCompra`

## Clases que ya existen y suman métodos

- `ArticuloBLL_575_AV`: `ListarParaReponer() : List<ArticuloBE_575_AV>`.
- `ReservaBLL_575_AV`: `ListarPendientesPorArticulo`, `MarcarDisponible`. `MP_Reserva_575_AV`: `ListarPorArticulo`, `ActualizarEstado`.
- `GestorDeExistencias_575_AV.Reponer`, `Liberar` y `BobinaBLL_575_AV.CrearBobina` **ya están programados** y son los que usa la recepción.

## Métodos de las clases nuevas

En notación de diagrama: `Nombre(parametro : Tipo) : Retorno`.

**`ProveedorBLL_575_AV`**

```
+ ListarProveedores() : List<ProveedorBE_575_AV>
+ BuscarPorCuit(cuit : string) : ProveedorBE_575_AV
+ CrearProveedor(proveedor : ProveedorBE_575_AV, usuario : string) : int
+ ModificarProveedor(proveedor : ProveedorBE_575_AV, usuario : string)
+ VerificarIntegridad() : bool
+ RecalcularDV()
- Validar(proveedor : ProveedorBE_575_AV)
- ValidarCuit(cuit : string) : bool
- PersistirDigito(proveedor : ProveedorBE_575_AV)
```

**`ListaPrecioBLL_575_AV`**

```
+ Previsualizar(ruta : string, id_proveedor : int) : ResumenImportacion_575_AV
+ Importar(lista : ListaPrecioBE_575_AV, items : List<ItemListaPrecioBE_575_AV>, usuario : string) : ResumenImportacion_575_AV
+ CompararPrecios(id_articulo : int) : List<ComparacionPrecio_575_AV>
+ ObtenerPrecioVigente(id_proveedor : int, id_articulo : int) : ItemListaPrecioBE_575_AV
+ VerificarIntegridad() : bool
+ RecalcularDV()
- PersistirDigito(entidad : IVerificable, tabla : string)
```

**`OrdenCompraBLL_575_AV`**

```
+ AgregarArticuloABorrador(id_proveedor : int, detalle : DetalleOrdenCompraBE_575_AV, usuario : string) : OrdenCompraBE_575_AV
+ GenerarOrdenCompra(orden : OrdenCompraBE_575_AV, detalles : List<DetalleOrdenCompraBE_575_AV>, usuario : string) : OrdenCompraBE_575_AV
+ ListarPendientes() : List<OrdenCompraBE_575_AV>
+ ListarParaRecepcion() : List<OrdenCompraBE_575_AV>
+ ObtenerDetalle(id_orden_compra : int) : List<DetalleOrdenCompraBE_575_AV>
+ Autorizar(orden : OrdenCompraBE_575_AV, usuario : string)
+ Rechazar(orden : OrdenCompraBE_575_AV, motivo : string, usuario : string)
+ VerificarIntegridad() : bool
+ RecalcularDV()
- Validar(orden : OrdenCompraBE_575_AV)
- AjustarAMultiplo(detalle : DetalleOrdenCompraBE_575_AV)
- PersistirDigito(entidad : IVerificable, tabla : string)
```

**`RecepcionBLL_575_AV`**

```
+ RegistrarRecepcion(orden : OrdenCompraBE_575_AV, recepcion : RecepcionBE_575_AV, detalles : List<DetalleRecepcionBE_575_AV>, usuario : string) : RecepcionBE_575_AV
+ VerificarIntegridad() : bool
+ RecalcularDV()
- Validar(orden : OrdenCompraBE_575_AV, detalles : List<DetalleRecepcionBE_575_AV>)
- PersistirDigito(entidad : IVerificable, tabla : string)
```

**`ReservaBLL_575_AV`**

```
+ RegistrarReserva(reserva : ReservaBE_575_AV, usuario : string) : ReservaBE_575_AV
+ ListarPendientesPorArticulo(id_articulo : int) : List<ReservaBE_575_AV>
+ MarcarDisponible(reserva : ReservaBE_575_AV, usuario : string)
+ VerificarIntegridad() : bool
+ RecalcularDV()
- PersistirDigito(reserva : ReservaBE_575_AV)
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
+ AgregarItem(item : ItemListaPrecioBE_575_AV) : int
+ ListarPreciosPorArticulo(id_articulo : int) : List<ComparacionPrecio_575_AV>
+ BuscarPrecioVigente(id_proveedor : int, id_articulo : int) : ItemListaPrecioBE_575_AV
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
+ ActualizarDVH(id_recepcion : int, dvh : string)
+ ActualizarDVHDetalle(id_detalle_recepcion : int, dvh : string)
```

**`MP_Reserva_575_AV`**

```
+ CrearReserva(reserva : ReservaBE_575_AV) : int
+ ListarPorArticulo(id_articulo : int) : List<ReservaBE_575_AV>
+ ActualizarEstado(id_reserva : int, estado : EstadoReserva_575_AV)
+ ActualizarDVH(id_reserva : int, dvh : string)
```

**`LectorListaPrecios_575_AV`**

```
+ Leer(ruta : string) : List<ItemListaPrecioBE_575_AV>
- LeerXlsx(ruta : string) : List<ItemListaPrecioBE_575_AV>
- LeerCsv(ruta : string) : List<ItemListaPrecioBE_575_AV>
```

## Tablas nuevas

`Proveedor`, `ListaPrecio`, `ItemListaPrecio`, `OrdenCompra`, `DetalleOrdenCompra`, `Recepcion`, `DetalleRecepcion`. Todas con `dvh` y registradas en `DigitoVertical`. Columnas: ver los DER `N02.2.2.x.d`.
