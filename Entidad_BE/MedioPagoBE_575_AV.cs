using System.Globalization;

namespace Entidad_BE
{
    /// <summary>
    /// Medio de pago habilitado para el cobro de una venta (RFN1.5).
    /// </summary>
    public class MedioPagoBE_575_AV : IVerificable
    {
        public int IdMedioPago { get; set; }
        public string Nombre { get; set; }
        public bool RequiereAutorizacion { get; set; }
        public bool Activo { get; set; }
        public string DVH { get; set; }

        public string digito { get { return DVH; } }

        public string ObtenerCamposDV()
        {
            return string.Join("|",
                IdMedioPago.ToString(CultureInfo.InvariantCulture),
                Nombre,
                RequiereAutorizacion,
                Activo);
        }

        public override string ToString()
        {
            return Nombre;
        }
    }
}
