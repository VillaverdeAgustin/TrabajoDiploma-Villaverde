using System.Globalization;

namespace Entidad_BE
{
    public class DetalleRecepcionBE_575_AV : IVerificable
    {
        public int id_detalle_recepcion { get; set; }
        public int id_recepcion { get; set; }
        public int id_articulo { get; set; }
        public decimal cantidad_recibida { get; set; }
        public int id_unidad_medida { get; set; }
        public string dvh { get; set; }

        public string digito { get { return dvh; } }

        public string ObtenerCamposDV()
        {
            return string.Join("|",
                id_detalle_recepcion.ToString(CultureInfo.InvariantCulture),
                id_recepcion.ToString(CultureInfo.InvariantCulture),
                id_articulo.ToString(CultureInfo.InvariantCulture),
                cantidad_recibida.ToString("F2", CultureInfo.InvariantCulture),
                id_unidad_medida.ToString(CultureInfo.InvariantCulture));
        }
    }
}
