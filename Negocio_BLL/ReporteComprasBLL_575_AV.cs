using Acceso_DAL;
using Entidad_BE;
using Servicios;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;

namespace Negocio_BLL
{
    // A02 - Reporte de compras por proveedor. Los datos los lee SP_ReporteCompras; aca se arma el reporte.
    public class ReporteComprasBLL_575_AV
    {
        // El porcentaje recibido se mide sobre las ordenes que ya se pueden recibir
        private static readonly EstadoOrdenCompra_575_AV[] EstadosConRecepcion =
        {
            EstadoOrdenCompra_575_AV.Emitida,
            EstadoOrdenCompra_575_AV.RecibidaParcial,
            EstadoOrdenCompra_575_AV.Recibida
        };

        private MP_OrdenCompra_575_AV mpOrden = new MP_OrdenCompra_575_AV();
        private ProveedorBLL_575_AV proveedorBLL = new ProveedorBLL_575_AV();
        private BitacoraBLL bitacora = new BitacoraBLL();
        private ExportadorPdf_575_AV exportador = new ExportadorPdf_575_AV();

        public List<ProveedorBE_575_AV> ListarProveedores()
        {
            return proveedorBLL.ListarProveedores().OrderBy(p => p.razon_social).ToList();
        }

        // Los estados por los que se puede filtrar: un borrador todavia no tiene fecha de emision
        public List<EstadoOrdenCompra_575_AV> ListarEstados()
        {
            return Enum.GetValues(typeof(EstadoOrdenCompra_575_AV)).Cast<EstadoOrdenCompra_575_AV>()
                .Where(e => e != EstadoOrdenCompra_575_AV.Borrador)
                .ToList();
        }

        public ReporteComprasBE_575_AV GenerarReporte(DateTime desde, DateTime hasta, int? id_proveedor,
                                                       EstadoOrdenCompra_575_AV? estado, string usuario)
        {
            if (desde.Date > hasta.Date)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("REPC_ERR_RANGO"));
            }
            if (estado == EstadoOrdenCompra_575_AV.Borrador)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("REPC_ERR_ESTADO"));
            }

            List<OrdenReporteCompra_575_AV> ordenes = mpOrden.ListarParaReporte(desde, hasta,
                id_proveedor.HasValue && id_proveedor.Value > 0 ? id_proveedor : null, estado);

            foreach (OrdenReporteCompra_575_AV orden in ordenes)
            {
                orden.porcentaje_recibido = Porcentaje(new[] { orden });
            }

            ReporteComprasBE_575_AV reporte = new ReporteComprasBE_575_AV
            {
                desde = desde.Date,
                hasta = hasta.Date,
                id_proveedor = id_proveedor,
                estado = estado
            };

            reporte.proveedores = ordenes
                .GroupBy(o => o.id_proveedor)
                .Select(g => Resumir(g.ToList(), false))
                .OrderBy(r => r.razon_social)
                .ToList();
            reporte.total = Resumir(ordenes, true);

            bitacora.RegistrarBitacora(usuario, TipoAccion.ConsultaReporteCompras);
            return reporte;
        }

        public ReportePdfBE_575_AV ExportarPdf(ReporteComprasBE_575_AV reporte, string ruta, string usuario)
        {
            if (reporte == null || reporte.total == null || reporte.total.cantidad_ordenes == 0)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("REPC_ERR_PDF_SIN_DATOS"));
            }
            if (string.IsNullOrWhiteSpace(ruta))
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("REPC_ERR_PDF_ESCRITURA"));
            }

            ReportePdfBE_575_AV documento = ArmarDocumentoPdf(reporte, usuario);
            try
            {
                exportador.Exportar(documento, ruta);
            }
            catch (IOException ex) when (ArchivoEnUso(ex))
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("REPC_ERR_PDF_EN_USO"), ex);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is ArgumentException
                                       || ex is NotSupportedException || ex is SecurityException)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("REPC_ERR_PDF_ESCRITURA"), ex);
            }

            bitacora.RegistrarBitacora(usuario, TipoAccion.ExportacionReporteCompras);
            return documento;
        }

        private ReportePdfBE_575_AV ArmarDocumentoPdf(ReporteComprasBE_575_AV reporte, string usuario)
        {
            GestorDeIdioma idioma = GestorDeIdioma.GetInstance;
            string todos = idioma.Traducir("COMUN_TODOS");

            string proveedor = todos;
            if (reporte.id_proveedor.HasValue && reporte.id_proveedor.Value > 0)
            {
                ResumenProveedorCompra_575_AV resumen = reporte.proveedores.FirstOrDefault(p => p.id_proveedor == reporte.id_proveedor.Value);
                proveedor = resumen != null ? resumen.razon_social
                    : ListarProveedores().Where(p => p.id_proveedor == reporte.id_proveedor.Value).Select(p => p.razon_social).FirstOrDefault() ?? todos;
            }
            string estado = reporte.estado.HasValue
                ? idioma.Traducir("OC_EST_" + reporte.estado.Value.ToString().ToUpperInvariant())
                : todos;

            ReportePdfBE_575_AV documento = new ReportePdfBE_575_AV
            {
                titulo = idioma.Traducir("REPC_TITULO"),
                subtitulo = string.Format(idioma.Traducir("REPC_PDF_PERIODO"),
                    reporte.desde.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    reporte.hasta.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)),
                filtros = string.Format(idioma.Traducir("REPC_PDF_FILTROS"), proveedor, estado),
                usuario = usuario,
                fecha_emision = DateTime.Now,
                etiqueta_usuario = idioma.Traducir("REPC_PDF_USUARIO"),
                etiqueta_emision = idioma.Traducir("REPC_PDF_EMISION"),
                formato_pagina = idioma.Traducir("REPC_PDF_PAGINA"),
                leyenda = idioma.Traducir("REPC_LBL_LEYENDA")
            };

            documento.columnas.Add(new ColumnaReportePdfBE_575_AV { titulo = idioma.Traducir("REPC_COL_PROVEEDOR"), ancho_cm = 5.6 });
            documento.columnas.Add(new ColumnaReportePdfBE_575_AV { titulo = idioma.Traducir("REPC_COL_ORDENES"), ancho_cm = 1.7, alinear_derecha = true });
            documento.columnas.Add(new ColumnaReportePdfBE_575_AV { titulo = idioma.Traducir("REPC_COL_MONTO_ARS"), ancho_cm = 2.9, alinear_derecha = true });
            documento.columnas.Add(new ColumnaReportePdfBE_575_AV { titulo = idioma.Traducir("REPC_COL_MONTO_USD"), ancho_cm = 2.5, alinear_derecha = true });
            documento.columnas.Add(new ColumnaReportePdfBE_575_AV { titulo = idioma.Traducir("REPC_COL_RECIBIDO"), ancho_cm = 2.0, alinear_derecha = true });
            documento.columnas.Add(new ColumnaReportePdfBE_575_AV { titulo = idioma.Traducir("REPC_COL_PENDIENTES"), ancho_cm = 2.3, alinear_derecha = true });

            foreach (ResumenProveedorCompra_575_AV fila in reporte.proveedores)
            {
                documento.filas.Add(FilaPdf(fila, fila.razon_social));
            }
            documento.totales = FilaPdf(reporte.total, idioma.Traducir("COMUN_TOTAL"));

            return documento;
        }

        private static string[] FilaPdf(ResumenProveedorCompra_575_AV fila, string nombre)
        {
            CultureInfo cultura = CultureInfo.CurrentCulture;
            return new[]
            {
                nombre,
                fila.cantidad_ordenes.ToString("N0", cultura),
                fila.monto_ars.ToString("N2", cultura),
                fila.monto_usd.ToString("N2", cultura),
                fila.porcentaje_recibido.HasValue ? fila.porcentaje_recibido.Value.ToString("N1", cultura) + " %" : "—",
                fila.pendientes_recepcion.ToString("N0", cultura)
            };
        }

        private static bool ArchivoEnUso(IOException ex)
        {
            int codigo = ex.HResult & 0xFFFF;
            return codigo == 32 || codigo == 33;
        }

        private static ResumenProveedorCompra_575_AV Resumir(List<OrdenReporteCompra_575_AV> ordenes, bool esTotal)
        {
            List<OrdenReporteCompra_575_AV> conRecepcion = ordenes.Where(o => EstadosConRecepcion.Contains(o.estado)).ToList();
            OrdenReporteCompra_575_AV primera = ordenes.FirstOrDefault();

            return new ResumenProveedorCompra_575_AV
            {
                id_proveedor = esTotal || primera == null ? 0 : primera.id_proveedor,
                razon_social = esTotal || primera == null ? null : primera.razon_social,
                cantidad_ordenes = ordenes.Count,
                monto_ars = ordenes.Sum(o => o.monto_ars),
                monto_usd = ordenes.Sum(o => o.monto_usd),
                cantidad_solicitada = conRecepcion.Sum(o => o.cantidad_solicitada),
                cantidad_recibida = conRecepcion.Sum(o => o.cantidad_recibida),
                porcentaje_recibido = Porcentaje(conRecepcion),
                pendientes_recepcion = ordenes.Count(o => o.estado == EstadoOrdenCompra_575_AV.Emitida),
                es_total = esTotal,
                ordenes = ordenes.OrderBy(o => o.fecha_emision).ThenBy(o => o.id_orden_compra).ToList()
            };
        }

        // Cantidad recibida sobre solicitada de las ordenes emitidas o recibidas; sin ninguna, no hay porcentaje
        private static decimal? Porcentaje(IEnumerable<OrdenReporteCompra_575_AV> ordenes)
        {
            List<OrdenReporteCompra_575_AV> medibles = ordenes.Where(o => EstadosConRecepcion.Contains(o.estado)).ToList();
            decimal solicitada = medibles.Sum(o => o.cantidad_solicitada);
            if (solicitada <= 0) { return null; }

            return decimal.Round(medibles.Sum(o => o.cantidad_recibida) * 100m / solicitada, 1);
        }
    }
}
