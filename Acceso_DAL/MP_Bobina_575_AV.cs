using Entidad_BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Acceso_DAL
{
    /// <summary>
    /// Mapeador de Bobina. El estado viaja como texto para que coincida con la
    /// restriccion CK_Bobina_Estado de la base y se lea directo en la tabla.
    /// </summary>
    public class MP_Bobina_575_AV
    {
        AccesoDatos conexDB = new AccesoDatos();

        public List<BobinaBE_575_AV> ListarBobinas()
        {
            return Proyectar(conexDB.LeerTabla("SP_ExtBobina", null));
        }

        /// <summary>
        /// Bobinas disponibles de un producto, primero las abiertas y de menor
        /// saldo: el fraccionamiento agota la que ya esta abierta antes de
        /// abrir una nueva.
        /// </summary>
        public List<BobinaBE_575_AV> ListarPorProducto(int idProducto)
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@IdProducto", idProducto);

            return Proyectar(conexDB.LeerTabla("SP_ExtBobinaPorProducto", parametros));
        }

        /// <summary>Alta. Devuelve el IdBobina generado para calcular el DVH.</summary>
        public int CrearBobina(BobinaBE_575_AV bobina)
        {
            SqlParameter[] parametros = new SqlParameter[6];
            parametros[0] = new SqlParameter("@IdProducto", bobina.IdProducto);
            parametros[1] = new SqlParameter("@Identificador", bobina.Identificador);
            parametros[2] = new SqlParameter("@MedidaInicial", bobina.MedidaInicial);
            parametros[3] = new SqlParameter("@SaldoActual", bobina.SaldoActual);
            parametros[4] = new SqlParameter("@Estado", bobina.Estado.ToString());
            parametros[5] = new SqlParameter("@FechaApertura",
                bobina.FechaApertura.HasValue ? (object)bobina.FechaApertura.Value : DBNull.Value);

            object id = conexDB.EscribirRetornar("SP_CrearBobina", parametros);
            return id == null ? 0 : Convert.ToInt32(id);
        }

        /// <summary>RFN1.8 - Persiste el saldo y el estado que resolvio la BLL.</summary>
        public void ActualizarSaldo(BobinaBE_575_AV bobina)
        {
            SqlParameter[] parametros = new SqlParameter[4];
            parametros[0] = new SqlParameter("@IdBobina", bobina.IdBobina);
            parametros[1] = new SqlParameter("@SaldoActual", bobina.SaldoActual);
            parametros[2] = new SqlParameter("@Estado", bobina.Estado.ToString());
            parametros[3] = new SqlParameter("@FechaApertura",
                bobina.FechaApertura.HasValue ? (object)bobina.FechaApertura.Value : DBNull.Value);

            conexDB.Escribir("SP_ActualizarSaldoBobina", parametros);
        }

        public void ActualizarDVH(int idBobina, string dvh)
        {
            SqlParameter[] parametros = new SqlParameter[3];
            parametros[0] = new SqlParameter("@tabla", "Bobina");
            parametros[1] = new SqlParameter("@id", idBobina);
            parametros[2] = new SqlParameter("@dvh", dvh);

            conexDB.Escribir("SP_ActualizarDVHNegocio", parametros);
        }

        private List<BobinaBE_575_AV> Proyectar(DataTable dt)
        {
            List<BobinaBE_575_AV> bobinas = new List<BobinaBE_575_AV>();
            foreach (DataRow dr in dt.Rows)
            {
                EstadoBobina_575_AV estado;
                if (!Enum.TryParse(dr["Estado"].ToString(), out estado))
                {
                    estado = EstadoBobina_575_AV.Cerrada;
                }

                bobinas.Add(new BobinaBE_575_AV
                {
                    IdBobina = Convert.ToInt32(dr["IdBobina"]),
                    IdProducto = Convert.ToInt32(dr["IdProducto"]),
                    Identificador = dr["Identificador"].ToString(),
                    MedidaInicial = Convert.ToDecimal(dr["MedidaInicial"]),
                    SaldoActual = Convert.ToDecimal(dr["SaldoActual"]),
                    FechaApertura = dr["FechaApertura"] == DBNull.Value
                        ? (DateTime?)null
                        : Convert.ToDateTime(dr["FechaApertura"]),
                    Estado = estado,
                    DVH = dr["DVH"] == DBNull.Value ? null : dr["DVH"].ToString()
                });
            }
            return bobinas;
        }
    }
}
