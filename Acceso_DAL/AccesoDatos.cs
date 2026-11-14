using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acceso_DAL
{
    public class AccesoDatos
    {
        private SqlConnection _conexion;

        public SqlConnection conexion { get => _conexion; }

        public void AbrirConexion()
        {
            _conexion = new SqlConnection(ConfiguracionConexion_575_AV.CadenaConexion());
            _conexion.Open();
        }

        public void CerrarConexion()
        {
            _conexion.Close();
        }

        public bool VerificarConexion()
        {
            try
            {
                AbrirConexion();
                if (_conexion.State == ConnectionState.Open)
                {
                    CerrarConexion();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable LeerTabla(string sp, SqlParameter[] datos)
        {
            try
            {
                AbrirConexion();
                DataTable dt = new DataTable();
                SqlDataAdapter ad = new SqlDataAdapter();
                ad.SelectCommand = new SqlCommand();
                ad.SelectCommand.CommandType = CommandType.StoredProcedure;
                ad.SelectCommand.CommandText = sp;
                if (datos != null)
                {
                    ad.SelectCommand.Parameters.AddRange(datos);
                }
                ad.SelectCommand.Connection = conexion;
                ad.Fill(dt);

                return dt;
            }
            catch (Exception) { throw; }
            finally { CerrarConexion(); }
        }

        public void Escribir(string sp, SqlParameter[] parametros)
        {
            SqlTransaction tr;
            AbrirConexion();
            tr = conexion.BeginTransaction();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = sp;
                cmd.Connection = conexion;
                cmd.Parameters.AddRange(parametros);
                cmd.Transaction = tr;
                cmd.ExecuteNonQuery();
                tr.Commit();
            }
            catch (Exception)
            {
                tr.Rollback();
                throw;
            }
            finally { CerrarConexion(); }
        }

        public object EscribirRetornar(string sp, SqlParameter[] parametros)
        {
            SqlTransaction tr;
            AbrirConexion();
            tr = conexion.BeginTransaction();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = sp;
                cmd.Connection = conexion;
                cmd.Parameters.AddRange(parametros);
                cmd.Transaction = tr;
                object result = cmd.ExecuteScalar();
                tr.Commit();
                return result;
            }
            catch (Exception)
            {
                tr.Rollback();
                throw;
            }
            finally { CerrarConexion(); }
        }

        public int Consulta(string sp, SqlParameter[] parametros)
        {
            int result;

            try
            {
                AbrirConexion();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = sp;
                cmd.Connection = conexion;
                cmd.Parameters.AddRange(parametros);
                cmd.Parameters.Add("@Result", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                return result = Convert.ToInt32(cmd.Parameters["@Result"].Value);
            }
            catch (Exception) { throw; }
            finally { CerrarConexion(); }
        }

        // T07 - BACKUP y RESTORE VERIFYONLY no se pueden ejecutar dentro de una transaccion y pueden tardar
        public void EjecutarSinTransaccion(string sp, SqlParameter[] parametros, int segundosEspera)
        {
            try
            {
                AbrirConexion();
                SqlCommand cmd = new SqlCommand(sp, conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = segundosEspera;
                if (parametros != null)
                {
                    cmd.Parameters.AddRange(parametros);
                }
                cmd.ExecuteNonQuery();
            }
            finally { CerrarConexion(); }
        }

        public object ExtraerDato(string sp, SqlParameter[] parametros)
        {
            try
            {
                AbrirConexion();
                SqlCommand cmd = new SqlCommand(sp, conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                if (parametros != null)
                {
                    cmd.Parameters.AddRange(parametros);
                }
                object valor = cmd.ExecuteScalar();
                if(valor == DBNull.Value) { return null; }
                else return valor;
            }
            catch (Exception)
            {
                throw;
            }
            finally { CerrarConexion(); }
        }
    }
}
