using Entidad_BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Acceso_DAL
{
    public class MP_ListaPrecio_575_AV
    {
        AccesoDatos conexDB = new AccesoDatos();

        public int CrearLista(ListaPrecioBE_575_AV lista)
        {
            SqlParameter[] parametros = new SqlParameter[6];
            parametros[0] = new SqlParameter("@id_proveedor", lista.id_proveedor);
            parametros[1] = new SqlParameter("@archivo", lista.archivo);
            parametros[2] = new SqlParameter("@fecha_vigencia", SqlDbType.Date) { Value = lista.fecha_vigencia.Date };
            parametros[3] = new SqlParameter("@moneda", lista.moneda);
            parametros[4] = new SqlParameter("@fecha_importacion", lista.fecha_importacion);
            parametros[5] = new SqlParameter("@usuario_importacion", lista.usuario_importacion);

            object id = conexDB.EscribirRetornar("SP_CrearListaPrecio", parametros);
            return id == null ? 0 : Convert.ToInt32(id);
        }

        public void DesactivarLista(int id_lista_precio)
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@id_lista_precio", id_lista_precio);

            conexDB.Escribir("SP_DesactivarListaPrecio", parametros);
        }

        public int AgregarItem(ItemListaPrecioBE_575_AV item)
        {
            SqlParameter[] parametros = new SqlParameter[7];
            parametros[0] = new SqlParameter("@id_lista_precio", item.id_lista_precio);
            parametros[1] = new SqlParameter("@id_articulo", item.id_articulo.HasValue ? (object)item.id_articulo.Value : DBNull.Value);
            parametros[2] = new SqlParameter("@codigo_proveedor", item.codigo_proveedor);
            parametros[3] = new SqlParameter("@descripcion", Texto(item.descripcion));
            parametros[4] = new SqlParameter("@precio_lista", item.precio_lista);
            parametros[5] = new SqlParameter("@unidad_venta", Texto(item.unidad_venta));
            parametros[6] = new SqlParameter("@plazo_entrega", item.plazo_entrega.HasValue ? (object)item.plazo_entrega.Value : DBNull.Value);

            object id = conexDB.EscribirRetornar("SP_AgregarItemListaPrecio", parametros);
            return id == null ? 0 : Convert.ToInt32(id);
        }

        public List<ComparacionPrecio_575_AV> ListarPreciosPorArticulo(int id_articulo, string moneda)
        {
            SqlParameter[] parametros = new SqlParameter[2];
            parametros[0] = new SqlParameter("@id_articulo", id_articulo);
            parametros[1] = new SqlParameter("@moneda", moneda);

            List<ComparacionPrecio_575_AV> precios = new List<ComparacionPrecio_575_AV>();
            foreach (DataRow dr in conexDB.LeerTabla("SP_CompararPreciosArticulo", parametros).Rows)
            {
                precios.Add(new ComparacionPrecio_575_AV
                {
                    id_proveedor = Convert.ToInt32(dr["id_proveedor"]),
                    razon_social = dr["razon_social"].ToString(),
                    id_lista_precio = Convert.ToInt32(dr["id_lista_precio"]),
                    fecha_vigencia = Convert.ToDateTime(dr["fecha_vigencia"]),
                    moneda = dr["moneda"].ToString(),
                    id_item_lista = Convert.ToInt32(dr["id_item_lista"]),
                    codigo_proveedor = dr["codigo_proveedor"].ToString(),
                    descripcion = Leer(dr["descripcion"]),
                    precio_lista = Convert.ToDecimal(dr["precio_lista"]),
                    unidad_venta = Leer(dr["unidad_venta"]),
                    plazo_entrega = dr["plazo_entrega"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["plazo_entrega"])
                });
            }
            return precios;
        }

        public ItemListaPrecioBE_575_AV BuscarPrecioVigente(int id_proveedor, int id_articulo)
        {
            SqlParameter[] parametros = new SqlParameter[2];
            parametros[0] = new SqlParameter("@id_proveedor", id_proveedor);
            parametros[1] = new SqlParameter("@id_articulo", id_articulo);

            return ProyectarItems(conexDB.LeerTabla("SP_BuscarPrecioVigente", parametros)).FirstOrDefault();
        }

        public List<ListaPrecioBE_575_AV> ListarListas()
        {
            List<ListaPrecioBE_575_AV> listas = new List<ListaPrecioBE_575_AV>();
            foreach (DataRow dr in conexDB.LeerTabla("SP_ExtListaPrecio", null).Rows)
            {
                listas.Add(new ListaPrecioBE_575_AV
                {
                    id_lista_precio = Convert.ToInt32(dr["id_lista_precio"]),
                    id_proveedor = Convert.ToInt32(dr["id_proveedor"]),
                    archivo = dr["archivo"].ToString(),
                    fecha_vigencia = Convert.ToDateTime(dr["fecha_vigencia"]),
                    moneda = dr["moneda"].ToString(),
                    fecha_importacion = Convert.ToDateTime(dr["fecha_importacion"]),
                    usuario_importacion = dr["usuario_importacion"].ToString(),
                    activo = Convert.ToBoolean(dr["activo"]),
                    dvh = Leer(dr["dvh"])
                });
            }
            return listas;
        }

        public List<ItemListaPrecioBE_575_AV> ListarItems()
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@id_lista_precio", DBNull.Value);

            return ProyectarItems(conexDB.LeerTabla("SP_ExtItemListaPrecio", parametros));
        }

        public void ActualizarDVH(int id_lista_precio, string dvh)
        {
            Dvh("ListaPrecio", id_lista_precio, dvh);
        }

        public void ActualizarDVHItem(int id_item_lista, string dvh)
        {
            Dvh("ItemListaPrecio", id_item_lista, dvh);
        }

        private void Dvh(string tabla, int id, string dvh)
        {
            SqlParameter[] parametros = new SqlParameter[3];
            parametros[0] = new SqlParameter("@tabla", tabla);
            parametros[1] = new SqlParameter("@id", id);
            parametros[2] = new SqlParameter("@dvh", dvh);

            conexDB.Escribir("SP_ActualizarDVHNegocio", parametros);
        }

        private static List<ItemListaPrecioBE_575_AV> ProyectarItems(DataTable dt)
        {
            List<ItemListaPrecioBE_575_AV> items = new List<ItemListaPrecioBE_575_AV>();
            foreach (DataRow dr in dt.Rows)
            {
                items.Add(new ItemListaPrecioBE_575_AV
                {
                    id_item_lista = Convert.ToInt32(dr["id_item_lista"]),
                    id_lista_precio = Convert.ToInt32(dr["id_lista_precio"]),
                    id_articulo = dr["id_articulo"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["id_articulo"]),
                    codigo_proveedor = dr["codigo_proveedor"].ToString(),
                    descripcion = Leer(dr["descripcion"]),
                    precio_lista = Convert.ToDecimal(dr["precio_lista"]),
                    unidad_venta = Leer(dr["unidad_venta"]),
                    plazo_entrega = dr["plazo_entrega"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["plazo_entrega"]),
                    dvh = Leer(dr["dvh"])
                });
            }
            return items;
        }

        private static object Texto(string valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? (object)DBNull.Value : valor;
        }

        private static string Leer(object valor)
        {
            return valor == DBNull.Value ? null : valor.ToString();
        }
    }
}
