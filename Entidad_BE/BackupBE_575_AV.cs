using System;
using System.IO;

namespace Entidad_BE
{
    // T07 - Un backup de ConectAR_DB: el que se acaba de generar, uno registrado en msdb o el que se eligio para restaurar.
    // La ruta es la del servidor de SQL Server, que es quien escribe y lee el archivo.
    public class BackupBE_575_AV
    {
        public string ruta { get; set; }
        public DateTime fecha { get; set; }
        public long tamano { get; set; }
        public string base_datos { get; set; }
        public bool verificado { get; set; }

        public string archivo
        {
            get { return string.IsNullOrEmpty(ruta) ? string.Empty : Path.GetFileName(ruta); }
        }

        public decimal tamano_mb
        {
            get { return decimal.Round(tamano / 1048576m, 2); }
        }
    }
}
