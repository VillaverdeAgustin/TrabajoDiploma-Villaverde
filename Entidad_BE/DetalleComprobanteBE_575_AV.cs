using System.Globalization;

namespace Entidad_BE
{
    /// <summary>
    /// Renglon del comprobante. Copia cantidad y precio del carrito en lugar de
    /// leerlos por la FK: el comprobante conserva lo que se emitio.
    /// </summary>
    public class DetalleComprobanteBE_575_AV : IVerificable
    {
        public int id_detalle_comprobante { get; set; }
        public int id_comprobante { get; set; }
        public int id_articulo { get; set; }
        public decimal cantidad { get; set; }
        public int id_unidad_medida { get; set; }
        public decimal precio_unitario { get; set; }
        public string dvh { get; set; }

        /// <summary>Codigo del articulo, que resuelve la BLL para pantalla.</summary>
        public string codigo_articulo { get; set; }

        /// <summary>Descripcion del articulo, que resuelve la BLL para pantalla.</summary>
        public string descripcion_articulo { get; set; }

        /// <summary>Abreviatura de la unidad, para mostrar junto a la cantidad.</summary>
        public string abreviatura_unidad { get; set; }

        /// <summary>Ubicacion en deposito, que necesita el despacho (CUN-005).</summary>
        public string deposito_ubicacion { get; set; }

        public decimal subtotal
        {
            get { return decimal.Round(cantidad * precio_unitario, 2); }
        }

        public string digito { get { return dvh; } }

        public string ObtenerCamposDV()
        {
            return string.Join("|",
                id_detalle_comprobante.ToString(CultureInfo.InvariantCulture),
                id_comprobante.ToString(CultureInfo.InvariantCulture),
                id_articulo.ToString(CultureInfo.InvariantCulture),
                cantidad.ToString("F2", CultureInfo.InvariantCulture),
                id_unidad_medida.ToString(CultureInfo.InvariantCulture),
                precio_unitario.ToString("F2", CultureInfo.InvariantCulture));
        }

        public override string ToString()
        {
            return cantidad.ToString("F2", CultureInfo.InvariantCulture) + " " +
                   abreviatura_unidad + " de " + descripcion_articulo;
        }
    }
}
