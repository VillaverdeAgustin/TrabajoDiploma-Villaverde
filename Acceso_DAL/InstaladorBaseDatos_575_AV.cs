using Entidad_BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Acceso_DAL
{
    public class InstaladorBaseDatos_575_AV
    {
        private const string BASE_SISTEMA = "master";
        private const string CARPETA_SCRIPTS = "db";
        private const int SEGUNDOS_PRUEBA = 5;
        private const int SEGUNDOS_SCRIPT = 600;

        private static readonly Regex nombreScript = new Regex(@"^\d{2}_.+\.sql$", RegexOptions.IgnoreCase);
        private static readonly Regex separador = new Regex(@"^\s*GO\s*$", RegexOptions.IgnoreCase);
        private static readonly Regex scriptDePrueba = new Regex(@"_prueba", RegexOptions.IgnoreCase);
        private const string PREFIJO_RESPALDO = "ConectAR_DB_antes_de_actualizar_";

        private readonly string carpetaScripts;

        public InstaladorBaseDatos_575_AV()
            : this(Path.Combine(Path.GetDirectoryName(typeof(InstaladorBaseDatos_575_AV).Assembly.Location), CARPETA_SCRIPTS)) { }

        public InstaladorBaseDatos_575_AV(string carpetaScripts)
        {
            this.carpetaScripts = carpetaScripts;
        }

        public bool ProbarServidor(string instancia)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(
                    ConfiguracionConexion_575_AV.ArmarCadena(instancia, BASE_SISTEMA, SEGUNDOS_PRUEBA)))
                {
                    conexion.Open();
                    return true;
                }
            }
            catch (SqlException) { return false; }
            catch (InvalidOperationException) { return false; }
            catch (ArgumentException) { return false; }
        }

        public bool ExisteBase(string instancia)
        {
            using (SqlConnection conexion = new SqlConnection(
                ConfiguracionConexion_575_AV.ArmarCadena(instancia, BASE_SISTEMA, SEGUNDOS_PRUEBA)))
            using (SqlCommand comando = new SqlCommand("SELECT DB_ID(@nombre)", conexion))
            {
                comando.Parameters.AddWithValue("@nombre", ConfiguracionConexion_575_AV.BASE_DATOS);
                conexion.Open();
                object id = comando.ExecuteScalar();
                return id != null && id != DBNull.Value;
            }
        }

        public List<string> ListarScripts()
        {
            if (!Directory.Exists(carpetaScripts)) { return new List<string>(); }

            return Directory.GetFiles(carpetaScripts, "*.sql")
                .Where(f => nombreScript.IsMatch(Path.GetFileName(f)))
                .OrderBy(f => Path.GetFileName(f), StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        // Actualizacion de una base existente: los mismos scripts sin los datos de prueba
        public List<string> ListarScriptsDeActualizacion()
        {
            return ListarScripts().Where(f => !scriptDePrueba.IsMatch(Path.GetFileName(f))).ToList();
        }

        public void EjecutarScripts(string instancia, IProgress<AvanceInstalacion_575_AV> avance)
        {
            Ejecutar(ListarScripts(), instancia, avance);
        }

        public void ActualizarBase(string instancia, IProgress<AvanceInstalacion_575_AV> avance)
        {
            Ejecutar(ListarScriptsDeActualizacion(), instancia, avance);
        }

        // Backup completo con los SP del T07, en la carpeta Backup de la instancia
        public string RespaldarBase(string instancia)
        {
            using (SqlConnection conexion = new SqlConnection(
                ConfiguracionConexion_575_AV.ArmarCadena(instancia, ConfiguracionConexion_575_AV.BASE_DATOS, null)))
            {
                conexion.Open();
                string carpeta;
                using (SqlCommand ruta = new SqlCommand("SP_RutaBackups", conexion) { CommandType = CommandType.StoredProcedure })
                {
                    carpeta = Convert.ToString(ruta.ExecuteScalar());
                }

                string archivo = Path.Combine(carpeta, PREFIJO_RESPALDO + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".bak");
                using (SqlCommand respaldo = new SqlCommand("SP_GenerarBackup", conexion)
                    { CommandType = CommandType.StoredProcedure, CommandTimeout = SEGUNDOS_SCRIPT })
                {
                    respaldo.Parameters.AddWithValue("@ruta", archivo);
                    respaldo.ExecuteNonQuery();
                }
                return archivo;
            }
        }

        public void RestaurarRespaldo(string instancia, string archivo)
        {
            string baseDatos = "[" + ConfiguracionConexion_575_AV.BASE_DATOS + "]";
            SqlConnection.ClearAllPools();

            using (SqlConnection conexion = new SqlConnection(
                ConfiguracionConexion_575_AV.ArmarCadena(instancia, BASE_SISTEMA, SEGUNDOS_PRUEBA)))
            {
                conexion.Open();
                EjecutarEnMaster(conexion, "ALTER DATABASE " + baseDatos + " SET SINGLE_USER WITH ROLLBACK IMMEDIATE;", null);
                try
                {
                    EjecutarEnMaster(conexion, "RESTORE DATABASE " + baseDatos + " FROM DISK = @ruta WITH REPLACE;", archivo);
                }
                finally
                {
                    EjecutarEnMaster(conexion, "ALTER DATABASE " + baseDatos + " SET MULTI_USER;", null);
                }
            }
            SqlConnection.ClearAllPools();
        }

        private static void EjecutarEnMaster(SqlConnection conexion, string sentencia, string ruta)
        {
            using (SqlCommand comando = new SqlCommand(sentencia, conexion) { CommandTimeout = SEGUNDOS_SCRIPT })
            {
                if (ruta != null) { comando.Parameters.AddWithValue("@ruta", ruta); }
                comando.ExecuteNonQuery();
            }
        }

        private void Ejecutar(List<string> scripts, string instancia, IProgress<AvanceInstalacion_575_AV> avance)
        {
            if (scripts.Count == 0)
            {
                throw new FileNotFoundException("CONEX_ERR_SIN_SCRIPTS");
            }

            using (SqlConnection conexion = new SqlConnection(
                ConfiguracionConexion_575_AV.ArmarCadena(instancia, BASE_SISTEMA, null)))
            {
                conexion.Open();

                for (int i = 0; i < scripts.Count; i++)
                {
                    string nombre = Path.GetFileName(scripts[i]);
                    Informar(avance, nombre, i + 1, scripts.Count, false);

                    foreach (string lote in Partir(File.ReadAllText(scripts[i], Encoding.UTF8)))
                    {
                        using (SqlCommand comando = new SqlCommand(lote, conexion) { CommandTimeout = SEGUNDOS_SCRIPT })
                        {
                            comando.ExecuteNonQuery();
                        }
                    }

                    Informar(avance, nombre, i + 1, scripts.Count, true);
                }
            }
        }

        public void BorrarBase(string instancia)
        {
            SqlConnection.ClearAllPools();

            using (SqlConnection conexion = new SqlConnection(
                ConfiguracionConexion_575_AV.ArmarCadena(instancia, BASE_SISTEMA, SEGUNDOS_PRUEBA)))
            using (SqlCommand comando = new SqlCommand(
                "IF DB_ID(N'" + ConfiguracionConexion_575_AV.BASE_DATOS + "') IS NOT NULL " +
                "BEGIN ALTER DATABASE [" + ConfiguracionConexion_575_AV.BASE_DATOS + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; " +
                "DROP DATABASE [" + ConfiguracionConexion_575_AV.BASE_DATOS + "]; END", conexion))
            {
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        private static IEnumerable<string> Partir(string texto)
        {
            StringBuilder lote = new StringBuilder();
            foreach (string linea in texto.Replace("\r\n", "\n").Split('\n'))
            {
                if (separador.IsMatch(linea))
                {
                    if (lote.ToString().Trim().Length > 0) { yield return lote.ToString(); }
                    lote.Clear();
                }
                else
                {
                    lote.AppendLine(linea);
                }
            }
            if (lote.ToString().Trim().Length > 0) { yield return lote.ToString(); }
        }

        private static void Informar(IProgress<AvanceInstalacion_575_AV> avance, string script, int numero, int total, bool terminado)
        {
            if (avance == null) { return; }
            avance.Report(new AvanceInstalacion_575_AV { script = script, numero = numero, total = total, terminado = terminado });
        }
    }
}
