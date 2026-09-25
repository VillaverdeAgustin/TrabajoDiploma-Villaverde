using System;
using System.Collections.Generic;
using System.Globalization;

namespace Entidad_BE
{
    /// <summary>
    /// Carrito de compras que arma el vendedor en el salon (CUN-001).
    ///
    /// <para>Guarda <c>nombre_cliente</c> como texto y no un id: el carrito se
    /// arma antes de que el cajero identifique al cliente por DNI.</para>
    /// </summary>
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

        /// <summary>
        /// Renglones del carrito. No entran al digito del carrito: cada uno
        /// tiene el suyo y DetalleCarrito lleva su propio DVV.
        /// </summary>
        public List<DetalleCarritoBE_575_AV> detalle { get; set; }

        public CarritoBE_575_AV()
        {
            detalle = new List<DetalleCarritoBE_575_AV>();
        }

        /// <summary>Solo un carrito abierto admite agregar o quitar renglones.</summary>
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
