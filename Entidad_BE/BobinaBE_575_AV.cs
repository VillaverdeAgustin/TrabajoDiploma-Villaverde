using System;
using System.Globalization;

namespace Entidad_BE
{
    public class BobinaBE_575_AV : IVerificable
    {
        public int id_bobina { get; set; }
        public int id_articulo { get; set; }

        public string identificador { get; set; }

        public decimal medida_inicial { get; set; }

        public decimal saldo_bobina { get; set; }

        public DateTime? fecha_apertura { get; set; }

        public EstadoBobina_575_AV estado { get; set; }

        public string dvh { get; set; }

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
