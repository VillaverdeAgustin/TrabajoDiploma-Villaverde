using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidad_BE
{
    public class Familia : Permiso
    {
        private List<Permiso> _hijos;
        private bool _esRol;

        public List<Permiso> Hijos
        {
            get { return _hijos; }
            set { _hijos = value; }
        }

        public bool EsRol
        {
            get { return _esRol; }
            set { _esRol = value; }
        }

        public Familia(string nombre, bool esRol = false) : base(nombre)
        {
            _hijos = new List<Permiso>();
            _esRol = esRol;
        }

        public override List<Permiso> RetornarListaHijos()
        {
            return _hijos;
        }

        public void AgregarHijo(Permiso hijo)
        {
            if (!_hijos.Contains(hijo))
            {
                _hijos.Add(hijo);
            }
        }

        public void EliminarHijo(Permiso hijo)
        {
            _hijos.Remove(hijo);
        }
    }
}