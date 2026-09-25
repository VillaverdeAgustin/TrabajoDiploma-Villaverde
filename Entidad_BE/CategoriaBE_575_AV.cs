using System.Globalization;

namespace Entidad_BE
{
    /// <summary>
    /// Categoria de articulo: cable de red, fibra optica, conectividad, etc.
    /// </summary>
    public class CategoriaBE_575_AV : IVerificable
    {
        public int id_categoria { get; set; }
        public string nombre { get; set; }
        public bool activo { get; set; }
        public string dvh { get; set; }

        public string digito { get { return dvh; } }

        public string ObtenerCamposDV()
        {
            return string.Join("|",
                id_categoria.ToString(CultureInfo.InvariantCulture),
                nombre,
                activo);
        }

        public override string ToString()
        {
            return nombre;
        }
    }
}
