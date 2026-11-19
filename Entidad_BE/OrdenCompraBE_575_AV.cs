using System;
using System.Collections.Generic;
using System.Globalization;

namespace Entidad_BE
{
    public class OrdenCompraBE_575_AV : IVerificable
    {
        public int id_orden_compra { get; set; }
        public string nro_orden_compra { get; set; }
        public int id_proveedor { get; set; }
        public DateTime? fecha_emision { get; set; }
        public string usuario_emisor { get; set; }
        public EstadoOrdenCompra_575_AV estado { get; set; }
        public decimal total { get; set; }
        public string observaciones { get; set; }
        public bool activo { get; set; }
        public string dvh { get; set; }

        public string razon_social { get; set; }

        public List<DetalleOrdenCompraBE_575_AV> detalle { get; set; }

        public int cantidad_renglones
        {
            get { return detalle.Count; }
        }

        public OrdenCompraBE_575_AV()
        {
            detalle = new List<DetalleOrdenCompraBE_575_AV>();
        }

        public string digito { get { return dvh; } }

        public string ObtenerCamposDV()
        {
            return string.Join("|",
                id_orden_compra.ToString(CultureInfo.InvariantCulture),
                nro_orden_compra,
                id_proveedor.ToString(CultureInfo.InvariantCulture),
                Fecha(fecha_emision),
                usuario_emisor,
                estado,
                total.ToString("F2", CultureInfo.InvariantCulture),
                observaciones,
                activo);
        }

        private static string Fecha(DateTime? fecha)
        {
            return fecha.HasValue
                ? fecha.Value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                : string.Empty;
        }
    }
}
