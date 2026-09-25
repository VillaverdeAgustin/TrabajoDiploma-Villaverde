namespace Entidad_BE
{
    /// <summary>
    /// Respuesta del autorizador de pagos.
    ///
    /// <para>No hay integracion bancaria posible en el alcance de la entrega:
    /// la autorizacion se simula. Este tipo existe para que el dia que haya una
    /// entidad real de por medio no haya que cambiar la firma de nada.</para>
    /// </summary>
    public class ResultadoPago_575_AV
    {
        public bool aprobado { get; set; }

        /// <summary>Codigo que devuelve la entidad. Vacio si el pago se rechazo.</summary>
        public string codigo_autorizacion { get; set; }

        /// <summary>Por que se rechazo. Vacio si el pago se aprobo.</summary>
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
