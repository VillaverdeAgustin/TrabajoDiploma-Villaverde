using Acceso_DAL;
using Entidad_BE;
using Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio_BLL
{
    public class BitacoraBLL
    {
        private MP_Bitacora mpBitacora = new MP_Bitacora();

        public List<EventoBE> ListarBitacora()
        {
            return mpBitacora.ListarEventos();
        }

        public void RegistrarBitacora(string us, TipoAccion acc)
        {
            mpBitacora.RegistrarEvento(Bitacora.RegistrarEvento(us, acc));
        }

        public List<EventoBE> BuscarEventos(string us, TipoAccion? acc, DateTime fIni, DateTime fFin)
        {
            return mpBitacora.BuscarEventos(us,acc,fIni, fFin);
        }
    }
}
