using System.Globalization;

namespace Entidad_BE
{
    public class DetalleCarritoBE_575_AV : IVerificable
    {
        public int id_detalle_carrito { get; set; }
        public int id_carrito { get; set; }
        public int id_articulo { get; set; }
        public decimal cantidad { get; set; }
        public int id_unidad_medida { get; set; }
        public decimal precio_unitario { get; set; }
        public decimal subtotal { get; set; }
        public string dvh { get; set; }

        public string codigo_articulo { get; set; }

        public string descripcion_articulo { get; set; }

        public string abreviatura_unidad { get; set; }

        public string digito { get { return dvh; } }

        public string ObtenerCamposDV()
        {
            return string.Join("|",
                id_detalle_carrito.ToString(CultureInfo.InvariantCulture),
                id_carrito.ToString(CultureInfo.InvariantCulture),
                id_articulo.ToString(CultureInfo.InvariantCulture),
                cantidad.ToString("F2", CultureInfo.InvariantCulture),
                id_unidad_medida.ToString(CultureInfo.InvariantCulture),
                precio_unitario.ToString("F2", CultureInfo.InvariantCulture),
                subtotal.ToString("F2", CultureInfo.InvariantCulture));
        }

        public override string ToString()
        {
            return cantidad.ToString("F2", CultureInfo.InvariantCulture) + " " +
                   abreviatura_unidad + " de " + descripcion_articulo;
        }
    }
}
