using Entidad_BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Acceso_DAL
{
    public class MP_Proveedor_575_AV
    {
        AccesoDatos conexDB = new AccesoDatos();

        public List<ProveedorBE_575_AV> ListarProveedores()
        {
            return Proyectar(conexDB.LeerTabla("SP_ExtProveedor", null));
        }

        public ProveedorBE_575_AV BuscarPorCuit(string cuit)
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@cuit", cuit);

            return Proyectar(conexDB.LeerTabla("SP_BuscarProveedorPorCuit", parametros)).FirstOrDefault();
        }

        public int CrearProveedor(ProveedorBE_575_AV proveedor)
        {
            object id = conexDB.EscribirRetornar("SP_CrearProveedor", Parametros(proveedor, false));
            return id == null ? 0 : Convert.ToInt32(id);
        }

        public void ActualizarProveedor(ProveedorBE_575_AV proveedor)
        {
            conexDB.Escribir("SP_ActualizarProveedor", Parametros(proveedor, true));
        }

        public void ActualizarDVH(int id_proveedor, string dvh)
        {
            SqlParameter[] parametros = new SqlParameter[3];
            parametros[0] = new SqlParameter("@tabla", "Proveedor");
            parametros[1] = new SqlParameter("@id", id_proveedor);
            parametros[2] = new SqlParameter("@dvh", dvh);

            conexDB.Escribir("SP_ActualizarDVHNegocio", parametros);
        }

        private static SqlParameter[] Parametros(ProveedorBE_575_AV p, bool modificacion)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();
            if (modificacion) { parametros.Add(new SqlParameter("@id_proveedor", p.id_proveedor)); }
            parametros.Add(new SqlParameter("@razon_social", p.razon_social));
            parametros.Add(new SqlParameter("@cuit", p.cuit));
            parametros.Add(new SqlParameter("@condicion_iva", p.condicion_iva.ToString()));
            parametros.Add(new SqlParameter("@rubro", Texto(p.rubro)));
            parametros.Add(new SqlParameter("@telefono", Texto(p.telefono)));
            parametros.Add(new SqlParameter("@correo_electronico", Texto(p.correo_electronico)));
            parametros.Add(new SqlParameter("@direccion", Texto(p.direccion)));
            if (modificacion) { parametros.Add(new SqlParameter("@activo", p.activo)); }
            return parametros.ToArray();
        }

        private static object Texto(string valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? (object)DBNull.Value : valor;
        }

        private static List<ProveedorBE_575_AV> Proyectar(DataTable dt)
        {
            List<ProveedorBE_575_AV> proveedores = new List<ProveedorBE_575_AV>();
            foreach (DataRow dr in dt.Rows)
            {
                CondicionIva_575_AV condicion;
                if (!Enum.TryParse(dr["condicion_iva"].ToString(), out condicion))
                {
                    condicion = CondicionIva_575_AV.ResponsableInscripto;
                }

                proveedores.Add(new ProveedorBE_575_AV
                {
                    id_proveedor = Convert.ToInt32(dr["id_proveedor"]),
                    razon_social = dr["razon_social"].ToString(),
                    cuit = dr["cuit"].ToString(),
                    condicion_iva = condicion,
                    rubro = Leer(dr["rubro"]),
                    telefono = Leer(dr["telefono"]),
                    correo_electronico = Leer(dr["correo_electronico"]),
                    direccion = Leer(dr["direccion"]),
                    activo = Convert.ToBoolean(dr["activo"]),
                    dvh = Leer(dr["dvh"])
                });
            }
            return proveedores;
        }

        private static string Leer(object valor)
        {
            return valor == DBNull.Value ? null : valor.ToString();
        }
    }
}
