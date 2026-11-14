using System.Globalization;

namespace Entidad_BE
{
    public class DetalleOrdenCompraBE_575_AV : IVerificable
    {
        public int id_detalle_orden_compra { get; set; }
        public int id_orden_compra { get; set; }
        public int id_articulo { get; set; }
        public decimal cantidad { get; set; }
        public int id_unidad_medida { get; set; }
        public decimal precio_unitario { get; set; }
        public decimal subtotal { get; set; }
        public decimal cantidad_recibida { get; set; }
        public string dvh { get; set; }

        public string codigo_articulo { get; set; }

        public string descripcion_articulo { get; set; }

        public string abreviatura_unidad { get; set; }

        public string unidad_venta { get; set; }

        public bool fraccionable { get; set; }

        public decimal cantidad_pendiente
        {
            get { return cantidad - cantidad_recibida; }
        }

        public string digito { get { return dvh; } }

        public string ObtenerCamposDV()
        {
            return string.Join("|",
                id_detalle_orden_compra.ToString(CultureInfo.InvariantCulture),
                id_orden_compra.ToString(CultureInfo.InvariantCulture),
                id_articulo.ToString(CultureInfo.InvariantCulture),
                cantidad.ToString("F2", CultureInfo.InvariantCulture),
                id_unidad_medida.ToString(CultureInfo.InvariantCulture),
                precio_unitario.ToString("F2", CultureInfo.InvariantCulture),
                subtotal.ToString("F2", CultureInfo.InvariantCulture),
                cantidad_recibida.ToString("F2", CultureInfo.InvariantCulture));
        }
    }
}
