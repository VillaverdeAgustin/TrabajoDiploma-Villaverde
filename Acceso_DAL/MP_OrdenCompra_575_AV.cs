using Entidad_BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Acceso_DAL
{
    public class MP_OrdenCompra_575_AV
    {
        AccesoDatos conexDB = new AccesoDatos();

        public string ProximoNumero()
        {
            object numero = conexDB.ExtraerDato("SP_ProximoNumeroOrdenCompra", null);
            return numero == null ? "OC-000001" : numero.ToString();
        }

        public OrdenCompraBE_575_AV BuscarBorrador(int id_proveedor)
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@id_proveedor", id_proveedor);

            return Proyectar(conexDB.LeerTabla("SP_BuscarOrdenCompraBorrador", parametros)).FirstOrDefault();
        }

        public int CrearOrdenCompra(OrdenCompraBE_575_AV orden)
        {
            SqlParameter[] parametros = new SqlParameter[2];
            parametros[0] = new SqlParameter("@id_proveedor", orden.id_proveedor);
            parametros[1] = new SqlParameter("@usuario_emisor", orden.usuario_emisor);

            object id = conexDB.EscribirRetornar("SP_CrearOrdenCompra", parametros);
            return id == null ? 0 : Convert.ToInt32(id);
        }

        public int AgregarDetalle(DetalleOrdenCompraBE_575_AV detalle)
        {
            SqlParameter[] parametros = new SqlParameter[6];
            parametros[0] = new SqlParameter("@id_orden_compra", detalle.id_orden_compra);
            parametros[1] = new SqlParameter("@id_articulo", detalle.id_articulo);
            parametros[2] = new SqlParameter("@cantidad", detalle.cantidad);
            parametros[3] = new SqlParameter("@id_unidad_medida", detalle.id_unidad_medida);
            parametros[4] = new SqlParameter("@precio_unitario", detalle.precio_unitario);
            parametros[5] = new SqlParameter("@subtotal", detalle.subtotal);

            object id = conexDB.EscribirRetornar("SP_AgregarDetalleOrdenCompra", parametros);
            return id == null ? 0 : Convert.ToInt32(id);
        }

        public void ActualizarTotal(int id_orden_compra, decimal total)
        {
            SqlParameter[] parametros = new SqlParameter[2];
            parametros[0] = new SqlParameter("@id_orden_compra", id_orden_compra);
            parametros[1] = new SqlParameter("@total", total);

            conexDB.Escribir("SP_ActualizarTotalOrdenCompra", parametros);
        }

        public void Emitir(int id_orden_compra, string nro_orden_compra, decimal total)
        {
            SqlParameter[] parametros = new SqlParameter[3];
            parametros[0] = new SqlParameter("@id_orden_compra", id_orden_compra);
            parametros[1] = new SqlParameter("@nro_orden_compra", nro_orden_compra);
            parametros[2] = new SqlParameter("@total", total);

            conexDB.Escribir("SP_EmitirOrdenCompra", parametros);
        }

        public List<OrdenCompraBE_575_AV> ListarPorEstado(EstadoOrdenCompra_575_AV[] estados)
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@estados",
                estados == null || estados.Length == 0
                    ? (object)DBNull.Value
                    : string.Join(",", estados.Select(e => e.ToString())));

            return Proyectar(conexDB.LeerTabla("SP_ExtOrdenCompraPorEstado", parametros));
        }

        public List<DetalleOrdenCompraBE_575_AV> ListarDetalle(int id_orden_compra)
        {
            return Detalles(id_orden_compra);
        }

        public List<DetalleOrdenCompraBE_575_AV> ListarDetalles()
        {
            return Detalles(null);
        }

        // A02 - Ordenes emitidas para el reporte de compras, con lo pedido, lo recibido y el monto por moneda
        public List<OrdenReporteCompra_575_AV> ListarParaReporte(DateTime desde, DateTime hasta, int? id_proveedor, EstadoOrdenCompra_575_AV? estado)
        {
            SqlParameter[] parametros = new SqlParameter[4];
            parametros[0] = new SqlParameter("@desde", SqlDbType.Date) { Value = desde.Date };
            parametros[1] = new SqlParameter("@hasta", SqlDbType.Date) { Value = hasta.Date };
            parametros[2] = new SqlParameter("@id_proveedor", id_proveedor.HasValue ? (object)id_proveedor.Value : DBNull.Value);
            parametros[3] = new SqlParameter("@estado", estado.HasValue ? (object)estado.Value.ToString() : DBNull.Value);

            List<OrdenReporteCompra_575_AV> ordenes = new List<OrdenReporteCompra_575_AV>();
            foreach (DataRow dr in conexDB.LeerTabla("SP_ReporteCompras", parametros).Rows)
            {
                EstadoOrdenCompra_575_AV estadoOrden;
                Enum.TryParse(dr["estado"].ToString(), out estadoOrden);

                ordenes.Add(new OrdenReporteCompra_575_AV
                {
                    id_orden_compra = Convert.ToInt32(dr["id_orden_compra"]),
                    nro_orden_compra = Leer(dr["nro_orden_compra"]),
                    id_proveedor = Convert.ToInt32(dr["id_proveedor"]),
                    razon_social = dr["razon_social"].ToString(),
                    fecha_emision = Convert.ToDateTime(dr["fecha_emision"]),
                    estado = estadoOrden,
                    total = Convert.ToDecimal(dr["total"]),
                    renglones = Convert.ToInt32(dr["renglones"]),
                    cantidad_solicitada = Convert.ToDecimal(dr["cantidad_solicitada"]),
                    cantidad_recibida = Convert.ToDecimal(dr["cantidad_recibida"]),
                    monto_ars = Convert.ToDecimal(dr["monto_ars"]),
                    monto_usd = Convert.ToDecimal(dr["monto_usd"])
                });
            }
            return ordenes;
        }

        public void RegistrarAutorizacion(OrdenCompraBE_575_AV orden)
        {
            SqlParameter[] parametros = new SqlParameter[4];
            parametros[0] = new SqlParameter("@id_orden_compra", orden.id_orden_compra);
            parametros[1] = new SqlParameter("@estado", orden.estado.ToString());
            parametros[2] = new SqlParameter("@usuario_autorizante", orden.usuario_autorizante);
            parametros[3] = new SqlParameter("@observaciones",
                string.IsNullOrWhiteSpace(orden.observaciones) ? (object)DBNull.Value : orden.observaciones);

            conexDB.Escribir("SP_RegistrarAutorizacionOrdenCompra", parametros);
        }

        public void ActualizarCantidadRecibida(int id_detalle_orden_compra, decimal cantidad)
        {
            SqlParameter[] parametros = new SqlParameter[2];
            parametros[0] = new SqlParameter("@id_detalle_orden_compra", id_detalle_orden_compra);
            parametros[1] = new SqlParameter("@cantidad_recibida", cantidad);

            conexDB.Escribir("SP_ActualizarRecibidoOrdenCompra", parametros);
        }

        public void ActualizarEstado(int id_orden_compra, EstadoOrdenCompra_575_AV estado)
        {
            SqlParameter[] parametros = new SqlParameter[2];
            parametros[0] = new SqlParameter("@id_orden_compra", id_orden_compra);
            parametros[1] = new SqlParameter("@estado", estado.ToString());

            conexDB.Escribir("SP_ActualizarEstadoOrdenCompra", parametros);
        }

        public void ActualizarDVH(int id_orden_compra, string dvh)
        {
            Dvh("OrdenCompra", id_orden_compra, dvh);
        }

        public void ActualizarDVHDetalle(int id_detalle_orden_compra, string dvh)
        {
            Dvh("DetalleOrdenCompra", id_detalle_orden_compra, dvh);
        }

        private void Dvh(string tabla, int id, string dvh)
        {
            SqlParameter[] parametros = new SqlParameter[3];
            parametros[0] = new SqlParameter("@tabla", tabla);
            parametros[1] = new SqlParameter("@id", id);
            parametros[2] = new SqlParameter("@dvh", dvh);

            conexDB.Escribir("SP_ActualizarDVHNegocio", parametros);
        }

        private List<DetalleOrdenCompraBE_575_AV> Detalles(int? id_orden_compra)
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@id_orden_compra",
                id_orden_compra.HasValue ? (object)id_orden_compra.Value : DBNull.Value);

            List<DetalleOrdenCompraBE_575_AV> detalles = new List<DetalleOrdenCompraBE_575_AV>();
            foreach (DataRow dr in conexDB.LeerTabla("SP_ExtDetalleOrdenCompra", parametros).Rows)
            {
                detalles.Add(new DetalleOrdenCompraBE_575_AV
                {
                    id_detalle_orden_compra = Convert.ToInt32(dr["id_detalle_orden_compra"]),
                    id_orden_compra = Convert.ToInt32(dr["id_orden_compra"]),
                    id_articulo = Convert.ToInt32(dr["id_articulo"]),
                    cantidad = Convert.ToDecimal(dr["cantidad"]),
                    id_unidad_medida = Convert.ToInt32(dr["id_unidad_medida"]),
                    precio_unitario = Convert.ToDecimal(dr["precio_unitario"]),
                    subtotal = Convert.ToDecimal(dr["subtotal"]),
                    cantidad_recibida = Convert.ToDecimal(dr["cantidad_recibida"]),
                    dvh = Leer(dr["dvh"])
                });
            }
            return detalles;
        }

        private static List<OrdenCompraBE_575_AV> Proyectar(DataTable dt)
        {
            List<OrdenCompraBE_575_AV> ordenes = new List<OrdenCompraBE_575_AV>();
            foreach (DataRow dr in dt.Rows)
            {
                EstadoOrdenCompra_575_AV estado;
                Enum.TryParse(dr["estado"].ToString(), out estado);

                ordenes.Add(new OrdenCompraBE_575_AV
                {
                    id_orden_compra = Convert.ToInt32(dr["id_orden_compra"]),
                    nro_orden_compra = Leer(dr["nro_orden_compra"]),
                    id_proveedor = Convert.ToInt32(dr["id_proveedor"]),
                    fecha_emision = dr["fecha_emision"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["fecha_emision"]),
                    usuario_emisor = dr["usuario_emisor"].ToString(),
                    estado = estado,
                    total = Convert.ToDecimal(dr["total"]),
                    fecha_autorizacion = dr["fecha_autorizacion"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["fecha_autorizacion"]),
                    usuario_autorizante = Leer(dr["usuario_autorizante"]),
                    observaciones = Leer(dr["observaciones"]),
                    activo = Convert.ToBoolean(dr["activo"]),
                    dvh = Leer(dr["dvh"]),
                    razon_social = Leer(dr["razon_social"])
                });
            }
            return ordenes;
        }

        private static string Leer(object valor)
        {
            return valor == DBNull.Value ? null : valor.ToString();
        }
    }
}
