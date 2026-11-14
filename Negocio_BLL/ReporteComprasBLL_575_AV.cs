using Acceso_DAL;
using Entidad_BE;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Negocio_BLL
{
    // A02 - Reporte de compras por proveedor. Los datos los lee SP_ReporteCompras; aca se arma el reporte.
    public class ReporteComprasBLL_575_AV
    {
        // El porcentaje recibido se mide sobre las ordenes que ya se pueden recibir
        private static readonly EstadoOrdenCompra_575_AV[] EstadosConRecepcion =
        {
            EstadoOrdenCompra_575_AV.Autorizada,
            EstadoOrdenCompra_575_AV.RecibidaParcial,
            EstadoOrdenCompra_575_AV.Recibida
        };

        private MP_OrdenCompra_575_AV mpOrden = new MP_OrdenCompra_575_AV();
        private ProveedorBLL_575_AV proveedorBLL = new ProveedorBLL_575_AV();
        private BitacoraBLL bitacora = new BitacoraBLL();

        public List<ProveedorBE_575_AV> ListarProveedores()
        {
            return proveedorBLL.ListarProveedores().OrderBy(p => p.razon_social).ToList();
        }

        // Los estados por los que se puede filtrar: un borrador todavia no tiene fecha de emision
        public List<EstadoOrdenCompra_575_AV> ListarEstados()
        {
            return Enum.GetValues(typeof(EstadoOrdenCompra_575_AV)).Cast<EstadoOrdenCompra_575_AV>()
                .Where(e => e != EstadoOrdenCompra_575_AV.Borrador)
                .ToList();
        }

        public ReporteComprasBE_575_AV GenerarReporte(DateTime desde, DateTime hasta, int? id_proveedor,
                                                       EstadoOrdenCompra_575_AV? estado, string usuario)
        {
            if (desde.Date > hasta.Date)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("REPC_ERR_RANGO"));
            }
            if (estado == EstadoOrdenCompra_575_AV.Borrador)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("REPC_ERR_ESTADO"));
            }

            List<OrdenReporteCompra_575_AV> ordenes = mpOrden.ListarParaReporte(desde, hasta,
                id_proveedor.HasValue && id_proveedor.Value > 0 ? id_proveedor : null, estado);

            foreach (OrdenReporteCompra_575_AV orden in ordenes)
            {
                orden.porcentaje_recibido = Porcentaje(new[] { orden });
            }

            ReporteComprasBE_575_AV reporte = new ReporteComprasBE_575_AV
            {
                desde = desde.Date,
                hasta = hasta.Date,
                id_proveedor = id_proveedor,
                estado = estado
            };

            reporte.proveedores = ordenes
                .GroupBy(o => o.id_proveedor)
                .Select(g => Resumir(g.ToList(), false))
                .OrderBy(r => r.razon_social)
                .ToList();
            reporte.total = Resumir(ordenes, true);

            bitacora.RegistrarBitacora(usuario, TipoAccion.ConsultaReporteCompras);
            return reporte;
        }

        private static ResumenProveedorCompra_575_AV Resumir(List<OrdenReporteCompra_575_AV> ordenes, bool esTotal)
        {
            List<OrdenReporteCompra_575_AV> conRecepcion = ordenes.Where(o => EstadosConRecepcion.Contains(o.estado)).ToList();
            OrdenReporteCompra_575_AV primera = ordenes.FirstOrDefault();

            return new ResumenProveedorCompra_575_AV
            {
                id_proveedor = esTotal || primera == null ? 0 : primera.id_proveedor,
                razon_social = esTotal || primera == null ? null : primera.razon_social,
                cantidad_ordenes = ordenes.Count,
                monto_ars = ordenes.Sum(o => o.monto_ars),
                monto_usd = ordenes.Sum(o => o.monto_usd),
                cantidad_solicitada = conRecepcion.Sum(o => o.cantidad_solicitada),
                cantidad_recibida = conRecepcion.Sum(o => o.cantidad_recibida),
                porcentaje_recibido = Porcentaje(conRecepcion),
                pendientes_autorizacion = ordenes.Count(o => o.estado == EstadoOrdenCompra_575_AV.PendienteAutorizacion),
                rechazadas = ordenes.Count(o => o.estado == EstadoOrdenCompra_575_AV.Rechazada),
                es_total = esTotal,
                ordenes = ordenes.OrderBy(o => o.fecha_emision).ThenBy(o => o.id_orden_compra).ToList()
            };
        }

        // Cantidad recibida sobre solicitada de las ordenes autorizadas o recibidas; sin ninguna, no hay porcentaje
        private static decimal? Porcentaje(IEnumerable<OrdenReporteCompra_575_AV> ordenes)
        {
            List<OrdenReporteCompra_575_AV> medibles = ordenes.Where(o => EstadosConRecepcion.Contains(o.estado)).ToList();
            decimal solicitada = medibles.Sum(o => o.cantidad_solicitada);
            if (solicitada <= 0) { return null; }

            return decimal.Round(medibles.Sum(o => o.cantidad_recibida) * 100m / solicitada, 1);
        }
    }
}
