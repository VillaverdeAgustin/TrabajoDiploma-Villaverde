using System;
using System.Globalization;

namespace Entidad_BE
{
    /// <summary>
    /// Bobina, caja o rollo fisico de un articulo fraccionable. Es la unidad
    /// sobre la que se lleva el saldo remanente cuando el articulo se vende por
    /// metro lineal (RFN1.8).
    /// </summary>
    public class BobinaBE_575_AV : IVerificable
    {
        public int id_bobina { get; set; }
        public int id_articulo { get; set; }

        /// <summary>Etiqueta fisica de la bobina o caja.</summary>
        public string identificador { get; set; }

        /// <summary>Medida con la que se recibio, por ejemplo 305 metros.</summary>
        public decimal medida_inicial { get; set; }

        public decimal saldo_bobina { get; set; }

        /// <summary>Queda en nulo mientras la bobina no se abrio.</summary>
        public DateTime? fecha_apertura { get; set; }

        public EstadoBobina_575_AV estado { get; set; }

        public string dvh { get; set; }

        /// <summary>Metros ya consumidos de esta bobina.</summary>
        public decimal consumido
        {
            get { return medida_inicial - saldo_bobina; }
        }

        public string digito { get { return dvh; } }

        public string ObtenerCamposDV()
        {
            return string.Join("|",
                id_bobina.ToString(CultureInfo.InvariantCulture),
                id_articulo.ToString(CultureInfo.InvariantCulture),
                identificador,
                medida_inicial.ToString("F2", CultureInfo.InvariantCulture),
                saldo_bobina.ToString("F2", CultureInfo.InvariantCulture),
                fecha_apertura.HasValue
                    ? fecha_apertura.Value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                    : string.Empty,
                estado);
        }

        public override string ToString()
        {
            return identificador + " (" +
                   saldo_bobina.ToString("F2", CultureInfo.InvariantCulture) + " de " +
                   medida_inicial.ToString("F2", CultureInfo.InvariantCulture) + ")";
        }
    }
}
