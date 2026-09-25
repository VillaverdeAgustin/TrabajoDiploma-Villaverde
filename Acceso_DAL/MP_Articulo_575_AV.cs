using Entidad_BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Acceso_DAL
{
    /// <summary>
    /// Mapeador del agregado Articulo. Toda persistencia por stored procedure y
    /// con parametros tipados; los criterios de busqueda no informados viajan
    /// como DBNull y el SP los ignora.
    /// </summary>
    public class MP_Articulo_575_AV
    {
        AccesoDatos conexDB = new AccesoDatos();

        public List<ArticuloBE_575_AV> ListarArticulos()
        {
            return Proyectar(conexDB.LeerTabla("SP_ExtArticulo", null));
        }

        /// <summary>
        /// RFN1.1 - Busqueda por descripcion, categoria, marca o codigo. Los
        /// criterios en nulo no filtran.
        /// </summary>
        public List<ArticuloBE_575_AV> BuscarArticulos(string descripcion, int? id_categoria,
                                                       int? id_marca, string codigo, bool solo_activos)
        {
            SqlParameter[] parametros = new SqlParameter[5];
            parametros[0] = new SqlParameter("@descripcion",
                string.IsNullOrWhiteSpace(descripcion) ? (object)DBNull.Value : descripcion);
            parametros[1] = new SqlParameter("@id_categoria",
                id_categoria.HasValue ? (object)id_categoria.Value : DBNull.Value);
            parametros[2] = new SqlParameter("@id_marca",
                id_marca.HasValue ? (object)id_marca.Value : DBNull.Value);
            parametros[3] = new SqlParameter("@codigo",
                string.IsNullOrWhiteSpace(codigo) ? (object)DBNull.Value : codigo);
            parametros[4] = new SqlParameter("@solo_activos", solo_activos);

            return Proyectar(conexDB.LeerTabla("SP_BuscarArticulo", parametros));
        }

        /// <summary>Alta. Devuelve el id_articulo generado para calcular el dvh.</summary>
        public int CrearArticulo(ArticuloBE_575_AV articulo)
        {
            SqlParameter[] parametros = new SqlParameter[10];
            parametros[0] = new SqlParameter("@codigo", articulo.codigo);
            parametros[1] = new SqlParameter("@descripcion", articulo.descripcion);
            parametros[2] = new SqlParameter("@id_categoria", articulo.id_categoria);
            parametros[3] = new SqlParameter("@id_marca", articulo.id_marca);
            parametros[4] = new SqlParameter("@id_unidad_medida", articulo.id_unidad_medida);
            parametros[5] = new SqlParameter("@precio_unitario", articulo.precio_unitario);
            parametros[6] = new SqlParameter("@cantidad_disponible", articulo.cantidad_disponible);
            parametros[7] = new SqlParameter("@punto_reposicion", articulo.punto_reposicion);
            parametros[8] = new SqlParameter("@deposito_ubicacion",
                string.IsNullOrWhiteSpace(articulo.deposito_ubicacion)
                    ? (object)DBNull.Value : articulo.deposito_ubicacion);
            parametros[9] = new SqlParameter("@estado_articulo", articulo.estado_articulo.ToString());

            object id = conexDB.EscribirRetornar("SP_CrearArticulo", parametros);
            return id == null ? 0 : Convert.ToInt32(id);
        }

        public void ActualizarArticulo(ArticuloBE_575_AV articulo)
        {
            SqlParameter[] parametros = new SqlParameter[12];
            parametros[0] = new SqlParameter("@id_articulo", articulo.id_articulo);
            parametros[1] = new SqlParameter("@codigo", articulo.codigo);
            parametros[2] = new SqlParameter("@descripcion", articulo.descripcion);
            parametros[3] = new SqlParameter("@id_categoria", articulo.id_categoria);
            parametros[4] = new SqlParameter("@id_marca", articulo.id_marca);
            parametros[5] = new SqlParameter("@id_unidad_medida", articulo.id_unidad_medida);
            parametros[6] = new SqlParameter("@precio_unitario", articulo.precio_unitario);
            parametros[7] = new SqlParameter("@cantidad_disponible", articulo.cantidad_disponible);
            parametros[8] = new SqlParameter("@punto_reposicion", articulo.punto_reposicion);
            parametros[9] = new SqlParameter("@deposito_ubicacion",
                string.IsNullOrWhiteSpace(articulo.deposito_ubicacion)
                    ? (object)DBNull.Value : articulo.deposito_ubicacion);
            parametros[10] = new SqlParameter("@estado_articulo", articulo.estado_articulo.ToString());
            parametros[11] = new SqlParameter("@activo", articulo.activo);

            conexDB.Escribir("SP_ActualizarArticulo", parametros);
        }

        /// <summary>Baja logica: nunca DELETE fisico.</summary>
        public void EliminarArticulo(int id_articulo)
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@id_articulo", id_articulo);

            conexDB.Escribir("SP_ElimArticulo", parametros);
        }

        /// <summary>RFN1.6 - Movimiento de existencias. La cantidad es un delta con signo.</summary>
        public void ActualizarStock(int id_articulo, decimal cantidad)
        {
            SqlParameter[] parametros = new SqlParameter[2];
            parametros[0] = new SqlParameter("@id_articulo", id_articulo);
            parametros[1] = new SqlParameter("@cantidad", cantidad);

            conexDB.Escribir("SP_ActualizarStock", parametros);
        }

        public void ActualizarEstado(int id_articulo, EstadoArticulo_575_AV estado)
        {
            SqlParameter[] parametros = new SqlParameter[2];
            parametros[0] = new SqlParameter("@id_articulo", id_articulo);
            parametros[1] = new SqlParameter("@estado_articulo", estado.ToString());

            conexDB.Escribir("SP_ActualizarEstadoArticulo", parametros);
        }

        /// <summary>Persiste el dvh recalculado, sin tocar los datos del articulo.</summary>
        public void ActualizarDVH(int id_articulo, string dvh)
        {
            SqlParameter[] parametros = new SqlParameter[3];
            parametros[0] = new SqlParameter("@tabla", "Articulo");
            parametros[1] = new SqlParameter("@id", id_articulo);
            parametros[2] = new SqlParameter("@dvh", dvh);

            conexDB.Escribir("SP_ActualizarDVHNegocio", parametros);
        }

        private List<ArticuloBE_575_AV> Proyectar(DataTable dt)
        {
            List<ArticuloBE_575_AV> articulos = new List<ArticuloBE_575_AV>();
            foreach (DataRow dr in dt.Rows)
            {
                EstadoArticulo_575_AV estado;
                if (!Enum.TryParse(dr["estado_articulo"].ToString(), out estado))
                {
                    estado = EstadoArticulo_575_AV.Disponible;
                }

                articulos.Add(new ArticuloBE_575_AV
                {
                    id_articulo = Convert.ToInt32(dr["id_articulo"]),
                    codigo = dr["codigo"].ToString(),
                    descripcion = dr["descripcion"].ToString(),
                    id_categoria = Convert.ToInt32(dr["id_categoria"]),
                    id_marca = Convert.ToInt32(dr["id_marca"]),
                    id_unidad_medida = Convert.ToInt32(dr["id_unidad_medida"]),
                    precio_unitario = Convert.ToDecimal(dr["precio_unitario"]),
                    cantidad_disponible = Convert.ToDecimal(dr["cantidad_disponible"]),
                    punto_reposicion = Convert.ToDecimal(dr["punto_reposicion"]),
                    deposito_ubicacion = dr["deposito_ubicacion"] == DBNull.Value
                        ? null : dr["deposito_ubicacion"].ToString(),
                    estado_articulo = estado,
                    activo = Convert.ToBoolean(dr["activo"]),
                    dvh = dr["dvh"] == DBNull.Value ? null : dr["dvh"].ToString()
                });
            }
            return articulos;
        }
    }
}
