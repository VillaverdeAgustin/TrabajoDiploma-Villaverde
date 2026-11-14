using System;
using System.Globalization;

namespace Entidad_BE
{
    public class RecepcionBE_575_AV : IVerificable
    {
        public int id_recepcion { get; set; }
        public int id_orden_compra { get; set; }
        public string remito_proveedor { get; set; }
        public DateTime fecha_recepcion { get; set; }
        public string usuario_deposito { get; set; }
        public string diferencias { get; set; }
        public string dvh { get; set; }

        public string digito { get { return dvh; } }

        public string ObtenerCamposDV()
        {
            return string.Join("|",
                id_recepcion.ToString(CultureInfo.InvariantCulture),
                id_orden_compra.ToString(CultureInfo.InvariantCulture),
                remito_proveedor,
                fecha_recepcion.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                usuario_deposito,
                diferencias);
        }
    }
}
