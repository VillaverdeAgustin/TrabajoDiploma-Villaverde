using System;

namespace Entidad_BE
{
    // A03 - Contenido del archivo XML de una orden de compra: cabecera con su detalle y el proveedor.
    public class DocumentoOrdenCompra_575_AV
    {
        public string version { get; set; }
        public DateTime fecha_generacion { get; set; }
        public string usuario { get; set; }
        public OrdenCompraBE_575_AV orden { get; set; }
        public ProveedorBE_575_AV proveedor { get; set; }
    }
}
