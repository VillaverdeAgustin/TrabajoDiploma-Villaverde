using Entidad_BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Acceso_DAL
{
    /// <summary>
    /// Mapeador del agregado Producto. Toda persistencia por stored procedure
    /// y con parametros tipados; los criterios de busqueda no informados viajan
    /// como DBNull y el SP los ignora.
    /// </summary>
    public class MP_Producto_575_AV
    {
        AccesoDatos conexDB = new AccesoDatos();

        public List<ProductoBE_575_AV> ListarProductos()
        {
            return Proyectar(conexDB.LeerTabla("SP_ExtProducto", null));
        }

        /// <summary>
        /// RFN1.1 - Busqueda por descripcion, categoria, marca o codigo.
        /// Los criterios en nulo no filtran.
        /// </summary>
        public List<ProductoBE_575_AV> BuscarProductos(string descripcion, int? idCategoria,
                                                       int? idMarca, string codigo, bool soloActivos)
        {
            SqlParameter[] parametros = new SqlParameter[5];
            parametros[0] = new SqlParameter("@Descripcion",
                string.IsNullOrWhiteSpace(descripcion) ? (object)DBNull.Value : descripcion);
            parametros[1] = new SqlParameter("@IdCategoria",
                idCategoria.HasValue ? (object)idCategoria.Value : DBNull.Value);
            parametros[2] = new SqlParameter("@IdMarca",
                idMarca.HasValue ? (object)idMarca.Value : DBNull.Value);
            parametros[3] = new SqlParameter("@Codigo",
                string.IsNullOrWhiteSpace(codigo) ? (object)DBNull.Value : codigo);
            parametros[4] = new SqlParameter("@SoloActivos", soloActivos);

            return Proyectar(conexDB.LeerTabla("SP_BuscarProducto", parametros));
        }

        /// <summary>Alta. Devuelve el IdProducto generado para calcular el DVH.</summary>
        public int CrearProducto(ProductoBE_575_AV producto)
        {
            SqlParameter[] parametros = new SqlParameter[8];
            parametros[0] = new SqlParameter("@Codigo", producto.Codigo);
            parametros[1] = new SqlParameter("@Descripcion", producto.Descripcion);
            parametros[2] = new SqlParameter("@IdCategoria", producto.IdCategoria);
            parametros[3] = new SqlParameter("@IdMarca", producto.IdMarca);
            parametros[4] = new SqlParameter("@IdUnidadMedida", producto.IdUnidadMedida);
            parametros[5] = new SqlParameter("@PrecioUnitario", producto.PrecioUnitario);
            parametros[6] = new SqlParameter("@StockActual", producto.StockActual);
            parametros[7] = new SqlParameter("@PuntoReposicion", producto.PuntoReposicion);

            object id = conexDB.EscribirRetornar("SP_CrearProducto", parametros);
            return id == null ? 0 : Convert.ToInt32(id);
        }

        public void ActualizarProducto(ProductoBE_575_AV producto)
        {
            SqlParameter[] parametros = new SqlParameter[10];
            parametros[0] = new SqlParameter("@IdProducto", producto.IdProducto);
            parametros[1] = new SqlParameter("@Codigo", producto.Codigo);
            parametros[2] = new SqlParameter("@Descripcion", producto.Descripcion);
            parametros[3] = new SqlParameter("@IdCategoria", producto.IdCategoria);
            parametros[4] = new SqlParameter("@IdMarca", producto.IdMarca);
            parametros[5] = new SqlParameter("@IdUnidadMedida", producto.IdUnidadMedida);
            parametros[6] = new SqlParameter("@PrecioUnitario", producto.PrecioUnitario);
            parametros[7] = new SqlParameter("@StockActual", producto.StockActual);
            parametros[8] = new SqlParameter("@PuntoReposicion", producto.PuntoReposicion);
            parametros[9] = new SqlParameter("@Activo", producto.Activo);

            conexDB.Escribir("SP_ActualizarProducto", parametros);
        }

        /// <summary>Baja logica: nunca DELETE fisico.</summary>
        public void EliminarProducto(int idProducto)
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@IdProducto", idProducto);

            conexDB.Escribir("SP_ElimProducto", parametros);
        }

        /// <summary>RFN1.6 - Movimiento de stock. La cantidad es un delta con signo.</summary>
        public void ActualizarStock(int idProducto, decimal cantidad)
        {
            SqlParameter[] parametros = new SqlParameter[2];
            parametros[0] = new SqlParameter("@IdProducto", idProducto);
            parametros[1] = new SqlParameter("@Cantidad", cantidad);

            conexDB.Escribir("SP_ActualizarStock", parametros);
        }

        /// <summary>Persiste el DVH recalculado, sin tocar los datos del producto.</summary>
        public void ActualizarDVH(int idProducto, string dvh)
        {
            SqlParameter[] parametros = new SqlParameter[3];
            parametros[0] = new SqlParameter("@tabla", "Producto");
            parametros[1] = new SqlParameter("@id", idProducto);
            parametros[2] = new SqlParameter("@dvh", dvh);

            conexDB.Escribir("SP_ActualizarDVHNegocio", parametros);
        }

        private List<ProductoBE_575_AV> Proyectar(DataTable dt)
        {
            List<ProductoBE_575_AV> productos = new List<ProductoBE_575_AV>();
            foreach (DataRow dr in dt.Rows)
            {
                productos.Add(new ProductoBE_575_AV
                {
                    IdProducto = Convert.ToInt32(dr["IdProducto"]),
                    Codigo = dr["Codigo"].ToString(),
                    Descripcion = dr["Descripcion"].ToString(),
                    IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                    IdMarca = Convert.ToInt32(dr["IdMarca"]),
                    IdUnidadMedida = Convert.ToInt32(dr["IdUnidadMedida"]),
                    PrecioUnitario = Convert.ToDecimal(dr["PrecioUnitario"]),
                    StockActual = Convert.ToDecimal(dr["StockActual"]),
                    PuntoReposicion = Convert.ToDecimal(dr["PuntoReposicion"]),
                    Activo = Convert.ToBoolean(dr["Activo"]),
                    DVH = dr["DVH"] == DBNull.Value ? null : dr["DVH"].ToString()
                });
            }
            return productos;
        }
    }
}
