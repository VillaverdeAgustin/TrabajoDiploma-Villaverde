# Evidencia de pruebas — RFN2 Gestión de Compras

Pruebas de CUN-007 a CUN-012 ejecutadas el 07/10/2026 sobre una base instalada desde cero con
`db\instalar_base.bat /limpio` (scripts 01 a 11). La prueba es automática: abre las pantallas reales
de la aplicación, carga los datos, presiona los botones y verifica los mensajes en pantalla y los
valores guardados en la base.

- **Resultado:** 115 verificaciones, 115 correctas.
- **Usuarios:** `compras` (CUN-007 a CUN-010), `propietario` (CUN-011) y `deposito` (CUN-012).
- **Listas de ejemplo:** `listas_ejemplo/lista_cablenet_2026-10.csv` y `listas_ejemplo/lista_fibrasur_2026-10.xlsx`.
- **Capturas:** `capturas/`, una por pantalla.
- **Regresión:** en la misma jornada, también sobre instalación limpia, pasaron las pruebas de la
  Entrega 1 en español, en inglés (incluye las 6 pantallas del RFN2) y de T08.

Las fechas de vigencia se expresan respecto del día de la prueba (H = 07/10/2026).

## CUN-007 Registrar Proveedor

| # | Caso | Datos | Resultado esperado | Resultado obtenido |
|---|---|---|---|---|
| 1 | Listado inicial | Base recién instalada | Se muestran los 3 proveedores de prueba | Correcto: 3 proveedores |
| 2 | CUIT con dígito verificador incorrecto | Conectividad del Plata S.A., CUIT 30-12345678-0 | Mensaje "CUIT inválido"; no se graba | Correcto: "CUIT inválido" |
| 3 | Alta | El mismo proveedor con CUIT 30-71234567-1 | "Proveedor registrado correctamente."; 4 proveedores; correo cifrado con AES-256 | Correcto; el correo guardado se descifra igual al ingresado |
| 4 | **Alternativo: CUIT repetido** | Otra razón social con CUIT 30708451238 (Cablenet) | Se cargan los datos de Cablenet para modificarlos, con el correo oculto | Correcto: "El CUIT ya corresponde a Cablenet Distribuidora S.A.: se cargaron sus datos para modificarlo." |
| 5 | Modificación | Teléfono de Cablenet: 11 4201-9999 | "Proveedor actualizado correctamente."; el correo cifrado no cambia | Correcto |
| 6 | Modificación con el CUIT de otro proveedor | CUIT de Cablenet sobre FibraSur | Rechazo indicando el CUIT ya registrado | Correcto: "Ya hay un proveedor registrado con el CUIT 30-70845123-8." |

## CUN-008 Importar Lista de Precios

| # | Caso | Datos | Resultado esperado | Resultado obtenido |
|---|---|---|---|---|
| 1 | Importar sin previsualizar | Archivo elegido, sin previsualizar | Botón Importar deshabilitado | Correcto |
| 2 | **Alternativo: importación con renglones rechazados** (CSV) | Lista de Cablenet, `.csv` en Windows-1252 con coma decimal: 9 renglones, uno sin código y uno con precio "consultar" | 5 actualizados · 2 incorporados · 2 rechazados, cada rechazo con su motivo | Correcto; se guardan los 7 válidos, los 2 nuevos con artículo vacío, acentos intactos y sin tocar el precio de venta |
| 3 | Importación desde Excel | Lista de FibraSur, `.xlsx` con un código repetido y un precio en 0 | 4 actualizados · 1 incorporado · 2 rechazados | Correcto |
| 4 | Archivo sin formato válido | `.csv` sin columna de precio | "El archivo no tiene el formato esperado" | Correcto |
| 5 | Extend a CUN-007 | Botón "Nuevo proveedor": primero un CUIT repetido, después Telecomunicaciones Burzaco S.R.L. | El proveedor nuevo queda elegido en la importación | Correcto |
| 6 | Importación que falla a mitad de camino | Lista de Telecomunicaciones Burzaco cuyo segundo renglón referencia un artículo inexistente | La lista queda sin efecto (`SP_DesactivarListaPrecio`), con bitácora `ImportacionListaRevertida` | Correcto: "No se pudo completar la importación: la lista quedó sin efecto."; la lista queda inactiva y no da precio vigente ni entra en la comparación |

## CUN-009 Comparar Precios

Además de las listas del CUN-008: lista de Redes Integrales de Hernan Ortiz con vigencia H − 45 días,
y una lista de Cablenet con vigencia H + 10 días.

| # | Caso | Datos | Resultado esperado | Resultado obtenido |
|---|---|---|---|---|
| 1 | Comparación | CAB-UTP6-EXT, moneda ARS | Un precio por proveedor, de menor a mayor | Correcto: Ortiz 1.199,90 < FibraSur 1.250,00 < Cablenet 1.290,50 |
| 2 | **Alternativo: lista vencida** | Lista de Ortiz con más de 30 días de vigencia | El renglón se resalta e indica la fecha de la última actualización | Correcto: "Lista vencida: última actualización H − 45" |
| 3 | Lista futura | Lista de Cablenet con vigencia H + 10 | Todavía no rige: sigue el precio 1.290,50 | Correcto |
| 4 | Selector de moneda | Moneda USD | No hay listas en dólares: "No se encontraron proveedores para el artículo" | Correcto; al volver a ARS reaparecen los 3 proveedores |
| 5 | Agregar a la orden | FibraSur, 610 m (dos cajas de 305 m) | Se crea el borrador de FibraSur con total 762.500,00 | Correcto |
| 6 | Artículo repetido en el borrador | El mismo cable otra vez | Rechazo | Correcto: "El artículo ya está en la orden de compra en borrador de ese proveedor." |
| 7 | Segundo artículo | FIC-RJ45-C6 × 2 de FibraSur | Mismo borrador, total 839.501,00 | Correcto |
| 8 | Ningún proveedor comercializa el artículo | RCK-12U-MUR | "No se encontraron proveedores para el artículo" | Correcto |

## CUN-010 Generar Orden de Compra

| # | Caso | Datos | Resultado esperado | Resultado obtenido |
|---|---|---|---|---|
| 1 | Artículos a reponer | Base de prueba | Los artículos en el punto de reposición o por debajo | Correcto: CER-DSX-600, JCK-RJ45-C6, RCK-12U-MUR |
| 2 | Extend a CUN-009 | Botón "Comparar precios" con JCK-RJ45-C6 elegido | Se abre Comparar Precios con ese artículo | Correcto: Cablenet 5.400,00 < FibraSur 5.650,00 |
| 3 | **Alternativo 4.1: cantidad no múltiplo** | JCK-RJ45-C6, 32 u, unidad de venta de FibraSur "Caja x 10" | Se ajusta a 40 u y se informa | Correcto: "La cantidad se ajustó a 40,00 u por la unidad de venta del proveedor (Caja x 10). Total de la orden: 1.065.501,00." |
| 4 | **Alternativo 5.1: sin precio vigente** | RCK-12U-MUR (FibraSur no lo lista) | Renglón resaltado; se pide el precio manual | Correcto: "El proveedor no tiene precio vigente para el artículo: ingrese el precio manualmente." |
| 5 | Sin cargar el precio manual | RCK-12U-MUR, precio 0 | Rechazo | Correcto: "El proveedor no tiene un precio vigente para el artículo." |
| 6 | Precio manual | RCK-12U-MUR, 1 u a 298.000,00 | Se agrega; total 1.363.501,00 | Correcto |
| 7 | Precio vigente | CER-DSX-600, 1 u | Se agrega a 9.100.000,00; borrador de 5 renglones, total 10.463.501,00 | Correcto |
| 8 | Generar orden | Borrador de FibraSur | "Se generó la orden de compra Nro. OC-000001"; estado PendienteAutorizacion con fecha de emisión | Correcto |
| 9 | Generar sin borrador | Cablenet, sin renglones | Rechazo | Correcto: "No hay una orden en borrador para el proveedor." |
| 10 | Segunda orden | Cablenet, JCK-RJ45-C6 × 32 (vende por unidad: sin ajuste) | Total 172.800,00 y orden OC-000002 | Correcto |

## CUN-011 Autorizar Orden de Compra

| # | Caso | Datos | Resultado esperado | Resultado obtenido |
|---|---|---|---|---|
| 1 | Órdenes pendientes | OC-000001 y OC-000002 | Se listan con proveedor, cantidad de renglones y total | Correcto: OC-000001, FibraSur, 5 renglones, 10.463.501,00 |
| 2 | Detalle | OC-000001 seleccionada | Se muestran los 5 renglones y el total | Correcto |
| 3 | **Alternativo 7.1: cancelar** | Autorizar y responder "No" | La orden sigue pendiente | Correcto: "Operación cancelada: la orden sigue pendiente." |
| 4 | Autorizar | Autorizar y confirmar | "Orden autorizada correctamente"; estado Autorizada con usuario autorizante y fecha | Correcto |
| 5 | **Alternativo 5.1: rechazar sin motivo** | OC-000002, motivo vacío | Se pide el motivo | Correcto: "Ingrese el motivo del rechazo." |
| 6 | Rechazar | OC-000002, motivo "Precio por encima del acordado con el proveedor" | Estado Rechazada con el motivo registrado | Correcto: "La orden OC-000002 quedó rechazada." |

## CUN-012 Registrar Recepción de Mercadería

Stock antes de recibir: CAB-UTP6-EXT 730 m, CER-DSX-600 0 u (SinStock), FIC-RJ45-C6 26, JCK-RJ45-C6 18, RCK-12U-MUR 3.

| # | Caso | Datos | Resultado esperado | Resultado obtenido |
|---|---|---|---|---|
| 1 | Órdenes para recibir | — | Solo OC-000001 (Autorizada); la rechazada no aparece | Correcto |
| 2 | **Alternativo 5.1: recibir de más** | JCK-RJ45-C6: 50 de 40 | "La cantidad recibida no puede superar la solicitada"; no se graba nada | Correcto |
| 3 | Sin remito | Remito vacío | Se pide el remito | Correcto: "Ingrese el número de remito del proveedor (hasta 30 caracteres)." |
| 4 | **Alternativo 7.1: recepción parcial** | Remito 0003-00012345: cable 305 de 610 m, certificador 0 de 1, el resto completo | Orden RecibidaParcial con la diferencia registrada; stock y bobina actualizados | Correcto: "…La orden quedó recibida parcialmente: CAB-UTP6-EXT: faltan 305,00 m; CER-DSX-600: faltan 1,00 u"; 4 renglones recibidos y 1 bobina nueva de 305 m |
| 5 | Pendiente después de la parcial | OC-000001 | Quedan 305 m de cable y 1 certificador | Correcto |
| 6 | Recepción total | Remito 0003-00012399 con lo pendiente | "Recepción registrada correctamente"; orden Recibida | Correcto; no quedan órdenes para recibir |
| 7 | Stock | — | Aumenta en lo recibido | Correcto: CAB-UTP6-EXT 1.340 m, CER-DSX-600 1 (pasa a Disponible), FIC-RJ45-C6 28, JCK-RJ45-C6 58, RCK-12U-MUR 4 |
| 8 | Bobinas | — | Una bobina por caja de 305 m, con su saldo inicial | Correcto: BOB-001-00001-001 y BOB-001-00002-001, 305 m cada una; el saldo de las bobinas del cable suma 1.340 m, igual al stock |

## Integridad y bitácora

| # | Caso | Resultado esperado | Resultado obtenido |
|---|---|---|---|
| 1 | Dígito verificador al final del circuito | Todas las tablas íntegras | Correcto |
| 2 | Bitácora del circuito | Cada apertura de pantalla, alta, modificación y cambio de estado registrado | Correcto: GestionProveedoresAbierta, AltaProveedor, ModificacionProveedor, ImportarListaAbierta, ImportacionListaPrecios, ImportacionListaRevertida, CompararPreciosAbierta, AltaOrdenCompra ×2, AltaDetalleOrdenCompra ×6, OrdenesCompraAbierta, EmisionOrdenCompra ×2, AutorizacionesAbierta, AutorizacionOrdenCompra, RechazoOrdenCompra, RecepcionAbierta, RecepcionParcial, RecepcionMercaderia, MovimientoStock ×6, AltaBobina ×2 |
