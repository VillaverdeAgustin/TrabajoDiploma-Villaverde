using Entidad_BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Acceso_DAL
{
    public class MP_OrdenPago_575_AV
    {
        AccesoDatos conexDB = new AccesoDatos();

        public string ProximoNumero()
        {
            object numero = conexDB.ExtraerDato("SP_ProximoNumeroOrdenPago", null);
            return numero == null ? "OP-000001" : numero.ToString();
        }

        public int CrearOrdenPago(OrdenPagoBE_575_AV orden)
        {
            SqlParameter[] parametros = new SqlParameter[5];
            parametros[0] = new SqlParameter("@nro_orden_pago", orden.nro_orden_pago);
            parametros[1] = new SqlParameter("@id_carrito", orden.id_carrito);
            parametros[2] = new SqlParameter("@nombre_cliente",
                string.IsNullOrWhiteSpace(orden.nombre_cliente)
                    ? (object)DBNull.Value : orden.nombre_cliente);
            parametros[3] = new SqlParameter("@fecha_emision", orden.fecha_emision);
            parametros[4] = new SqlParameter("@precio_total", orden.precio_total);

            object id = conexDB.EscribirRetornar("SP_CrearOrdenPago", parametros);
            return id == null ? 0 : Convert.ToInt32(id);
        }

        public OrdenPagoBE_575_AV BuscarPorNumero(string nro_orden_pago)
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@nro_orden_pago", nro_orden_pago);

            return Proyectar(conexDB.LeerTabla("SP_BuscarOrdenPago", parametros))
                .FirstOrDefault();
        }

        public List<OrdenPagoBE_575_AV> ListarOrdenes()
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@nro_orden_pago", DBNull.Value);

            return Proyectar(conexDB.LeerTabla("SP_BuscarOrdenPago", parametros));
        }

        public List<OrdenPagoBE_575_AV> ListarPendientes()
        {
            return Proyectar(conexDB.LeerTabla("SP_ExtOrdenPagoPendientes", null));
        }

        public void ActualizarEstado(int id_orden_pago, EstadoOrdenPago_575_AV estado)
        {
            SqlParameter[] parametros = new SqlParameter[2];
            parametros[0] = new SqlParameter("@id_orden_pago", id_orden_pago);
            parametros[1] = new SqlParameter("@estado", estado.ToString());

            conexDB.Escribir("SP_ActualizarEstadoOrdenPago", parametros);
        }

        public void ActualizarDVH(int id_orden_pago, string dvh)
        {
            SqlParameter[] parametros = new SqlParameter[3];
            parametros[0] = new SqlParameter("@tabla", "OrdenPago");
            parametros[1] = new SqlParameter("@id", id_orden_pago);
            parametros[2] = new SqlParameter("@dvh", dvh);

            conexDB.Escribir("SP_ActualizarDVHNegocio", parametros);
        }

        private List<OrdenPagoBE_575_AV> Proyectar(DataTable dt)
        {
            List<OrdenPagoBE_575_AV> ordenes = new List<OrdenPagoBE_575_AV>();
            foreach (DataRow dr in dt.Rows)
            {
                EstadoOrdenPago_575_AV estado;
                if (!Enum.TryParse(dr["estado"].ToString(), out estado))
                {
                    estado = EstadoOrdenPago_575_AV.Pendiente;
                }

                ordenes.Add(new OrdenPagoBE_575_AV
                {
                    id_orden_pago = Convert.ToInt32(dr["id_orden_pago"]),
                    nro_orden_pago = dr["nro_orden_pago"].ToString(),
                    id_carrito = Convert.ToInt32(dr["id_carrito"]),
                    nombre_cliente = dr["nombre_cliente"] == DBNull.Value
                        ? null : dr["nombre_cliente"].ToString(),
                    fecha_emision = Convert.ToDateTime(dr["fecha_emision"]),
                    precio_total = Convert.ToDecimal(dr["precio_total"]),
                    estado = estado,
                    activo = Convert.ToBoolean(dr["activo"]),
                    dvh = dr["dvh"] == DBNull.Value ? null : dr["dvh"].ToString()
                });
            }
            return ordenes;
        }
    }
}
