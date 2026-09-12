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
                    IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                    Nombre = dr["Nombre"].ToString(),
                    Activo = Convert.ToBoolean(dr["Activo"]),
                    DVH = dr["DVH"] == DBNull.Value ? null : dr["DVH"].ToString()
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
                    IdMarca = Convert.ToInt32(dr["IdMarca"]),
                    Nombre = dr["Nombre"].ToString(),
                    Activo = Convert.ToBoolean(dr["Activo"]),
                    DVH = dr["DVH"] == DBNull.Value ? null : dr["DVH"].ToString()
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
                    IdUnidadMedida = Convert.ToInt32(dr["IdUnidadMedida"]),
                    Nombre = dr["Nombre"].ToString(),
                    Abreviatura = dr["Abreviatura"].ToString(),
                    Fraccionable = Convert.ToBoolean(dr["Fraccionable"]),
                    DVH = dr["DVH"] == DBNull.Value ? null : dr["DVH"].ToString()
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
                    IdMedioPago = Convert.ToInt32(dr["IdMedioPago"]),
                    Nombre = dr["Nombre"].ToString(),
                    RequiereAutorizacion = Convert.ToBoolean(dr["RequiereAutorizacion"]),
                    Activo = Convert.ToBoolean(dr["Activo"]),
                    DVH = dr["DVH"] == DBNull.Value ? null : dr["DVH"].ToString()
                });
            }
            return medios;
        }

        /// <summary>Alta de marca desde el ABM de producto. Devuelve el Id generado.</summary>
        public int CrearMarca(string nombre)
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@Nombre", nombre);

            object id = conexDB.EscribirRetornar("SP_CrearMarca", parametros);
            return id == null ? 0 : Convert.ToInt32(id);
        }
    }
}
