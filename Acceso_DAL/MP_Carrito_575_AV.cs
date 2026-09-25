using Entidad_BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Acceso_DAL
{
    /// <summary>
    /// Mapeador del agregado Carrito. Atiende dos tablas, asi que cada una
    /// tiene su propio ActualizarDVH: Carrito y DetalleCarrito llevan cada cual
    /// su fila en DigitoVertical.
    /// </summary>
    public class MP_Carrito_575_AV
    {
        AccesoDatos conexDB = new AccesoDatos();

        #region Consulta

        public List<CarritoBE_575_AV> ListarCarritos()
        {
            return Proyectar(conexDB.LeerTabla("SP_ExtCarrito", null));
        }

        /// <summary>Los carritos que el cajero puede tomar.</summary>
        public List<CarritoBE_575_AV> ListarAbiertos()
        {
            return Proyectar(conexDB.LeerTabla("SP_ExtCarritoAbiertos", null));
        }

        public List<DetalleCarritoBE_575_AV> ListarDetalle(int id_carrito)
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@id_carrito", id_carrito);

            return ProyectarDetalle(conexDB.LeerTabla("SP_ExtDetalleCarrito", parametros));
        }

        /// <summary>Todos los renglones, para el control de integridad de la tabla.</summary>
        public List<DetalleCarritoBE_575_AV> ListarDetalles()
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@id_carrito", DBNull.Value);

            return ProyectarDetalle(conexDB.LeerTabla("SP_ExtDetalleCarrito", parametros));
        }

        /// <summary>Numero de carrito siguiente, con el formato CAR-000000.</summary>
        public string ProximoNumero()
        {
            object numero = conexDB.ExtraerDato("SP_ProximoNumeroCarrito", null);
            return numero == null ? "CAR-000001" : numero.ToString();
        }

        #endregion

        #region Escritura

        public int CrearCarrito(CarritoBE_575_AV carrito)
        {
            SqlParameter[] parametros = new SqlParameter[4];
            parametros[0] = new SqlParameter("@nro_carrito", carrito.nro_carrito);
            parametros[1] = new SqlParameter("@usuario_vendedor", carrito.usuario_vendedor);
            parametros[2] = new SqlParameter("@nombre_cliente",
                string.IsNullOrWhiteSpace(carrito.nombre_cliente)
                    ? (object)DBNull.Value : carrito.nombre_cliente);
            parametros[3] = new SqlParameter("@fecha_apertura", carrito.fecha_apertura);

            object id = conexDB.EscribirRetornar("SP_CrearCarrito", parametros);
            return id == null ? 0 : Convert.ToInt32(id);
        }

        public int AgregarDetalle(DetalleCarritoBE_575_AV detalle)
        {
            SqlParameter[] parametros = new SqlParameter[7];
            parametros[0] = new SqlParameter("@id_carrito", detalle.id_carrito);
            parametros[1] = new SqlParameter("@id_articulo", detalle.id_articulo);
            parametros[2] = new SqlParameter("@cantidad", detalle.cantidad);
            parametros[3] = new SqlParameter("@id_unidad_medida", detalle.id_unidad_medida);
            parametros[4] = new SqlParameter("@precio_unitario", detalle.precio_unitario);
            parametros[5] = new SqlParameter("@subtotal", detalle.subtotal);
            parametros[6] = new SqlParameter("@id_bobina",
                detalle.id_bobina.HasValue ? (object)detalle.id_bobina.Value : DBNull.Value);

            object id = conexDB.EscribirRetornar("SP_AgregarDetalleCarrito", parametros);
            return id == null ? 0 : Convert.ToInt32(id);
        }

        public void QuitarDetalle(int id_detalle_carrito)
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@id_detalle_carrito", id_detalle_carrito);

            conexDB.Escribir("SP_QuitarDetalleCarrito", parametros);
        }

        public void ActualizarTotal(int id_carrito, decimal precio_total)
        {
            SqlParameter[] parametros = new SqlParameter[2];
            parametros[0] = new SqlParameter("@id_carrito", id_carrito);
            parametros[1] = new SqlParameter("@precio_total", precio_total);

            conexDB.Escribir("SP_ActualizarTotalCarrito", parametros);
        }

        public void ActualizarEstado(int id_carrito, EstadoCarrito_575_AV estado)
        {
            SqlParameter[] parametros = new SqlParameter[2];
            parametros[0] = new SqlParameter("@id_carrito", id_carrito);
            parametros[1] = new SqlParameter("@estado", estado.ToString());

            conexDB.Escribir("SP_ActualizarEstadoCarrito", parametros);
        }

        /// <summary>
        /// Cierra el carrito: el total se recalcula desde el detalle en la base,
        /// para que no dependa de lo que traiga la pantalla.
        /// </summary>
        public void ConfirmarCarrito(int id_carrito, string nombre_cliente)
        {
            SqlParameter[] parametros = new SqlParameter[2];
            parametros[0] = new SqlParameter("@id_carrito", id_carrito);
            parametros[1] = new SqlParameter("@nombre_cliente",
                string.IsNullOrWhiteSpace(nombre_cliente) ? (object)DBNull.Value : nombre_cliente);

            conexDB.Escribir("SP_ConfirmarCarrito", parametros);
        }

        #endregion

        #region Digito verificador

        public void ActualizarDVH(int id_carrito, string dvh)
        {
            ActualizarDVH("Carrito", id_carrito, dvh);
        }

        public void ActualizarDVHDetalle(int id_detalle_carrito, string dvh)
        {
            ActualizarDVH("DetalleCarrito", id_detalle_carrito, dvh);
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

        #region Proyeccion

        private List<CarritoBE_575_AV> Proyectar(DataTable dt)
        {
            List<CarritoBE_575_AV> carritos = new List<CarritoBE_575_AV>();
            foreach (DataRow dr in dt.Rows)
            {
                EstadoCarrito_575_AV estado;
                if (!Enum.TryParse(dr["estado"].ToString(), out estado))
                {
                    estado = EstadoCarrito_575_AV.Abierto;
                }

                carritos.Add(new CarritoBE_575_AV
                {
                    id_carrito = Convert.ToInt32(dr["id_carrito"]),
                    nro_carrito = dr["nro_carrito"].ToString(),
                    usuario_vendedor = dr["usuario_vendedor"].ToString(),
                    nombre_cliente = dr["nombre_cliente"] == DBNull.Value
                        ? null : dr["nombre_cliente"].ToString(),
                    fecha_apertura = Convert.ToDateTime(dr["fecha_apertura"]),
                    estado = estado,
                    precio_total = Convert.ToDecimal(dr["precio_total"]),
                    activo = Convert.ToBoolean(dr["activo"]),
                    dvh = dr["dvh"] == DBNull.Value ? null : dr["dvh"].ToString()
                });
            }
            return carritos;
        }

        private List<DetalleCarritoBE_575_AV> ProyectarDetalle(DataTable dt)
        {
            List<DetalleCarritoBE_575_AV> detalles = new List<DetalleCarritoBE_575_AV>();
            foreach (DataRow dr in dt.Rows)
            {
                detalles.Add(new DetalleCarritoBE_575_AV
                {
                    id_detalle_carrito = Convert.ToInt32(dr["id_detalle_carrito"]),
                    id_carrito = Convert.ToInt32(dr["id_carrito"]),
                    id_articulo = Convert.ToInt32(dr["id_articulo"]),
                    cantidad = Convert.ToDecimal(dr["cantidad"]),
                    id_unidad_medida = Convert.ToInt32(dr["id_unidad_medida"]),
                    precio_unitario = Convert.ToDecimal(dr["precio_unitario"]),
                    subtotal = Convert.ToDecimal(dr["subtotal"]),
                    id_bobina = dr["id_bobina"] == DBNull.Value
                        ? (int?)null : Convert.ToInt32(dr["id_bobina"]),
                    dvh = dr["dvh"] == DBNull.Value ? null : dr["dvh"].ToString()
                });
            }
            return detalles;
        }

        #endregion
    }
}
