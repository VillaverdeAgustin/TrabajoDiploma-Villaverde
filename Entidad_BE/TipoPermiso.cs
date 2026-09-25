using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidad_BE
{
    /// <summary>
    /// Permisos simples del sistema: las hojas del arbol Composite (T04).
    ///
    /// <para>El permiso se guarda y se compara <b>como texto</b>: cada valor de
    /// aqui tiene que existir con el mismo nombre en la columna
    /// <c>Permiso.Nombre_Permiso</c> que carga <c>03_datos_seguridad.sql</c>.
    /// Renombrar un valor obliga a renombrar la fila.</para>
    ///
    /// <para>Sin acentos, porque el nombre viaja a la base: el texto que ve el
    /// usuario sale de la tabla Traduccion (T05).</para>
    /// </summary>
    public enum TipoPermiso
    {
        // RFN1 - Gestion de Venta por mostrador
        CompletarCarrito,
        GenerarOrdenPago,
        RegistrarCliente,
        ValidarPedidoYCobrar,
        EntregarArticulos,
        GestionArticulos,

        // Transversales
        GestionUsuarios,
        GestionPerfiles,
        GestionBackup,
        GestionBitacora
    }
}
