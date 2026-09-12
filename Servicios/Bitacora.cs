using Entidad_BE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios
{
    public class Bitacora
    {
        public static EventoBE RegistrarEvento(string us, TipoAccion accion)
        {
            EventoBE evento = new EventoBE();
            evento.usuario = us;
            evento.accion = accion;
            evento.fecha = DateTime.Now;
            return evento;
        }
    }
}