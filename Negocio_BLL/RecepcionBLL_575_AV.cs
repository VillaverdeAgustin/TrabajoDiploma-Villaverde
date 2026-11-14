using Acceso_DAL;
using Entidad_BE;
using Servicios;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Negocio_BLL
{
    public class RecepcionBLL_575_AV
    {
        private const string TABLA = "Recepcion";
        private const string TABLA_DETALLE = "DetalleRecepcion";
        private const string TABLA_ORDEN = "OrdenCompra";
        private const string TABLA_DETALLE_ORDEN = "DetalleOrdenCompra";
        private const int LARGO_REMITO = 30;
        private const int LARGO_DIFERENCIAS = 300;

        private MP_Recepcion_575_AV mpRecepcion = new MP_Recepcion_575_AV();
        private MP_OrdenCompra_575_AV mpOrden = new MP_OrdenCompra_575_AV();
        private OrdenCompraBLL_575_AV ordenCompraBLL = new OrdenCompraBLL_575_AV();
        private ListaPrecioBLL_575_AV listaPrecioBLL = new ListaPrecioBLL_575_AV();
        private BobinaBLL_575_AV bobinaBLL = new BobinaBLL_575_AV();
        private BitacoraBLL bitacora = new BitacoraBLL();
        private VerificadorIntegridadBLL verificador = new VerificadorIntegridadBLL();

        #region Recepcion (CUN-012)

        public RecepcionBE_575_AV RegistrarRecepcion(OrdenCompraBE_575_AV orden, RecepcionBE_575_AV recepcion,
                                                     List<DetalleRecepcionBE_575_AV> detalles, string usuario)
        {
            OrdenCompraBE_575_AV guardada = orden == null ? null : ordenCompraBLL.ListarParaRecepcion()
                .FirstOrDefault(o => o.id_orden_compra == orden.id_orden_compra);
            if (guardada == null)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("RCP_ERR_ORDEN"));
            }

            string remito = recepcion == null || recepcion.remito_proveedor == null ? string.Empty : recepcion.remito_proveedor.Trim();
            if (remito.Length == 0 || remito.Length > LARGO_REMITO)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("RCP_ERR_REMITO"));
            }

            Validar(guardada, detalles);

            List<DetalleRecepcionBE_575_AV> recibidos = detalles.Where(d => d.cantidad_recibida > 0).ToList();

            recepcion.remito_proveedor = remito;
            recepcion.id_orden_compra = guardada.id_orden_compra;
            recepcion.usuario_deposito = usuario;
            recepcion.fecha_recepcion = DateTime.Now;
            recepcion.diferencias = Diferencias(guardada.detalle, recibidos);

            recepcion.id_recepcion = mpRecepcion.CrearRecepcion(recepcion);
            recepcion = mpRecepcion.ListarRecepciones().First(r => r.id_recepcion == recepcion.id_recepcion);
            PersistirDigito(recepcion, TABLA);

            foreach (DetalleRecepcionBE_575_AV recibido in recibidos)
            {
                DetalleOrdenCompraBE_575_AV renglon = guardada.detalle.First(d => d.id_articulo == recibido.id_articulo);

                recibido.id_recepcion = recepcion.id_recepcion;
                recibido.id_unidad_medida = renglon.id_unidad_medida;
                recibido.id_detalle_recepcion = mpRecepcion.AgregarDetalle(recibido);
                PersistirDVH(recibido, TABLA_DETALLE);

                renglon.cantidad_recibida += recibido.cantidad_recibida;
                mpOrden.ActualizarCantidadRecibida(renglon.id_detalle_orden_compra, renglon.cantidad_recibida);
                PersistirDVH(renglon, TABLA_DETALLE_ORDEN);

                GestorDeExistencias_575_AV.GetInstance.Reponer(recibido.id_articulo, recibido.cantidad_recibida, usuario);

                if (renglon.fraccionable)
                {
                    CrearBobinas(guardada.id_proveedor, recepcion.id_recepcion, recibido, usuario);
                }
            }
            verificador.ActualizarDVV(TABLA_DETALLE);
            verificador.ActualizarDVV(TABLA_DETALLE_ORDEN);

            bool completa = guardada.detalle.All(d => d.cantidad_recibida >= d.cantidad);
            EstadoOrdenCompra_575_AV estado = completa ? EstadoOrdenCompra_575_AV.Recibida : EstadoOrdenCompra_575_AV.RecibidaParcial;

            mpOrden.ActualizarEstado(guardada.id_orden_compra, estado);
            PersistirDigito(mpOrden.ListarPorEstado(new[] { estado }).First(o => o.id_orden_compra == guardada.id_orden_compra), TABLA_ORDEN);

            bitacora.RegistrarBitacora(usuario, completa ? TipoAccion.RecepcionMercaderia : TipoAccion.RecepcionParcial);

            return recepcion;
        }

        private void Validar(OrdenCompraBE_575_AV orden, List<DetalleRecepcionBE_575_AV> detalles)
        {
            if (detalles == null || !detalles.Any(d => d.cantidad_recibida > 0))
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("RCP_ERR_SIN_CANTIDADES"));
            }

            foreach (DetalleRecepcionBE_575_AV detalle in detalles)
            {
                DetalleOrdenCompraBE_575_AV renglon = orden.detalle.FirstOrDefault(d => d.id_articulo == detalle.id_articulo);
                if (renglon == null || detalle.cantidad_recibida < 0)
                {
                    throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("RCP_ERR_RENGLON"));
                }
                if (detalle.cantidad_recibida > renglon.cantidad_pendiente)
                {
                    throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("RCP_ERR_SUPERA"));
                }
                if (!renglon.fraccionable && detalle.cantidad_recibida != decimal.Truncate(detalle.cantidad_recibida))
                {
                    throw new InvalidOperationException(string.Format(
                        GestorDeIdioma.GetInstance.Traducir("RCP_ERR_ENTERA"), renglon.codigo_articulo));
                }
            }
        }

        private static string Diferencias(List<DetalleOrdenCompraBE_575_AV> renglones, List<DetalleRecepcionBE_575_AV> recibidos)
        {
            List<string> faltantes = new List<string>();
            foreach (DetalleOrdenCompraBE_575_AV renglon in renglones)
            {
                decimal falta = renglon.cantidad_pendiente
                    - recibidos.Where(r => r.id_articulo == renglon.id_articulo).Sum(r => r.cantidad_recibida);
                if (falta > 0)
                {
                    faltantes.Add(string.Format(GestorDeIdioma.GetInstance.Traducir("RCP_DIF_FALTAN"),
                        renglon.codigo_articulo, falta.ToString("N2", CultureInfo.CurrentCulture), renglon.abreviatura_unidad));
                }
            }

            if (faltantes.Count == 0) { return null; }

            string texto = string.Join("; ", faltantes);
            return texto.Length > LARGO_DIFERENCIAS ? texto.Substring(0, LARGO_DIFERENCIAS) : texto;
        }

        private void CrearBobinas(int id_proveedor, int id_recepcion, DetalleRecepcionBE_575_AV recibido, string usuario)
        {
            ItemListaPrecioBE_575_AV vigente = listaPrecioBLL.ObtenerPrecioVigente(id_proveedor, recibido.id_articulo);
            decimal medida = vigente == null ? 1 : vigente.multiplo_venta;

            decimal resto = recibido.cantidad_recibida;
            int numero = 0;
            while (resto > 0)
            {
                decimal inicial = medida > 1 && resto > medida ? medida : resto;
                numero++;

                bobinaBLL.CrearBobina(new BobinaBE_575_AV
                {
                    id_articulo = recibido.id_articulo,
                    identificador = string.Format(CultureInfo.InvariantCulture, "BOB-{0:D3}-{1:D5}-{2:D3}",
                        recibido.id_articulo, id_recepcion, numero),
                    medida_inicial = inicial
                }, usuario);

                resto -= inicial;
            }
        }

        #endregion

        #region Integridad

        public bool VerificarIntegridad()
        {
            bool recepcionesOk = verificador.VerificarIntegridad(
                mpRecepcion.ListarRecepciones().OrderBy(r => r.id_recepcion).ToList(), TABLA);
            bool detallesOk = verificador.VerificarIntegridad(
                mpRecepcion.ListarDetalles().OrderBy(d => d.id_detalle_recepcion).ToList(), TABLA_DETALLE);

            return recepcionesOk && detallesOk;
        }

        public void RecalcularDV()
        {
            foreach (RecepcionBE_575_AV recepcion in mpRecepcion.ListarRecepciones().OrderBy(r => r.id_recepcion))
            {
                recepcion.dvh = VerificadorIntegridad.CalcularDVH(recepcion);
                mpRecepcion.ActualizarDVH(recepcion.id_recepcion, recepcion.dvh);
            }
            verificador.ActualizarDVV(TABLA);

            foreach (DetalleRecepcionBE_575_AV detalle in mpRecepcion.ListarDetalles().OrderBy(d => d.id_detalle_recepcion))
            {
                detalle.dvh = VerificadorIntegridad.CalcularDVH(detalle);
                mpRecepcion.ActualizarDVHDetalle(detalle.id_detalle_recepcion, detalle.dvh);
            }
            verificador.ActualizarDVV(TABLA_DETALLE);
        }

        private void PersistirDigito(IVerificable entidad, string tabla)
        {
            PersistirDVH(entidad, tabla);
            verificador.ActualizarDVV(tabla);
        }

        private void PersistirDVH(IVerificable entidad, string tabla)
        {
            string dvh = VerificadorIntegridad.CalcularDVH(entidad);

            switch (tabla)
            {
                case TABLA:
                    RecepcionBE_575_AV recepcion = (RecepcionBE_575_AV)entidad;
                    recepcion.dvh = dvh;
                    mpRecepcion.ActualizarDVH(recepcion.id_recepcion, dvh);
                    break;

                case TABLA_DETALLE:
                    DetalleRecepcionBE_575_AV detalle = (DetalleRecepcionBE_575_AV)entidad;
                    detalle.dvh = dvh;
                    mpRecepcion.ActualizarDVHDetalle(detalle.id_detalle_recepcion, dvh);
                    break;

                case TABLA_ORDEN:
                    OrdenCompraBE_575_AV orden = (OrdenCompraBE_575_AV)entidad;
                    orden.dvh = dvh;
                    mpOrden.ActualizarDVH(orden.id_orden_compra, dvh);
                    break;

                case TABLA_DETALLE_ORDEN:
                    DetalleOrdenCompraBE_575_AV renglon = (DetalleOrdenCompraBE_575_AV)entidad;
                    renglon.dvh = dvh;
                    mpOrden.ActualizarDVHDetalle(renglon.id_detalle_orden_compra, dvh);
                    break;

                default:
                    throw new ArgumentException("Tabla no contemplada: " + tabla);
            }
        }

        #endregion
    }
}
