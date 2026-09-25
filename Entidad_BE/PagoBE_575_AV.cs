using System;
using System.Globalization;

namespace Entidad_BE
{
    /// <summary>
    /// Cobro registrado sobre un comprobante (RFN1.5).
    ///
    /// <para>El codigo de autorizacion lo devuelve el autorizador de pagos, que
    /// en el alcance de la entrega simula la respuesta de la entidad.</para>
    /// </summary>
    public class PagoBE_575_AV : IVerificable
    {
        public int id_pago { get; set; }
        public int id_comprobante { get; set; }
        public int id_medio_pago { get; set; }
        public decimal monto { get; set; }
        public DateTime fecha_hora { get; set; }
        public string codigo_autorizacion { get; set; }
        public EstadoPago_575_AV estado { get; set; }
        public string dvh { get; set; }

        /// <summary>Nombre del medio de pago, que resuelve la BLL para pantalla.</summary>
        public string nombre_medio_pago { get; set; }

        public string digito { get { return dvh; } }

        public string ObtenerCamposDV()
        {
            return string.Join("|",
                id_pago.ToString(CultureInfo.InvariantCulture),
                id_comprobante.ToString(CultureInfo.InvariantCulture),
                id_medio_pago.ToString(CultureInfo.InvariantCulture),
                monto.ToString("F2", CultureInfo.InvariantCulture),
                fecha_hora.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                codigo_autorizacion,
                estado);
        }

        public override string ToString()
        {
            return nombre_medio_pago + " " +
                   monto.ToString("F2", CultureInfo.InvariantCulture) + " (" + estado + ")";
        }
    }
}
