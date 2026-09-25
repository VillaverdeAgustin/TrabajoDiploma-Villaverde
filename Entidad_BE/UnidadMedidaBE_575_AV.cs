using System.Globalization;

namespace Entidad_BE
{
    /// <summary>
    /// Unidad en la que se comercializa un articulo.
    /// <para><c>fraccionable</c> habilita el circuito de bobina: "Unidad" va en
    /// falso, "Metro lineal" en verdadero.</para>
    /// </summary>
    public class UnidadMedidaBE_575_AV : IVerificable
    {
        public int id_unidad_medida { get; set; }
        public string nombre { get; set; }
        public string abreviatura { get; set; }
        public bool fraccionable { get; set; }
        public string dvh { get; set; }

        public string digito { get { return dvh; } }

        public string ObtenerCamposDV()
        {
            return string.Join("|",
                id_unidad_medida.ToString(CultureInfo.InvariantCulture),
                nombre,
                abreviatura,
                fraccionable);
        }

        public override string ToString()
        {
            return nombre;
        }
    }
}
