namespace Entidad_BE
{
    /// <summary>
    /// Estados por los que pasa una orden de pago.
    /// Se persiste como texto en <c>OrdenPago.estado</c>.
    /// </summary>
    public enum EstadoOrdenPago_575_AV
    {
        /// <summary>Emitida por el vendedor y a la espera de cobro en la caja.</summary>
        Pendiente = 0,

        /// <summary>Cobrada: ya tiene su comprobante.</summary>
        Pagada = 1,

        /// <summary>Se descarto sin llegar a cobrarse.</summary>
        Anulada = 2
    }
}
