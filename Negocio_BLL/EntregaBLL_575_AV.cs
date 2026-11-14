using Acceso_DAL;
using Entidad_BE;
using Servicios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Negocio_BLL
{
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

        public List<DetalleComprobanteBE_575_AV> ValidarComprobante(string nro_comprobante)
        {
            if (string.IsNullOrWhiteSpace(nro_comprobante))
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("DEP_ERR_NRO"));
            }

            ComprobanteBE_575_AV comprobante =
                mpComprobante.BuscarPorNumero(nro_comprobante.Trim());

            if (comprobante == null)
            {
                throw new InvalidOperationException(
                    string.Format(GestorDeIdioma.GetInstance.Traducir("DEP_ERR_NO_EXISTE"),
                        nro_comprobante.Trim()));
            }
            if (!comprobante.se_puede_entregar)
            {
                throw new InvalidOperationException(
                    string.Format(GestorDeIdioma.GetInstance.Traducir("DEP_ERR_YA_ENTREGADO"),
                        comprobante.nro_comprobante));
            }

            return Completar(mpComprobante.ListarDetalle(comprobante.id_comprobante));
        }

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

        public EntregaBE_575_AV RegistrarEntrega(ComprobanteBE_575_AV comprobante,
                                                 List<DetalleEntregaBE_575_AV> detalles,
                                                 string usuario)
        {
            if (comprobante == null) { throw new ArgumentNullException("comprobante"); }
            if (detalles == null || detalles.Count == 0)
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("DEP_ERR_SIN_ARTICULOS"));
            }

            ComprobanteBE_575_AV vigente = mpComprobante.ListarComprobantes()
                .FirstOrDefault(c => c.id_comprobante == comprobante.id_comprobante);

            if (vigente == null)
            {
                throw new InvalidOperationException(
                    string.Format(GestorDeIdioma.GetInstance.Traducir("DEP_ERR_NO_EXISTE"),
                        comprobante.nro_comprobante));
            }
            if (!vigente.se_puede_entregar)
            {
                throw new InvalidOperationException(
                    string.Format(GestorDeIdioma.GetInstance.Traducir("DEP_ERR_YA_ENTREGADO"),
                        vigente.nro_comprobante));
            }

            List<DetalleComprobanteBE_575_AV> facturado =
                Completar(mpComprobante.ListarDetalle(vigente.id_comprobante));

            bool completa = ValidarCantidades(facturado, detalles);

            EntregaBE_575_AV entrega = new EntregaBE_575_AV
            {
                id_comprobante = vigente.id_comprobante,
                usuario_deposito = usuario,
                fecha_entrega = DateTime.Now,
                estado = completa ? EstadoEntrega_575_AV.Total : EstadoEntrega_575_AV.Parcial
            };

            entrega.id_entrega = mpEntrega.CrearEntrega(entrega);

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
                throw new ArgumentException("Tabla no contemplada: " + tabla);
            }

            verificador.ActualizarDVV(tabla);
        }

        #endregion

        #region Apoyo

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
                        string.Format(GestorDeIdioma.GetInstance.Traducir("DEP_ERR_EXCEDE"),
                            renglon.descripcion_articulo));
                }

                if (cantidad < renglon.cantidad) { completa = false; }
            }

            foreach (DetalleEntregaBE_575_AV renglon in entregado)
            {
                if (!facturado.Any(f => f.id_articulo == renglon.id_articulo))
                {
                    throw new InvalidOperationException(
                        GestorDeIdioma.GetInstance.Traducir("DEP_ERR_AJENO"));
                }
            }

            return completa;
        }

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
