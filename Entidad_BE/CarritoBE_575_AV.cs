using System;
using System.Collections.Generic;
using System.Globalization;

namespace Entidad_BE
{
    public class CarritoBE_575_AV : IVerificable
    {
        public int id_carrito { get; set; }
        public string nro_carrito { get; set; }
        public string usuario_vendedor { get; set; }
        public string nombre_cliente { get; set; }
        public DateTime fecha_apertura { get; set; }
        public EstadoCarrito_575_AV estado { get; set; }
        public decimal precio_total { get; set; }
        public bool activo { get; set; }
        public string dvh { get; set; }

        public List<DetalleCarritoBE_575_AV> detalle { get; set; }

        public CarritoBE_575_AV()
        {
            detalle = new List<DetalleCarritoBE_575_AV>();
        }

        public bool se_puede_editar
        {
            get { return activo && estado == EstadoCarrito_575_AV.Abierto; }
        }

        public string digito { get { return dvh; } }

        public string ObtenerCamposDV()
        {
            return string.Join("|",
                id_carrito.ToString(CultureInfo.InvariantCulture),
                nro_carrito,
                usuario_vendedor,
                nombre_cliente,
                fecha_apertura.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                estado,
                precio_total.ToString("F2", CultureInfo.InvariantCulture),
                activo);
        }

        public override string ToString()
        {
            return nro_carrito + " - " + (string.IsNullOrWhiteSpace(nombre_cliente)
                ? "sin cliente"
                : nombre_cliente);
        }
    }
}
