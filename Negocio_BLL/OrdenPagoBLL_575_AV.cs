using Acceso_DAL;
using Entidad_BE;
using Servicios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Negocio_BLL
{
    public class OrdenPagoBLL_575_AV
    {
        private const string TABLA = "OrdenPago";

        private MP_OrdenPago_575_AV mpOrden = new MP_OrdenPago_575_AV();
        private MP_Carrito_575_AV mpCarrito = new MP_Carrito_575_AV();
        private BitacoraBLL bitacora = new BitacoraBLL();
        private VerificadorIntegridadBLL verificador = new VerificadorIntegridadBLL();

        #region Consulta

        public List<OrdenPagoBE_575_AV> ListarPendientes()
        {
            return Completar(mpOrden.ListarPendientes());
        }

        public OrdenPagoBE_575_AV BuscarOrdenPago(string nro_orden_pago)
        {
            if (string.IsNullOrWhiteSpace(nro_orden_pago)) { return null; }

            OrdenPagoBE_575_AV orden = mpOrden.BuscarPorNumero(nro_orden_pago.Trim());
            if (orden == null) { return null; }

            Completar(new List<OrdenPagoBE_575_AV> { orden });
            return orden;
        }

        #endregion

        #region Emision

        public OrdenPagoBE_575_AV GenerarOrdenPago(CarritoBE_575_AV carrito,
                                                   string nombre_cliente,
                                                   string usuario)
        {
            Validar(carrito);

            CarritoBE_575_AV guardado = mpCarrito.ListarCarritos()
                .FirstOrDefault(c => c.id_carrito == carrito.id_carrito);

            if (guardado == null)
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("CAR_ERR_NO_EXISTE"));
            }
            if (guardado.estado != EstadoCarrito_575_AV.Confirmado)
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("OP_ERR_NO_CONFIRMADO"));
            }
            if (mpOrden.ListarOrdenes().Any(o => o.id_carrito == guardado.id_carrito))
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("OP_ERR_YA_EMITIDA"));
            }

            OrdenPagoBE_575_AV orden = new OrdenPagoBE_575_AV
            {
                nro_orden_pago = mpOrden.ProximoNumero(),
                id_carrito = guardado.id_carrito,
                nombre_cliente = string.IsNullOrWhiteSpace(nombre_cliente)
                    ? guardado.nombre_cliente
                    : nombre_cliente,
                fecha_emision = DateTime.Now,
                precio_total = guardado.precio_total,
                estado = EstadoOrdenPago_575_AV.Pendiente,
                activo = true
            };

            orden.id_orden_pago = mpOrden.CrearOrdenPago(orden);

            OrdenPagoBE_575_AV emitida = mpOrden.ListarOrdenes()
                .FirstOrDefault(o => o.id_orden_pago == orden.id_orden_pago) ?? orden;

            PersistirDigito(emitida);
            bitacora.RegistrarBitacora(usuario, TipoAccion.EmisionOrdenPago);

            emitida.nro_carrito = guardado.nro_carrito;
            return emitida;
        }

        #endregion

        #region Integridad

        public bool VerificarIntegridad()
        {
            List<OrdenPagoBE_575_AV> ordenes = mpOrden.ListarOrdenes()
                .OrderBy(o => o.id_orden_pago)
                .ToList();

            return verificador.VerificarIntegridad(ordenes, TABLA);
        }

        public void RecalcularDV()
        {
            List<OrdenPagoBE_575_AV> ordenes = mpOrden.ListarOrdenes()
                .OrderBy(o => o.id_orden_pago)
                .ToList();

            foreach (OrdenPagoBE_575_AV orden in ordenes)
            {
                orden.dvh = VerificadorIntegridad.CalcularDVH(orden);
                mpOrden.ActualizarDVH(orden.id_orden_pago, orden.dvh);
            }

            verificador.ActualizarDVV(TABLA);
        }

        private void PersistirDigito(OrdenPagoBE_575_AV orden)
        {
            orden.dvh = VerificadorIntegridad.CalcularDVH(orden);
            mpOrden.ActualizarDVH(orden.id_orden_pago, orden.dvh);
            verificador.ActualizarDVV(TABLA);
        }

        #endregion

        #region Apoyo

        private List<OrdenPagoBE_575_AV> Completar(List<OrdenPagoBE_575_AV> ordenes)
        {
            if (ordenes.Count == 0) { return ordenes; }

            Dictionary<int, CarritoBE_575_AV> carritos =
                mpCarrito.ListarCarritos().ToDictionary(c => c.id_carrito);

            foreach (OrdenPagoBE_575_AV orden in ordenes)
            {
                CarritoBE_575_AV carrito;
                if (carritos.TryGetValue(orden.id_carrito, out carrito))
                {
                    orden.nro_carrito = carrito.nro_carrito;
                }
            }

            return ordenes;
        }

        private void Validar(CarritoBE_575_AV carrito)
        {
            if (carrito == null) { throw new ArgumentNullException("carrito"); }

            if (carrito.id_carrito <= 0)
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("CAR_ERR_NO_IDENTIFICADO"));
            }
        }

        #endregion
    }
}
