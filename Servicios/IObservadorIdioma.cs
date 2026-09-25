namespace Servicios
{
    /// <summary>
    /// Observador del patron Observer de idiomas. Todo formulario o control
    /// que muestre textos traducibles implementa esta interfaz y se suscribe
    /// al GestorDeIdioma para ser notificado cuando cambia el idioma activo.
    /// </summary>
    public interface IObservadorIdioma
    {
        void ActualizarTextos();
    }
}
