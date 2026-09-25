using System;
using System.Globalization;

namespace Entidad_BE
{
    /// <summary>
    /// Orden de pago (CUN-002): el documento que emite el vendedor a partir del
    /// carrito confirmado y con el que el cliente se presenta en la caja.
    ///
    /// <para>No es un comprobante: no descuenta existencias ni tiene valor
    /// fiscal. Copia el nombre del cliente y el total del carrito, para
    /// conservar lo que se emitio aunque despues cambie el precio del
    /// articulo.</para>
    /// </summary>
    public class OrdenPagoBE_575_AV : IVerificable
    {
        public int id_orden_pago { get; set; }
        public string nro_orden_pago { get; set; }
        public int id_carrito { get; set; }
        public string nombre_cliente { get; set; }
        public DateTime fecha_emision { get; set; }
        public decimal precio_total { get; set; }
        public EstadoOrdenPago_575_AV estado { get; set; }
        public bool activo { get; set; }
        public string dvh { get; set; }

        /// <summary>Numero del carrito de origen, que resuelve la BLL para pantalla.</summary>
        public string nro_carrito { get; set; }

        /// <summary>Se puede cobrar: activa y todavia pendiente.</summary>
        public bool se_puede_cobrar
        {
            get { return activo && estado == EstadoOrdenPago_575_AV.Pendiente; }
        }

        public string digito { get { return dvh; } }

        public string ObtenerCamposDV()
        {
            return string.Join("|",
                id_orden_pago.ToString(CultureInfo.InvariantCulture),
                nro_orden_pago,
                id_carrito.ToString(CultureInfo.InvariantCulture),
                nombre_cliente,
                fecha_emision.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                precio_total.ToString("F2", CultureInfo.InvariantCulture),
                estado,
                activo);
        }

        public override string ToString()
        {
            return nro_orden_pago + " - " +
                   precio_total.ToString("F2", CultureInfo.InvariantCulture);
        }
    }
}
