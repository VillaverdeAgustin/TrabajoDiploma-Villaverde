using System.Globalization;

namespace Entidad_BE
{
    public class ProveedorBE_575_AV : IVerificable
    {
        public int id_proveedor { get; set; }
        public string razon_social { get; set; }
        public string cuit { get; set; }
        public CondicionIva_575_AV condicion_iva { get; set; }
        public string rubro { get; set; }
        public string telefono { get; set; }
        public string correo_electronico { get; set; }
        public string direccion { get; set; }
        public bool activo { get; set; }
        public string dvh { get; set; }

        public string digito { get { return dvh; } }

        public string ObtenerCamposDV()
        {
            return string.Join("|",
                id_proveedor.ToString(CultureInfo.InvariantCulture),
                razon_social,
                cuit,
                condicion_iva,
                rubro,
                telefono,
                correo_electronico,
                direccion,
                activo);
        }

        public override string ToString()
        {
            return razon_social;
        }
    }
}
