using System.Globalization;

namespace Entidad_BE
{
    /// <summary>
    /// Articulo que comercializa la distribuidora.
    ///
    /// <para><see cref="StockActual"/> es decimal y no entero: los articulos que
    /// se venden por unidad de medida se fraccionan a partir de una caja o un
    /// rollo, y el stock es la suma de los saldos de sus bobinas.</para>
    /// </summary>
    public class ProductoBE_575_AV : IVerificable
    {
        public int IdProducto { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public int IdCategoria { get; set; }
        public int IdMarca { get; set; }
        public int IdUnidadMedida { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal StockActual { get; set; }
        public decimal PuntoReposicion { get; set; }
        public bool Activo { get; set; }
        public string DVH { get; set; }

        /// <summary>Nombre de la categoria, resuelto por la BLL para mostrar en pantalla.</summary>
        public string NombreCategoria { get; set; }

        /// <summary>Nombre de la marca, resuelto por la BLL para mostrar en pantalla.</summary>
        public string NombreMarca { get; set; }

        /// <summary>Abreviatura de la unidad de medida, para mostrar junto a la cantidad.</summary>
        public string AbreviaturaUnidad { get; set; }

        /// <summary>Si la unidad de medida admite fraccionamiento por bobina.</summary>
        public bool Fraccionable { get; set; }

        /// <summary>El stock cayo al punto de reposicion o por debajo.</summary>
        public bool BajoPuntoReposicion
        {
            get { return StockActual <= PuntoReposicion; }
        }

        public string digito { get { return DVH; } }

        /// <summary>
        /// Solo los campos persistidos entran en el digito verificador: los
        /// derivados y los que resuelve la BLL para pantalla quedan afuera.
        /// Los decimales se formatean con cultura invariante para que el DVH
        /// no dependa de la configuracion regional de la maquina.
        /// </summary>
        public string ObtenerCamposDV()
        {
            return string.Join("|",
                IdProducto.ToString(CultureInfo.InvariantCulture),
                Codigo,
                Descripcion,
                IdCategoria.ToString(CultureInfo.InvariantCulture),
                IdMarca.ToString(CultureInfo.InvariantCulture),
                IdUnidadMedida.ToString(CultureInfo.InvariantCulture),
                PrecioUnitario.ToString("F2", CultureInfo.InvariantCulture),
                StockActual.ToString("F2", CultureInfo.InvariantCulture),
                PuntoReposicion.ToString("F2", CultureInfo.InvariantCulture),
                Activo);
        }

        public override string ToString()
        {
            return Codigo + " - " + Descripcion;
        }
    }
}
