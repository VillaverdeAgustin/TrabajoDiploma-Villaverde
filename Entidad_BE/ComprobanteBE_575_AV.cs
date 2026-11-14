using System;
using System.Collections.Generic;
using System.Globalization;

namespace Entidad_BE
{
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

        public List<DetalleComprobanteBE_575_AV> detalle { get; set; }

        public string nombre_cliente { get; set; }

        public string nro_orden_pago { get; set; }

        public ClienteBE_575_AV cliente { get; set; }

        public List<PagoBE_575_AV> pagos { get; set; }

        public ComprobanteBE_575_AV()
        {
            detalle = new List<DetalleComprobanteBE_575_AV>();
            pagos = new List<PagoBE_575_AV>();
        }

        public bool es_consumidor_final
        {
            get { return cliente != null && cliente.condicion_iva == CondicionIva_575_AV.ConsumidorFinal; }
        }

        public decimal otros_impuestos_indirectos
        {
            get { return 0m; }
        }

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
