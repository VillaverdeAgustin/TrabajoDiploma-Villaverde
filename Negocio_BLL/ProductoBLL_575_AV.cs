using Acceso_DAL;
using Entidad_BE;
using Servicios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Negocio_BLL
{
    /// <summary>
    /// Logica de producto y de los catalogos que lo componen.
    ///
    /// Toda escritura registra bitacora (T06) y recalcula el digito verificador
    /// de la fila y de la tabla (T08). Las bajas son logicas.
    /// </summary>
    public class ProductoBLL_575_AV
    {
        private const string TABLA = "Producto";

        private MP_Producto_575_AV mpProducto = new MP_Producto_575_AV();
        private MP_Catalogo_575_AV mpCatalogo = new MP_Catalogo_575_AV();
        private BitacoraBLL bitacora = new BitacoraBLL();
        private VerificadorIntegridadBLL verificador = new VerificadorIntegridadBLL();

        #region Catalogos

        public List<CategoriaBE_575_AV> ListarCategorias()
        {
            return mpCatalogo.ListarCategorias();
        }

        public List<MarcaBE_575_AV> ListarMarcas()
        {
            return mpCatalogo.ListarMarcas();
        }

        public List<UnidadMedidaBE_575_AV> ListarUnidadesMedida()
        {
            return mpCatalogo.ListarUnidadesMedida();
        }

        public List<MedioPagoBE_575_AV> ListarMediosPago()
        {
            return mpCatalogo.ListarMediosPago();
        }

        #endregion

        #region Consulta

        /// <summary>Listado completo, con los nombres de catalogo ya resueltos.</summary>
        public List<ProductoBE_575_AV> ListarProductos()
        {
            return Completar(mpProducto.ListarProductos());
        }

        /// <summary>
        /// RFN1.1 - Busca por descripcion, categoria, marca o codigo. Los
        /// criterios que llegan vacios o nulos no filtran.
        /// </summary>
        public List<ProductoBE_575_AV> BuscarProductos(string descripcion, int? idCategoria,
                                                       int? idMarca, string codigo,
                                                       bool soloActivos = true)
        {
            return Completar(mpProducto.BuscarProductos(descripcion, idCategoria, idMarca,
                                                        codigo, soloActivos));
        }

        public ProductoBE_575_AV ObtenerPorId(int idProducto)
        {
            return ListarProductos().FirstOrDefault(p => p.IdProducto == idProducto);
        }

        /// <summary>
        /// RFN1.2 - Verifica que haya disponibilidad para la cantidad pedida.
        /// En los articulos fraccionables no alcanza con el stock total: tiene
        /// que existir saldo de bobina suficiente, porque un pedido de 200 m no
        /// se puede cubrir con dos bobinas de 100 m sin empalmar.
        /// </summary>
        public bool ValidarDisponibilidad(ProductoBE_575_AV producto, decimal cantidad)
        {
            if (producto == null) { throw new ArgumentNullException("producto"); }
            if (cantidad <= 0) { throw new ArgumentException("La cantidad tiene que ser mayor a cero."); }

            if (!producto.Activo) { return false; }
            if (producto.StockActual < cantidad) { return false; }

            if (producto.Fraccionable)
            {
                BobinaBLL_575_AV bobinaBLL = new BobinaBLL_575_AV();
                return bobinaBLL.ExisteSaldoSuficiente(producto.IdProducto, cantidad);
            }

            return true;
        }

        #endregion

        #region Alta, modificacion y baja

        public int CrearProducto(ProductoBE_575_AV producto, string usuario)
        {
            Validar(producto);

            if (mpProducto.BuscarProductos(null, null, null, producto.Codigo, false).Any())
            {
                throw new InvalidOperationException("Ya existe un producto con el codigo " + producto.Codigo + ".");
            }

            producto.Activo = true;
            producto.IdProducto = mpProducto.CrearProducto(producto);

            PersistirDigito(producto);
            bitacora.RegistrarBitacora(usuario, TipoAccion.AltaProducto);

            return producto.IdProducto;
        }

        public void ActualizarProducto(ProductoBE_575_AV producto, string usuario)
        {
            Validar(producto);

            if (producto.IdProducto <= 0)
            {
                throw new InvalidOperationException("El producto no esta identificado.");
            }

            bool codigoRepetido = mpProducto
                .BuscarProductos(null, null, null, producto.Codigo, false)
                .Any(p => p.IdProducto != producto.IdProducto);

            if (codigoRepetido)
            {
                throw new InvalidOperationException("Ya existe otro producto con el codigo " + producto.Codigo + ".");
            }

            mpProducto.ActualizarProducto(producto);

            PersistirDigito(producto);
            bitacora.RegistrarBitacora(usuario, TipoAccion.ModificacionProducto);
        }

        /// <summary>Baja logica. El producto deja de ofrecerse pero conserva su historia.</summary>
        public void EliminarProducto(int idProducto, string usuario)
        {
            ProductoBE_575_AV producto = ObtenerPorId(idProducto);
            if (producto == null)
            {
                throw new InvalidOperationException("El producto no existe.");
            }

            mpProducto.EliminarProducto(idProducto);

            producto.Activo = false;
            PersistirDigito(producto);
            bitacora.RegistrarBitacora(usuario, TipoAccion.BajaProducto);
        }

        /// <summary>
        /// RFN1.6 - Movimiento de stock. La cantidad es un delta con signo:
        /// negativo descuenta una venta, positivo repone.
        /// </summary>
        public void ActualizarStock(int idProducto, decimal cantidad, string usuario)
        {
            if (cantidad == 0) { return; }

            mpProducto.ActualizarStock(idProducto, cantidad);

            ProductoBE_575_AV producto = ObtenerPorId(idProducto);
            if (producto != null) { PersistirDigito(producto); }

            bitacora.RegistrarBitacora(usuario, TipoAccion.MovimientoStock);
        }

        #endregion

        #region Integridad

        /// <summary>T08 - Verifica la tabla Producto contra su DVV.</summary>
        public bool VerificarIntegridad()
        {
            List<ProductoBE_575_AV> productos = mpProducto.ListarProductos()
                .OrderBy(p => p.IdProducto)
                .ToList();

            return verificador.VerificarIntegridad(productos, TABLA);
        }

        /// <summary>T08 - Recalcula el DVH de cada fila y el DVV de la tabla.</summary>
        public void RecalcularDV()
        {
            List<ProductoBE_575_AV> productos = mpProducto.ListarProductos()
                .OrderBy(p => p.IdProducto)
                .ToList();

            foreach (ProductoBE_575_AV producto in productos)
            {
                producto.DVH = VerificadorIntegridad.CalcularDVH(producto);
                mpProducto.ActualizarDVH(producto.IdProducto, producto.DVH);
            }

            verificador.ActualizarDVV(TABLA);
        }

        #endregion

        #region Apoyo

        /// <summary>Calcula y persiste el DVH de la fila y el DVV de la tabla.</summary>
        private void PersistirDigito(ProductoBE_575_AV producto)
        {
            producto.DVH = VerificadorIntegridad.CalcularDVH(producto);
            mpProducto.ActualizarDVH(producto.IdProducto, producto.DVH);
            verificador.ActualizarDVV(TABLA);
        }

        /// <summary>
        /// Completa los datos de catalogo que la pantalla necesita mostrar y que
        /// no viven en la tabla Producto. Se resuelven en memoria: los catalogos
        /// son cuatro tablas chicas y evita un join por fila.
        /// </summary>
        private List<ProductoBE_575_AV> Completar(List<ProductoBE_575_AV> productos)
        {
            if (productos.Count == 0) { return productos; }

            Dictionary<int, CategoriaBE_575_AV> categorias =
                ListarCategorias().ToDictionary(c => c.IdCategoria);
            Dictionary<int, MarcaBE_575_AV> marcas =
                ListarMarcas().ToDictionary(m => m.IdMarca);
            Dictionary<int, UnidadMedidaBE_575_AV> unidades =
                ListarUnidadesMedida().ToDictionary(u => u.IdUnidadMedida);

            foreach (ProductoBE_575_AV producto in productos)
            {
                CategoriaBE_575_AV categoria;
                if (categorias.TryGetValue(producto.IdCategoria, out categoria))
                {
                    producto.NombreCategoria = categoria.Nombre;
                }

                MarcaBE_575_AV marca;
                if (marcas.TryGetValue(producto.IdMarca, out marca))
                {
                    producto.NombreMarca = marca.Nombre;
                }

                UnidadMedidaBE_575_AV unidad;
                if (unidades.TryGetValue(producto.IdUnidadMedida, out unidad))
                {
                    producto.AbreviaturaUnidad = unidad.Abreviatura;
                    producto.Fraccionable = unidad.Fraccionable;
                }
            }

            return productos;
        }

        private void Validar(ProductoBE_575_AV producto)
        {
            if (producto == null) { throw new ArgumentNullException("producto"); }

            if (string.IsNullOrWhiteSpace(producto.Codigo))
            {
                throw new InvalidOperationException("El codigo del producto es obligatorio.");
            }
            if (string.IsNullOrWhiteSpace(producto.Descripcion))
            {
                throw new InvalidOperationException("La descripcion del producto es obligatoria.");
            }
            if (producto.IdCategoria <= 0)
            {
                throw new InvalidOperationException("Hay que elegir una categoria.");
            }
            if (producto.IdMarca <= 0)
            {
                throw new InvalidOperationException("Hay que elegir una marca.");
            }
            if (producto.IdUnidadMedida <= 0)
            {
                throw new InvalidOperationException("Hay que elegir una unidad de medida.");
            }
            if (producto.PrecioUnitario < 0)
            {
                throw new InvalidOperationException("El precio no puede ser negativo.");
            }
            if (producto.StockActual < 0)
            {
                throw new InvalidOperationException("El stock no puede ser negativo.");
            }
            if (producto.PuntoReposicion < 0)
            {
                throw new InvalidOperationException("El punto de reposicion no puede ser negativo.");
            }
        }

        #endregion
    }
}
