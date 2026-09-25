using Entidad_BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Acceso_DAL
{
    /// <summary>
    /// Mapeador de Reserva. El bloqueo del articulo no se hace aca: lo dispara
    /// la BLL por el gestor de existencias, para que cada procedimiento escriba
    /// una sola tabla.
    /// </summary>
    public class MP_Reserva_575_AV
    {
        AccesoDatos conexDB = new AccesoDatos();

        public List<ReservaBE_575_AV> ListarReservas()
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@id_articulo", DBNull.Value);

            return Proyectar(conexDB.LeerTabla("SP_ExtReservaPorArticulo", parametros));
        }

        public List<ReservaBE_575_AV> ListarPorArticulo(int id_articulo)
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@id_articulo", id_articulo);

            return Proyectar(conexDB.LeerTabla("SP_ExtReservaPorArticulo", parametros));
        }

        /// <summary>Alta. Devuelve el id_reserva generado para calcular el dvh.</summary>
        public int CrearReserva(ReservaBE_575_AV reserva)
        {
            SqlParameter[] parametros = new SqlParameter[6];
            parametros[0] = new SqlParameter("@id_articulo", reserva.id_articulo);
            parametros[1] = new SqlParameter("@id_cliente",
                reserva.id_cliente.HasValue ? (object)reserva.id_cliente.Value : DBNull.Value);
            parametros[2] = new SqlParameter("@nombre_cliente",
                string.IsNullOrWhiteSpace(reserva.nombre_cliente)
                    ? (object)DBNull.Value : reserva.nombre_cliente);
            parametros[3] = new SqlParameter("@cantidad", reserva.cantidad);
            parametros[4] = new SqlParameter("@fecha_reserva", reserva.fecha_reserva);
            parametros[5] = new SqlParameter("@plazo_entrega", reserva.plazo_entrega);

            object id = conexDB.EscribirRetornar("SP_CrearReserva", parametros);
            return id == null ? 0 : Convert.ToInt32(id);
        }

        public void ActualizarEstado(int id_reserva, EstadoReserva_575_AV estado)
        {
            SqlParameter[] parametros = new SqlParameter[2];
            parametros[0] = new SqlParameter("@id_reserva", id_reserva);
            parametros[1] = new SqlParameter("@estado", estado.ToString());

            conexDB.Escribir("SP_ActualizarEstadoReserva", parametros);
        }

        public void ActualizarDVH(int id_reserva, string dvh)
        {
            SqlParameter[] parametros = new SqlParameter[3];
            parametros[0] = new SqlParameter("@tabla", "Reserva");
            parametros[1] = new SqlParameter("@id", id_reserva);
            parametros[2] = new SqlParameter("@dvh", dvh);

            conexDB.Escribir("SP_ActualizarDVHNegocio", parametros);
        }

        private List<ReservaBE_575_AV> Proyectar(DataTable dt)
        {
            List<ReservaBE_575_AV> reservas = new List<ReservaBE_575_AV>();
            foreach (DataRow dr in dt.Rows)
            {
                EstadoReserva_575_AV estado;
                if (!Enum.TryParse(dr["estado"].ToString(), out estado))
                {
                    estado = EstadoReserva_575_AV.Pendiente;
                }

                reservas.Add(new ReservaBE_575_AV
                {
                    id_reserva = Convert.ToInt32(dr["id_reserva"]),
                    id_articulo = Convert.ToInt32(dr["id_articulo"]),
                    id_cliente = dr["id_cliente"] == DBNull.Value
                        ? (int?)null : Convert.ToInt32(dr["id_cliente"]),
                    nombre_cliente = dr["nombre_cliente"] == DBNull.Value
                        ? null : dr["nombre_cliente"].ToString(),
                    cantidad = Convert.ToDecimal(dr["cantidad"]),
                    fecha_reserva = Convert.ToDateTime(dr["fecha_reserva"]),
                    plazo_entrega = Convert.ToInt32(dr["plazo_entrega"]),
                    estado = estado,
                    dvh = dr["dvh"] == DBNull.Value ? null : dr["dvh"].ToString()
                });
            }
            return reservas;
        }
    }
}
