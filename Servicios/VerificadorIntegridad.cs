using Entidad_BE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios
{
    public class VerificadorIntegridad
    {
        public static string CalcularDVH(IVerificable entidad)
        {
            string datos = entidad.ObtenerCamposDV();
            return Encriptador.EncriptarIrrev(datos);
        }

        public static string CalcularDVV(List<string>dvh)
        {
            string aux = string.Join("|", dvh);
            return Encriptador.EncriptarIrrev(aux);
        }

        public static bool VerificarIntegridad(IEnumerable<IVerificable> entidad,string DVV)
        {
            List<string>dvhs = new List<string>();

            foreach(IVerificable ent in entidad)
            {
                string auxDVH = CalcularDVH(ent);

                if(auxDVH == ent.digito)
                {
                    dvhs.Add(auxDVH);
                }
                else return false;
            }

            string auxDVV = CalcularDVV(dvhs);
            return auxDVV == DVV;
        }
    }
}
