using Entidad_BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Acceso_DAL
{
    // T07 - Backup y Restore de ConectAR_DB.
    // Backup, verificacion y listado van por stored procedures (script 02).
    // Excepcion documentada: la lectura de la cabecera del .bak y el RESTORE no pueden ir por SP de
    // ConectAR_DB (no se reemplaza una base desde adentro de ella), asi que se ejecutan como comandos
    // parametrizados contra master. El nombre de la base es una constante, nunca un dato del usuario.
    public class MP_Backup_575_AV
    {
        private const int SEGUNDOS_ESPERA = 600;
        private const string BASE_SISTEMA = "master";

        AccesoDatos conexDB = new AccesoDatos();

        public string ObtenerCarpetaPredeterminada()
        {
            object ruta = conexDB.ExtraerDato("SP_RutaBackups", null);
            return ruta == null ? null : ruta.ToString();
        }

        public void GenerarBackup(string ruta)
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@ruta", SqlDbType.NVarChar, 400) { Value = ruta };

            conexDB.EjecutarSinTransaccion("SP_GenerarBackup", parametros, SEGUNDOS_ESPERA);
        }

        public void VerificarBackup(string ruta)
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@ruta", SqlDbType.NVarChar, 400) { Value = ruta };

            conexDB.EjecutarSinTransaccion("SP_VerificarBackup", parametros, SEGUNDOS_ESPERA);
        }

        public List<BackupBE_575_AV> ListarBackups()
        {
            List<BackupBE_575_AV> backups = new List<BackupBE_575_AV>();
            foreach (DataRow dr in conexDB.LeerTabla("SP_ListarBackups", null).Rows)
            {
                backups.Add(new BackupBE_575_AV
                {
                    fecha = Convert.ToDateTime(dr["fecha"]),
                    tamano = Convert.ToInt64(dr["tamano"]),
                    ruta = dr["ruta"].ToString(),
                    base_datos = dr["base_datos"].ToString()
                });
            }
            return backups;
        }

        // Cabecera del archivo: de que base es, cuando termino y cuanto ocupa. Falla si el archivo no existe
        // o no es un backup de SQL Server.
        public BackupBE_575_AV LeerCabecera(string ruta)
        {
            using (SqlConnection cn = new SqlConnection(CadenaMaster()))
            using (SqlCommand cmd = new SqlCommand("RESTORE HEADERONLY FROM DISK = @ruta;", cn))
            {
                cmd.CommandTimeout = SEGUNDOS_ESPERA;
                cmd.Parameters.Add(new SqlParameter("@ruta", SqlDbType.NVarChar, 400) { Value = ruta });
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read()) { return null; }
                    return new BackupBE_575_AV
                    {
                        ruta = ruta,
                        base_datos = dr["DatabaseName"].ToString(),
                        fecha = Convert.ToDateTime(dr["BackupFinishDate"]),
                        tamano = Convert.ToInt64(dr["BackupSize"])
                    };
                }
            }
        }

        // SINGLE_USER WITH ROLLBACK IMMEDIATE -> RESTORE ... WITH REPLACE -> MULTI_USER, siempre desde master
        public void Restaurar(string ruta)
        {
            string baseDatos = "[" + ConfiguracionConexion_575_AV.BASE_DATOS + "]";
            SqlConnection.ClearAllPools();
            using (SqlConnection cn = new SqlConnection(CadenaMaster()))
            {
                cn.Open();
                Ejecutar(cn, "ALTER DATABASE " + baseDatos + " SET SINGLE_USER WITH ROLLBACK IMMEDIATE;", null);
                bool restaurada = false;
                try
                {
                    Ejecutar(cn, "RESTORE DATABASE " + baseDatos + " FROM DISK = @ruta WITH REPLACE;", ruta);
                    restaurada = true;
                }
                finally
                {
                    // Si el RESTORE fallo, se informa ese error y no el de volver a MULTI_USER
                    try { Ejecutar(cn, "ALTER DATABASE " + baseDatos + " SET MULTI_USER;", null); }
                    catch (SqlException) { if (restaurada) { throw; } }
                }
            }
            SqlConnection.ClearAllPools();
        }

        private static void Ejecutar(SqlConnection cn, string comando, string ruta)
        {
            using (SqlCommand cmd = new SqlCommand(comando, cn))
            {
                cmd.CommandTimeout = SEGUNDOS_ESPERA;
                if (ruta != null)
                {
                    cmd.Parameters.Add(new SqlParameter("@ruta", SqlDbType.NVarChar, 400) { Value = ruta });
                }
                cmd.ExecuteNonQuery();
            }
        }

        private static string CadenaMaster()
        {
            return ConfiguracionConexion_575_AV.ArmarCadena(ConfiguracionConexion_575_AV.ObtenerInstancia(), BASE_SISTEMA, null);
        }
    }
}
