using Entidad_BE;
using System;

namespace Servicios
{
    /// <summary>
    /// Autorizacion del medio de pago (RFN1.5).
    ///
    /// <para><b>Simula</b> la respuesta de la entidad: en el alcance de la
    /// entrega no hay integracion bancaria posible. Los medios que no requieren
    /// autorizacion se acreditan directo; los que si la requieren reciben un
    /// codigo generado aca.</para>
    ///
    /// <para>Esta en Servicios y no en la BLL porque el dia que haya una entidad
    /// real de por medio, lo que cambia es la implementacion de este servicio y
    /// no la logica de la venta.</para>
    /// </summary>
    public class AutorizadorPago_575_AV
    {
        /// <summary>
        /// Autoriza el cobro. Rechaza si el medio esta dado de baja, si el monto
        /// no es positivo, o si un medio que exige autorizacion llega sin los
        /// datos del pago.
        /// </summary>
        public ResultadoPago_575_AV Autorizar(MedioPagoBE_575_AV medio_pago,
                                              string datos_pago,
                                              decimal monto)
        {
            if (medio_pago == null)
            {
                return ResultadoPago_575_AV.Rechazado("No se indico el medio de pago.");
            }
            if (!medio_pago.activo)
            {
                return ResultadoPago_575_AV.Rechazado(
                    "El medio de pago no esta habilitado.");
            }
            if (monto <= 0)
            {
                return ResultadoPago_575_AV.Rechazado("El monto a cobrar no es valido.");
            }

            if (!medio_pago.requiere_autorizacion)
            {
                return ResultadoPago_575_AV.Aprobado(string.Empty);
            }

            if (string.IsNullOrWhiteSpace(datos_pago))
            {
                return ResultadoPago_575_AV.Rechazado(
                    "Faltan los datos del pago para autorizar la operacion.");
            }

            return ResultadoPago_575_AV.Aprobado(GenerarCodigo());
        }

        /// <summary>
        /// Codigo de autorizacion simulado, con el formato AUT-aaaaMMdd-NNNNNN.
        /// </summary>
        private static string GenerarCodigo()
        {
            int azar = Math.Abs(Guid.NewGuid().GetHashCode()) % 1000000;
            return "AUT-" + DateTime.Now.ToString("yyyyMMdd") + "-" + azar.ToString("D6");
        }
    }
}
