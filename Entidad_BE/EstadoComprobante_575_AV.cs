namespace Entidad_BE
{
    /// <summary>
    /// Estados del comprobante. Es lo que impide que uno se use dos veces para
    /// retirar mercaderia: pasa a Entregado cuando el deposito lo despacha.
    /// </summary>
    public enum EstadoComprobante_575_AV
    {
        Emitido = 0,
        Entregado = 1
    }
}
