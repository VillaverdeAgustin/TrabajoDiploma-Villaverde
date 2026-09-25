using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidad_BE;

namespace Acceso_DAL
{
    public class PerfilDAL
    {
        private AccesoDatos conexDB = new AccesoDatos();

        public List<Permiso> ListaPermisos(string tipo = "")
        {
            string command;
            SqlCommand cmd;
            if (tipo == "Compuesto" || tipo == "Simple")
            {
                command = "select * from Permiso where Tipo = @Tipo";
                cmd = new SqlCommand(command);
                cmd.Parameters.AddWithValue("@Tipo", tipo);
            }
            else if (tipo == "Rol")
            {
                command = "select * from Permiso where Rol = 1";
                cmd = new SqlCommand(command);
            }
            else
            {
                command = "select * from Permiso";
                cmd = new SqlCommand(command);
            }

            conexDB.AbrirConexion();
            cmd.Connection = conexDB.conexion;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            conexDB.CerrarConexion();

            List<Permiso> listaPermiso = new List<Permiso>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                if (dr[1].ToString() == "Simple")
                {
                    listaPermiso.Add(new PermisoSimple(dr[0].ToString()));
                }
                else
                {
                    if (bool.Parse(dr[2].ToString()))
                    {
                        listaPermiso.Add(new Familia(dr[0].ToString(), true));
                    }
                    else
                    {
                        listaPermiso.Add(new Familia(dr[0].ToString(), false));
                    }
                }
            }

            return listaPermiso;
        }

        public bool PerfilEnUso(string nombre)
        {
            conexDB.AbrirConexion();
            string query = "SELECT * FROM Usuarios WHERE Rol = @Nombre";
            SqlCommand cmd = new SqlCommand(query, conexDB.conexion);
            cmd.Parameters.AddWithValue("@Nombre", nombre);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            conexDB.CerrarConexion();

            int count = ds.Tables[0].Rows.Count;
            return count > 0;
        }

        public List<Permiso> ListaPermisosEnArbol()
        {
            List<Permiso> todos = new List<Permiso>();
            Dictionary<string, string> padres = new Dictionary<string, string>();

            string command = "select * from Permiso";
            conexDB.AbrirConexion();
            SqlCommand cmd = new SqlCommand(command, conexDB.conexion);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            conexDB.CerrarConexion();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Permiso p;
                if (dr[1].ToString() == "Simple")
                {
                    p = new PermisoSimple(dr[0].ToString());
                }
                else
                {
                    p = new Familia(dr[0].ToString(), bool.Parse(dr[2].ToString()));
                }

                todos.Add(p);
                if (dr[3] != DBNull.Value)
                {
                    padres[p.Nombre] = dr[3].ToString();
                }
            }

            List<Familia> compuestos = todos.OfType<Familia>().ToList();

            foreach (Permiso hijo in todos)
            {
                if (padres.TryGetValue(hijo.Nombre, out string nombrePadre))
                {
                    Familia padre = compuestos.Find(f => f.Nombre == nombrePadre);
                    padre?.AgregarHijo(hijo);
                }
            }

            return compuestos.Cast<Permiso>().ToList();
        }

        public List<Permiso> ListaPermisosRaiz()
        {
            List<Permiso> arbol = ListaPermisosEnArbol();

            HashSet<string> raices = new HashSet<string>();
            conexDB.AbrirConexion();
            SqlCommand cmd = new SqlCommand("Select Nombre_Permiso from Permiso where Nombre_PermisoPadre is null", conexDB.conexion);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            conexDB.CerrarConexion();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                raices.Add(dr[0].ToString());
            }

            return arbol.Where(x => raices.Contains(x.Nombre)).ToList();
        }

        public void AgregarFamilia(Familia pFamilia)
        {
            conexDB.AbrirConexion();
            string query = "INSERT INTO Permiso (Nombre_Permiso, Tipo, Rol) VALUES (@Nombre, 'Compuesto', @Rol)";
            SqlCommand cmd = new SqlCommand(query, conexDB.conexion);
            cmd.Parameters.AddWithValue("@Nombre", pFamilia.Nombre);
            cmd.Parameters.AddWithValue("@Rol", pFamilia.EsRol ? 1 : 0);
            cmd.ExecuteNonQuery();
            conexDB.CerrarConexion();
        }

        public void EliminarFamilia(Familia pFamilia)
        {
            conexDB.AbrirConexion();

            string command = "update Permiso set Nombre_PermisoPadre = null where Nombre_PermisoPadre = @Nombre";
            SqlCommand cmd = new SqlCommand(command, conexDB.conexion);
            cmd.Parameters.AddWithValue("@Nombre", pFamilia.Nombre);
            cmd.ExecuteNonQuery();

            command = "delete from Permiso where Nombre_Permiso = @Nombre";
            cmd = new SqlCommand(command, conexDB.conexion);
            cmd.Parameters.AddWithValue("@Nombre", pFamilia.Nombre);
            cmd.ExecuteNonQuery();

            conexDB.CerrarConexion();
        }

        public void ModificarFamilia(Familia pFamilia, List<string> permisos)
        {
            conexDB.AbrirConexion();

            string command = "update Permiso set Nombre_PermisoPadre = null where Nombre_PermisoPadre = @Nombre";
            SqlCommand cmd = new SqlCommand(command, conexDB.conexion);
            cmd.Parameters.AddWithValue("@Nombre", pFamilia.Nombre);
            cmd.ExecuteNonQuery();

            foreach (var p in permisos)
            {
                command = "update Permiso set Nombre_PermisoPadre = @Familia where Nombre_Permiso = @Permiso";
                cmd = new SqlCommand(command, conexDB.conexion);
                cmd.Parameters.AddWithValue("@Familia", pFamilia.Nombre);
                cmd.Parameters.AddWithValue("@Permiso", p);
                cmd.ExecuteNonQuery();
            }

            conexDB.CerrarConexion();
        }

        public void AgregarPermisoAFamilia(string familia, string permiso)
        {
            conexDB.AbrirConexion();
            string command = "update Permiso set Nombre_PermisoPadre = @Familia where Nombre_Permiso = @Permiso";
            SqlCommand cmd = new SqlCommand(command, conexDB.conexion);
            cmd.Parameters.AddWithValue("@Familia", familia);
            cmd.Parameters.AddWithValue("@Permiso", permiso);
            cmd.ExecuteNonQuery();
            conexDB.CerrarConexion();
        }

        public void EliminarPermisoDeFamilia(string familia, string permiso)
        {
            conexDB.AbrirConexion();
            string command = "update Permiso set Nombre_PermisoPadre = null where Nombre_PermisoPadre = @Familia AND Nombre_Permiso = @Permiso";
            SqlCommand cmd = new SqlCommand(command, conexDB.conexion);
            cmd.Parameters.AddWithValue("@Familia", familia);
            cmd.Parameters.AddWithValue("@Permiso", permiso);
            cmd.ExecuteNonQuery();
            conexDB.CerrarConexion();
        }
    }
}

