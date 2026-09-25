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
        GestionArticulosAbierta = 18,
        AltaArticulo = 19,
        ModificacionArticulo = 20,
        BajaArticulo = 21,
        MovimientoStock = 22,
        AltaBobina = 23,
        FraccionamientoBobina = 24,
        BloqueoArticulo = 25,
        LiberacionArticulo = 26,
        AltaCliente = 27,
        // 28 y 29 quedan reservados: el ABM de cliente no entra en la Entrega 1,
        // pero la bitacora guarda el entero y la numeracion no se mueve.
        ModificacionCliente = 28,
        BajaCliente = 29,
        CarritoComprasAbierto = 30,
        AltaCarrito = 31,
        AltaDetalleCarrito = 32,
        BajaDetalleCarrito = 33,
        ConfirmacionCarrito = 34,
        AltaReserva = 35,
        OrdenPagoAbierta = 36,
        EmisionOrdenPago = 37,
        CajaAbierta = 38,
        CobroRegistrado = 39,
        CobroRechazado = 40,
        EmisionComprobante = 41,
        DepositoAbierto = 42,
        EntregaRegistrada = 43,
        EntregaParcial = 44
    }
}
