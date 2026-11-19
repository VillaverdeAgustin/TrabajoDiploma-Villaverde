using System;
using System.Collections.Generic;

namespace Entidad_BE
{
    public class ReportePdfBE_575_AV
    {
        public string titulo { get; set; }
        public string subtitulo { get; set; }
        public string filtros { get; set; }
        public string usuario { get; set; }
        public DateTime fecha_emision { get; set; }
        public string etiqueta_usuario { get; set; }
        public string etiqueta_emision { get; set; }
        public string formato_pagina { get; set; }
        public List<ColumnaReportePdfBE_575_AV> columnas { get; set; }
        public List<string[]> filas { get; set; }
        public string[] totales { get; set; }
        public string leyenda { get; set; }

        public ReportePdfBE_575_AV()
        {
            columnas = new List<ColumnaReportePdfBE_575_AV>();
            filas = new List<string[]>();
        }
    }
}
