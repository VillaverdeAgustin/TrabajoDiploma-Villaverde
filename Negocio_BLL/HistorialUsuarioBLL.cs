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
    public class HistorialUsuarioBLL
    {
        private MP_HistorialUsuario mpHistorial = new MP_HistorialUsuario();
        private MP_Usuario mpUsuario = new MP_Usuario();
        private BitacoraBLL bitacora = new BitacoraBLL();

        public List<HistorialUsuarioBE> ListarHistorial()
        {
            return mpHistorial.ListarHistorial();
        }
        public List<HistorialUsuarioBE> BuscarHistorial(TipoAccion? accion, DateTime fIni, DateTime fFin)
        {
            return mpHistorial.ListarHistorial(accion, fIni, fFin);
        }

        public void RestaurarVersion(int idHistorial, string usuarioResponsable)
        {
            HistorialUsuarioBE version = mpHistorial.ObtenerVersion(idHistorial);
            if (version == null) { throw new Exception("La versión seleccionada no existe."); }

            UsuarioBE us = mpUsuario.ExtraerUsuario(version.usuario); // trae el estado actual (incluye pass, que no se toca)
            if (us == null) { throw new Exception("El usuario ya no existe."); }

            us.dni = Convert.ToInt32(version.dni);
            us.nomb = version.nombre;
            us.ape = version.apellido;
            us.rol = version.rol;
            us.dir = version.direccion;
            us.tel = version.telefono;
            us.email = version.email;
            us.estado = version.activo;
            us.bloq = version.bloqueado;
            us.dvh = VerificadorIntegridad.CalcularDVH(us);

            mpUsuario.ActualizarUsuarioConHistorial(us, usuarioResponsable, TipoAccion.RestauracionUsuario);
            bitacora.RegistrarBitacora(usuarioResponsable, TipoAccion.RestauracionUsuario);
        }
    }
}
