using System.Globalization;

namespace Entidad_BE
{
    public class MedioPagoBE_575_AV : IVerificable
    {
        public int id_medio_pago { get; set; }
        public string nombre { get; set; }
        public bool requiere_autorizacion { get; set; }
        public bool activo { get; set; }
        public string dvh { get; set; }

        public string digito { get { return dvh; } }

        public string ObtenerCamposDV()
        {
            return string.Join("|",
                id_medio_pago.ToString(CultureInfo.InvariantCulture),
                nombre,
                requiere_autorizacion,
                activo);
        }

        public override string ToString()
        {
            return nombre;
        }
    }
}
