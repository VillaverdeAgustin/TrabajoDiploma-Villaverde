using System;
using System.Globalization;

namespace Entidad_BE
{
    public class ListaPrecioBE_575_AV : IVerificable
    {
        public int id_lista_precio { get; set; }
        public int id_proveedor { get; set; }
        public string archivo { get; set; }
        public DateTime fecha_vigencia { get; set; }
        public string moneda { get; set; }
        public DateTime fecha_importacion { get; set; }
        public string usuario_importacion { get; set; }
        public bool activo { get; set; }
        public string dvh { get; set; }

        public string digito { get { return dvh; } }

        public string ObtenerCamposDV()
        {
            return string.Join("|",
                id_lista_precio.ToString(CultureInfo.InvariantCulture),
                id_proveedor.ToString(CultureInfo.InvariantCulture),
                archivo,
                fecha_vigencia.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                moneda,
                fecha_importacion.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                usuario_importacion,
                activo);
        }
    }
}
