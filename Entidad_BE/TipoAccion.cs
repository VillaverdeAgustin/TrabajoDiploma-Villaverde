using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidad_BE
{
    public enum TipoAccion
    {
        Login = 1,
        Logout = 2,
        AltaUsuario = 3,
        BajaUsuario = 4,
        ModificacionUsuario = 5,
        CambioClave = 6,
        BitacoraAbierta = 7,
        GestionUsuariosAbierta = 8,
        AppClose = 9,
        DesbloqueoUsuario = 10,
        BloqueoUsuario = 11,
        LoginFail = 12,
        NoSesion = 13,
        CambioIdioma = 14,
        AltaIdioma = 15,
        ModificacionIdioma = 16,
        RestauracionUsuario = 17,

        // Negocio - RFN1 Gestion de Ventas
        GestionProductosAbierta = 18,
        AltaProducto = 19,
        ModificacionProducto = 20,
        BajaProducto = 21,
        MovimientoStock = 22,
        AltaBobina = 23,
        FraccionamientoBobina = 24
    }
}
