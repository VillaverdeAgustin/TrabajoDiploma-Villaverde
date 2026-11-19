using Entidad_BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Acceso_DAL
{
    public class MP_ArticuloHistorial_575_AV
    {
        AccesoDatos conexDB = new AccesoDatos();

        public List<ArticuloHistorialBE_575_AV> BuscarHistorial(int? id_articulo, string operacion, DateTime desde, DateTime hasta)
        {
            SqlParameter[] parametros = new SqlParameter[4];
            parametros[0] = new SqlParameter("@id_articulo", id_articulo.HasValue ? (object)id_articulo.Value : DBNull.Value);
            parametros[1] = new SqlParameter("@operacion", string.IsNullOrWhiteSpace(operacion) ? (object)DBNull.Value : operacion);
            parametros[2] = new SqlParameter("@desde", SqlDbType.Date) { Value = desde.Date };
            parametros[3] = new SqlParameter("@hasta", SqlDbType.Date) { Value = hasta.Date };

            return conexDB.LeerTabla("SP_BuscarArticuloHistorial_575_AV", parametros).Rows.Cast<DataRow>().Select(Mapear).ToList();
        }

        public List<ArticuloHistorialBE_575_AV> ObtenerCambio(int id_cambio)
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@id_cambio", id_cambio);

            return conexDB.LeerTabla("SP_ExtCambioArticulo_575_AV", parametros).Rows.Cast<DataRow>().Select(Mapear).ToList();
        }

        public List<ArticuloHistorialBE_575_AV> ListarHistorial()
        {
            return conexDB.LeerTabla("SP_ExtArticuloHistorial_575_AV", null).Rows.Cast<DataRow>().Select(Mapear).ToList();
        }

        public List<ArticuloHistorialBE_575_AV> ListarSinDigito()
        {
            return conexDB.LeerTabla("SP_ExtArticuloHistorialSinDVH_575_AV", null).Rows.Cast<DataRow>().Select(Mapear).ToList();
        }

        public void ActualizarDVH(int id_historial, string dvh)
        {
            SqlParameter[] parametros = new SqlParameter[3];
            parametros[0] = new SqlParameter("@tabla", "ArticuloHistorial_575_AV");
            parametros[1] = new SqlParameter("@id", id_historial);
            parametros[2] = new SqlParameter("@dvh", dvh);

            conexDB.Escribir("SP_ActualizarDVHNegocio", parametros);
        }

        private ArticuloHistorialBE_575_AV Mapear(DataRow fila)
        {
            return new ArticuloHistorialBE_575_AV
            {
                id_historial = Convert.ToInt32(fila["id_historial"]),
                id_cambio = Convert.ToInt32(fila["id_cambio"]),
                id_articulo = Convert.ToInt32(fila["id_articulo"]),
                imagen = fila["imagen"].ToString(),
                operacion = fila["operacion"].ToString(),
                codigo = fila["codigo"].ToString(),
                descripcion = fila["descripcion"].ToString(),
                id_categoria = Convert.ToInt32(fila["id_categoria"]),
                id_marca = Convert.ToInt32(fila["id_marca"]),
                id_unidad_medida = Convert.ToInt32(fila["id_unidad_medida"]),
                precio_unitario = Convert.ToDecimal(fila["precio_unitario"]),
                cantidad_disponible = Convert.ToDecimal(fila["cantidad_disponible"]),
                punto_reposicion = Convert.ToDecimal(fila["punto_reposicion"]),
                deposito_ubicacion = fila["deposito_ubicacion"] == DBNull.Value ? null : fila["deposito_ubicacion"].ToString(),
                estado_articulo = fila["estado_articulo"].ToString(),
                activo = Convert.ToBoolean(fila["activo"]),
                usuario_modificacion = fila["usuario_modificacion"] == DBNull.Value ? null : fila["usuario_modificacion"].ToString(),
                fecha = Convert.ToDateTime(fila["fecha"]),
                dvh = fila["dvh"] == DBNull.Value ? null : fila["dvh"].ToString()
            };
        }
    }
}
