namespace Entidad_BE
{
    /// <summary>
    /// Operaciones de persistencia que necesita el gestor de existencias.
    ///
    /// <para>Existe para que <c>GestorDeExistencias_575_AV</c>, que vive en
    /// Servicios, pueda llegar a los datos sin que Servicios dependa de la DAL:
    /// la implementa la BLL y se la entrega al gestor al iniciar. Asi se
    /// mantiene el orden de capas GUI -> BLL -> DAL.</para>
    /// </summary>
    public interface IExistencias_575_AV
    {
        ArticuloBE_575_AV ObtenerArticulo(int id_articulo);

        /// <summary>Si alguna bobina del articulo puede cubrir la cantidad por si sola.</summary>
        bool HaySaldoDeBobina(int id_articulo, decimal cantidad);

        /// <summary>Aplica un movimiento con signo sobre las existencias.</summary>
        void MoverCantidad(int id_articulo, decimal cantidad, string usuario);

        void CambiarEstado(int id_articulo, EstadoArticulo_575_AV estado, string usuario);
    }
}
