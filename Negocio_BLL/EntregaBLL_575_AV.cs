using Acceso_DAL;
using Entidad_BE;
using Servicios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Negocio_BLL
{
    /// <summary>
    /// Entrega de mercaderia en deposito (CUN-005).
    ///
    /// <para>El cliente presenta el comprobante y el encargado despacha. No
    /// mueve existencias: el stock ya bajo al emitirse el comprobante.</para>
    ///
    /// <para>Un comprobante se despacha una sola vez. Si lo que se entrega es
    /// menos de lo facturado, la entrega queda marcada como Parcial y se deja
    /// asentada como tal en la bitacora.</para>
    /// </summary>
    public class EntregaBLL_575_AV
    {
        private const string TABLA = "Entrega";
        private const string TABLA_DETALLE = "DetalleEntrega";

        private MP_Entrega_575_AV mpEntrega = new MP_Entrega_575_AV();
        private MP_Comprobante_575_AV mpComprobante = new MP_Comprobante_575_AV();
        private ArticuloBLL_575_AV articuloBLL = new ArticuloBLL_575_AV();
        private BitacoraBLL bitacora = new BitacoraBLL();
        private VerificadorIntegridadBLL verificador = new VerificadorIntegridadBLL();

        #region Validacion del comprobante

        /// <summary>
        /// Valida el comprobante que presenta el cliente y devuelve lo que hay
        /// que entregar, con la ubicacion de deposito de cada articulo.
        ///
        /// <para>Rechaza el comprobante inexistente y el que ya se uso para
        /// retirar mercaderia.</para>
        /// </summary>
        public List<DetalleComprobanteBE_575_AV> ValidarComprobante(string nro_comprobante)
        {
            if (string.IsNullOrWhiteSpace(nro_comprobante))
            {
                throw new InvalidOperationException("Hay que indicar el numero de comprobante.");
            }

            ComprobanteBE_575_AV comprobante =
                mpComprobante.BuscarPorNumero(nro_comprobante.Trim());

            if (comprobante == null)
            {
                throw new InvalidOperationException(
                    "No existe el comprobante " + nro_comprobante.Trim() + ".");
            }
            if (!comprobante.se_puede_entregar)
            {
                throw new InvalidOperationException(
                    "El comprobante " + comprobante.nro_comprobante +
                    " ya fue utilizado para retirar la mercaderia.");
            }

            return Completar(mpComprobante.ListarDetalle(comprobante.id_comprobante));
        }

        /// <summary>Devuelve el comprobante listo para despachar, o nulo si no se puede.</summary>
        public ComprobanteBE_575_AV ObtenerComprobante(string nro_comprobante)
        {
            if (string.IsNullOrWhiteSpace(nro_comprobante)) { return null; }

            ComprobanteBE_575_AV comprobante =
                mpComprobante.BuscarPorNumero(nro_comprobante.Trim());

            if (comprobante == null) { return null; }

            comprobante.detalle = Completar(mpComprobante.ListarDetalle(comprobante.id_comprobante));
            return comprobante;
        }

        #endregion

        #region Registro

        /// <summary>
        /// Registra el despacho y marca el comprobante como entregado.
        ///
        /// <para>Ningun renglon puede superar lo facturado. Si el total
        /// entregado es menor, la entrega queda como Parcial.</para>
        /// </summary>
        public EntregaBE_575_AV RegistrarEntrega(ComprobanteBE_575_AV comprobante,
                                                 List<DetalleEntregaBE_575_AV> detalles,
                                                 string usuario)
        {
            if (comprobante == null) { throw new ArgumentNullException("comprobante"); }
            if (detalles == null || detalles.Count == 0)
            {
                throw new InvalidOperationException("No hay articulos para entregar.");
            }

            // El comprobante se relee: no se despacha dos veces el mismo.
            ComprobanteBE_575_AV vigente = mpComprobante.ListarComprobantes()
                .FirstOrDefault(c => c.id_comprobante == comprobante.id_comprobante);

            if (vigente == null)
            {
                throw new InvalidOperationException("El comprobante no existe.");
            }
            if (!vigente.se_puede_entregar)
            {
                throw new InvalidOperationException(
                    "El comprobante " + vigente.nro_comprobante +
                    " ya fue utilizado para retirar la mercaderia.");
            }

            List<DetalleComprobanteBE_575_AV> facturado =
                mpComprobante.ListarDetalle(vigente.id_comprobante);

            bool completa = ValidarCantidades(facturado, detalles);

            EntregaBE_575_AV entrega = new EntregaBE_575_AV
            {
                id_comprobante = vigente.id_comprobante,
                usuario_deposito = usuario,
                fecha_entrega = DateTime.Now,
                estado = completa ? EstadoEntrega_575_AV.Total : EstadoEntrega_575_AV.Parcial
            };

            entrega.id_entrega = mpEntrega.CrearEntrega(entrega);

            // La base fija la fecha con su propio redondeo: se relee antes de
            // calcular el dvh.
            EntregaBE_575_AV registrada = mpEntrega.ListarEntregas()
                .FirstOrDefault(e => e.id_entrega == entrega.id_entrega) ?? entrega;

            PersistirDigito(registrada, TABLA);

            foreach (DetalleEntregaBE_575_AV renglon in detalles)
            {
                renglon.id_entrega = registrada.id_entrega;
                renglon.id_detalle_entrega = mpEntrega.AgregarDetalle(renglon);

                PersistirDigito(renglon, TABLA_DETALLE);
                registrada.detalle.Add(renglon);
            }

            // El comprobante se consume: no sirve para un segundo retiro.
            mpComprobante.ActualizarEstado(vigente.id_comprobante, EstadoComprobante_575_AV.Entregado);

            ComprobanteBE_575_AV cerrado = mpComprobante.ListarComprobantes()
                .First(c => c.id_comprobante == vigente.id_comprobante);

            cerrado.dvh = VerificadorIntegridad.CalcularDVH(cerrado);
            mpComprobante.ActualizarDVH(cerrado.id_comprobante, cerrado.dvh);
            verificador.ActualizarDVV("Comprobante");

            bitacora.RegistrarBitacora(usuario,
                completa ? TipoAccion.EntregaRegistrada : TipoAccion.EntregaParcial);

            registrada.nro_comprobante = vigente.nro_comprobante;
            return registrada;
        }

        #endregion

        #region Integridad

        public bool VerificarIntegridad()
        {
            bool entregasOk = verificador.VerificarIntegridad(
                mpEntrega.ListarEntregas().OrderBy(e => e.id_entrega).ToList(), TABLA);

            bool detallesOk = verificador.VerificarIntegridad(
                mpEntrega.ListarDetalles().OrderBy(d => d.id_detalle_entrega).ToList(),
                TABLA_DETALLE);

            return entregasOk && detallesOk;
        }

        public void RecalcularDV()
        {
            foreach (EntregaBE_575_AV e in mpEntrega.ListarEntregas().OrderBy(x => x.id_entrega))
            {
                e.dvh = VerificadorIntegridad.CalcularDVH(e);
                mpEntrega.ActualizarDVH(e.id_entrega, e.dvh);
            }
            verificador.ActualizarDVV(TABLA);

            foreach (DetalleEntregaBE_575_AV d in mpEntrega.ListarDetalles()
                        .OrderBy(x => x.id_detalle_entrega))
            {
                d.dvh = VerificadorIntegridad.CalcularDVH(d);
                mpEntrega.ActualizarDVHDetalle(d.id_detalle_entrega, d.dvh);
            }
            verificador.ActualizarDVV(TABLA_DETALLE);
        }

        /// <summary>
        /// Calcula y persiste el dvh de una fila de las que toca esta clase, y
        /// el DVV de su tabla.
        /// </summary>
        private void PersistirDigito(IVerificable entidad, string tabla)
        {
            string dvh = VerificadorIntegridad.CalcularDVH(entidad);

            if (tabla == TABLA)
            {
                EntregaBE_575_AV entrega = (EntregaBE_575_AV)entidad;
                entrega.dvh = dvh;
                mpEntrega.ActualizarDVH(entrega.id_entrega, dvh);
            }
            else if (tabla == TABLA_DETALLE)
            {
                DetalleEntregaBE_575_AV detalle = (DetalleEntregaBE_575_AV)entidad;
                detalle.dvh = dvh;
                mpEntrega.ActualizarDVHDetalle(detalle.id_detalle_entrega, dvh);
            }
            else
            {
                throw new InvalidOperationException("Tabla no contemplada: " + tabla);
            }

            verificador.ActualizarDVV(tabla);
        }

        #endregion

        #region Apoyo

        /// <summary>
        /// Controla lo entregado contra lo facturado. Devuelve verdadero si la
        /// entrega cubre todo el comprobante.
        /// </summary>
        private bool ValidarCantidades(List<DetalleComprobanteBE_575_AV> facturado,
                                       List<DetalleEntregaBE_575_AV> entregado)
        {
            bool completa = true;

            foreach (DetalleComprobanteBE_575_AV renglon in facturado)
            {
                decimal cantidad = entregado
                    .Where(e => e.id_articulo == renglon.id_articulo)
                    .Sum(e => e.cantidad_entregada);

                if (cantidad > renglon.cantidad)
                {
                    throw new InvalidOperationException(
                        "No se puede entregar mas de lo facturado del articulo " +
                        renglon.id_articulo + ".");
                }

                if (cantidad < renglon.cantidad) { completa = false; }
            }

            // Ningun renglon entregado puede ser ajeno al comprobante.
            foreach (DetalleEntregaBE_575_AV renglon in entregado)
            {
                if (!facturado.Any(f => f.id_articulo == renglon.id_articulo))
                {
                    throw new InvalidOperationException(
                        "Hay un articulo que no figura en el comprobante.");
                }
            }

            return completa;
        }

        /// <summary>Completa los datos de articulo que necesita el despacho.</summary>
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

        #endregion
    }
}
