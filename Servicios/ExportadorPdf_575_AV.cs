using Entidad_BE;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using System;
using System.Globalization;

namespace Servicios
{
    public class ExportadorPdf_575_AV
    {
        public const string EMPRESA = "ConectAR S.R.L.";
        public const string CREADOR = "ConectAR 575_AV";

        private const string FUENTE = "Arial";
        private static readonly Color Navy = new Color(13, 43, 78);
        private static readonly Color Gris = new Color(90, 98, 110);
        private static readonly Color FondoTotal = new Color(222, 228, 236);
        private static readonly Color Borde = new Color(190, 198, 210);

        public void Exportar(ReportePdfBE_575_AV reporte, string ruta)
        {
            Document documento = ArmarDocumento(reporte);

            PdfDocumentRenderer renderizador = new PdfDocumentRenderer(true) { Document = documento };
            renderizador.RenderDocument();
            renderizador.PdfDocument.Info.Title = reporte.titulo;
            renderizador.PdfDocument.Info.Author = reporte.usuario;
            renderizador.PdfDocument.Info.Creator = CREADOR;
            renderizador.PdfDocument.Save(ruta);
        }

        private Document ArmarDocumento(ReportePdfBE_575_AV reporte)
        {
            Document documento = new Document();
            documento.Info.Title = reporte.titulo;
            documento.Info.Author = reporte.usuario;

            Style normal = documento.Styles[StyleNames.Normal];
            normal.Font.Name = FUENTE;
            normal.Font.Size = 9;

            Section seccion = documento.AddSection();
            seccion.PageSetup = documento.DefaultPageSetup.Clone();
            seccion.PageSetup.PageFormat = PageFormat.A4;
            seccion.PageSetup.TopMargin = Unit.FromCentimeter(2);
            seccion.PageSetup.BottomMargin = Unit.FromCentimeter(2);
            seccion.PageSetup.LeftMargin = Unit.FromCentimeter(2);
            seccion.PageSetup.RightMargin = Unit.FromCentimeter(2);
            seccion.PageSetup.FooterDistance = Unit.FromCentimeter(1);

            Encabezado(seccion, reporte);
            Tabla(seccion, reporte);

            if (!string.IsNullOrWhiteSpace(reporte.leyenda))
            {
                Paragraph leyenda = seccion.AddParagraph(reporte.leyenda);
                leyenda.Format.SpaceBefore = Unit.FromCentimeter(0.4);
                leyenda.Format.Font.Size = 8;
                leyenda.Format.Font.Color = Gris;
            }

            Pie(seccion, reporte.formato_pagina);
            return documento;
        }

        private static void Encabezado(Section seccion, ReportePdfBE_575_AV reporte)
        {
            Paragraph empresa = seccion.AddParagraph(EMPRESA);
            empresa.Format.Font.Size = 14;
            empresa.Format.Font.Bold = true;
            empresa.Format.Font.Color = Navy;

            Paragraph titulo = seccion.AddParagraph(reporte.titulo);
            titulo.Format.Font.Size = 12;
            titulo.Format.Font.Bold = true;
            titulo.Format.SpaceBefore = Unit.FromPoint(2);

            if (!string.IsNullOrWhiteSpace(reporte.subtitulo))
            {
                Paragraph subtitulo = seccion.AddParagraph(reporte.subtitulo);
                subtitulo.Format.SpaceBefore = Unit.FromPoint(4);
            }
            if (!string.IsNullOrWhiteSpace(reporte.filtros))
            {
                seccion.AddParagraph(reporte.filtros);
            }

            Paragraph emision = seccion.AddParagraph(string.Format("{0}: {1} \u00B7 {2}: {3}",
                reporte.etiqueta_usuario, reporte.usuario,
                reporte.etiqueta_emision, reporte.fecha_emision.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)));
            emision.Format.Font.Color = Gris;
            emision.Format.SpaceAfter = Unit.FromCentimeter(0.4);
            emision.Format.Borders.Bottom.Width = Unit.FromPoint(0.75);
            emision.Format.Borders.Bottom.Color = Navy;
            emision.Format.Borders.DistanceFromBottom = Unit.FromPoint(4);
        }

        private static void Tabla(Section seccion, ReportePdfBE_575_AV reporte)
        {
            Table tabla = seccion.AddTable();
            tabla.Borders.Width = Unit.FromPoint(0.5);
            tabla.Borders.Color = Borde;
            tabla.Format.SpaceBefore = Unit.FromPoint(2);

            foreach (ColumnaReportePdfBE_575_AV columna in reporte.columnas)
            {
                Column c = tabla.AddColumn(Unit.FromCentimeter(columna.ancho_cm));
                c.Format.Alignment = columna.alinear_derecha ? ParagraphAlignment.Right : ParagraphAlignment.Left;
            }

            Row encabezado = tabla.AddRow();
            encabezado.HeadingFormat = true;
            encabezado.Shading.Color = Navy;
            encabezado.Format.Font.Bold = true;
            encabezado.Format.Font.Color = Colors.White;
            encabezado.VerticalAlignment = VerticalAlignment.Center;
            for (int i = 0; i < reporte.columnas.Count; i++)
            {
                Celda(encabezado, i, reporte.columnas[i].titulo);
            }

            foreach (string[] fila in reporte.filas)
            {
                Row row = tabla.AddRow();
                for (int i = 0; i < reporte.columnas.Count; i++)
                {
                    Celda(row, i, i < fila.Length ? fila[i] : string.Empty);
                }
            }

            if (reporte.totales != null)
            {
                Row total = tabla.AddRow();
                total.Format.Font.Bold = true;
                total.Shading.Color = FondoTotal;
                for (int i = 0; i < reporte.columnas.Count; i++)
                {
                    Celda(total, i, i < reporte.totales.Length ? reporte.totales[i] : string.Empty);
                }
            }
        }

        private static void Celda(Row fila, int columna, string texto)
        {
            Cell celda = fila.Cells[columna];
            celda.AddParagraph(texto ?? string.Empty);
            celda.Format.LeftIndent = Unit.FromPoint(2);
            celda.Format.RightIndent = Unit.FromPoint(2);
            celda.Format.SpaceBefore = Unit.FromPoint(3);
            celda.Format.SpaceAfter = Unit.FromPoint(3);
        }

        private static void Pie(Section seccion, string formatoPagina)
        {
            Paragraph pie = seccion.Footers.Primary.AddParagraph();
            pie.Format.Alignment = ParagraphAlignment.Center;
            pie.Format.Font.Size = 8;
            pie.Format.Font.Color = Gris;

            string formato = string.IsNullOrEmpty(formatoPagina) ? "{0} / {1}" : formatoPagina;
            int pagina = formato.IndexOf("{0}", StringComparison.Ordinal);
            int total = formato.IndexOf("{1}", StringComparison.Ordinal);
            if (pagina < 0 || total < pagina)
            {
                pie.AddPageField();
                pie.AddText(" / ");
                pie.AddNumPagesField();
                return;
            }

            pie.AddText(formato.Substring(0, pagina));
            pie.AddPageField();
            pie.AddText(formato.Substring(pagina + 3, total - pagina - 3));
            pie.AddNumPagesField();
            pie.AddText(formato.Substring(total + 3));
        }
    }
}
