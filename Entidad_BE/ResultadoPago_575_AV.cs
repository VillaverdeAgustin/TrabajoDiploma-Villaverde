namespace Entidad_BE
{
    public class ResultadoPago_575_AV
    {
        public bool aprobado { get; set; }

        public string codigo_autorizacion { get; set; }

        public string motivo { get; set; }

        public static ResultadoPago_575_AV Aprobado(string codigo)
        {
            return new ResultadoPago_575_AV { aprobado = true, codigo_autorizacion = codigo };
        }

        public static ResultadoPago_575_AV Rechazado(string motivo)
        {
            return new ResultadoPago_575_AV { aprobado = false, motivo = motivo };
        }
    }
}
