using Entidad_BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Acceso_DAL
{
    public class IdiomaDAL
    {
        private AccesoDatos acceso = new AccesoDatos();

        public List<IdiomaBE> ObtenerIdiomas()
        {
            List<IdiomaBE> idiomas = new List<IdiomaBE>();
            DataTable dt = acceso.LeerTabla("SP_ExtIdiomas", null);

            foreach (DataRow fila in dt.Rows)
            {
                IdiomaBE idioma = new IdiomaBE();
                idioma.Id = Convert.ToInt32(fila["Id"]);
                idioma.Codigo = fila["Codigo"].ToString();
                idioma.Nombre = fila["Nombre"].ToString();
                idiomas.Add(idioma);
            }
            return idiomas;
        }

        public Dictionary<string, string> ObtenerTraducciones(int idIdioma)
        {
            Dictionary<string, string> traducciones = new Dictionary<string, string>();
            foreach (DataRow fila in ObtenerTablaTraducciones(idIdioma).Rows)
            {
                traducciones[fila["Clave"].ToString()] = fila["Texto"].ToString();
            }
            return traducciones;
        }

        public DataTable ObtenerTablaTraducciones(int idIdioma)
        {
            SqlParameter[] parametros = new SqlParameter[]
            {
                new SqlParameter("@IdIdioma", idIdioma)
            };
            return acceso.LeerTabla("SP_ExtTraducciones", parametros);
        }

        public void CrearIdioma(string codigo, string nombre, string codigoBase)
        {
            SqlParameter[] parametros = new SqlParameter[]
            {
                new SqlParameter("@Codigo", codigo),
                new SqlParameter("@Nombre", nombre),
                new SqlParameter("@CodigoBase", codigoBase)
            };
            acceso.Escribir("SP_CrearIdioma", parametros);
        }

        public void ActualizarTraduccion(int idIdioma, string clave, string texto)
        {
            SqlParameter[] parametros = new SqlParameter[]
            {
                new SqlParameter("@IdIdioma", idIdioma),
                new SqlParameter("@Clave", clave),
                new SqlParameter("@Texto", texto)
            };
            acceso.Escribir("SP_ActualizarTraduccion", parametros);
        }
    }
}
