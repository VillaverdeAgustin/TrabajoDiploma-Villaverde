using System.Globalization;

namespace Entidad_BE
{
    /// <summary>Renglon del despacho: cuanto se entrego de cada articulo.</summary>
    public class DetalleEntregaBE_575_AV : IVerificable
    {
        public int id_detalle_entrega { get; set; }
        public int id_entrega { get; set; }
        public int id_articulo { get; set; }
        public decimal cantidad_entregada { get; set; }
        public int id_unidad_medida { get; set; }
        public string dvh { get; set; }

        /// <summary>Codigo del articulo, que resuelve la BLL para pantalla.</summary>
        public string codigo_articulo { get; set; }

        /// <summary>Descripcion del articulo, que resuelve la BLL para pantalla.</summary>
        public string descripcion_articulo { get; set; }

        /// <summary>Abreviatura de la unidad, para mostrar junto a la cantidad.</summary>
        public string abreviatura_unidad { get; set; }

        public string digito { get { return dvh; } }

        public string ObtenerCamposDV()
        {
            return string.Join("|",
                id_detalle_entrega.ToString(CultureInfo.InvariantCulture),
                id_entrega.ToString(CultureInfo.InvariantCulture),
                id_articulo.ToString(CultureInfo.InvariantCulture),
                cantidad_entregada.ToString("F2", CultureInfo.InvariantCulture),
                id_unidad_medida.ToString(CultureInfo.InvariantCulture));
        }

        public override string ToString()
        {
            return cantidad_entregada.ToString("F2", CultureInfo.InvariantCulture) + " " +
                   abreviatura_unidad + " de " + descripcion_articulo;
        }
    }
}
