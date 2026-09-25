using Entidad_BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Acceso_DAL
{
    /// <summary>
    /// Mapeador del agregado Entrega. Atiende dos tablas, asi que cada una tiene
    /// su propio ActualizarDVH.
    /// </summary>
    public class MP_Entrega_575_AV
    {
        AccesoDatos conexDB = new AccesoDatos();

        #region Consulta

        /// <summary>Devuelve nulo si el comprobante todavia no se despacho.</summary>
        public EntregaBE_575_AV BuscarPorComprobante(int id_comprobante)
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@id_comprobante", id_comprobante);

            return Proyectar(conexDB.LeerTabla("SP_ExtEntregaPorComprobante", parametros))
                .FirstOrDefault();
        }

        /// <summary>Todas las entregas, para el control de integridad.</summary>
        public List<EntregaBE_575_AV> ListarEntregas()
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@id_comprobante", DBNull.Value);

            return Proyectar(conexDB.LeerTabla("SP_ExtEntregaPorComprobante", parametros));
        }

        public List<DetalleEntregaBE_575_AV> ListarDetalle(int id_entrega)
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@id_entrega", id_entrega);

            return ProyectarDetalle(conexDB.LeerTabla("SP_ExtDetalleEntrega", parametros));
        }

        /// <summary>Todos los renglones, para el control de integridad.</summary>
        public List<DetalleEntregaBE_575_AV> ListarDetalles()
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@id_entrega", DBNull.Value);

            return ProyectarDetalle(conexDB.LeerTabla("SP_ExtDetalleEntrega", parametros));
        }

        #endregion

        #region Escritura

        public int CrearEntrega(EntregaBE_575_AV entrega)
        {
            SqlParameter[] parametros = new SqlParameter[4];
            parametros[0] = new SqlParameter("@id_comprobante", entrega.id_comprobante);
            parametros[1] = new SqlParameter("@usuario_deposito", entrega.usuario_deposito);
            parametros[2] = new SqlParameter("@fecha_entrega", entrega.fecha_entrega);
            parametros[3] = new SqlParameter("@estado", entrega.estado.ToString());

            object id = conexDB.EscribirRetornar("SP_CrearEntrega", parametros);
            return id == null ? 0 : Convert.ToInt32(id);
        }

        public int AgregarDetalle(DetalleEntregaBE_575_AV detalle)
        {
            SqlParameter[] parametros = new SqlParameter[4];
            parametros[0] = new SqlParameter("@id_entrega", detalle.id_entrega);
            parametros[1] = new SqlParameter("@id_articulo", detalle.id_articulo);
            parametros[2] = new SqlParameter("@cantidad_entregada", detalle.cantidad_entregada);
            parametros[3] = new SqlParameter("@id_unidad_medida", detalle.id_unidad_medida);

            object id = conexDB.EscribirRetornar("SP_AgregarDetalleEntrega", parametros);
            return id == null ? 0 : Convert.ToInt32(id);
        }

        #endregion

        #region Digito verificador

        public void ActualizarDVH(int id_entrega, string dvh)
        {
            ActualizarDVH("Entrega", id_entrega, dvh);
        }

        public void ActualizarDVHDetalle(int id_detalle_entrega, string dvh)
        {
            ActualizarDVH("DetalleEntrega", id_detalle_entrega, dvh);
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

        private List<EntregaBE_575_AV> Proyectar(DataTable dt)
        {
            List<EntregaBE_575_AV> entregas = new List<EntregaBE_575_AV>();
            foreach (DataRow dr in dt.Rows)
            {
                EstadoEntrega_575_AV estado;
                if (!Enum.TryParse(dr["estado"].ToString(), out estado))
                {
                    estado = EstadoEntrega_575_AV.Total;
                }

                entregas.Add(new EntregaBE_575_AV
                {
                    id_entrega = Convert.ToInt32(dr["id_entrega"]),
                    id_comprobante = Convert.ToInt32(dr["id_comprobante"]),
                    usuario_deposito = dr["usuario_deposito"].ToString(),
                    fecha_entrega = Convert.ToDateTime(dr["fecha_entrega"]),
                    estado = estado,
                    dvh = dr["dvh"] == DBNull.Value ? null : dr["dvh"].ToString()
                });
            }
            return entregas;
        }

        private List<DetalleEntregaBE_575_AV> ProyectarDetalle(DataTable dt)
        {
            List<DetalleEntregaBE_575_AV> detalles = new List<DetalleEntregaBE_575_AV>();
            foreach (DataRow dr in dt.Rows)
            {
                detalles.Add(new DetalleEntregaBE_575_AV
                {
                    id_detalle_entrega = Convert.ToInt32(dr["id_detalle_entrega"]),
                    id_entrega = Convert.ToInt32(dr["id_entrega"]),
                    id_articulo = Convert.ToInt32(dr["id_articulo"]),
                    cantidad_entregada = Convert.ToDecimal(dr["cantidad_entregada"]),
                    id_unidad_medida = Convert.ToInt32(dr["id_unidad_medida"]),
                    dvh = dr["dvh"] == DBNull.Value ? null : dr["dvh"].ToString()
                });
            }
            return detalles;
        }

        #endregion
    }
}
