namespace Entidad_BE
{
    public interface IExistencias_575_AV
    {
        ArticuloBE_575_AV ObtenerArticulo(int id_articulo);

        bool HaySaldoDeBobina(int id_articulo, decimal cantidad);

        void MoverCantidad(int id_articulo, decimal cantidad, string usuario);

        void CambiarEstado(int id_articulo, EstadoArticulo_575_AV estado, string usuario);
    }
}
