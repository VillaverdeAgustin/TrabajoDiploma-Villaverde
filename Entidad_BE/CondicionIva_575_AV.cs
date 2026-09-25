namespace Entidad_BE
{
    /// <summary>
    /// Condicion del cliente frente al IVA. Define que comprobante le
    /// corresponde y como se discrimina el impuesto.
    ///
    /// <para>Se persiste como texto en <c>Cliente.condicion_iva</c>, sin
    /// espacios ni acentos, para que el mapeo con la base sea directo. El texto
    /// que ve el usuario sale de la tabla Traduccion (T05).</para>
    /// </summary>
    public enum CondicionIva_575_AV
    {
        ConsumidorFinal = 0,
        ResponsableInscripto = 1,
        Monotributista = 2,
        Exento = 3
    }
}
