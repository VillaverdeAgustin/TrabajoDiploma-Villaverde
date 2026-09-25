namespace Entidad_BE
{
    /// <summary>
    /// Resultado del despacho en deposito.
    /// Se persiste como texto en <c>Entrega.estado</c>.
    /// </summary>
    public enum EstadoEntrega_575_AV
    {
        /// <summary>Se entrego todo lo facturado.</summary>
        Total = 0,

        /// <summary>Se entrego menos de lo facturado.</summary>
        Parcial = 1
    }
}
