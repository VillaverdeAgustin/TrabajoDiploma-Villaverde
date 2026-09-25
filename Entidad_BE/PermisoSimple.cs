using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidad_BE
{
    public class PermisoSimple : Permiso
    {
        public PermisoSimple(string nombre) : base(nombre)
        {
        }

        public override List<Permiso> RetornarListaHijos()
        {
            return new List<Permiso>();
        }
    }
}
