using System;
using System.Globalization;

namespace Entidad_BE
{
    public class ClienteBE_575_AV : IVerificable
    {
        public int id_cliente { get; set; }
        public string razon_social { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string dni { get; set; }
        public string cuit { get; set; }
        public CondicionIva_575_AV condicion_iva { get; set; }
        public string direccion { get; set; }
        public string telefono { get; set; }

        public string correo_electronico { get; set; }

        public DateTime fecha_actualizacion { get; set; }
        public bool activo { get; set; }
        public string dvh { get; set; }

        public string documento_para_mostrar
        {
            get { return string.IsNullOrWhiteSpace(cuit) ? dni : cuit; }
        }

        public string nombre_para_mostrar
        {
            get
            {
                return string.IsNullOrWhiteSpace(razon_social)
                    ? (apellido + ", " + nombre)
                    : razon_social;
            }
        }

        public string digito { get { return dvh; } }

        public string ObtenerCamposDV()
        {
            return string.Join("|",
                id_cliente.ToString(CultureInfo.InvariantCulture),
                razon_social,
                nombre,
                apellido,
                dni,
                cuit,
                condicion_iva,
                direccion,
                telefono,
                correo_electronico,
                fecha_actualizacion.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                activo);
        }

        public override string ToString()
        {
            return dni + " - " + nombre_para_mostrar;
        }
    }
}
