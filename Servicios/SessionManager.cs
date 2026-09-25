using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Entidad_BE;

namespace Servicios
{
    public class SessionManager
    {
        private static SessionManager _sesion = null;
        private static object _lock = new object();//Bloquear acceso multihilo
        private UsuarioBE _usuario;
        private bool logged = false;

        private SessionManager() { }

        public static SessionManager GetInstance
        {
            get
            {
                if (_sesion == null)
                {
                    lock (_lock)
                    {
                        if (_sesion == null)
                        {
                            _sesion = new SessionManager();
                        }
                    }
                }
                return _sesion;
            }
        }

        public UsuarioBE UsuarioActual()
        {
            return _usuario;
        }

        public void Login(UsuarioBE usuario)
        {
            logged = true;
            _usuario = usuario;
        }

        public void Logout()
        {
            logged = false;
            _usuario = null;
            _sesion = null;
        }

        public static bool Logged()
        {
            return GetInstance.logged;
        }
    }
}
