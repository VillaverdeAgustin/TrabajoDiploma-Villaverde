using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidad_BE
{
    public abstract class Permiso
    {
        protected string _nombre;

        public string Nombre
        {
            get { return _nombre; }
            set { _nombre = value; }
        }

        public Permiso(string nombre)
        {
            _nombre = nombre;
        }

        public abstract List<Permiso> RetornarListaHijos();

        public string getPermisoNombre()
        {
            return _nombre;
        }
    }
}
