using System;

namespace Entidad_BE
{
    // A02 - Una orden de compra emitida tal como la lee el reporte de compras (solo lectura).
    // monto_ars y monto_usd separan el total segun la moneda de la lista de precios de cada renglon.
    public class OrdenReporteCompra_575_AV
    {
        public int id_orden_compra { get; set; }
        public string nro_orden_compra { get; set; }
        public int id_proveedor { get; set; }
        public string razon_social { get; set; }
        public DateTime fecha_emision { get; set; }
        public EstadoOrdenCompra_575_AV estado { get; set; }
        public decimal total { get; set; }
        public decimal monto_ars { get; set; }
        public decimal monto_usd { get; set; }
        public int renglones { get; set; }
        public decimal cantidad_solicitada { get; set; }
        public decimal cantidad_recibida { get; set; }
        public decimal? porcentaje_recibido { get; set; }
    }
}
