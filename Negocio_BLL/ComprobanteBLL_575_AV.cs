using Acceso_DAL;
using Entidad_BE;
using Servicios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Negocio_BLL
{
    public class ComprobanteBLL_575_AV
    {
        private MP_Comprobante_575_AV mpComprobante = new MP_Comprobante_575_AV();
        private MP_OrdenPago_575_AV mpOrden = new MP_OrdenPago_575_AV();
        private MP_Cliente_575_AV mpCliente = new MP_Cliente_575_AV();
        private ArticuloBLL_575_AV articuloBLL = new ArticuloBLL_575_AV();
        private BitacoraBLL bitacora = new BitacoraBLL();

        public ComprobanteBE_575_AV ObtenerComprobante(int id_comprobante)
        {
            ComprobanteBE_575_AV comprobante = mpComprobante.ListarComprobantes()
                .FirstOrDefault(c => c.id_comprobante == id_comprobante);
            if (comprobante == null)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("COMPR_ERR_NO_EXISTE"));
            }

            Completar(new List<ComprobanteBE_575_AV> { comprobante });
            comprobante.detalle = CompletarDetalle(mpComprobante.ListarDetalle(comprobante.id_comprobante));
            comprobante.pagos = CompletarPagos(mpComprobante.ListarPagosPorComprobante(comprobante.id_comprobante));
            return comprobante;
        }

        public List<ComprobanteBE_575_AV> BuscarComprobantes(string numero, string cliente, DateTime? desde, DateTime? hasta)
        {
            List<ComprobanteBE_575_AV> comprobantes = Completar(mpComprobante.ListarComprobantes());
            string nro = (numero ?? string.Empty).Trim();
            string texto = (cliente ?? string.Empty).Trim();

            return comprobantes
                .Where(c => nro.Length == 0 || Contiene(c.nro_comprobante, nro))
                .Where(c => texto.Length == 0 || (c.cliente != null &&
                       (Contiene(c.cliente.nombre_para_mostrar, texto) || Contiene(c.cliente.dni, texto) || Contiene(c.cliente.cuit, texto))))
                .Where(c => !desde.HasValue || c.fecha_emision.Date >= desde.Value.Date)
                .Where(c => !hasta.HasValue || c.fecha_emision.Date <= hasta.Value.Date)
                .OrderByDescending(c => c.fecha_emision)
                .ToList();
        }

        public void RegistrarConsulta(string usuario)
        {
            bitacora.RegistrarBitacora(usuario, TipoAccion.ConsultaComprobante);
        }

        private List<ComprobanteBE_575_AV> Completar(List<ComprobanteBE_575_AV> comprobantes)
        {
            if (comprobantes.Count == 0) { return comprobantes; }

            Dictionary<int, ClienteBE_575_AV> clientes = mpCliente.ListarClientes().ToDictionary(c => c.id_cliente);
            Dictionary<int, OrdenPagoBE_575_AV> ordenes = mpOrden.ListarOrdenes().ToDictionary(o => o.id_orden_pago);

            foreach (ComprobanteBE_575_AV comprobante in comprobantes)
            {
                ClienteBE_575_AV cliente;
                if (clientes.TryGetValue(comprobante.id_cliente, out cliente))
                {
                    comprobante.cliente = cliente;
                    comprobante.nombre_cliente = cliente.nombre_para_mostrar;
                }
                OrdenPagoBE_575_AV orden;
                if (ordenes.TryGetValue(comprobante.id_orden_pago, out orden))
                {
                    comprobante.nro_orden_pago = orden.nro_orden_pago;
                }
            }
            return comprobantes;
        }

        private List<DetalleComprobanteBE_575_AV> CompletarDetalle(List<DetalleComprobanteBE_575_AV> detalles)
        {
            if (detalles.Count == 0) { return detalles; }

            Dictionary<int, ArticuloBE_575_AV> articulos = articuloBLL.ListarArticulos().ToDictionary(a => a.id_articulo);
            Dictionary<int, UnidadMedidaBE_575_AV> unidades = articuloBLL.ListarUnidadesMedida().ToDictionary(u => u.id_unidad_medida);

            foreach (DetalleComprobanteBE_575_AV detalle in detalles)
            {
                ArticuloBE_575_AV articulo;
                if (articulos.TryGetValue(detalle.id_articulo, out articulo))
                {
                    detalle.codigo_articulo = articulo.codigo;
                    detalle.descripcion_articulo = articulo.descripcion;
                }
                UnidadMedidaBE_575_AV unidad;
                if (unidades.TryGetValue(detalle.id_unidad_medida, out unidad))
                {
                    detalle.abreviatura_unidad = unidad.abreviatura;
                }
            }
            return detalles;
        }

        private List<PagoBE_575_AV> CompletarPagos(List<PagoBE_575_AV> pagos)
        {
            if (pagos.Count == 0) { return pagos; }

            Dictionary<int, MedioPagoBE_575_AV> medios = articuloBLL.ListarMediosPago().ToDictionary(m => m.id_medio_pago);
            foreach (PagoBE_575_AV pago in pagos)
            {
                MedioPagoBE_575_AV medio;
                if (medios.TryGetValue(pago.id_medio_pago, out medio)) { pago.nombre_medio_pago = medio.nombre; }
            }
            return pagos;
        }

        private static bool Contiene(string valor, string buscado)
        {
            return !string.IsNullOrEmpty(valor) && valor.IndexOf(buscado, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
