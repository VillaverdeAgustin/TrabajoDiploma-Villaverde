using Acceso_DAL;
using Entidad_BE;
using Servicios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Negocio_BLL
{
    /// <summary>
    /// Validacion del pedido y cobro (CUN-004; RFN1.5, RFN1.6 y RFN1.7).
    ///
    /// <para>Es la operacion mas larga del circuito: autoriza el pago, emite el
    /// comprobante con su detalle, registra el cobro, confirma la salida de
    /// mercaderia y cierra la orden de pago y el carrito.</para>
    ///
    /// <para>Toca seis tablas. Como cada procedimiento escribe una sola, la
    /// secuencia la arma esta clase, y cada escritura recalcula el dvh de su
    /// fila y el DVV de su tabla. El orden no es casual: primero lo que puede
    /// fallar sin dejar rastro (la autorizacion), y recien despues lo que
    /// persiste.</para>
    ///
    /// <para>El comprobante se emite como <b>comprobante no valido como
    /// factura</b>: no hay integracion con ARCA.</para>
    /// </summary>
    public class VentaBLL_575_AV
    {
        private const string TABLA = "Comprobante";
        private const string TABLA_DETALLE = "DetalleComprobante";
        private const string TABLA_PAGO = "Pago";

        /// <summary>Alicuota general de IVA.</summary>
        private const decimal ALICUOTA_IVA = 0.21m;

        private MP_Comprobante_575_AV mpComprobante = new MP_Comprobante_575_AV();
        private MP_OrdenPago_575_AV mpOrden = new MP_OrdenPago_575_AV();
        private MP_Carrito_575_AV mpCarrito = new MP_Carrito_575_AV();
        private ArticuloBLL_575_AV articuloBLL = new ArticuloBLL_575_AV();
        private ClienteBLL_575_AV clienteBLL = new ClienteBLL_575_AV();
        private AutorizadorPago_575_AV autorizador = new AutorizadorPago_575_AV();
        private BitacoraBLL bitacora = new BitacoraBLL();
        private VerificadorIntegridadBLL verificador = new VerificadorIntegridadBLL();

        #region Consulta

        /// <summary>
        /// Busca un comprobante por su numero, con el detalle y el cliente ya
        /// resueltos. Devuelve nulo si no existe.
        /// </summary>
        public ComprobanteBE_575_AV BuscarComprobante(string nro_comprobante)
        {
            if (string.IsNullOrWhiteSpace(nro_comprobante)) { return null; }

            ComprobanteBE_575_AV comprobante =
                mpComprobante.BuscarPorNumero(nro_comprobante.Trim());

            if (comprobante == null) { return null; }

            comprobante.detalle = Completar(mpComprobante.ListarDetalle(comprobante.id_comprobante));
            return comprobante;
        }

        #endregion

        #region Cobro

        /// <summary>
        /// Cobra la orden de pago y emite el comprobante.
        ///
        /// <para>Si la entidad rechaza el pago no se emite nada: se deja el
        /// rechazo en la bitacora y se corta.</para>
        /// </summary>
        public ComprobanteBE_575_AV Cobrar(OrdenPagoBE_575_AV orden,
                                           ClienteBE_575_AV cliente,
                                           List<DetalleCarritoBE_575_AV> detalles,
                                           MedioPagoBE_575_AV medio_pago,
                                           string datos_pago,
                                           string usuario)
        {
            Validar(orden, cliente, detalles);

            // 1. La orden se relee: no se cobra dos veces la misma.
            OrdenPagoBE_575_AV vigente = mpOrden.ListarOrdenes()
                .FirstOrDefault(o => o.id_orden_pago == orden.id_orden_pago);

            if (vigente == null)
            {
                throw new InvalidOperationException("La orden de pago no existe.");
            }
            if (!vigente.se_puede_cobrar)
            {
                throw new InvalidOperationException(
                    "La orden de pago " + vigente.nro_orden_pago + " ya no admite cobro.");
            }

            // 2. Autorizacion. Es lo unico que puede fallar sin dejar rastro.
            ResultadoPago_575_AV resultado =
                autorizador.Autorizar(medio_pago, datos_pago, vigente.precio_total);

            if (!resultado.aprobado)
            {
                bitacora.RegistrarBitacora(usuario, TipoAccion.CobroRechazado);
                throw new InvalidOperationException("El pago fue rechazado: " + resultado.motivo);
            }

            // 3. Comprobante. El tipo sale de la condicion del cliente.
            ComprobanteBE_575_AV comprobante = ArmarComprobante(vigente, cliente);
            comprobante.id_comprobante = mpComprobante.CrearComprobante(comprobante);

            ComprobanteBE_575_AV emitido = mpComprobante.ListarComprobantes()
                .FirstOrDefault(c => c.id_comprobante == comprobante.id_comprobante) ?? comprobante;

            PersistirDigito(emitido, TABLA);

            // 4. Detalle del comprobante, copiado del carrito.
            foreach (DetalleCarritoBE_575_AV renglon in detalles)
            {
                DetalleComprobanteBE_575_AV detalle = new DetalleComprobanteBE_575_AV
                {
                    id_comprobante = emitido.id_comprobante,
                    id_articulo = renglon.id_articulo,
                    cantidad = renglon.cantidad,
                    id_unidad_medida = renglon.id_unidad_medida,
                    precio_unitario = renglon.precio_unitario
                };

                detalle.id_detalle_comprobante = mpComprobante.AgregarDetalle(detalle);
                PersistirDigito(detalle, TABLA_DETALLE);
                emitido.detalle.Add(detalle);
            }

            // 5. Cobro acreditado.
            PagoBE_575_AV pago = new PagoBE_575_AV
            {
                id_comprobante = emitido.id_comprobante,
                id_medio_pago = medio_pago.id_medio_pago,
                monto = emitido.precio_total,
                fecha_hora = DateTime.Now,
                codigo_autorizacion = resultado.codigo_autorizacion,
                estado = EstadoPago_575_AV.Acreditado
            };

            pago.id_pago = mpComprobante.RegistrarPago(pago);

            PagoBE_575_AV acreditado = mpComprobante.ListarPagos()
                .FirstOrDefault(p => p.id_pago == pago.id_pago) ?? pago;

            PersistirDigito(acreditado, TABLA_PAGO);
            bitacora.RegistrarBitacora(usuario, TipoAccion.CobroRegistrado);

            // 6. RFN1.6 - Salida de mercaderia. En los fraccionables la bobina
            // ya se descontó al armar el carrito, asi que aca solo baja la
            // cantidad disponible del articulo.
            foreach (DetalleCarritoBE_575_AV renglon in detalles)
            {
                GestorDeExistencias_575_AV.GetInstance
                    .ConfirmarSalida(renglon.id_articulo, renglon.cantidad, usuario);
            }

            // 7. Cierre de la orden y del carrito.
            mpOrden.ActualizarEstado(vigente.id_orden_pago, EstadoOrdenPago_575_AV.Pagada);
            PersistirDigito(Releer(mpOrden.ListarOrdenes(),
                o => o.id_orden_pago == vigente.id_orden_pago), "OrdenPago");

            mpCarrito.ActualizarEstado(vigente.id_carrito, EstadoCarrito_575_AV.Facturado);
            PersistirDigito(Releer(mpCarrito.ListarCarritos(),
                c => c.id_carrito == vigente.id_carrito), "Carrito");

            bitacora.RegistrarBitacora(usuario, TipoAccion.EmisionComprobante);

            emitido.nombre_cliente = cliente.nombre_para_mostrar;
            return emitido;
        }

        #endregion

        #region Integridad

        public bool VerificarIntegridad()
        {
            bool comprobantesOk = verificador.VerificarIntegridad(
                mpComprobante.ListarComprobantes().OrderBy(c => c.id_comprobante).ToList(), TABLA);

            bool detallesOk = verificador.VerificarIntegridad(
                mpComprobante.ListarDetalles().OrderBy(d => d.id_detalle_comprobante).ToList(),
                TABLA_DETALLE);

            bool pagosOk = verificador.VerificarIntegridad(
                mpComprobante.ListarPagos().OrderBy(p => p.id_pago).ToList(), TABLA_PAGO);

            return comprobantesOk && detallesOk && pagosOk;
        }

        public void RecalcularDV()
        {
            foreach (ComprobanteBE_575_AV c in mpComprobante.ListarComprobantes()
                        .OrderBy(x => x.id_comprobante))
            {
                c.dvh = VerificadorIntegridad.CalcularDVH(c);
                mpComprobante.ActualizarDVH(c.id_comprobante, c.dvh);
            }
            verificador.ActualizarDVV(TABLA);

            foreach (DetalleComprobanteBE_575_AV d in mpComprobante.ListarDetalles()
                        .OrderBy(x => x.id_detalle_comprobante))
            {
                d.dvh = VerificadorIntegridad.CalcularDVH(d);
                mpComprobante.ActualizarDVHDetalle(d.id_detalle_comprobante, d.dvh);
            }
            verificador.ActualizarDVV(TABLA_DETALLE);

            foreach (PagoBE_575_AV p in mpComprobante.ListarPagos().OrderBy(x => x.id_pago))
            {
                p.dvh = VerificadorIntegridad.CalcularDVH(p);
                mpComprobante.ActualizarDVHPago(p.id_pago, p.dvh);
            }
            verificador.ActualizarDVV(TABLA_PAGO);
        }

        /// <summary>
        /// Calcula y persiste el dvh de una fila cualquiera de las que toca esta
        /// clase, y el DVV de su tabla.
        /// </summary>
        private void PersistirDigito(IVerificable entidad, string tabla)
        {
            string dvh = VerificadorIntegridad.CalcularDVH(entidad);

            switch (tabla)
            {
                case TABLA:
                    ComprobanteBE_575_AV comprobante = (ComprobanteBE_575_AV)entidad;
                    comprobante.dvh = dvh;
                    mpComprobante.ActualizarDVH(comprobante.id_comprobante, dvh);
                    break;

                case TABLA_DETALLE:
                    DetalleComprobanteBE_575_AV detalle = (DetalleComprobanteBE_575_AV)entidad;
                    detalle.dvh = dvh;
                    mpComprobante.ActualizarDVHDetalle(detalle.id_detalle_comprobante, dvh);
                    break;

                case TABLA_PAGO:
                    PagoBE_575_AV pago = (PagoBE_575_AV)entidad;
                    pago.dvh = dvh;
                    mpComprobante.ActualizarDVHPago(pago.id_pago, dvh);
                    break;

                case "OrdenPago":
                    OrdenPagoBE_575_AV orden = (OrdenPagoBE_575_AV)entidad;
                    orden.dvh = dvh;
                    mpOrden.ActualizarDVH(orden.id_orden_pago, dvh);
                    break;

                case "Carrito":
                    CarritoBE_575_AV carrito = (CarritoBE_575_AV)entidad;
                    carrito.dvh = dvh;
                    mpCarrito.ActualizarDVH(carrito.id_carrito, dvh);
                    break;

                default:
                    throw new InvalidOperationException("Tabla no contemplada: " + tabla);
            }

            verificador.ActualizarDVV(tabla);
        }

        #endregion

        #region Apoyo

        /// <summary>
        /// Arma el comprobante a partir de la orden y del cliente. El tipo sale
        /// de la condicion frente al IVA: "A" para responsable inscripto, con el
        /// impuesto discriminado, y "B" para el resto. El total ya incluye IVA,
        /// asi que el neto se obtiene desagregandolo.
        /// </summary>
        private ComprobanteBE_575_AV ArmarComprobante(OrdenPagoBE_575_AV orden,
                                                      ClienteBE_575_AV cliente)
        {
            string tipo = cliente.condicion_iva == CondicionIva_575_AV.ResponsableInscripto
                ? "A"
                : "B";

            decimal neto = decimal.Round(orden.precio_total / (1 + ALICUOTA_IVA), 2);

            return new ComprobanteBE_575_AV
            {
                nro_comprobante = mpComprobante.ProximoNumero(tipo),
                tipo_comprobante = tipo,
                id_orden_pago = orden.id_orden_pago,
                id_cliente = cliente.id_cliente,
                fecha_emision = DateTime.Now,
                neto = neto,
                iva = orden.precio_total - neto,
                precio_total = orden.precio_total,
                estado = EstadoComprobante_575_AV.Emitido
            };
        }

        private static T Releer<T>(List<T> filas, Func<T, bool> criterio)
        {
            T fila = filas.FirstOrDefault(criterio);
            if (fila == null)
            {
                throw new InvalidOperationException("No se encontro la fila a verificar.");
            }
            return fila;
        }

        /// <summary>Completa los datos de articulo que necesitan la pantalla y el despacho.</summary>
        private List<DetalleComprobanteBE_575_AV> Completar(List<DetalleComprobanteBE_575_AV> detalles)
        {
            if (detalles.Count == 0) { return detalles; }

            Dictionary<int, ArticuloBE_575_AV> articulos =
                articuloBLL.ListarArticulos().ToDictionary(a => a.id_articulo);

            foreach (DetalleComprobanteBE_575_AV detalle in detalles)
            {
                ArticuloBE_575_AV articulo;
                if (articulos.TryGetValue(detalle.id_articulo, out articulo))
                {
                    detalle.codigo_articulo = articulo.codigo;
                    detalle.descripcion_articulo = articulo.descripcion;
                    detalle.abreviatura_unidad = articulo.abreviatura_unidad;
                    detalle.deposito_ubicacion = articulo.deposito_ubicacion;
                }
            }

            return detalles;
        }

        private void Validar(OrdenPagoBE_575_AV orden,
                             ClienteBE_575_AV cliente,
                             List<DetalleCarritoBE_575_AV> detalles)
        {
            if (orden == null) { throw new ArgumentNullException("orden"); }

            if (cliente == null || cliente.id_cliente <= 0)
            {
                throw new InvalidOperationException(
                    "Hay que identificar al cliente antes de cobrar.");
            }
            if (detalles == null || detalles.Count == 0)
            {
                throw new InvalidOperationException("El pedido no tiene articulos.");
            }
        }

        #endregion
    }
}
