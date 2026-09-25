using System.Globalization;

namespace Entidad_BE
{
    /// <summary>
    /// Marca comercial del articulo.
    /// </summary>
    public class MarcaBE_575_AV : IVerificable
    {
        public int id_marca { get; set; }
        public string nombre { get; set; }
        public bool activo { get; set; }
        public string dvh { get; set; }

        public string digito { get { return dvh; } }

        public string ObtenerCamposDV()
        {
            return string.Join("|",
                id_marca.ToString(CultureInfo.InvariantCulture),
                nombre,
                activo);
        }

        public override string ToString()
        {
            return nombre;
        }
    }
}
