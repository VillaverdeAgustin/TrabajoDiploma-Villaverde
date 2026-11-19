using System.Collections.Generic;

namespace Entidad_BE
{
    // A02 - Una fila del reporte de compras: lo comprado a un proveedor en el periodo.
    // La fila de totales es la misma clase con es_total en verdadero y todas las ordenes.
    public class ResumenProveedorCompra_575_AV
    {
        public int id_proveedor { get; set; }
        public string razon_social { get; set; }
        public int cantidad_ordenes { get; set; }
        public decimal monto_ars { get; set; }
        public decimal monto_usd { get; set; }
        public decimal cantidad_solicitada { get; set; }
        public decimal cantidad_recibida { get; set; }
        public decimal? porcentaje_recibido { get; set; }
        public int pendientes_recepcion { get; set; }
        public bool es_total { get; set; }
        public List<OrdenReporteCompra_575_AV> ordenes { get; set; }

        public ResumenProveedorCompra_575_AV()
        {
            ordenes = new List<OrdenReporteCompra_575_AV>();
        }
    }
}
