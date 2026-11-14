using System.Collections.Generic;
using System.Linq;

namespace Entidad_BE
{
    // A03 - Resultado de serializar o deserializar una orden: archivo, SHA-256, documento y verificaciones.
    public class ResultadoSerializacion_575_AV
    {
        public string ruta { get; set; }
        public string hash { get; set; }
        public DocumentoOrdenCompra_575_AV documento { get; set; }
        public List<ResultadoVerificacion_575_AV> verificaciones { get; set; }

        public ResultadoSerializacion_575_AV()
        {
            verificaciones = new List<ResultadoVerificacion_575_AV>();
        }

        public bool correcto
        {
            get { return verificaciones.Count > 0 && verificaciones.All(v => v.correcto); }
        }
    }
}
