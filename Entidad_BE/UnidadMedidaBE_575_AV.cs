using System.Globalization;

namespace Entidad_BE
{
    /// <summary>
    /// Unidad en la que se comercializa un producto.
    /// <see cref="Fraccionable"/> es la que habilita el circuito de bobina:
    /// "Unidad" va en falso, "Metro lineal" en verdadero.
    /// </summary>
    public class UnidadMedidaBE_575_AV : IVerificable
    {
        public int IdUnidadMedida { get; set; }
        public string Nombre { get; set; }
        public string Abreviatura { get; set; }
        public bool Fraccionable { get; set; }
        public string DVH { get; set; }

        public string digito { get { return DVH; } }

        public string ObtenerCamposDV()
        {
            return string.Join("|",
                IdUnidadMedida.ToString(CultureInfo.InvariantCulture),
                Nombre,
                Abreviatura,
                Fraccionable);
        }

        public override string ToString()
        {
            return Nombre;
        }
    }
}
