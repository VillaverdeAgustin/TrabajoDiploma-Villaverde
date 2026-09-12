namespace Entidad_BE
{
    /// <summary>
    /// Estados por los que pasa una bobina o caja.
    /// Los nombres coinciden con la restriccion CK_Bobina_Estado de la base:
    /// se persisten como texto, no como numero.
    /// </summary>
    public enum EstadoBobina_575_AV
    {
        /// <summary>Sin abrir: el saldo es igual a la medida inicial.</summary>
        Cerrada = 0,

        /// <summary>Abierta y con saldo remanente disponible.</summary>
        Abierta = 1,

        /// <summary>Sin saldo: ya no participa del fraccionamiento.</summary>
        Agotada = 2
    }
}
