using Acceso_DAL;
using Entidad_BE;
using Servicios;
using System.Collections.Generic;

namespace Negocio_BLL
{
    public class GestorDeIdioma
    {
        private static GestorDeIdioma _gestor = null;
        private static object _lock = new object();

        public const string IdiomaPorDefecto = "ES";

        private readonly List<IObservadorIdioma> _observadores = new List<IObservadorIdioma>();
        private Dictionary<string, string> _traducciones = new Dictionary<string, string>();
        private IdiomaBE _idiomaActual = null;
        private IdiomaDAL idiomaDAL = new IdiomaDAL();

        private GestorDeIdioma() { }

        public static GestorDeIdioma GetInstance
        {
            get
            {
                if (_gestor == null)
                {
                    lock (_lock)
                    {
                        if (_gestor == null)
                        {
                            _gestor = new GestorDeIdioma();
                        }
                    }
                }
                return _gestor;
            }
        }

        public IdiomaBE IdiomaActual
        {
            get
            {
                AsegurarIdiomaCargado();
                return _idiomaActual;
            }
        }

        public List<IdiomaBE> ObtenerIdiomas()
        {
            return idiomaDAL.ObtenerIdiomas();
        }

        public void Suscribir(IObservadorIdioma observador)
        {
            if (!_observadores.Contains(observador))
            {
                _observadores.Add(observador);
            }
            AsegurarIdiomaCargado();
            if (_idiomaActual != null)
            {
                observador.ActualizarTextos();
            }
        }

        public void Desuscribir(IObservadorIdioma observador)
        {
            _observadores.Remove(observador);
        }

        public void CambiarIdioma(string codigo)
        {
            foreach (IdiomaBE idioma in idiomaDAL.ObtenerIdiomas())
            {
                if (idioma.Codigo == codigo)
                {
                    _traducciones = idiomaDAL.ObtenerTraducciones(idioma.Id);
                    _idiomaActual = idioma;
                    Notificar();
                    return;
                }
            }
            throw new KeyNotFoundException($"El idioma '{codigo}' no se encuentra registrado en el sistema");
        }

        public string Traducir(string clave)
        {
            AsegurarIdiomaCargado();
            string texto;
            if (_traducciones.TryGetValue(clave, out texto))
            {
                return texto;
            }
            return clave;
        }

        private void Notificar()
        {
            foreach (IObservadorIdioma observador in _observadores.ToArray())
            {
                observador.ActualizarTextos();
            }
        }

        private void AsegurarIdiomaCargado()
        {
            if (_idiomaActual == null)
            {
                try
                {
                    CambiarIdioma(IdiomaPorDefecto);
                }
                catch
                {
                }
            }
        }
    }
}
