namespace Entidad_BE
{
    /// <summary>
    /// Estados por los que pasa la reserva de un faltante.
    /// Se persiste como texto en <c>Reserva.estado</c>.
    /// </summary>
    public enum EstadoReserva_575_AV
    {
        /// <summary>Registrada: el articulo quedo bloqueado y espera reposicion.</summary>
        Pendiente = 0,

        /// <summary>Ya hay una orden de compra en curso que la cubre (RFN2).</summary>
        EnCompra = 1,

        /// <summary>La mercaderia llego y se entrego al cliente.</summary>
        Cumplida = 2,

        /// <summary>El cliente desistio o se dio de baja la reserva.</summary>
        Anulada = 3
    }
}
