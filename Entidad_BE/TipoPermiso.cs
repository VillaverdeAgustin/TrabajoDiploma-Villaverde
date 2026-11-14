using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidad_BE
{
    public enum TipoPermiso
    {
        CompletarCarrito,
        GenerarOrdenPago,
        RegistrarCliente,
        ValidarPedidoYCobrar,
        EntregarArticulos,
        GestionArticulos,

        GestionUsuarios,
        GestionPerfiles,
        GestionBackup,
        GestionBitacora,

        RegistrarProveedor,
        ImportarListaPrecios,
        CompararPrecios,
        GenerarOrdenCompra,
        AutorizarOrdenCompra,
        RegistrarRecepcion,
        ReporteCompras,
        SerializarOrdenCompra
    }
}
