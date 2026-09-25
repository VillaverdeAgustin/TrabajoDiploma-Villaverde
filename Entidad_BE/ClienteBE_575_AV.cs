using System;
using System.Globalization;

namespace Entidad_BE
{
    /// <summary>
    /// Cliente de la distribuidora: integrador, instalador, empresa que amplia
    /// su instalacion o consumidor final de mostrador.
    ///
    /// <para><c>correo_electronico</c> viaja y se guarda <b>cifrado</b> con
    /// AES-256 (T03.2). La BLL lo cifra al dar de alta y lo descifra solo cuando
    /// hay que mostrarlo, detras del boton protegido por clave.</para>
    /// </summary>
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

        /// <summary>Correo cifrado, tal como esta en la base.</summary>
        public string correo_electronico { get; set; }

        public DateTime fecha_actualizacion { get; set; }
        public bool activo { get; set; }
        public string dvh { get; set; }

        /// <summary>Razon social si la tiene; si no, apellido y nombre.</summary>
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

        /// <summary>
        /// El correo entra al digito <b>cifrado</b>, que es como esta guardado:
        /// asi el control detecta que alguien lo cambio a mano en la base, sin
        /// necesidad de descifrarlo para verificar.
        /// </summary>
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
