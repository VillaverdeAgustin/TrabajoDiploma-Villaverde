using System.Globalization;

namespace Entidad_BE
{
    /// <summary>
    /// Renglon del carrito: un articulo con su cantidad y su precio.
    ///
    /// <para><c>id_bobina</c> queda en nulo salvo que el articulo se haya
    /// fraccionado de una bobina concreta (RFN1.8): ahi queda anotado de cual,
    /// para poder rastrear el metro vendido hasta su caja de origen.</para>
    /// </summary>
    public class DetalleCarritoBE_575_AV : IVerificable
    {
        public int id_detalle_carrito { get; set; }
        public int id_carrito { get; set; }
        public int id_articulo { get; set; }
        public decimal cantidad { get; set; }
        public int id_unidad_medida { get; set; }
        public decimal precio_unitario { get; set; }
        public decimal subtotal { get; set; }
        public int? id_bobina { get; set; }
        public string dvh { get; set; }

        /// <summary>Codigo del articulo, que resuelve la BLL para mostrar en pantalla.</summary>
        public string codigo_articulo { get; set; }

        /// <summary>Descripcion del articulo, que resuelve la BLL para pantalla.</summary>
        public string descripcion_articulo { get; set; }

        /// <summary>Abreviatura de la unidad, para mostrar junto a la cantidad.</summary>
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
                subtotal.ToString("F2", CultureInfo.InvariantCulture),
                id_bobina.HasValue
                    ? id_bobina.Value.ToString(CultureInfo.InvariantCulture)
                    : string.Empty);
        }

        public override string ToString()
        {
            return cantidad.ToString("F2", CultureInfo.InvariantCulture) + " " +
                   abreviatura_unidad + " de " + descripcion_articulo;
        }
    }
}
