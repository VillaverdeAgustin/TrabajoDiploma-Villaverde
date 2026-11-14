using Entidad_BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Acceso_DAL
{
    public class MP_Recepcion_575_AV
    {
        AccesoDatos conexDB = new AccesoDatos();

        public int CrearRecepcion(RecepcionBE_575_AV recepcion)
        {
            SqlParameter[] parametros = new SqlParameter[5];
            parametros[0] = new SqlParameter("@id_orden_compra", recepcion.id_orden_compra);
            parametros[1] = new SqlParameter("@remito_proveedor", recepcion.remito_proveedor);
            parametros[2] = new SqlParameter("@fecha_recepcion", recepcion.fecha_recepcion);
            parametros[3] = new SqlParameter("@usuario_deposito", recepcion.usuario_deposito);
            parametros[4] = new SqlParameter("@diferencias",
                string.IsNullOrWhiteSpace(recepcion.diferencias) ? (object)DBNull.Value : recepcion.diferencias);

            object id = conexDB.EscribirRetornar("SP_CrearRecepcion", parametros);
            return id == null ? 0 : Convert.ToInt32(id);
        }

        public int AgregarDetalle(DetalleRecepcionBE_575_AV detalle)
        {
            SqlParameter[] parametros = new SqlParameter[4];
            parametros[0] = new SqlParameter("@id_recepcion", detalle.id_recepcion);
            parametros[1] = new SqlParameter("@id_articulo", detalle.id_articulo);
            parametros[2] = new SqlParameter("@cantidad_recibida", detalle.cantidad_recibida);
            parametros[3] = new SqlParameter("@id_unidad_medida", detalle.id_unidad_medida);

            object id = conexDB.EscribirRetornar("SP_AgregarDetalleRecepcion", parametros);
            return id == null ? 0 : Convert.ToInt32(id);
        }

        public List<RecepcionBE_575_AV> ListarRecepciones()
        {
            List<RecepcionBE_575_AV> recepciones = new List<RecepcionBE_575_AV>();
            foreach (DataRow dr in conexDB.LeerTabla("SP_ExtRecepcion", null).Rows)
            {
                recepciones.Add(new RecepcionBE_575_AV
                {
                    id_recepcion = Convert.ToInt32(dr["id_recepcion"]),
                    id_orden_compra = Convert.ToInt32(dr["id_orden_compra"]),
                    remito_proveedor = dr["remito_proveedor"].ToString(),
                    fecha_recepcion = Convert.ToDateTime(dr["fecha_recepcion"]),
                    usuario_deposito = dr["usuario_deposito"].ToString(),
                    diferencias = dr["diferencias"] == DBNull.Value ? null : dr["diferencias"].ToString(),
                    dvh = dr["dvh"] == DBNull.Value ? null : dr["dvh"].ToString()
                });
            }
            return recepciones;
        }

        public List<DetalleRecepcionBE_575_AV> ListarDetalles()
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@id_recepcion", DBNull.Value);

            List<DetalleRecepcionBE_575_AV> detalles = new List<DetalleRecepcionBE_575_AV>();
            foreach (DataRow dr in conexDB.LeerTabla("SP_ExtDetalleRecepcion", parametros).Rows)
            {
                detalles.Add(new DetalleRecepcionBE_575_AV
                {
                    id_detalle_recepcion = Convert.ToInt32(dr["id_detalle_recepcion"]),
                    id_recepcion = Convert.ToInt32(dr["id_recepcion"]),
                    id_articulo = Convert.ToInt32(dr["id_articulo"]),
                    cantidad_recibida = Convert.ToDecimal(dr["cantidad_recibida"]),
                    id_unidad_medida = Convert.ToInt32(dr["id_unidad_medida"]),
                    dvh = dr["dvh"] == DBNull.Value ? null : dr["dvh"].ToString()
                });
            }
            return detalles;
        }

        public void ActualizarDVH(int id_recepcion, string dvh)
        {
            Dvh("Recepcion", id_recepcion, dvh);
        }

        public void ActualizarDVHDetalle(int id_detalle_recepcion, string dvh)
        {
            Dvh("DetalleRecepcion", id_detalle_recepcion, dvh);
        }

        private void Dvh(string tabla, int id, string dvh)
        {
            SqlParameter[] parametros = new SqlParameter[3];
            parametros[0] = new SqlParameter("@tabla", tabla);
            parametros[1] = new SqlParameter("@id", id);
            parametros[2] = new SqlParameter("@dvh", dvh);

            conexDB.Escribir("SP_ActualizarDVHNegocio", parametros);
        }
    }
}
