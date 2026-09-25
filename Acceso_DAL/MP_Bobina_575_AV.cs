using Entidad_BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Acceso_DAL
{
    /// <summary>
    /// Mapeador de Bobina. El estado viaja como texto para que coincida con la
    /// restriccion CK_Bobina_estado de la base y se lea directo en la tabla.
    /// </summary>
    public class MP_Bobina_575_AV
    {
        AccesoDatos conexDB = new AccesoDatos();

        public List<BobinaBE_575_AV> ListarBobinas()
        {
            return Proyectar(conexDB.LeerTabla("SP_ExtBobina", null));
        }

        /// <summary>
        /// Bobinas disponibles de un articulo, primero las abiertas y de menor
        /// saldo: el fraccionamiento agota la que ya esta abierta antes de abrir
        /// una nueva.
        /// </summary>
        public List<BobinaBE_575_AV> ListarPorArticulo(int id_articulo)
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@id_articulo", id_articulo);

            return Proyectar(conexDB.LeerTabla("SP_ExtBobinaPorArticulo", parametros));
        }

        /// <summary>Alta. Devuelve el id_bobina generado para calcular el dvh.</summary>
        public int CrearBobina(BobinaBE_575_AV bobina)
        {
            SqlParameter[] parametros = new SqlParameter[6];
            parametros[0] = new SqlParameter("@id_articulo", bobina.id_articulo);
            parametros[1] = new SqlParameter("@identificador", bobina.identificador);
            parametros[2] = new SqlParameter("@medida_inicial", bobina.medida_inicial);
            parametros[3] = new SqlParameter("@saldo_bobina", bobina.saldo_bobina);
            parametros[4] = new SqlParameter("@estado", bobina.estado.ToString());
            parametros[5] = new SqlParameter("@fecha_apertura",
                bobina.fecha_apertura.HasValue ? (object)bobina.fecha_apertura.Value : DBNull.Value);

            object id = conexDB.EscribirRetornar("SP_CrearBobina", parametros);
            return id == null ? 0 : Convert.ToInt32(id);
        }

        /// <summary>RFN1.8 - Persiste el saldo y el estado que resolvio la BLL.</summary>
        public void ActualizarSaldo(BobinaBE_575_AV bobina)
        {
            SqlParameter[] parametros = new SqlParameter[4];
            parametros[0] = new SqlParameter("@id_bobina", bobina.id_bobina);
            parametros[1] = new SqlParameter("@saldo_bobina", bobina.saldo_bobina);
            parametros[2] = new SqlParameter("@estado", bobina.estado.ToString());
            parametros[3] = new SqlParameter("@fecha_apertura",
                bobina.fecha_apertura.HasValue ? (object)bobina.fecha_apertura.Value : DBNull.Value);

            conexDB.Escribir("SP_ActualizarSaldoBobina", parametros);
        }

        public void ActualizarDVH(int id_bobina, string dvh)
        {
            SqlParameter[] parametros = new SqlParameter[3];
            parametros[0] = new SqlParameter("@tabla", "Bobina");
            parametros[1] = new SqlParameter("@id", id_bobina);
            parametros[2] = new SqlParameter("@dvh", dvh);

            conexDB.Escribir("SP_ActualizarDVHNegocio", parametros);
        }

        private List<BobinaBE_575_AV> Proyectar(DataTable dt)
        {
            List<BobinaBE_575_AV> bobinas = new List<BobinaBE_575_AV>();
            foreach (DataRow dr in dt.Rows)
            {
                EstadoBobina_575_AV estado;
                if (!Enum.TryParse(dr["estado"].ToString(), out estado))
                {
                    estado = EstadoBobina_575_AV.Cerrada;
                }

                bobinas.Add(new BobinaBE_575_AV
                {
                    id_bobina = Convert.ToInt32(dr["id_bobina"]),
                    id_articulo = Convert.ToInt32(dr["id_articulo"]),
                    identificador = dr["identificador"].ToString(),
                    medida_inicial = Convert.ToDecimal(dr["medida_inicial"]),
                    saldo_bobina = Convert.ToDecimal(dr["saldo_bobina"]),
                    fecha_apertura = dr["fecha_apertura"] == DBNull.Value
                        ? (DateTime?)null
                        : Convert.ToDateTime(dr["fecha_apertura"]),
                    estado = estado,
                    dvh = dr["dvh"] == DBNull.Value ? null : dr["dvh"].ToString()
                });
            }
            return bobinas;
        }
    }
}
