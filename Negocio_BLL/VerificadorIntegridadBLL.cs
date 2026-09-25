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
        /// T08 - Controla la integridad de TODAS las tablas, no solo de Usuarios,
        /// y devuelve los nombres de las que no pasan. Lista vacia significa que
        /// esta todo bien.
        ///
        /// <para>Las BLL de negocio se instancian aca adentro y no como campos de
        /// la clase: <c>ArticuloBLL_575_AV</c> tiene un campo de este tipo, y si
        /// este tuviera a su vez uno de aquel, construir cualquiera de los dos
        /// entraria en recursion infinita.</para>
        /// </summary>
        public List<string> VerificarIntegridadTotal()
        {
            List<string> falladas = new List<string>();

            if (!VerificarIntegridad()) { falladas.Add("Usuarios"); }

            ArticuloBLL_575_AV articuloBLL = new ArticuloBLL_575_AV();
            BobinaBLL_575_AV bobinaBLL = new BobinaBLL_575_AV();
            ClienteBLL_575_AV clienteBLL = new ClienteBLL_575_AV();
            CarritoBLL_575_AV carritoBLL = new CarritoBLL_575_AV();
            ReservaBLL_575_AV reservaBLL = new ReservaBLL_575_AV();
            OrdenPagoBLL_575_AV ordenPagoBLL = new OrdenPagoBLL_575_AV();
            VentaBLL_575_AV ventaBLL = new VentaBLL_575_AV();
            EntregaBLL_575_AV entregaBLL = new EntregaBLL_575_AV();

            falladas.AddRange(articuloBLL.VerificarIntegridadCatalogos());

            if (!articuloBLL.VerificarIntegridad()) { falladas.Add("Articulo"); }
            if (!bobinaBLL.VerificarIntegridad()) { falladas.Add("Bobina"); }
            if (!clienteBLL.VerificarIntegridad()) { falladas.Add("Cliente"); }
            if (!carritoBLL.VerificarIntegridad()) { falladas.Add("Carrito"); }
            if (!reservaBLL.VerificarIntegridad()) { falladas.Add("Reserva"); }
            if (!ordenPagoBLL.VerificarIntegridad()) { falladas.Add("OrdenPago"); }
            if (!ventaBLL.VerificarIntegridad()) { falladas.Add("Comprobante"); }
            if (!entregaBLL.VerificarIntegridad()) { falladas.Add("Entrega"); }

            return falladas;
        }

        /// <summary>
        /// T08 - Recalcula el digito verificador de todas las tablas. Mismo
        /// recorrido que <see cref="VerificarIntegridadTotal"/>.
        /// </summary>
        public void RecalcularDVTotal()
        {
            RecalcularDV();

            ArticuloBLL_575_AV articuloBLL = new ArticuloBLL_575_AV();
            BobinaBLL_575_AV bobinaBLL = new BobinaBLL_575_AV();
            ClienteBLL_575_AV clienteBLL = new ClienteBLL_575_AV();
            CarritoBLL_575_AV carritoBLL = new CarritoBLL_575_AV();
            ReservaBLL_575_AV reservaBLL = new ReservaBLL_575_AV();
            OrdenPagoBLL_575_AV ordenPagoBLL = new OrdenPagoBLL_575_AV();
            VentaBLL_575_AV ventaBLL = new VentaBLL_575_AV();
            EntregaBLL_575_AV entregaBLL = new EntregaBLL_575_AV();

            articuloBLL.RecalcularDVCatalogos();
            articuloBLL.RecalcularDV();
            bobinaBLL.RecalcularDV();
            clienteBLL.RecalcularDV();
            carritoBLL.RecalcularDV();
            reservaBLL.RecalcularDV();
            ordenPagoBLL.RecalcularDV();
            ventaBLL.RecalcularDV();
            entregaBLL.RecalcularDV();
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
