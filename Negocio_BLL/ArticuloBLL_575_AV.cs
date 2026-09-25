using Acceso_DAL;
using Entidad_BE;
using Servicios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Negocio_BLL
{
    /// <summary>
    /// Logica de articulo y de los catalogos que lo componen.
    ///
    /// <para>Implementa <see cref="IExistencias_575_AV"/> y se registra en
    /// <see cref="GestorDeExistencias_575_AV"/>: el gestor es el unico que
    /// descuenta existencias o bloquea un articulo, y llega a los datos por
    /// esta clase sin que Servicios dependa de la DAL.</para>
    ///
    /// <para>Toda escritura registra bitacora (T06) y recalcula el digito
    /// verificador de la fila y de la tabla (T08). Las bajas son logicas.</para>
    /// </summary>
    public class ArticuloBLL_575_AV : IExistencias_575_AV
    {
        private const string TABLA = "Articulo";

        private MP_Articulo_575_AV mpArticulo = new MP_Articulo_575_AV();
        private MP_Catalogo_575_AV mpCatalogo = new MP_Catalogo_575_AV();
        private BobinaBLL_575_AV bobinaBLL = new BobinaBLL_575_AV();
        private BitacoraBLL bitacora = new BitacoraBLL();
        private VerificadorIntegridadBLL verificador = new VerificadorIntegridadBLL();

        public ArticuloBLL_575_AV()
        {
            // El gestor conserva el primer origen que reciba; las llamadas
            // siguientes no hacen nada.
            GestorDeExistencias_575_AV.GetInstance.Configurar(this);
        }

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
        public List<ArticuloBE_575_AV> ListarArticulos()
        {
            return Completar(mpArticulo.ListarArticulos());
        }

        /// <summary>
        /// RFN1.1 - Busca por descripcion, categoria, marca o codigo. Los
        /// criterios que llegan vacios o nulos no filtran.
        /// </summary>
        public List<ArticuloBE_575_AV> BuscarArticulos(string descripcion, int? id_categoria,
                                                       int? id_marca, string codigo,
                                                       bool solo_activos = true)
        {
            return Completar(mpArticulo.BuscarArticulos(descripcion, id_categoria, id_marca,
                                                        codigo, solo_activos));
        }

        public ArticuloBE_575_AV ObtenerPorId(int id_articulo)
        {
            return ListarArticulos().FirstOrDefault(a => a.id_articulo == id_articulo);
        }

        /// <summary>
        /// RFN1.2 - Verifica la disponibilidad. Delega en el gestor de
        /// existencias, que es el punto unico de verdad.
        /// </summary>
        public bool ValidarDisponibilidad(int id_articulo, decimal cantidad)
        {
            return GestorDeExistencias_575_AV.GetInstance.HayDisponibilidad(id_articulo, cantidad);
        }

        #endregion

        #region Alta, modificacion y baja

        public int CrearArticulo(ArticuloBE_575_AV articulo, string usuario)
        {
            Validar(articulo);

            if (mpArticulo.BuscarArticulos(null, null, null, articulo.codigo, false).Any())
            {
                throw new InvalidOperationException(
                    "Ya existe un articulo con el codigo " + articulo.codigo + ".");
            }

            articulo.activo = true;
            articulo.estado_articulo = articulo.cantidad_disponible > 0
                ? EstadoArticulo_575_AV.Disponible
                : EstadoArticulo_575_AV.SinStock;

            articulo.id_articulo = mpArticulo.CrearArticulo(articulo);

            PersistirDigito(articulo);
            bitacora.RegistrarBitacora(usuario, TipoAccion.AltaArticulo);

            return articulo.id_articulo;
        }

        public void ActualizarArticulo(ArticuloBE_575_AV articulo, string usuario)
        {
            Validar(articulo);

            if (articulo.id_articulo <= 0)
            {
                throw new InvalidOperationException("El articulo no esta identificado.");
            }

            bool codigoRepetido = mpArticulo
                .BuscarArticulos(null, null, null, articulo.codigo, false)
                .Any(a => a.id_articulo != articulo.id_articulo);

            if (codigoRepetido)
            {
                throw new InvalidOperationException(
                    "Ya existe otro articulo con el codigo " + articulo.codigo + ".");
            }

            mpArticulo.ActualizarArticulo(articulo);

            PersistirDigito(articulo);
            bitacora.RegistrarBitacora(usuario, TipoAccion.ModificacionArticulo);
        }

        /// <summary>Baja logica. El articulo deja de ofrecerse pero conserva su historia.</summary>
        public void EliminarArticulo(int id_articulo, string usuario)
        {
            ArticuloBE_575_AV articulo = ObtenerPorId(id_articulo);
            if (articulo == null)
            {
                throw new InvalidOperationException("El articulo no existe.");
            }

            mpArticulo.EliminarArticulo(id_articulo);

            articulo.activo = false;
            PersistirDigito(articulo);
            bitacora.RegistrarBitacora(usuario, TipoAccion.BajaArticulo);
        }

        #endregion

        #region IExistencias_575_AV

        // El gestor de existencias entra por aca. Ningun otro camino descuenta
        // existencias ni cambia el estado del articulo.

        ArticuloBE_575_AV IExistencias_575_AV.ObtenerArticulo(int id_articulo)
        {
            return ObtenerPorId(id_articulo);
        }

        bool IExistencias_575_AV.HaySaldoDeBobina(int id_articulo, decimal cantidad)
        {
            return bobinaBLL.ExisteSaldoSuficiente(id_articulo, cantidad);
        }

        void IExistencias_575_AV.MoverCantidad(int id_articulo, decimal cantidad, string usuario)
        {
            if (cantidad == 0) { return; }

            mpArticulo.ActualizarStock(id_articulo, cantidad);

            ArticuloBE_575_AV articulo = ObtenerPorId(id_articulo);
            if (articulo != null) { PersistirDigito(articulo); }

            bitacora.RegistrarBitacora(usuario, TipoAccion.MovimientoStock);
        }

        void IExistencias_575_AV.CambiarEstado(int id_articulo, EstadoArticulo_575_AV estado, string usuario)
        {
            mpArticulo.ActualizarEstado(id_articulo, estado);

            ArticuloBE_575_AV articulo = ObtenerPorId(id_articulo);
            if (articulo != null) { PersistirDigito(articulo); }

            bitacora.RegistrarBitacora(usuario,
                estado == EstadoArticulo_575_AV.Bloqueado
                    ? TipoAccion.BloqueoArticulo
                    : TipoAccion.LiberacionArticulo);
        }

        #endregion

        #region Integridad

        /// <summary>T08 - Verifica la tabla Articulo contra su DVV.</summary>
        public bool VerificarIntegridad()
        {
            List<ArticuloBE_575_AV> articulos = mpArticulo.ListarArticulos()
                .OrderBy(a => a.id_articulo)
                .ToList();

            return verificador.VerificarIntegridad(articulos, TABLA);
        }

        /// <summary>T08 - Recalcula el dvh de cada fila y el DVV de la tabla.</summary>
        public void RecalcularDV()
        {
            List<ArticuloBE_575_AV> articulos = mpArticulo.ListarArticulos()
                .OrderBy(a => a.id_articulo)
                .ToList();

            foreach (ArticuloBE_575_AV articulo in articulos)
            {
                articulo.dvh = VerificadorIntegridad.CalcularDVH(articulo);
                mpArticulo.ActualizarDVH(articulo.id_articulo, articulo.dvh);
            }

            verificador.ActualizarDVV(TABLA);
        }

        #endregion

        #region Integridad de los catalogos

        // Los cuatro catalogos no tienen BLL propia: los atiende esta clase, que
        // es la que ya los expone. Cada tabla se verifica y se recalcula por
        // separado, porque cada una tiene su propia fila en DigitoVertical.

        /// <summary>Tablas de catalogo que no pasan el control de integridad.</summary>
        public List<string> VerificarIntegridadCatalogos()
        {
            List<string> falladas = new List<string>();

            if (!verificador.VerificarIntegridad(
                    ListarCategorias().OrderBy(c => c.id_categoria).ToList(), "Categoria"))
            {
                falladas.Add("Categoria");
            }
            if (!verificador.VerificarIntegridad(
                    ListarMarcas().OrderBy(m => m.id_marca).ToList(), "Marca"))
            {
                falladas.Add("Marca");
            }
            if (!verificador.VerificarIntegridad(
                    ListarUnidadesMedida().OrderBy(u => u.id_unidad_medida).ToList(), "UnidadMedida"))
            {
                falladas.Add("UnidadMedida");
            }
            if (!verificador.VerificarIntegridad(
                    ListarMediosPago().OrderBy(p => p.id_medio_pago).ToList(), "MedioPago"))
            {
                falladas.Add("MedioPago");
            }

            return falladas;
        }

        /// <summary>Recalcula el dvh de cada fila de catalogo y el DVV de cada tabla.</summary>
        public void RecalcularDVCatalogos()
        {
            foreach (CategoriaBE_575_AV categoria in ListarCategorias().OrderBy(c => c.id_categoria))
            {
                categoria.dvh = VerificadorIntegridad.CalcularDVH(categoria);
                mpCatalogo.ActualizarDVHCategoria(categoria.id_categoria, categoria.dvh);
            }
            verificador.ActualizarDVV("Categoria");

            foreach (MarcaBE_575_AV marca in ListarMarcas().OrderBy(m => m.id_marca))
            {
                marca.dvh = VerificadorIntegridad.CalcularDVH(marca);
                mpCatalogo.ActualizarDVHMarca(marca.id_marca, marca.dvh);
            }
            verificador.ActualizarDVV("Marca");

            foreach (UnidadMedidaBE_575_AV unidad in ListarUnidadesMedida().OrderBy(u => u.id_unidad_medida))
            {
                unidad.dvh = VerificadorIntegridad.CalcularDVH(unidad);
                mpCatalogo.ActualizarDVHUnidadMedida(unidad.id_unidad_medida, unidad.dvh);
            }
            verificador.ActualizarDVV("UnidadMedida");

            foreach (MedioPagoBE_575_AV medio in ListarMediosPago().OrderBy(p => p.id_medio_pago))
            {
                medio.dvh = VerificadorIntegridad.CalcularDVH(medio);
                mpCatalogo.ActualizarDVHMedioPago(medio.id_medio_pago, medio.dvh);
            }
            verificador.ActualizarDVV("MedioPago");
        }

        #endregion

        #region Apoyo

        /// <summary>Calcula y persiste el dvh de la fila y el DVV de la tabla.</summary>
        private void PersistirDigito(ArticuloBE_575_AV articulo)
        {
            articulo.dvh = VerificadorIntegridad.CalcularDVH(articulo);
            mpArticulo.ActualizarDVH(articulo.id_articulo, articulo.dvh);
            verificador.ActualizarDVV(TABLA);
        }

        /// <summary>
        /// Completa los datos de catalogo que la pantalla necesita mostrar y que
        /// no viven en la tabla Articulo. Se resuelven en memoria: los catalogos
        /// son cuatro tablas chicas y evita un join por fila.
        /// </summary>
        private List<ArticuloBE_575_AV> Completar(List<ArticuloBE_575_AV> articulos)
        {
            if (articulos.Count == 0) { return articulos; }

            Dictionary<int, CategoriaBE_575_AV> categorias =
                ListarCategorias().ToDictionary(c => c.id_categoria);
            Dictionary<int, MarcaBE_575_AV> marcas =
                ListarMarcas().ToDictionary(m => m.id_marca);
            Dictionary<int, UnidadMedidaBE_575_AV> unidades =
                ListarUnidadesMedida().ToDictionary(u => u.id_unidad_medida);

            foreach (ArticuloBE_575_AV articulo in articulos)
            {
                CategoriaBE_575_AV categoria;
                if (categorias.TryGetValue(articulo.id_categoria, out categoria))
                {
                    articulo.nombre_categoria = categoria.nombre;
                }

                MarcaBE_575_AV marca;
                if (marcas.TryGetValue(articulo.id_marca, out marca))
                {
                    articulo.nombre_marca = marca.nombre;
                }

                UnidadMedidaBE_575_AV unidad;
                if (unidades.TryGetValue(articulo.id_unidad_medida, out unidad))
                {
                    articulo.abreviatura_unidad = unidad.abreviatura;
                    articulo.fraccionable = unidad.fraccionable;
                }
            }

            return articulos;
        }

        private void Validar(ArticuloBE_575_AV articulo)
        {
            if (articulo == null) { throw new ArgumentNullException("articulo"); }

            if (string.IsNullOrWhiteSpace(articulo.codigo))
            {
                throw new InvalidOperationException("El codigo del articulo es obligatorio.");
            }
            if (string.IsNullOrWhiteSpace(articulo.descripcion))
            {
                throw new InvalidOperationException("La descripcion del articulo es obligatoria.");
            }
            if (articulo.id_categoria <= 0)
            {
                throw new InvalidOperationException("Hay que elegir una categoria.");
            }
            if (articulo.id_marca <= 0)
            {
                throw new InvalidOperationException("Hay que elegir una marca.");
            }
            if (articulo.id_unidad_medida <= 0)
            {
                throw new InvalidOperationException("Hay que elegir una unidad de medida.");
            }
            if (articulo.precio_unitario < 0)
            {
                throw new InvalidOperationException("El precio no puede ser negativo.");
            }
            if (articulo.cantidad_disponible < 0)
            {
                throw new InvalidOperationException("Las existencias no pueden ser negativas.");
            }
            if (articulo.punto_reposicion < 0)
            {
                throw new InvalidOperationException("El punto de reposicion no puede ser negativo.");
            }
        }

        #endregion
    }
}
