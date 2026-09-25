namespace Entidad_BE
{
    /// <summary>
    /// Estado de un articulo frente a la venta.
    ///
    /// <para>Se persiste como texto en <c>Articulo.estado_articulo</c>. El valor
    /// <see cref="SinStock"/> va sin espacio para que el mapeo con la base sea
    /// directo; el texto que ve el usuario sale de la tabla Traduccion (T05).</para>
    /// </summary>
    public enum EstadoArticulo_575_AV
    {
        /// <summary>Con existencias y sin reservas que lo comprometan.</summary>
        Disponible = 0,

        /// <summary>
        /// Comprometido por la reserva de un faltante. Queda asi hasta que la
        /// compra se concrete.
        /// </summary>
        Bloqueado = 1,

        /// <summary>Sin existencias.</summary>
        SinStock = 2
    }
}
