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

        public List<string> VerificarIntegridadTotal()
        {
            List<string> falladas = new List<string>();

            if (!VerificarIntegridad()) { falladas.Add("Usuarios"); }

            ArticuloBLL_575_AV articuloBLL = new ArticuloBLL_575_AV();
            BobinaBLL_575_AV bobinaBLL = new BobinaBLL_575_AV();
            ClienteBLL_575_AV clienteBLL = new ClienteBLL_575_AV();
            CarritoBLL_575_AV carritoBLL = new CarritoBLL_575_AV();
            OrdenPagoBLL_575_AV ordenPagoBLL = new OrdenPagoBLL_575_AV();
            VentaBLL_575_AV ventaBLL = new VentaBLL_575_AV();
            EntregaBLL_575_AV entregaBLL = new EntregaBLL_575_AV();

            falladas.AddRange(articuloBLL.VerificarIntegridadCatalogos());

            if (!articuloBLL.VerificarIntegridad()) { falladas.Add("Articulo"); }
            if (!bobinaBLL.VerificarIntegridad()) { falladas.Add("Bobina"); }
            if (!clienteBLL.VerificarIntegridad()) { falladas.Add("Cliente"); }
            if (!carritoBLL.VerificarIntegridad()) { falladas.Add("Carrito"); }
            if (!ordenPagoBLL.VerificarIntegridad()) { falladas.Add("OrdenPago"); }
            if (!ventaBLL.VerificarIntegridad()) { falladas.Add("Comprobante"); }
            if (!entregaBLL.VerificarIntegridad()) { falladas.Add("Entrega"); }

            if (!new ProveedorBLL_575_AV().VerificarIntegridad()) { falladas.Add("Proveedor"); }
            if (!new ListaPrecioBLL_575_AV().VerificarIntegridad()) { falladas.Add("ListaPrecio"); }
            if (!new OrdenCompraBLL_575_AV().VerificarIntegridad()) { falladas.Add("OrdenCompra"); }
            if (!new RecepcionBLL_575_AV().VerificarIntegridad()) { falladas.Add("Recepcion"); }

            return falladas;
        }

        public void RecalcularDVTotal()
        {
            RecalcularDV();

            ArticuloBLL_575_AV articuloBLL = new ArticuloBLL_575_AV();
            BobinaBLL_575_AV bobinaBLL = new BobinaBLL_575_AV();
            ClienteBLL_575_AV clienteBLL = new ClienteBLL_575_AV();
            CarritoBLL_575_AV carritoBLL = new CarritoBLL_575_AV();
            OrdenPagoBLL_575_AV ordenPagoBLL = new OrdenPagoBLL_575_AV();
            VentaBLL_575_AV ventaBLL = new VentaBLL_575_AV();
            EntregaBLL_575_AV entregaBLL = new EntregaBLL_575_AV();

            articuloBLL.RecalcularDVCatalogos();
            articuloBLL.RecalcularDV();
            bobinaBLL.RecalcularDV();
            clienteBLL.RecalcularDV();
            carritoBLL.RecalcularDV();
            ordenPagoBLL.RecalcularDV();
            ventaBLL.RecalcularDV();
            entregaBLL.RecalcularDV();

            new ProveedorBLL_575_AV().RecalcularDV();
            new ListaPrecioBLL_575_AV().RecalcularDV();
            new OrdenCompraBLL_575_AV().RecalcularDV();
            new RecepcionBLL_575_AV().RecalcularDV();
        }

        public void ActualizarDVV(string tabla)
        {
            verificadorMP.ActualizarDVV(
                VerificadorIntegridad.CalcularDVV(verificadorMP.ExtraerDVH(tabla)),
                tabla);
        }

        public bool VerificarIntegridad(IEnumerable<IVerificable> entidades, string tabla)
        {
            return VerificadorIntegridad.VerificarIntegridad(entidades, verificadorMP.ExtraerDVV(tabla));
        }

        public void RecalcularDV()
        {
            MP_Usuario mpUsuario = new MP_Usuario();
            List<UsuarioBE> usuarios = mpUsuario.ListarUsuarios();
            List<string> dvhs = new List<string>();
            usuarios = usuarios
                .OrderBy(u => u.cod)
                .ToList();
            foreach (UsuarioBE ent  in usuarios)
            {
                ent.dvh = VerificadorIntegridad.CalcularDVH(ent);
                dvhs.Add(ent.dvh);
                mpUsuario.ActualizarUsuario(ent);
            }
            verificadorMP.ActualizarDVV(VerificadorIntegridad.CalcularDVV(dvhs),"Usuarios");
        }
    }
}
