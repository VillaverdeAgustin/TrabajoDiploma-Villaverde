using Acceso_DAL;
using Entidad_BE;
using Servicios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Negocio_BLL
{
    // T07 - Backup y Restore de ConectAR_DB. La pantalla solo elige la carpeta o el archivo y confirma;
    // aca se arma el nombre, se valida el archivo, se verifica y, despues de un restore, se recalculan los DV,
    // se registra el evento y se cierra la sesion.
    public class BackupBLL_575_AV
    {
        public const string PREFIJO = "ConectAR_DB_";
        public const string EXTENSION = ".bak";

        private MP_Backup_575_AV mpBackup = new MP_Backup_575_AV();
        private BitacoraBLL bitacora = new BitacoraBLL();

        public string CarpetaPredeterminada()
        {
            return mpBackup.ObtenerCarpetaPredeterminada();
        }

        // ConectAR_DB_aaaammdd_hhmmss.bak
        public static string NombreArchivo(DateTime momento)
        {
            return PREFIJO + momento.ToString("yyyyMMdd_HHmmss") + EXTENSION;
        }

        #region Backup

        public BackupBE_575_AV GenerarBackup(string carpeta, string usuario)
        {
            if (string.IsNullOrWhiteSpace(carpeta))
            {
                carpeta = CarpetaPredeterminada();
            }
            carpeta = carpeta.Trim();
            if (carpeta.IndexOfAny(Path.GetInvalidPathChars()) >= 0 || carpeta.IndexOfAny(new[] { '*', '?', '"', '<', '>', '|' }) >= 0
                || !Path.IsPathRooted(carpeta))
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("BKP_ERR_RUTA"));
            }

            string ruta = Path.Combine(carpeta, NombreArchivo(DateTime.Now));
            try
            {
                mpBackup.GenerarBackup(ruta);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("BKP_ERR_GENERAR"), ex);
            }

            BackupBE_575_AV backup = mpBackup.LeerCabecera(ruta) ?? new BackupBE_575_AV { ruta = ruta };
            backup.verificado = Verificar(ruta);

            bitacora.RegistrarBitacora(usuario, TipoAccion.BackupGenerado);
            return backup;
        }

        private bool Verificar(string ruta)
        {
            try
            {
                mpBackup.VerificarBackup(ruta);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region Restore

        public List<BackupBE_575_AV> ListarBackups()
        {
            return mpBackup.ListarBackups();
        }

        // Lee la cabecera del archivo: tiene que ser un backup de SQL Server y de ConectAR_DB
        public BackupBE_575_AV ValidarArchivo(string ruta)
        {
            if (string.IsNullOrWhiteSpace(ruta))
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("RST_ERR_ELEGIR"));
            }

            BackupBE_575_AV cabecera;
            try
            {
                cabecera = mpBackup.LeerCabecera(ruta.Trim());
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("RST_ERR_ARCHIVO"), ex);
            }

            if (cabecera == null)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("RST_ERR_ARCHIVO"));
            }
            if (!string.Equals(cabecera.base_datos, ConfiguracionConexion_575_AV.BASE_DATOS, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(string.Format(
                    GestorDeIdioma.GetInstance.Traducir("RST_ERR_OTRA_BASE"), cabecera.base_datos));
            }
            return cabecera;
        }

        public ResultadoRestore_575_AV Restaurar(string ruta, string usuario)
        {
            BackupBE_575_AV backup = ValidarArchivo(ruta);
            backup.verificado = Verificar(backup.ruta);
            if (!backup.verificado)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("RST_ERR_VERIFICACION"));
            }

            try
            {
                mpBackup.Restaurar(backup.ruta);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("RST_ERR_RESTAURAR"), ex);
            }

            // La base restaurada trae los DV del momento del backup: se recalculan todos (T07 dispara el recalculo)
            VerificadorIntegridadBLL integridad = new VerificadorIntegridadBLL();
            integridad.RecalcularDVTotal();
            bitacora.RegistrarBitacora(usuario, TipoAccion.RestoreRealizado);

            ResultadoRestore_575_AV resultado = new ResultadoRestore_575_AV
            {
                backup = backup,
                tablas_con_error = integridad.VerificarIntegridadTotal()
            };

            // Los datos de la sesion pueden no existir en la base restaurada: se vuelve a entrar
            new UsuarioBLL().Logout();
            return resultado;
        }

        #endregion
    }
}
