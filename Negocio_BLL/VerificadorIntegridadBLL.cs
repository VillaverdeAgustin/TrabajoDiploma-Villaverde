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
    public class VerificadorIntegridadBLL
    {
        MP_VerificadorIntegridad verificadorMP = new MP_VerificadorIntegridad();

        public List<string> ExtraerDVH()
        {
            return verificadorMP.ExtraerDVH("Usuarios");
        }

        public string ExtraerDVV()
        {
            return verificadorMP.ExtraerDVV("Usuarios");
        }

        public void ActualizarDVV()
        {
            verificadorMP.ActualizarDVV(VerificadorIntegridad.CalcularDVV(ExtraerDVH()),"Usuarios");
        }

        public bool VerificarIntegridad()
        {
            UsuarioBLL usuarioBLL = new UsuarioBLL();
            List<UsuarioBE> usuarios = usuarioBLL.ListarUsuarios();
            usuarios = usuarios
                .OrderBy(u => u.cod)
                .ToList();

            return VerificadorIntegridad.VerificarIntegridad(usuarios,ExtraerDVV());
        }

        /// <summary>
        /// Recalcula el DVV de cualquier tabla a partir de los DVH ya
        /// persistidos. Lo usan las BLL de negocio despues de cada alta,
        /// modificacion o baja, para no dejar la tabla marcada como corrupta.
        /// </summary>
        public void ActualizarDVV(string tabla)
        {
            verificadorMP.ActualizarDVV(
                VerificadorIntegridad.CalcularDVV(verificadorMP.ExtraerDVH(tabla)),
                tabla);
        }

        /// <summary>
        /// Verifica una tabla cualquiera contra su DVV. Las entidades tienen que
        /// venir en el mismo orden que devuelve SP_ExtraerDVH, es decir por
        /// clave primaria ascendente.
        /// </summary>
        public bool VerificarIntegridad(IEnumerable<IVerificable> entidades, string tabla)
        {
            return VerificadorIntegridad.VerificarIntegridad(entidades, verificadorMP.ExtraerDVV(tabla));
        }

        public void RecalcularDV()
        {
            UsuarioBLL usuario = new UsuarioBLL();
            List<UsuarioBE> usuarios = usuario.ListarUsuarios();
            List<string> dvhs = new List<string>();
            usuarios = usuarios
                .OrderBy(u => u.cod)
                .ToList();
            foreach (UsuarioBE ent  in usuarios)
            {
                ent.dvh = VerificadorIntegridad.CalcularDVH(ent);
                dvhs.Add(ent.dvh);
                usuario.ActualizarUsuario(ent);
            }
            verificadorMP.ActualizarDVV(VerificadorIntegridad.CalcularDVV(dvhs),"Usuarios");
        }
    }
}
