using Acceso_DAL;
using Entidad_BE;
using Servicios;
using System.Collections.Generic;

namespace Negocio_BLL
{
    /// <summary>
    /// Sujeto observable del patron Observer de idiomas (Singleton).
    /// Mantiene el idioma activo, el diccionario de traducciones cargado
    /// desde la base de datos y la lista de observadores suscriptos.
    /// Al cambiar el idioma notifica a todos los observadores para que
    /// refresquen sus textos sin reiniciar la aplicacion.
    /// </summary>
    public class GestorDeIdioma
    {
        private static GestorDeIdioma _gestor = null;
        private static object _lock = new object();//Bloquear acceso multihilo

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

        public System.Data.DataTable ObtenerTablaTraducciones(int idIdioma)
        {
            return idiomaDAL.ObtenerTablaTraducciones(idIdioma);
        }

        /// <summary>
        /// Da de alta un idioma. Si se indica un archivo de traducciones
        /// (formato CLAVE;Texto) se aplican sus textos sobre el idioma nuevo.
        /// Devuelve la cantidad de traducciones importadas desde el archivo.
        /// </summary>
        public int AgregarIdioma(string codigo, string nombre, string rutaArchivo = null)
        {
            codigo = (codigo ?? "").Trim().ToUpper();
            nombre = (nombre ?? "").Trim();

            if (codigo == "" || nombre == "")
            {
                throw new System.ArgumentException(Traducir("IDI_ERR_DATOS"));
            }
            if (codigo.Length > 5 || nombre.Length > 50)
            {
                throw new System.ArgumentException(Traducir("IDI_ERR_LONGITUD"));
            }
            if (ObtenerIdiomas().Exists(i => i.Codigo == codigo))
            {
                throw new System.ArgumentException(Traducir("IDI_ERR_CODIGO_EXISTE"));
            }

            //El archivo se lee ANTES de crear el idioma: si esta mal formado
            //no queda un idioma a medias en la base
            Dictionary<string, string> archivo = null;
            if (!string.IsNullOrEmpty(rutaArchivo))
            {
                archivo = LectorTraducciones.LeerArchivo(rutaArchivo);
            }

            //El idioma nuevo nace con las traducciones del idioma base copiadas
            //como punto de partida, para que la interfaz nunca quede sin textos
            idiomaDAL.CrearIdioma(codigo, nombre, IdiomaPorDefecto);

            if (archivo == null) { return 0; }

            //Se aplican solo las claves que existen en el catalogo del idioma
            IdiomaBE nuevo = ObtenerIdiomas().Find(i => i.Codigo == codigo);
            Dictionary<string, string> catalogo = idiomaDAL.ObtenerTraducciones(nuevo.Id);
            Dictionary<string, string> aplicables = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> par in archivo)
            {
                if (catalogo.ContainsKey(par.Key))
                {
                    aplicables[par.Key] = par.Value;
                }
            }
            GuardarTraducciones(nuevo.Id, aplicables);
            return aplicables.Count;
        }

        public void GuardarTraducciones(int idIdioma, Dictionary<string, string> cambios)
        {
            foreach (KeyValuePair<string, string> cambio in cambios)
            {
                idiomaDAL.ActualizarTraduccion(idIdioma, cambio.Key, cambio.Value);
            }

            //Si se edito el idioma activo se recarga el diccionario y se
            //notifica a los observadores: las pantallas abiertas se actualizan
            if (_idiomaActual != null && _idiomaActual.Id == idIdioma)
            {
                CambiarIdioma(_idiomaActual.Codigo);
            }
        }

        public void Suscribir(IObservadorIdioma observador)
        {
            if (!_observadores.Contains(observador))
            {
                _observadores.Add(observador);
            }
            //El nuevo observador arranca alineado con el idioma vigente
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
            return clave;//Si falta la traduccion se muestra la clave para detectarla facil
        }

        private void Notificar()
        {
            //Copia defensiva: un observador puede desuscribirse durante la notificacion
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
                    //Sin conexion o sin migracion de idiomas: la UI conserva
                    //los textos del disenio y Traducir devuelve las claves
                }
            }
        }
    }
}
