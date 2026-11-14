using System;

namespace Entidad_BE
{
    public class ComparacionPrecio_575_AV
    {
        public int id_proveedor { get; set; }
        public string razon_social { get; set; }
        public int id_lista_precio { get; set; }
        public DateTime fecha_vigencia { get; set; }
        public string moneda { get; set; }
        public int id_item_lista { get; set; }
        public string codigo_proveedor { get; set; }
        public string descripcion { get; set; }
        public decimal precio_lista { get; set; }
        public string unidad_venta { get; set; }
        public int? plazo_entrega { get; set; }
        public bool vencida { get; set; }
    }
}
