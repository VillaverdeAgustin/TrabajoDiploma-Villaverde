using System;
using System.Collections.Generic;
using System.Globalization;

namespace Entidad_BE
{
    /// <summary>
    /// Comprobante de la venta (CUN-004, RFN1.7).
    ///
    /// <para>Se emite como <b>comprobante no valido como factura</b>: no hay
    /// integracion con ARCA, ni CAE, ni servicios web del organismo.</para>
    ///
    /// <para><c>tipo_comprobante</c> sale de la condicion del cliente frente al
    /// IVA: "A" para responsable inscripto, con el impuesto discriminado, y "B"
    /// para el resto.</para>
    /// </summary>
    public class ComprobanteBE_575_AV : IVerificable
    {
        public int id_comprobante { get; set; }
        public string nro_comprobante { get; set; }
        public string tipo_comprobante { get; set; }
        public int id_orden_pago { get; set; }
        public int id_cliente { get; set; }
        public DateTime fecha_emision { get; set; }
        public decimal neto { get; set; }
        public decimal iva { get; set; }
        public decimal precio_total { get; set; }
        public EstadoComprobante_575_AV estado { get; set; }
        public string dvh { get; set; }

        /// <summary>Renglones del comprobante. Cada uno lleva su propio dvh.</summary>
        public List<DetalleComprobanteBE_575_AV> detalle { get; set; }

        /// <summary>Nombre del cliente, que resuelve la BLL para pantalla.</summary>
        public string nombre_cliente { get; set; }

        public ComprobanteBE_575_AV()
        {
            detalle = new List<DetalleComprobanteBE_575_AV>();
        }

        /// <summary>Todavia no se retiro la mercaderia.</summary>
        public bool se_puede_entregar
        {
            get { return estado == EstadoComprobante_575_AV.Emitido; }
        }

        public string digito { get { return dvh; } }

        public string ObtenerCamposDV()
        {
            return string.Join("|",
                id_comprobante.ToString(CultureInfo.InvariantCulture),
                nro_comprobante,
                tipo_comprobante,
                id_orden_pago.ToString(CultureInfo.InvariantCulture),
                id_cliente.ToString(CultureInfo.InvariantCulture),
                fecha_emision.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                neto.ToString("F2", CultureInfo.InvariantCulture),
                iva.ToString("F2", CultureInfo.InvariantCulture),
                precio_total.ToString("F2", CultureInfo.InvariantCulture),
                estado);
        }

        public override string ToString()
        {
            return nro_comprobante + " - " +
                   precio_total.ToString("F2", CultureInfo.InvariantCulture);
        }
    }
}
