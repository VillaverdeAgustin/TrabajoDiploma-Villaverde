using Acceso_DAL;
using Entidad_BE;
using Servicios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Negocio_BLL
{
    public class OrdenCompraBLL_575_AV
    {
        private const string TABLA = "OrdenCompra";
        private const string TABLA_DETALLE = "DetalleOrdenCompra";

        private MP_OrdenCompra_575_AV mpOrden = new MP_OrdenCompra_575_AV();
        private ArticuloBLL_575_AV articuloBLL = new ArticuloBLL_575_AV();
        private ListaPrecioBLL_575_AV listaPrecioBLL = new ListaPrecioBLL_575_AV();
        private BitacoraBLL bitacora = new BitacoraBLL();
        private VerificadorIntegridadBLL verificador = new VerificadorIntegridadBLL();

        #region Borrador y generacion (CUN-009 y CUN-010)

        public OrdenCompraBE_575_AV AgregarArticuloABorrador(int id_proveedor, DetalleOrdenCompraBE_575_AV detalle, string usuario)
        {
            if (id_proveedor <= 0)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("OC_ERR_PROVEEDOR"));
            }
            if (detalle == null || detalle.cantidad <= 0)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("OC_ERR_CANTIDAD"));
            }

            ArticuloBE_575_AV articulo = articuloBLL.ObtenerPorId(detalle.id_articulo);
            if (articulo == null)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("ART_ERR_NO_EXISTE"));
            }

            ItemListaPrecioBE_575_AV vigente = listaPrecioBLL.ObtenerPrecioVigente(id_proveedor, detalle.id_articulo);
            if (vigente != null)
            {
                detalle.precio_unitario = vigente.precio_lista;
                detalle.unidad_venta = vigente.unidad_venta;
            }
            else if (detalle.precio_unitario > 0)
            {
                detalle.unidad_venta = null;
            }
            else
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("OC_ERR_SIN_PRECIO"));
            }

            detalle.fraccionable = articulo.fraccionable;
            AjustarAMultiplo(detalle);

            OrdenCompraBE_575_AV orden = mpOrden.BuscarBorrador(id_proveedor);
            if (orden == null)
            {
                orden = new OrdenCompraBE_575_AV
                {
                    id_proveedor = id_proveedor,
                    usuario_emisor = usuario,
                    estado = EstadoOrdenCompra_575_AV.Borrador,
                    activo = true
                };
                orden.id_orden_compra = mpOrden.CrearOrdenCompra(orden);
                orden = mpOrden.BuscarBorrador(id_proveedor);

                PersistirDigito(orden, TABLA);
                bitacora.RegistrarBitacora(usuario, TipoAccion.AltaOrdenCompra);
            }
            else if (mpOrden.ListarDetalle(orden.id_orden_compra).Any(d => d.id_articulo == detalle.id_articulo))
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("OC_ERR_ARTICULO_EN_BORRADOR"));
            }

            detalle.id_orden_compra = orden.id_orden_compra;
            detalle.id_unidad_medida = articulo.id_unidad_medida;
            detalle.subtotal = decimal.Round(detalle.precio_unitario * detalle.cantidad, 2);
            detalle.cantidad_recibida = 0;

            detalle.id_detalle_orden_compra = mpOrden.AgregarDetalle(detalle);
            PersistirDigito(detalle, TABLA_DETALLE);

            orden.total = mpOrden.ListarDetalle(orden.id_orden_compra).Sum(d => d.subtotal);
            mpOrden.ActualizarTotal(orden.id_orden_compra, orden.total);
            PersistirDigito(orden, TABLA);

            bitacora.RegistrarBitacora(usuario, TipoAccion.AltaDetalleOrdenCompra);

            orden.detalle = ObtenerDetalle(orden.id_orden_compra);
            return orden;
        }

        public OrdenCompraBE_575_AV ObtenerBorrador(int id_proveedor)
        {
            OrdenCompraBE_575_AV orden = mpOrden.BuscarBorrador(id_proveedor);
            if (orden != null) { orden.detalle = ObtenerDetalle(orden.id_orden_compra); }
            return orden;
        }

        public OrdenCompraBE_575_AV GenerarOrdenCompra(OrdenCompraBE_575_AV orden, List<DetalleOrdenCompraBE_575_AV> detalles, string usuario)
        {
            if (orden == null || orden.id_orden_compra <= 0)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("OC_ERR_SIN_BORRADOR"));
            }

            OrdenCompraBE_575_AV borrador = Releer(orden.id_orden_compra);
            if (borrador == null || borrador.estado != EstadoOrdenCompra_575_AV.Borrador)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("OC_ERR_SIN_BORRADOR"));
            }

            borrador.detalle = mpOrden.ListarDetalle(borrador.id_orden_compra);
            Validar(borrador);

            bool desactualizado = detalles == null
                || detalles.Count != borrador.detalle.Count
                || detalles.Any(d => !borrador.detalle.Any(b => b.id_detalle_orden_compra == d.id_detalle_orden_compra
                                                             && b.cantidad == d.cantidad));
            if (desactualizado)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("OC_ERR_DESACTUALIZADA"));
            }

            decimal total = borrador.detalle.Sum(d => d.subtotal);
            mpOrden.Emitir(borrador.id_orden_compra, mpOrden.ProximoNumero(), total);

            OrdenCompraBE_575_AV emitida = Releer(borrador.id_orden_compra);
            PersistirDigito(emitida, TABLA);
            bitacora.RegistrarBitacora(usuario, TipoAccion.EmisionOrdenCompra);

            emitida.detalle = ObtenerDetalle(emitida.id_orden_compra);
            return emitida;
        }

        #endregion

        #region Consulta

        public List<OrdenCompraBE_575_AV> ListarParaRecepcion()
        {
            return ConDetalle(mpOrden.ListarPorEstado(new[]
            {
                EstadoOrdenCompra_575_AV.Emitida,
                EstadoOrdenCompra_575_AV.RecibidaParcial
            }));
        }

        // A03 - Ordenes ya emitidas (todas menos los borradores), con su detalle
        public List<OrdenCompraBE_575_AV> ListarEmitidas()
        {
            return ConDetalle(mpOrden.ListarPorEstado(new[]
            {
                EstadoOrdenCompra_575_AV.Emitida,
                EstadoOrdenCompra_575_AV.RecibidaParcial,
                EstadoOrdenCompra_575_AV.Recibida
            }));
        }

        // A03 - Una orden con su detalle, o null si no existe
        public OrdenCompraBE_575_AV ObtenerOrden(int id_orden_compra)
        {
            OrdenCompraBE_575_AV orden = Releer(id_orden_compra);
            if (orden != null) { orden.detalle = ObtenerDetalle(orden.id_orden_compra); }
            return orden;
        }

        public List<DetalleOrdenCompraBE_575_AV> ObtenerDetalle(int id_orden_compra)
        {
            List<DetalleOrdenCompraBE_575_AV> detalles = mpOrden.ListarDetalle(id_orden_compra);
            if (detalles.Count == 0) { return detalles; }

            Dictionary<int, ArticuloBE_575_AV> articulos = articuloBLL.ListarArticulos().ToDictionary(a => a.id_articulo);
            foreach (DetalleOrdenCompraBE_575_AV detalle in detalles)
            {
                ArticuloBE_575_AV articulo;
                if (articulos.TryGetValue(detalle.id_articulo, out articulo))
                {
                    detalle.codigo_articulo = articulo.codigo;
                    detalle.descripcion_articulo = articulo.descripcion;
                    detalle.abreviatura_unidad = articulo.abreviatura_unidad;
                    detalle.fraccionable = articulo.fraccionable;
                }
            }
            return detalles;
        }

        private List<OrdenCompraBE_575_AV> ConDetalle(List<OrdenCompraBE_575_AV> ordenes)
        {
            foreach (OrdenCompraBE_575_AV orden in ordenes)
            {
                orden.detalle = ObtenerDetalle(orden.id_orden_compra);
            }
            return ordenes;
        }

        #endregion

        #region Validacion

        private void Validar(OrdenCompraBE_575_AV orden)
        {
            if (orden.detalle == null || orden.detalle.Count == 0)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("OC_ERR_SIN_RENGLONES"));
            }
            if (orden.detalle.Any(d => d.cantidad <= 0))
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("OC_ERR_CANTIDAD"));
            }
            if (orden.detalle.Any(d => d.precio_unitario <= 0))
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("OC_ERR_SIN_PRECIO"));
            }
        }

        private void AjustarAMultiplo(DetalleOrdenCompraBE_575_AV detalle)
        {
            decimal cantidad = detalle.fraccionable ? detalle.cantidad : Math.Ceiling(detalle.cantidad);

            decimal multiplo = ItemListaPrecioBE_575_AV.MultiploDe(detalle.unidad_venta);
            if (multiplo > 1)
            {
                cantidad = Math.Ceiling(cantidad / multiplo) * multiplo;
            }

            detalle.cantidad = cantidad;
        }

        private OrdenCompraBE_575_AV Releer(int id_orden_compra)
        {
            return mpOrden.ListarPorEstado(null).FirstOrDefault(o => o.id_orden_compra == id_orden_compra);
        }

        #endregion

        #region Integridad

        public bool VerificarIntegridad()
        {
            bool ordenesOk = verificador.VerificarIntegridad(
                mpOrden.ListarPorEstado(null).OrderBy(o => o.id_orden_compra).ToList(), TABLA);
            bool detallesOk = verificador.VerificarIntegridad(
                mpOrden.ListarDetalles().OrderBy(d => d.id_detalle_orden_compra).ToList(), TABLA_DETALLE);

            return ordenesOk && detallesOk;
        }

        public void RecalcularDV()
        {
            foreach (OrdenCompraBE_575_AV orden in mpOrden.ListarPorEstado(null).OrderBy(o => o.id_orden_compra))
            {
                orden.dvh = VerificadorIntegridad.CalcularDVH(orden);
                mpOrden.ActualizarDVH(orden.id_orden_compra, orden.dvh);
            }
            verificador.ActualizarDVV(TABLA);

            foreach (DetalleOrdenCompraBE_575_AV detalle in mpOrden.ListarDetalles().OrderBy(d => d.id_detalle_orden_compra))
            {
                detalle.dvh = VerificadorIntegridad.CalcularDVH(detalle);
                mpOrden.ActualizarDVHDetalle(detalle.id_detalle_orden_compra, detalle.dvh);
            }
            verificador.ActualizarDVV(TABLA_DETALLE);
        }

        private void PersistirDigito(IVerificable entidad, string tabla)
        {
            string dvh = VerificadorIntegridad.CalcularDVH(entidad);

            switch (tabla)
            {
                case TABLA:
                    OrdenCompraBE_575_AV orden = (OrdenCompraBE_575_AV)entidad;
                    orden.dvh = dvh;
                    mpOrden.ActualizarDVH(orden.id_orden_compra, dvh);
                    break;

                case TABLA_DETALLE:
                    DetalleOrdenCompraBE_575_AV detalle = (DetalleOrdenCompraBE_575_AV)entidad;
                    detalle.dvh = dvh;
                    mpOrden.ActualizarDVHDetalle(detalle.id_detalle_orden_compra, dvh);
                    break;

                default:
                    throw new ArgumentException("Tabla no contemplada: " + tabla);
            }

            verificador.ActualizarDVV(tabla);
        }

        #endregion
    }
}
