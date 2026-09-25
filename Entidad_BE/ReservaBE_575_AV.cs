using System;
using System.Globalization;

namespace Entidad_BE
{
    /// <summary>
    /// Reserva de un articulo faltante (RFN1.9).
    ///
    /// <para>Compromete la entrega de material que hoy no alcanza e informa el
    /// plazo estimado de reposicion. Al registrarse, el articulo queda
    /// bloqueado hasta que la compra se concrete.</para>
    ///
    /// <para><c>id_cliente</c> queda en nulo cuando el cliente todavia no esta
    /// registrado: el alta entra recien en la caja.</para>
    /// </summary>
    public class ReservaBE_575_AV : IVerificable
    {
        public int id_reserva { get; set; }
        public int id_articulo { get; set; }
        public int? id_cliente { get; set; }
        public string nombre_cliente { get; set; }
        public decimal cantidad { get; set; }
        public DateTime fecha_reserva { get; set; }

        /// <summary>Plazo estimado de reposicion, en dias.</summary>
        public int plazo_entrega { get; set; }

        public EstadoReserva_575_AV estado { get; set; }
        public string dvh { get; set; }

        /// <summary>Descripcion del articulo, que resuelve la BLL para pantalla.</summary>
        public string descripcion_articulo { get; set; }

        /// <summary>Fecha estimada de entrega, a partir del plazo informado.</summary>
        public DateTime fecha_estimada_entrega
        {
            get { return fecha_reserva.AddDays(plazo_entrega); }
        }

        public string digito { get { return dvh; } }

        public string ObtenerCamposDV()
        {
            return string.Join("|",
                id_reserva.ToString(CultureInfo.InvariantCulture),
                id_articulo.ToString(CultureInfo.InvariantCulture),
                id_cliente.HasValue
                    ? id_cliente.Value.ToString(CultureInfo.InvariantCulture)
                    : string.Empty,
                nombre_cliente,
                cantidad.ToString("F2", CultureInfo.InvariantCulture),
                fecha_reserva.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                plazo_entrega.ToString(CultureInfo.InvariantCulture),
                estado);
        }

        public override string ToString()
        {
            return "Reserva " + id_reserva.ToString(CultureInfo.InvariantCulture) + " - " +
                   cantidad.ToString("F2", CultureInfo.InvariantCulture) + " de " +
                   descripcion_articulo;
        }
    }
}
