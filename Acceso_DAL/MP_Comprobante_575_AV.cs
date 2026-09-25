using Entidad_BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Acceso_DAL
{
    /// <summary>
    /// Mapeador del agregado Comprobante. Atiende tres tablas, asi que cada una
    /// tiene su propio ActualizarDVH: Comprobante, DetalleComprobante y Pago
    /// llevan cada cual su fila en DigitoVertical.
    /// </summary>
    public class MP_Comprobante_575_AV
    {
        AccesoDatos conexDB = new AccesoDatos();

        #region Consulta

        /// <summary>Numero siguiente para el tipo de comprobante indicado.</summary>
        public string ProximoNumero(string tipo_comprobante)
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@tipo_comprobante", tipo_comprobante);

            object numero = conexDB.ExtraerDato("SP_ProximoNumeroComprobante", parametros);
            return numero == null ? tipo_comprobante + "-0001-00000001" : numero.ToString();
        }

        /// <summary>Devuelve nulo si no existe.</summary>
        public ComprobanteBE_575_AV BuscarPorNumero(string nro_comprobante)
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@nro_comprobante", nro_comprobante);

            return Proyectar(conexDB.LeerTabla("SP_BuscarComprobante", parametros))
                .FirstOrDefault();
        }

        /// <summary>Todos los comprobantes, para el control de integridad.</summary>
        public List<ComprobanteBE_575_AV> ListarComprobantes()
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@nro_comprobante", DBNull.Value);

            return Proyectar(conexDB.LeerTabla("SP_BuscarComprobante", parametros));
        }

        public List<DetalleComprobanteBE_575_AV> ListarDetalle(int id_comprobante)
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@id_comprobante", id_comprobante);

            return ProyectarDetalle(conexDB.LeerTabla("SP_ExtDetalleComprobante", parametros));
        }

        /// <summary>Todos los renglones, para el control de integridad.</summary>
        public List<DetalleComprobanteBE_575_AV> ListarDetalles()
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@id_comprobante", DBNull.Value);

            return ProyectarDetalle(conexDB.LeerTabla("SP_ExtDetalleComprobante", parametros));
        }

        /// <summary>Todos los pagos, para el control de integridad.</summary>
        public List<PagoBE_575_AV> ListarPagos()
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@id_comprobante", DBNull.Value);

            return ProyectarPago(conexDB.LeerTabla("SP_ExtPago", parametros));
        }

        public List<PagoBE_575_AV> ListarPagosPorComprobante(int id_comprobante)
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@id_comprobante", id_comprobante);

            return ProyectarPago(conexDB.LeerTabla("SP_ExtPago", parametros));
        }

        #endregion

        #region Escritura

        public int CrearComprobante(ComprobanteBE_575_AV comprobante)
        {
            SqlParameter[] parametros = new SqlParameter[8];
            parametros[0] = new SqlParameter("@nro_comprobante", comprobante.nro_comprobante);
            parametros[1] = new SqlParameter("@tipo_comprobante", comprobante.tipo_comprobante);
            parametros[2] = new SqlParameter("@id_orden_pago", comprobante.id_orden_pago);
            parametros[3] = new SqlParameter("@id_cliente", comprobante.id_cliente);
            parametros[4] = new SqlParameter("@fecha_emision", comprobante.fecha_emision);
            parametros[5] = new SqlParameter("@neto", comprobante.neto);
            parametros[6] = new SqlParameter("@iva", comprobante.iva);
            parametros[7] = new SqlParameter("@precio_total", comprobante.precio_total);

            object id = conexDB.EscribirRetornar("SP_CrearComprobante", parametros);
            return id == null ? 0 : Convert.ToInt32(id);
        }

        public int AgregarDetalle(DetalleComprobanteBE_575_AV detalle)
        {
            SqlParameter[] parametros = new SqlParameter[5];
            parametros[0] = new SqlParameter("@id_comprobante", detalle.id_comprobante);
            parametros[1] = new SqlParameter("@id_articulo", detalle.id_articulo);
            parametros[2] = new SqlParameter("@cantidad", detalle.cantidad);
            parametros[3] = new SqlParameter("@id_unidad_medida", detalle.id_unidad_medida);
            parametros[4] = new SqlParameter("@precio_unitario", detalle.precio_unitario);

            object id = conexDB.EscribirRetornar("SP_AgregarDetalleComprobante", parametros);
            return id == null ? 0 : Convert.ToInt32(id);
        }

        public int RegistrarPago(PagoBE_575_AV pago)
        {
            SqlParameter[] parametros = new SqlParameter[6];
            parametros[0] = new SqlParameter("@id_comprobante", pago.id_comprobante);
            parametros[1] = new SqlParameter("@id_medio_pago", pago.id_medio_pago);
            parametros[2] = new SqlParameter("@monto", pago.monto);
            parametros[3] = new SqlParameter("@fecha_hora", pago.fecha_hora);
            parametros[4] = new SqlParameter("@codigo_autorizacion",
                string.IsNullOrWhiteSpace(pago.codigo_autorizacion)
                    ? (object)DBNull.Value : pago.codigo_autorizacion);
            parametros[5] = new SqlParameter("@estado", pago.estado.ToString());

            object id = conexDB.EscribirRetornar("SP_RegistrarPago", parametros);
            return id == null ? 0 : Convert.ToInt32(id);
        }

        public void ActualizarEstado(int id_comprobante, EstadoComprobante_575_AV estado)
        {
            SqlParameter[] parametros = new SqlParameter[2];
            parametros[0] = new SqlParameter("@id_comprobante", id_comprobante);
            parametros[1] = new SqlParameter("@estado", estado.ToString());

            conexDB.Escribir("SP_ActualizarEstadoComprobante", parametros);
        }

        #endregion

        #region Digito verificador

        public void ActualizarDVH(int id_comprobante, string dvh)
        {
            ActualizarDVH("Comprobante", id_comprobante, dvh);
        }

        public void ActualizarDVHDetalle(int id_detalle_comprobante, string dvh)
        {
            ActualizarDVH("DetalleComprobante", id_detalle_comprobante, dvh);
        }

        public void ActualizarDVHPago(int id_pago, string dvh)
        {
            ActualizarDVH("Pago", id_pago, dvh);
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

        private List<ComprobanteBE_575_AV> Proyectar(DataTable dt)
        {
            List<ComprobanteBE_575_AV> comprobantes = new List<ComprobanteBE_575_AV>();
            foreach (DataRow dr in dt.Rows)
            {
                EstadoComprobante_575_AV estado;
                if (!Enum.TryParse(dr["estado"].ToString(), out estado))
                {
                    estado = EstadoComprobante_575_AV.Emitido;
                }

                comprobantes.Add(new ComprobanteBE_575_AV
                {
                    id_comprobante = Convert.ToInt32(dr["id_comprobante"]),
                    nro_comprobante = dr["nro_comprobante"].ToString(),
                    tipo_comprobante = dr["tipo_comprobante"].ToString(),
                    id_orden_pago = Convert.ToInt32(dr["id_orden_pago"]),
                    id_cliente = Convert.ToInt32(dr["id_cliente"]),
                    fecha_emision = Convert.ToDateTime(dr["fecha_emision"]),
                    neto = Convert.ToDecimal(dr["neto"]),
                    iva = Convert.ToDecimal(dr["iva"]),
                    precio_total = Convert.ToDecimal(dr["precio_total"]),
                    estado = estado,
                    dvh = dr["dvh"] == DBNull.Value ? null : dr["dvh"].ToString()
                });
            }
            return comprobantes;
        }

        private List<DetalleComprobanteBE_575_AV> ProyectarDetalle(DataTable dt)
        {
            List<DetalleComprobanteBE_575_AV> detalles = new List<DetalleComprobanteBE_575_AV>();
            foreach (DataRow dr in dt.Rows)
            {
                detalles.Add(new DetalleComprobanteBE_575_AV
                {
                    id_detalle_comprobante = Convert.ToInt32(dr["id_detalle_comprobante"]),
                    id_comprobante = Convert.ToInt32(dr["id_comprobante"]),
                    id_articulo = Convert.ToInt32(dr["id_articulo"]),
                    cantidad = Convert.ToDecimal(dr["cantidad"]),
                    id_unidad_medida = Convert.ToInt32(dr["id_unidad_medida"]),
                    precio_unitario = Convert.ToDecimal(dr["precio_unitario"]),
                    dvh = dr["dvh"] == DBNull.Value ? null : dr["dvh"].ToString()
                });
            }
            return detalles;
        }

        private List<PagoBE_575_AV> ProyectarPago(DataTable dt)
        {
            List<PagoBE_575_AV> pagos = new List<PagoBE_575_AV>();
            foreach (DataRow dr in dt.Rows)
            {
                EstadoPago_575_AV estado;
                if (!Enum.TryParse(dr["estado"].ToString(), out estado))
                {
                    estado = EstadoPago_575_AV.Acreditado;
                }

                pagos.Add(new PagoBE_575_AV
                {
                    id_pago = Convert.ToInt32(dr["id_pago"]),
                    id_comprobante = Convert.ToInt32(dr["id_comprobante"]),
                    id_medio_pago = Convert.ToInt32(dr["id_medio_pago"]),
                    monto = Convert.ToDecimal(dr["monto"]),
                    fecha_hora = Convert.ToDateTime(dr["fecha_hora"]),
                    codigo_autorizacion = dr["codigo_autorizacion"] == DBNull.Value
                        ? null : dr["codigo_autorizacion"].ToString(),
                    estado = estado,
                    dvh = dr["dvh"] == DBNull.Value ? null : dr["dvh"].ToString()
                });
            }
            return pagos;
        }

        #endregion
    }
}
