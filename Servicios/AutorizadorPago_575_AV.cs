using Entidad_BE;
using System;

namespace Servicios
{
    public class AutorizadorPago_575_AV
    {
        public ResultadoPago_575_AV Autorizar(MedioPagoBE_575_AV medio_pago,
                                              string datos_pago,
                                              decimal monto)
        {
            if (medio_pago == null)
            {
                return ResultadoPago_575_AV.Rechazado("PAGO_MOTIVO_SIN_MEDIO");
            }
            if (!medio_pago.activo)
            {
                return ResultadoPago_575_AV.Rechazado("PAGO_MOTIVO_MEDIO_INACTIVO");
            }
            if (monto <= 0)
            {
                return ResultadoPago_575_AV.Rechazado("PAGO_MOTIVO_MONTO");
            }

            if (!medio_pago.requiere_autorizacion)
            {
                return ResultadoPago_575_AV.Aprobado(string.Empty);
            }

            if (string.IsNullOrWhiteSpace(datos_pago))
            {
                return ResultadoPago_575_AV.Rechazado("PAGO_MOTIVO_SIN_DATOS");
            }

            return ResultadoPago_575_AV.Aprobado(GenerarCodigo());
        }

        private static string GenerarCodigo()
        {
            int azar = Math.Abs(Guid.NewGuid().GetHashCode()) % 1000000;
            return "AUT-" + DateTime.Now.ToString("yyyyMMdd") + "-" + azar.ToString("D6");
        }
    }
}
