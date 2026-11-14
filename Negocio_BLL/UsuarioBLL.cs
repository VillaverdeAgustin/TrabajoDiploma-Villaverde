using Acceso_DAL;
using Entidad_BE;
using Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Negocio_BLL
{
    public class UsuarioBLL
    {
        private const string ROL_ADMINISTRADOR = "Admin";
        public int maxIntentos = 3;
        private UsuarioBE usuario;
        private MP_Usuario mpUsuario = new MP_Usuario();
        private BitacoraBLL bitacora = new BitacoraBLL();
        private VerificadorIntegridadBLL integridad = new VerificadorIntegridadBLL();

        public List<UsuarioBE> ListarUsuarios()
        {
            return mpUsuario.ListarUsuarios();
        }

        public LoginResult Login(UsuarioBE us)
        {
            LoginResult AuthOK;
            us.pass = Encriptador.EncriptarIrrev(us.pass);

            if (SessionManager.Logged())
            {
                usuario = SessionManager.GetInstance.UsuarioActual();
                if ((usuario.user == us.user) && (usuario.pass == us.pass))
                {
                    maxIntentos = 3;
                    return LoginResult.SesionIniciada;
                }
                return LoginResult.ExisteSesion;
            }

            AuthOK = mpUsuario.Login(us);

            if (AuthOK == LoginResult.LoginOK)
            {
                us = mpUsuario.ExtraerUsuario(us.user);
                SessionManager.GetInstance.Login(us);
                bitacora.RegistrarBitacora(us.user, TipoAccion.Login);
                maxIntentos = 3;
                return AuthOK;
            }

            maxIntentos--;
            if (maxIntentos == 0)
            {
                usuario = mpUsuario.ExtraerUsuario(us.user);
                if (usuario != null && !string.IsNullOrEmpty(usuario.user))
                {
                    usuario.bloq = true;
                    usuario.pass = Encriptador.EncriptarIrrev(" ");
                    usuario.dvh = VerificadorIntegridad.CalcularDVH(usuario);
                    mpUsuario.ActualizarBloqueo(usuario);
                    integridad.ActualizarDVV();
                }
                bitacora.RegistrarBitacora(us.user, TipoAccion.BloqueoUsuario);
                AuthOK = LoginResult.FinIntentos;
            }
            bitacora.RegistrarBitacora(us.user, TipoAccion.LoginFail);
            return AuthOK;
        }

        public int VerifUsuario(UsuarioBE us, int tipo)
        {
            string encPass = Encriptador.EncriptarIrrev(us.pass);
            if (SessionManager.GetInstance.UsuarioActual().pass == encPass)
            {
                maxIntentos = 3;
                return 1;
            }
            else
            {
                if (tipo == 1)
                {
                    UsuarioBE aux = SessionManager.GetInstance.UsuarioActual();
                    aux.pass = encPass;
                    aux.dvh = VerificadorIntegridad.CalcularDVH(aux);
                    mpUsuario.CambiarPass(aux);
                    maxIntentos = 3;
                    bitacora.RegistrarBitacora(aux.user, TipoAccion.CambioClave);
                    return 2;
                }
                maxIntentos--;
                if (maxIntentos == 0)
                {
                    if (tipo == 0)
                    {
                        UsuarioBE aux = SessionManager.GetInstance.UsuarioActual();
                        aux.bloq = true;
                        aux.dvh = VerificadorIntegridad.CalcularDVH(aux);
                        mpUsuario.ActualizarBloqueo(aux);
                        bitacora.RegistrarBitacora(aux.user, TipoAccion.BloqueoUsuario);
                        return 2;
                    }
                    else
                    {
                        maxIntentos = 3;
                        return 3;
                    }
                }
                bitacora.RegistrarBitacora(SessionManager.GetInstance.UsuarioActual().user, TipoAccion.LoginFail);
                return 0;
            }
        }

        public bool EsAdministrador(UsuarioBE usuario)
        {
            return usuario != null && usuario.rol == ROL_ADMINISTRADOR;
        }

        public void Logout()
        {
            if(SessionManager.Logged())
            {
                bitacora.RegistrarBitacora(SessionManager.GetInstance.UsuarioActual().user, TipoAccion.Logout);
                integridad.ActualizarDVV();
                SessionManager.GetInstance.Logout();
            }
        }

        public void DesbloquearUS(UsuarioBE us)
        {
            usuario = us;
            us.pass = Encriptador.EncriptarIrrev(GenerarPass(us.ape, us.dni.ToString()));
            us.bloq = false;
            us.dvh = VerificadorIntegridad.CalcularDVH(us);
            mpUsuario.ActualizarBloqueo(us);
            bitacora.RegistrarBitacora(SessionManager.GetInstance.UsuarioActual().user, TipoAccion.DesbloqueoUsuario);
        }

        public void CrearUsuario(UsuarioBE us)
        {
            usuario = us;
            us.pass = Encriptador.EncriptarIrrev(us.pass);
            string responsable = SessionManager.GetInstance.UsuarioActual().user;

            us.cod = mpUsuario.CrearUsuarioConHistorial(us, responsable);
            us.dvh = VerificadorIntegridad.CalcularDVH(us);
            mpUsuario.ActualizarUsuario(usuario);

            bitacora.RegistrarBitacora(responsable, TipoAccion.AltaUsuario);
        }

        public void EliminarUs(UsuarioBE us)
        {
            us.dvh = VerificadorIntegridad.CalcularDVH(us);
            string responsable = SessionManager.GetInstance.UsuarioActual().user;

            mpUsuario.EliminarUsuarioConHistorial(us, responsable);
            bitacora.RegistrarBitacora(responsable, TipoAccion.BajaUsuario);
        }

        public void ActualizarUsuario(UsuarioBE us)
        {
            us.dvh = VerificadorIntegridad.CalcularDVH(us);
            if(SessionManager.Logged())
            {
                string responsable = SessionManager.GetInstance.UsuarioActual().user;
                mpUsuario.ActualizarUsuarioConHistorial(us, responsable, TipoAccion.ModificacionUsuario);
                bitacora.RegistrarBitacora(responsable, TipoAccion.ModificacionUsuario);
            }
        }

        public string GenerarPass(string ape, string dni)
        {
            string pass = ape.Substring(0, 3) + dni.Substring(0, 3);
            return pass;
        }
    }
}
