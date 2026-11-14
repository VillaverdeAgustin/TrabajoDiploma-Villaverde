using System.Globalization;

namespace Entidad_BE
{
    public class ArticuloBE_575_AV : IVerificable
    {
        public int id_articulo { get; set; }
        public string codigo { get; set; }
        public string descripcion { get; set; }
        public int id_categoria { get; set; }
        public int id_marca { get; set; }
        public int id_unidad_medida { get; set; }
        public decimal precio_unitario { get; set; }
        public decimal cantidad_disponible { get; set; }
        public decimal punto_reposicion { get; set; }
        public string deposito_ubicacion { get; set; }
        public EstadoArticulo_575_AV estado_articulo { get; set; }
        public bool activo { get; set; }
        public string dvh { get; set; }

        public string nombre_categoria { get; set; }

        public string nombre_marca { get; set; }

        public string abreviatura_unidad { get; set; }

        public bool fraccionable { get; set; }

        public bool bajo_punto_reposicion
        {
            get { return cantidad_disponible <= punto_reposicion; }
        }

        public bool se_puede_vender
        {
            get
            {
                return activo
                    && estado_articulo == EstadoArticulo_575_AV.Disponible
                    && cantidad_disponible > 0;
            }
        }

        public string digito { get { return dvh; } }

        public string ObtenerCamposDV()
        {
            return string.Join("|",
                id_articulo.ToString(CultureInfo.InvariantCulture),
                codigo,
                descripcion,
                id_categoria.ToString(CultureInfo.InvariantCulture),
                id_marca.ToString(CultureInfo.InvariantCulture),
                id_unidad_medida.ToString(CultureInfo.InvariantCulture),
                precio_unitario.ToString("F2", CultureInfo.InvariantCulture),
                cantidad_disponible.ToString("F2", CultureInfo.InvariantCulture),
                punto_reposicion.ToString("F2", CultureInfo.InvariantCulture),
                deposito_ubicacion,
                estado_articulo,
                activo);
        }

        public override string ToString()
        {
            return codigo + " - " + descripcion;
        }
    }
}
