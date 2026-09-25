using Entidad_BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Acceso_DAL
{
    /// <summary>
    /// Mapeador de los cuatro catalogos del dominio: categoria, marca, unidad
    /// de medida y medio de pago. Van juntos porque son tablas chicas, de solo
    /// lectura en el circuito de venta y sin logica propia.
    /// </summary>
    public class MP_Catalogo_575_AV
    {
        AccesoDatos conexDB = new AccesoDatos();

        public List<CategoriaBE_575_AV> ListarCategorias()
        {
            List<CategoriaBE_575_AV> categorias = new List<CategoriaBE_575_AV>();
            DataTable dt = conexDB.LeerTabla("SP_ExtCategoria", null);
            foreach (DataRow dr in dt.Rows)
            {
                categorias.Add(new CategoriaBE_575_AV
                {
                    id_categoria = Convert.ToInt32(dr["id_categoria"]),
                    nombre = dr["nombre"].ToString(),
                    activo = Convert.ToBoolean(dr["activo"]),
                    dvh = dr["dvh"] == DBNull.Value ? null : dr["dvh"].ToString()
                });
            }
            return categorias;
        }

        public List<MarcaBE_575_AV> ListarMarcas()
        {
            List<MarcaBE_575_AV> marcas = new List<MarcaBE_575_AV>();
            DataTable dt = conexDB.LeerTabla("SP_ExtMarca", null);
            foreach (DataRow dr in dt.Rows)
            {
                marcas.Add(new MarcaBE_575_AV
                {
                    id_marca = Convert.ToInt32(dr["id_marca"]),
                    nombre = dr["nombre"].ToString(),
                    activo = Convert.ToBoolean(dr["activo"]),
                    dvh = dr["dvh"] == DBNull.Value ? null : dr["dvh"].ToString()
                });
            }
            return marcas;
        }

        public List<UnidadMedidaBE_575_AV> ListarUnidadesMedida()
        {
            List<UnidadMedidaBE_575_AV> unidades = new List<UnidadMedidaBE_575_AV>();
            DataTable dt = conexDB.LeerTabla("SP_ExtUnidadMedida", null);
            foreach (DataRow dr in dt.Rows)
            {
                unidades.Add(new UnidadMedidaBE_575_AV
                {
                    id_unidad_medida = Convert.ToInt32(dr["id_unidad_medida"]),
                    nombre = dr["nombre"].ToString(),
                    abreviatura = dr["abreviatura"].ToString(),
                    fraccionable = Convert.ToBoolean(dr["fraccionable"]),
                    dvh = dr["dvh"] == DBNull.Value ? null : dr["dvh"].ToString()
                });
            }
            return unidades;
        }

        public List<MedioPagoBE_575_AV> ListarMediosPago()
        {
            List<MedioPagoBE_575_AV> medios = new List<MedioPagoBE_575_AV>();
            DataTable dt = conexDB.LeerTabla("SP_ExtMedioPago", null);
            foreach (DataRow dr in dt.Rows)
            {
                medios.Add(new MedioPagoBE_575_AV
                {
                    id_medio_pago = Convert.ToInt32(dr["id_medio_pago"]),
                    nombre = dr["nombre"].ToString(),
                    requiere_autorizacion = Convert.ToBoolean(dr["requiere_autorizacion"]),
                    activo = Convert.ToBoolean(dr["activo"]),
                    dvh = dr["dvh"] == DBNull.Value ? null : dr["dvh"].ToString()
                });
            }
            return medios;
        }

        /// <summary>Alta de marca desde el ABM de articulo. Devuelve el id generado.</summary>
        public int CrearMarca(string nombre)
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@nombre", nombre);

            object id = conexDB.EscribirRetornar("SP_CrearMarca", parametros);
            return id == null ? 0 : Convert.ToInt32(id);
        }

        #region Digito verificador

        // Este mapeador atiende cuatro tablas, asi que cada una tiene su propio
        // ActualizarDVH. Todos llaman a SP_ActualizarDVHNegocio con su @tabla.

        public void ActualizarDVHCategoria(int id_categoria, string dvh)
        {
            ActualizarDVH("Categoria", id_categoria, dvh);
        }

        public void ActualizarDVHMarca(int id_marca, string dvh)
        {
            ActualizarDVH("Marca", id_marca, dvh);
        }

        public void ActualizarDVHUnidadMedida(int id_unidad_medida, string dvh)
        {
            ActualizarDVH("UnidadMedida", id_unidad_medida, dvh);
        }

        public void ActualizarDVHMedioPago(int id_medio_pago, string dvh)
        {
            ActualizarDVH("MedioPago", id_medio_pago, dvh);
        }

        private void ActualizarDVH(string tabla, int id, string dvh)
        {
            SqlParameter[] parametros = new SqlParameter[3];
            parametros[0] = new SqlParameter("@tabla", tabla);
            parametros[1] = new SqlParameter("@id", id);
            parametros[2] = new SqlParameter("@dvh", dvh);

            conexDB.Escribir("SP_ActualizarDVHNegocio", parametros);
        }

        #endregion
    }
}
