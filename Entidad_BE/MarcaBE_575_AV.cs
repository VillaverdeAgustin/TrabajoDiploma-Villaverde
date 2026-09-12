using System.Globalization;

namespace Entidad_BE
{
    /// <summary>
    /// Marca comercial del producto.
    /// </summary>
    public class MarcaBE_575_AV : IVerificable
    {
        public int IdMarca { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
        public string DVH { get; set; }

        public string digito { get { return DVH; } }

        public string ObtenerCamposDV()
        {
            return string.Join("|",
                IdMarca.ToString(CultureInfo.InvariantCulture),
                Nombre,
                Activo);
        }

        public override string ToString()
        {
            return Nombre;
        }
    }
}
