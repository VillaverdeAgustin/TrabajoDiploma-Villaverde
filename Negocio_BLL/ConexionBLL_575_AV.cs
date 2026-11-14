using Acceso_DAL;
using Entidad_BE;
using System;
using System.Collections.Generic;
using System.IO;

namespace Negocio_BLL
{
    public class ConexionBLL_575_AV
    {
        private InstaladorBaseDatos_575_AV instalador = new InstaladorBaseDatos_575_AV();

        public List<string> ListarInstancias()
        {
            return ConfiguracionConexion_575_AV.ListarInstanciasLocales();
        }

        public string ObtenerInstanciaActual()
        {
            return ConfiguracionConexion_575_AV.ObtenerInstancia();
        }

        public EstadoConexion_575_AV ProbarConexion(string instancia)
        {
            if (string.IsNullOrWhiteSpace(instancia))
            {
                throw new InvalidOperationException("CONEX_ERR_INSTANCIA");
            }

            if (!instalador.ProbarServidor(instancia.Trim()))
            {
                return EstadoConexion_575_AV.ServidorNoDisponible;
            }

            return instalador.ExisteBase(instancia.Trim())
                ? EstadoConexion_575_AV.Lista
                : EstadoConexion_575_AV.BaseInexistente;
        }

        public bool VerificarConexionActual()
        {
            if (!ConfiguracionConexion_575_AV.HayInstanciaConfigurada()) { return false; }

            try
            {
                return ProbarConexion(ObtenerInstanciaActual()) == EstadoConexion_575_AV.Lista;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void CrearBaseDatos(string instancia, IProgress<AvanceInstalacion_575_AV> avance)
        {
            EstadoConexion_575_AV estado = ProbarConexion(instancia);
            if (estado == EstadoConexion_575_AV.ServidorNoDisponible)
            {
                throw new InvalidOperationException("CONEX_MSG_SIN_SERVIDOR");
            }
            if (estado == EstadoConexion_575_AV.Lista)
            {
                throw new InvalidOperationException("CONEX_ERR_BASE_EXISTE");
            }

            try
            {
                instalador.EjecutarScripts(instancia.Trim(), avance);
            }
            catch (FileNotFoundException ex)
            {
                throw new InvalidOperationException("CONEX_ERR_SIN_SCRIPTS", ex);
            }
            catch (Exception ex)
            {
                try { instalador.BorrarBase(instancia.Trim()); }
                catch (Exception) { }
                throw new InvalidOperationException("CONEX_ERR_CREAR", ex);
            }
        }

        public void GuardarInstancia(string instancia)
        {
            if (ProbarConexion(instancia) != EstadoConexion_575_AV.Lista)
            {
                throw new InvalidOperationException("CONEX_ERR_NO_LISTA");
            }

            try
            {
                ConfiguracionConexion_575_AV.GuardarInstancia(instancia.Trim());
            }
            catch (IOException ex)
            {
                throw new InvalidOperationException("CONEX_ERR_GUARDAR", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new InvalidOperationException("CONEX_ERR_GUARDAR", ex);
            }
        }
    }
}
