using System.Globalization;

namespace Entidad_BE
{
    /// <summary>
    /// Categoria de producto: cable de red, fibra optica, conectividad, etc.
    /// </summary>
    public class CategoriaBE_575_AV : IVerificable
    {
        public int IdCategoria { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
        public string DVH { get; set; }

        public string digito { get { return DVH; } }

        public string ObtenerCamposDV()
        {
            return string.Join("|",
                IdCategoria.ToString(CultureInfo.InvariantCulture),
                Nombre,
                Activo);
        }

        public override string ToString()
        {
            return Nombre;
        }
    }
}
