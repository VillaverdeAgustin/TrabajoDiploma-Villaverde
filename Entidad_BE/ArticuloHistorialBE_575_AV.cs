using System;
using System.Globalization;

namespace Entidad_BE
{
    public class ArticuloHistorialBE_575_AV : IVerificable
    {
        public int id_historial { get; set; }
        public int id_cambio { get; set; }
        public int id_articulo { get; set; }
        public string imagen { get; set; }
        public string operacion { get; set; }
        public string codigo { get; set; }
        public string descripcion { get; set; }
        public int id_categoria { get; set; }
        public int id_marca { get; set; }
        public int id_unidad_medida { get; set; }
        public decimal precio_unitario { get; set; }
        public decimal cantidad_disponible { get; set; }
        public decimal punto_reposicion { get; set; }
        public string deposito_ubicacion { get; set; }
        public string estado_articulo { get; set; }
        public bool activo { get; set; }
        public string usuario_modificacion { get; set; }
        public DateTime fecha { get; set; }
        public string dvh { get; set; }

        public string digito { get { return dvh; } }

        public string ObtenerCamposDV()
        {
            return string.Join("|",
                id_historial.ToString(CultureInfo.InvariantCulture),
                id_cambio.ToString(CultureInfo.InvariantCulture),
                id_articulo.ToString(CultureInfo.InvariantCulture),
                imagen,
                operacion,
                codigo,
                descripcion,
                id_categoria.ToString(CultureInfo.InvariantCulture),
                id_marca.ToString(CultureInfo.InvariantCulture),
                id_unidad_medida.ToString(CultureInfo.InvariantCulture),
                precio_unitario.ToString("F2", CultureInfo.InvariantCulture),
                cantidad_disponible.ToString("F2", CultureInfo.InvariantCulture),
                punto_reposicion.ToString("F2", CultureInfo.InvariantCulture),
                deposito_ubicacion,
                estado_articulo,
                activo,
                usuario_modificacion,
                fecha.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
        }
    }
}
