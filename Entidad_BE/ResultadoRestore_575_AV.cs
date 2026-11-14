using System.Collections.Generic;

namespace Entidad_BE
{
    // T07 - Resultado de un restore: el backup aplicado y las tablas que siguen con error despues de recalcular los DV.
    public class ResultadoRestore_575_AV
    {
        public BackupBE_575_AV backup { get; set; }
        public List<string> tablas_con_error { get; set; }

        public ResultadoRestore_575_AV()
        {
            tablas_con_error = new List<string>();
        }

        public bool integridad_ok
        {
            get { return tablas_con_error.Count == 0; }
        }
    }
}
