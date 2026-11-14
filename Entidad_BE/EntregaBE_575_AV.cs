using System;
using System.Collections.Generic;
using System.Globalization;

namespace Entidad_BE
{
    public class EntregaBE_575_AV : IVerificable
    {
        public int id_entrega { get; set; }
        public int id_comprobante { get; set; }
        public string usuario_deposito { get; set; }
        public DateTime fecha_entrega { get; set; }
        public EstadoEntrega_575_AV estado { get; set; }
        public string dvh { get; set; }

        public List<DetalleEntregaBE_575_AV> detalle { get; set; }

        public string nro_comprobante { get; set; }

        public EntregaBE_575_AV()
        {
            detalle = new List<DetalleEntregaBE_575_AV>();
        }

        public string digito { get { return dvh; } }

        public string ObtenerCamposDV()
        {
            return string.Join("|",
                id_entrega.ToString(CultureInfo.InvariantCulture),
                id_comprobante.ToString(CultureInfo.InvariantCulture),
                usuario_deposito,
                fecha_entrega.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                estado);
        }

        public override string ToString()
        {
            return "Entrega " + id_entrega.ToString(CultureInfo.InvariantCulture) +
                   " (" + estado + ") del comprobante " + nro_comprobante;
        }
    }
}
