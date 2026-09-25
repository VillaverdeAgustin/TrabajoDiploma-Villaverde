using System.Globalization;

namespace Entidad_BE
{
    /// <summary>
    /// Articulo que comercializa la distribuidora.
    ///
    /// <para><c>cantidad_disponible</c> es decimal y no entero: los articulos que
    /// se venden por unidad de medida se fraccionan a partir de una caja o un
    /// rollo, y su disponibilidad es la suma de los saldos de sus bobinas.</para>
    /// </summary>
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

        /// <summary>Nombre de la categoria, que resuelve la BLL para mostrar en pantalla.</summary>
        public string nombre_categoria { get; set; }

        /// <summary>Nombre de la marca, que resuelve la BLL para mostrar en pantalla.</summary>
        public string nombre_marca { get; set; }

        /// <summary>Abreviatura de la unidad, para mostrar junto a la cantidad.</summary>
        public string abreviatura_unidad { get; set; }

        /// <summary>Si la unidad de medida admite fraccionamiento por bobina.</summary>
        public bool fraccionable { get; set; }

        /// <summary>Las existencias cayeron al punto de reposicion o por debajo.</summary>
        public bool bajo_punto_reposicion
        {
            get { return cantidad_disponible <= punto_reposicion; }
        }

        /// <summary>Se puede vender: activo, disponible y con existencias.</summary>
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

        /// <summary>
        /// Solo los campos persistidos entran en el digito verificador: los
        /// derivados y los que resuelve la BLL para pantalla quedan afuera.
        /// Los decimales se formatean con cultura invariante para que el dvh no
        /// dependa de la configuracion regional de la maquina.
        /// </summary>
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
