using System;
using System.Collections.Generic;
using System.Globalization;

namespace Entidad_BE
{
    /// <summary>
    /// Despacho de mercaderia en deposito (CUN-005, RFN1.9).
    ///
    /// <para>El cliente presenta el comprobante y el encargado entrega los
    /// articulos. No mueve existencias: el stock ya bajo al emitirse el
    /// comprobante.</para>
    ///
    /// <para>Un comprobante se despacha una sola vez. Si se entrega menos de lo
    /// facturado, la entrega queda marcada como Parcial.</para>
    /// </summary>
    public class EntregaBE_575_AV : IVerificable
    {
        public int id_entrega { get; set; }
        public int id_comprobante { get; set; }
        public string usuario_deposito { get; set; }
        public DateTime fecha_entrega { get; set; }
        public EstadoEntrega_575_AV estado { get; set; }
        public string dvh { get; set; }

        /// <summary>Renglones del despacho. Cada uno lleva su propio dvh.</summary>
        public List<DetalleEntregaBE_575_AV> detalle { get; set; }

        /// <summary>Numero del comprobante, que resuelve la BLL para pantalla.</summary>
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
