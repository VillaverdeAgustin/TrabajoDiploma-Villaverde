using System;
using System.Globalization;

namespace Entidad_BE
{
    /// <summary>
    /// Bobina, caja o rollo fisico de un producto fraccionable.
    /// Es la unidad sobre la que se lleva el saldo remanente cuando el articulo
    /// se vende por metro lineal (RFN1.8).
    /// </summary>
    public class BobinaBE_575_AV : IVerificable
    {
        public int IdBobina { get; set; }
        public int IdProducto { get; set; }

        /// <summary>Etiqueta fisica de la bobina o caja.</summary>
        public string Identificador { get; set; }

        /// <summary>Medida con la que se recibio, por ejemplo 305 metros.</summary>
        public decimal MedidaInicial { get; set; }

        public decimal SaldoActual { get; set; }

        /// <summary>Queda en nulo mientras la bobina no se abrio.</summary>
        public DateTime? FechaApertura { get; set; }

        public EstadoBobina_575_AV Estado { get; set; }

        public string DVH { get; set; }

        /// <summary>Metros ya consumidos de esta bobina.</summary>
        public decimal Consumido
        {
            get { return MedidaInicial - SaldoActual; }
        }

        public string digito { get { return DVH; } }

        public string ObtenerCamposDV()
        {
            return string.Join("|",
                IdBobina.ToString(CultureInfo.InvariantCulture),
                IdProducto.ToString(CultureInfo.InvariantCulture),
                Identificador,
                MedidaInicial.ToString("F2", CultureInfo.InvariantCulture),
                SaldoActual.ToString("F2", CultureInfo.InvariantCulture),
                FechaApertura.HasValue
                    ? FechaApertura.Value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                    : string.Empty,
                Estado);
        }

        public override string ToString()
        {
            return Identificador + " (" +
                   SaldoActual.ToString("F2", CultureInfo.InvariantCulture) + " de " +
                   MedidaInicial.ToString("F2", CultureInfo.InvariantCulture) + ")";
        }
    }
}
