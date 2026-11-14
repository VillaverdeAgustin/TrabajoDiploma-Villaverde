using System.Globalization;
using System.Text.RegularExpressions;

namespace Entidad_BE
{
    public class ItemListaPrecioBE_575_AV : IVerificable
    {
        public int id_item_lista { get; set; }
        public int id_lista_precio { get; set; }
        public int? id_articulo { get; set; }
        public string codigo_proveedor { get; set; }
        public string descripcion { get; set; }
        public decimal precio_lista { get; set; }
        public string unidad_venta { get; set; }
        public int? plazo_entrega { get; set; }
        public string dvh { get; set; }

        public int numero_renglon { get; set; }

        public ResultadoImportacion_575_AV resultado { get; set; }

        public string motivo { get; set; }

        public string descripcion_articulo { get; set; }

        public decimal multiplo_venta
        {
            get { return MultiploDe(unidad_venta); }
        }

        public static decimal MultiploDe(string unidad_venta)
        {
            if (string.IsNullOrWhiteSpace(unidad_venta)) { return 1; }

            Match m = Regex.Match(unidad_venta, @"\bx\s*(\d+(?:[.,]\d+)?)", RegexOptions.IgnoreCase);
            decimal multiplo;
            if (m.Success && decimal.TryParse(m.Groups[1].Value.Replace(',', '.'), NumberStyles.Number,
                    CultureInfo.InvariantCulture, out multiplo) && multiplo > 0)
            {
                return multiplo;
            }
            return 1;
        }

        public string digito { get { return dvh; } }

        public string ObtenerCamposDV()
        {
            return string.Join("|",
                id_item_lista.ToString(CultureInfo.InvariantCulture),
                id_lista_precio.ToString(CultureInfo.InvariantCulture),
                id_articulo.HasValue ? id_articulo.Value.ToString(CultureInfo.InvariantCulture) : string.Empty,
                codigo_proveedor,
                descripcion,
                precio_lista.ToString("F2", CultureInfo.InvariantCulture),
                unidad_venta,
                plazo_entrega.HasValue ? plazo_entrega.Value.ToString(CultureInfo.InvariantCulture) : string.Empty);
        }
    }
}
