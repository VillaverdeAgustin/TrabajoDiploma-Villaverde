using System;
using System.Collections.Generic;
using System.Linq;

namespace Entidad_BE
{
    // A02 - Reporte de compras: filtros aplicados, una fila por proveedor y la fila de totales.
    public class ReporteComprasBE_575_AV
    {
        public DateTime desde { get; set; }
        public DateTime hasta { get; set; }
        public int? id_proveedor { get; set; }
        public EstadoOrdenCompra_575_AV? estado { get; set; }
        public List<ResumenProveedorCompra_575_AV> proveedores { get; set; }
        public ResumenProveedorCompra_575_AV total { get; set; }

        public ReporteComprasBE_575_AV()
        {
            proveedores = new List<ResumenProveedorCompra_575_AV>();
            total = new ResumenProveedorCompra_575_AV { es_total = true };
        }

        // Las filas de la grilla: los proveedores y, al final, los totales
        public List<ResumenProveedorCompra_575_AV> filas
        {
            get { return proveedores.Concat(new[] { total }).ToList(); }
        }
    }
}
