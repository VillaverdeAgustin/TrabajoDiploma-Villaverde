# Evidencia de pruebas — A02 Reporte de compras y A03 Serialización

Pruebas ejecutadas el 06/11/2026 sobre una base instalada desde cero con
`db\instalar_base.bat /limpio` (scripts 01 a 11). La prueba es automática
(`PruebaA02A03`): abre las pantallas reales de la aplicación, carga los filtros, presiona los
botones y verifica lo que muestra la pantalla contra la base y contra los archivos generados.

- **Resultado:** 30 casos (34 verificaciones), todos correctos.
- **Datos de partida:** `PruebaCompras` (RFN2, 115 verificaciones correctas en la misma jornada) deja
  OC-000001 de FibraSur recibida y OC-000002 de Cablenet rechazada. La prueba suma, por la misma BLL
  que usan las pantallas del RFN2:
  - una lista de precios **en dólares** de Redes Integrales de Hernan Ortiz (rack mural 12U a USD 310 y
    certificador DSX-600 a USD 4.200);
  - OC-000003 de Ortiz con esos dos artículos, pendiente de autorización;
  - OC-000004 de Cablenet por 610 m de cable, autorizada por `propietario` y recibida a medias
    (305 m) por `deposito`;
  - OC-000005 de FibraSur, pendiente de autorización.
- **Usuario:** `compras` (rol AdministradorCompras).
- **Capturas:** `capturas/frmReporteCompras_575_AV.png`, `capturas/frmSerializacion_575_AV_serializar.png`,
  `capturas/frmSerializacion_575_AV_deserializar.png` y `capturas/frmSerializacion_575_AV_error.png`.
- **Archivo de ejemplo:** `serializacion_ejemplo/OC-000001.xml`, el que generó la prueba
  (SHA-256 `d72908d7b0cd76c8aae49a2994bb1ba24e7a1d6aadd6b3a237fdb84b2f168443`). Ese hash es el del
  archivo con fines de línea CRLF, como lo escribe la aplicación y como queda en un clon de Windows.
  Git lo guarda con LF, así que una copia bajada de GitHub da otro SHA-256 aunque el contenido sea el
  mismo; se deserializa y verifica igual.
- **Regresión:** `PruebaTraducciones` (español → inglés → español con cada formulario abierto) da OK en
  los 27 formularios, incluidos los dos nuevos. La auditoría de claves da 722 claves en ES y EN, sin
  faltantes, duplicadas ni sin uso. La verificación de integridad (DVH y DVV de todas las tablas) da OK
  al terminar.

## Reglas que aplica el reporte

- **Órdenes que entran:** las emitidas (con fecha de emisión) en el período elegido. Un borrador no
  tiene fecha de emisión, por eso no se ofrece en el filtro de estado.
- **Monto ARS / USD:** cada renglón toma la moneda de la lista de precios de la que salió su precio
  (la última lista del proveedor vigente a la fecha de emisión que tiene el artículo a ese mismo
  precio). Un precio cargado a mano, sin lista, es ARS. Así una orden con renglones de listas
  distintas reparte su total entre las dos columnas.
- **% recibido:** cantidad recibida sobre cantidad solicitada, sumando solo las órdenes autorizadas,
  recibidas parcialmente o recibidas. Si el proveedor no tiene ninguna, se muestra "—".
- **Datos:** los lee `SP_ReporteCompras` (script 10, solo lectura) a través de
  `MP_OrdenCompra_575_AV.ListarParaReporte`. `ReporteComprasBLL_575_AV.GenerarReporte` valida los
  filtros, calcula los porcentajes, agrupa por proveedor y arma la fila de totales.

## Verificaciones de la serialización

La serialización usa `XmlSerializer` en `Serializador_575_AV` (Servicios). El archivo contiene un
`DocumentoOrdenCompra_575_AV`: versión, fecha de generación, usuario, la orden (cabecera y detalle) y
el proveedor. `SerializacionOrdenCompraBLL_575_AV` hace las verificaciones.

| Al serializar (archivo contra el objeto) | Al deserializar (archivo contra la base) |
|---|---|
| El archivo se escribió y se pudo releer | SHA-256 del archivo |
| Cabecera de la orden, campo por campo | Dígito verificador (DVH) de la orden y de cada renglón, recalculado sobre lo que dice el archivo |
| Cantidad de renglones | La orden existe en la base con el mismo número |
| Cada renglón, campo por campo | Proveedor (id y CUIT) |
| Proveedor, campo por campo | Cantidad de renglones |
| SHA-256 del archivo | Cada renglón: artículo, cantidad, precio unitario y subtotal |
| | Total de la orden, y que coincida con la suma de los subtotales del archivo |

Si la orden no existe en la base, no se sigue comparando contra la base. Un archivo que no es una
orden de compra serializada se rechaza antes de mostrar nada.

## A02 Reporte de compras

| # | Caso | Datos | Resultado esperado | Resultado obtenido |
|---|---|---|---|---|
| A02-1 | Reporte del período (últimos 30 días, todos los proveedores y estados) | 5 órdenes emitidas hoy de 3 proveedores | Una fila por proveedor más la fila de totales; 5 órdenes | Correcto: Cablenet Distribuidora S.A.: 2 \| FibraSur S.R.L.: 2 \| Redes Integrales de Hernan Ortiz: 1 \| Total: 5 |
| A02-2 | Proveedor con una orden rechazada y otra recibida a medias | Cablenet: OC-000002 rechazada, OC-000004 autorizada con 305 de 610 m recibidos | 2 órdenes, 1 rechazada, 0 pendientes, 50,0 % recibido (la rechazada no cuenta) | Correcto: Órdenes 2 · Monto ARS 960.005,00 · Monto USD 0,00 · % recibido 50,0 % · Pend. autorización 0 · Rechazadas 1 |
| A02-3 | Proveedor con una orden recibida y otra pendiente | FibraSur: OC-000001 recibida completa, OC-000005 pendiente de autorización | 2 órdenes, 1 pendiente, 100,0 % recibido (la pendiente no cuenta) | Correcto: Órdenes 2 · Monto ARS 14.313.551,00 · Monto USD 0,00 · % recibido 100,0 % · Pend. autorización 1 · Rechazadas 0 |
| A02-4 | Orden con precios de una lista en dólares | Ortiz: OC-000003 con 2 racks a USD 310 y 1 certificador a USD 4.200 (lista USD) | Monto USD 4.820,00 y ARS 0; sin % recibido porque está pendiente | Correcto: Órdenes 1 · Monto ARS 0,00 · Monto USD 4.820,00 · % recibido — · Pend. autorización 1 · Rechazadas 0 |
| A02-5 | Fila de totales | Las 5 órdenes | ARS + USD = suma de los totales emitidos (15.278.376,00); % = (654 + 305) / (654 + 610) = 75,9 %; 2 pendientes; 1 rechazada | Correcto: Órdenes 5 · Monto ARS 15.273.556,00 · Monto USD 4.820,00 · % recibido 75,9 % · Pend. autorización 2 · Rechazadas 1 |
| A02-6 | Detalle del proveedor seleccionado | Clic en Cablenet | El recuadro inferior muestra sus 2 órdenes con el estado traducido | Correcto: Órdenes de Cablenet Distribuidora S.A.: OC-000002 Rechazada —, OC-000004 Recibida parcialmente 50,0 % |
| A02-7 | Detalle de la fila de totales | Clic en Total | Las 5 órdenes de todos los proveedores | Correcto: Órdenes de todos los proveedores: 5 órdenes |
| A02-8 | Filtro por proveedor | Proveedor: FibraSur S.R.L. | Solo FibraSur y los totales (2 órdenes) | Correcto: FibraSur S.R.L.: 2 \| Total: 2 — 2 orden(es) de compra de 1 proveedor(es). |
| A02-9 | Filtro por estado | Estado: Pendiente de autorización | FibraSur y Ortiz, una orden pendiente cada uno | Correcto: FibraSur S.R.L.: 1 \| Redes Integrales de Hernan Ortiz: 1 \| Total: 2 |
| A02-10 | **Error: rango de fechas inválido** | Desde: mañana; hasta: hoy | Mensaje de error, grilla vacía y sin registro en la bitácora | Correcto: "La fecha desde no puede ser posterior a la fecha hasta." (0 filas) |
| A02-11 | **Sin resultados** | Desde 01/01/2026 hasta 31/01/2026 | Mensaje informativo y grilla vacía | Correcto: "No hay órdenes de compra emitidas con esos filtros." (0 filas) |
| A02-12 | **Error: estado Borrador pedido a la BLL** | Estado Borrador (la pantalla no lo ofrece) | La BLL rechaza el filtro | Correcto: "Un borrador no tiene fecha de emisión: elija otro estado." |
| A02-13 | Bitácora | Apertura más 3 reportes generados (el de rango inválido no cuenta) | 4 registros ConsultaReporteCompras (68) del usuario compras | Correcto: 4 registros; usuario compras |
| A02-14 | Estados que ofrece el filtro | Combo Estado | (Todos) y los 5 estados con fecha de emisión, sin Borrador | Correcto: (Todos), Pendiente de autorización, Autorizada, Rechazada, Recibida parcialmente, Recibida |

## A03 Serialización de órdenes de compra

| # | Caso | Datos | Resultado esperado | Resultado obtenido |
|---|---|---|---|---|
| A03-1 | Órdenes para serializar | Base con 5 órdenes emitidas | Las 5 órdenes, sin borradores | Correcto: 5 órdenes |
| A03-2 | **Error: serializar sin elegir una orden** | Ninguna fila seleccionada | Pide seleccionar una orden y no crea el archivo | Correcto: "Seleccione una orden de compra." |
| A03-3 | Serializar a XML | OC-000001 (FibraSur, 5 renglones, recibida) en OC-000001.xml | Archivo creado; 10 verificaciones correctas (releído, cabecera, cantidad de renglones, 5 renglones, proveedor, SHA-256) | Correcto: "La orden OC-000001 se serializó y la verificación es correcta." — 10/10 correctas |
| A03-4 | SHA-256 del archivo | Hash calculado por la prueba con SHA256 de .NET sobre el mismo archivo | Coincide con el que muestra la pantalla | Correcto: d72908d7b0cd76c8aae49a2994bb1ba24e7a1d6aadd6b3a237fdb84b2f168443 |
| A03-5 | Contenido del XML | Lectura del archivo con XmlDocument, sin la aplicación | Cabecera, detalle (5 renglones) y proveedor (CUIT de FibraSur) | Correcto: raíz DocumentoOrdenCompra_575_AV, nro OC-000001, 5 renglones, CUIT 30-71562948-4, total 10463501.00 |
| A03-6 | **Error: ubicación donde no se puede escribir** | Carpeta inexistente | Mensaje de error sin detalles técnicos | Correcto: "No se pudo escribir el archivo en esa ubicación." |
| A03-7 | **Error: serializar una orden inexistente** | id_orden_compra 99999 pedido a la BLL | La BLL rechaza la orden | Correcto: "La orden de compra no existe o todavía es un borrador." |
| A03-8 | Deserializar y verificar contra la base | El archivo OC-000001.xml recién generado | Se muestra la orden recuperada (5 renglones); SHA-256, dígito verificador, existencia, proveedor, cantidad de renglones, cada renglón y total: todo correcto | Correcto: OC-000001 · FibraSur S.R.L. · 5 renglones · 11/11 correctas — "La orden OC-000001 se recuperó del archivo y coincide con la base." |
| A03-9 | Cambio de idioma con resultados en pantalla | Inglés y vuelta a español | Las verificaciones se redibujan en el idioma elegido | Correcto: "Order OC-000001 exists in the database / Passed" y de vuelta "La orden OC-000001 existe en la base" |
| A03-10 | **Error: archivo inválido** | Archivo de texto con extensión .xml | Mensaje de archivo inválido y pantalla limpia | Correcto: "El archivo no es una orden de compra serializada válida." |
| A03-11 | **Error: XML bien formado de otro tipo** | XML con raíz ListaPrecio | Mensaje de archivo inválido | Correcto: "El archivo no es una orden de compra serializada válida." |
| A03-12 | **Error: valor que no se puede convertir** | El total de la orden reemplazado por texto | Mensaje de archivo inválido | Correcto: "El archivo no es una orden de compra serializada válida." |
| A03-13 | **Error: archivo que no existe** | Ruta de un archivo borrado | Mensaje de lectura | Correcto: "No se pudo leer el archivo." |
| A03-14 | **Error: orden inexistente** | Copia del archivo con id_orden_compra 9999 y número OC-009999 | Falla "La orden existe en la base" (y el dígito verificador); no se compara contra la base | Correcto: fallan 2 de 3: Dígito verificador de la orden y sus renglones [OC-009999]; La orden OC-009999 existe en la base [id_orden_compra 9999] — "La orden OC-009999 se recuperó del archivo, pero no coincide con la base: revise las verificaciones." |
| A03-15 | **Error: archivo modificado a mano** | Cantidad del renglón 1 cambiada de 610.00 a 999.00 con un editor | Fallan el dígito verificador y el renglón 1 (cantidad distinta de la base); el resto coincide; otro SHA-256 | Correcto: fallan 2 de 11: Dígito verificador de la orden y sus renglones [#1 CAB-UTP6-EXT]; Renglón 1 (CAB-UTP6-EXT): artículo, cantidad, precio y subtotal [cantidad: 999,00 ≠ 610,00] — SHA-256 distinto |
| A03-16 | Bitácora | 1 serialización correcta; 3 archivos deserializados (los inválidos no se leen) | 1 SerializacionOrdenCompra (69) y 3 DeserializacionOrdenCompra (70) | Correcto: 1 y 3 |

## Permisos

Las dos opciones están en el menú Compras. Siguiendo el Composite (T04), los permisos se agregan
como permisos simples de la familia **AdministradorCompras** en el script 03: `ReporteCompras` (A02) y
`SerializarOrdenCompra` (A03). Comprobado con `PerfilBLL.TienePermiso`:

| Rol | Reporte de compras | Serialización de órdenes |
|---|---|---|
| AdministradorCompras | Sí | Sí |
| Admin (hereda las familias) | Sí | Sí |
| Propietario, Deposito, Ventas | No | No |

## Cómo repetir la prueba

```bat
db\instalar_base.bat /limpio
```

Después, desde `_notas\pruebas_automatizadas`:

```bash
./correr.sh PruebaCompras.cs <carpeta de listas_ejemplo> <carpeta de capturas>
./correr.sh PruebaA02A03.cs <carpeta de capturas> <tabla de casos .md> <carpeta del XML de ejemplo>
./correr.sh PruebaTraducciones.cs
```
