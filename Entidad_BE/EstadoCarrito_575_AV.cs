namespace Entidad_BE
{
    /// <summary>
    /// Estados por los que pasa un carrito de compras.
    /// Se persiste como texto en <c>Carrito.estado</c>.
    /// </summary>
    public enum EstadoCarrito_575_AV
    {
        /// <summary>El vendedor lo esta armando: admite agregar y quitar renglones.</summary>
        Abierto = 0,

        /// <summary>Cerrado por el vendedor, a la espera de la caja.</summary>
        Confirmado = 1,

        /// <summary>Ya se emitio su comprobante.</summary>
        Facturado = 2,

        /// <summary>Se descarto sin llegar a facturarse.</summary>
        Anulado = 3
    }
}
