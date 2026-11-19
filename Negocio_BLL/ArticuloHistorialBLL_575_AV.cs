using Acceso_DAL;
using Entidad_BE;
using Servicios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Negocio_BLL
{
    public class ArticuloHistorialBLL_575_AV
    {
        private const string TABLA = "ArticuloHistorial_575_AV";

        private MP_ArticuloHistorial_575_AV mpHistorial = new MP_ArticuloHistorial_575_AV();
        private VerificadorIntegridadBLL verificador = new VerificadorIntegridadBLL();

        public List<ArticuloHistorialBE_575_AV> BuscarHistorial(int? id_articulo, string operacion, DateTime desde, DateTime hasta)
        {
            ValidarRango(desde, hasta);

            if (!VerificarIntegridad())
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("HIS_ERR_INTEGRIDAD"));
            }

            return mpHistorial.BuscarHistorial(id_articulo, operacion, desde.Date, hasta.Date);
        }

        public List<ArticuloHistorialBE_575_AV> ObtenerCambio(int id_cambio)
        {
            return mpHistorial.ObtenerCambio(id_cambio);
        }

        public void PersistirDigitosPendientes()
        {
            List<ArticuloHistorialBE_575_AV> pendientes = mpHistorial.ListarSinDigito();
            if (pendientes.Count == 0) { return; }

            foreach (ArticuloHistorialBE_575_AV historial in pendientes)
            {
                historial.dvh = VerificadorIntegridad.CalcularDVH(historial);
                mpHistorial.ActualizarDVH(historial.id_historial, historial.dvh);
            }
            verificador.ActualizarDVV(TABLA);
        }

        public bool VerificarIntegridad()
        {
            return verificador.VerificarIntegridad(mpHistorial.ListarHistorial().OrderBy(h => h.id_historial).ToList(), TABLA);
        }

        public void RecalcularDV()
        {
            foreach (ArticuloHistorialBE_575_AV historial in mpHistorial.ListarHistorial().OrderBy(h => h.id_historial))
            {
                historial.dvh = VerificadorIntegridad.CalcularDVH(historial);
                mpHistorial.ActualizarDVH(historial.id_historial, historial.dvh);
            }
            verificador.ActualizarDVV(TABLA);
        }

        private void ValidarRango(DateTime desde, DateTime hasta)
        {
            if (desde.Date > hasta.Date)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("HIS_ERR_RANGO"));
            }
        }
    }
}
