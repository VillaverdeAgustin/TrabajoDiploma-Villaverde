using Acceso_DAL;
using Entidad_BE;
using Servicios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Negocio_BLL
{
    public class ArticuloBLL_575_AV : IExistencias_575_AV
    {
        private const string TABLA = "Articulo";

        private MP_Articulo_575_AV mpArticulo = new MP_Articulo_575_AV();
        private MP_Catalogo_575_AV mpCatalogo = new MP_Catalogo_575_AV();
        private BobinaBLL_575_AV bobinaBLL = new BobinaBLL_575_AV();
        private BitacoraBLL bitacora = new BitacoraBLL();
        private VerificadorIntegridadBLL verificador = new VerificadorIntegridadBLL();
        private ArticuloHistorialBLL_575_AV historialBLL = new ArticuloHistorialBLL_575_AV();

        public ArticuloBLL_575_AV()
        {
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

        public List<ArticuloBE_575_AV> ListarArticulos()
        {
            return Completar(mpArticulo.ListarArticulos());
        }

        public List<ArticuloBE_575_AV> BuscarArticulos(string descripcion, int? id_categoria,
                                                       int? id_marca, string codigo,
                                                       bool solo_activos = true)
        {
            return Completar(mpArticulo.BuscarArticulos(descripcion, id_categoria, id_marca,
                                                        codigo, solo_activos));
        }

        public ArticuloBE_575_AV ObtenerPorId(int id_articulo)
        {
            ArticuloBE_575_AV articulo = mpArticulo.ObtenerPorId(id_articulo);
            return articulo == null ? null : Completar(new List<ArticuloBE_575_AV> { articulo })[0];
        }

        public List<ArticuloBE_575_AV> ListarParaReponer()
        {
            return ListarArticulos()
                .Where(a => a.activo && a.bajo_punto_reposicion)
                .OrderBy(a => a.codigo)
                .ToList();
        }

        public bool ValidarDisponibilidad(int id_articulo, decimal cantidad)
        {
            try
            {
                return GestorDeExistencias_575_AV.GetInstance.HayDisponibilidad(id_articulo, cantidad);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir(ex.Message), ex);
            }
        }

        #endregion

        #region Alta, modificacion y baja

        public int CrearArticulo(ArticuloBE_575_AV articulo, string usuario)
        {
            Validar(articulo);

            if (mpArticulo.BuscarArticulos(null, null, null, articulo.codigo, false).Any())
            {
                throw new InvalidOperationException(
                    string.Format(GestorDeIdioma.GetInstance.Traducir("ART_ERR_CODIGO_EXISTE"),
                        articulo.codigo));
            }

            articulo.activo = true;
            articulo.estado_articulo = articulo.cantidad_disponible > 0
                ? EstadoArticulo_575_AV.Disponible
                : EstadoArticulo_575_AV.SinStock;

            articulo.id_articulo = mpArticulo.CrearArticulo(articulo, usuario);

            PersistirDigito(articulo);
            historialBLL.PersistirDigitosPendientes();
            bitacora.RegistrarBitacora(usuario, TipoAccion.AltaArticulo);

            return articulo.id_articulo;
        }

        public void ActualizarArticulo(ArticuloBE_575_AV articulo, string usuario)
        {
            Validar(articulo);

            if (articulo.id_articulo <= 0)
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("ART_ERR_NO_IDENTIFICADO"));
            }

            bool codigoRepetido = mpArticulo
                .BuscarArticulos(null, null, null, articulo.codigo, false)
                .Any(a => a.id_articulo != articulo.id_articulo);

            if (codigoRepetido)
            {
                throw new InvalidOperationException(
                    string.Format(GestorDeIdioma.GetInstance.Traducir("ART_ERR_CODIGO_EXISTE"),
                        articulo.codigo));
            }

            mpArticulo.ActualizarArticulo(articulo, usuario);

            PersistirDigito(articulo);
            historialBLL.PersistirDigitosPendientes();
            bitacora.RegistrarBitacora(usuario, TipoAccion.ModificacionArticulo);
        }

        public void EliminarArticulo(int id_articulo, string usuario)
        {
            ArticuloBE_575_AV articulo = ObtenerPorId(id_articulo);
            if (articulo == null)
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("ART_ERR_NO_EXISTE"));
            }

            mpArticulo.EliminarArticulo(id_articulo, usuario);

            articulo.activo = false;
            PersistirDigito(articulo);
            historialBLL.PersistirDigitosPendientes();
            bitacora.RegistrarBitacora(usuario, TipoAccion.BajaArticulo);
        }

        #endregion

        #region Restauracion (CUS-017)

        public void RestaurarVersion(ArticuloHistorialBE_575_AV version, string usuario)
        {
            ArticuloBE_575_AV actual = version == null ? null : ObtenerPorId(version.id_articulo);
            if (actual == null)
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("ART_ERR_NO_EXISTE"));
            }

            if (actual.codigo == version.codigo && actual.descripcion == version.descripcion
                && actual.id_categoria == version.id_categoria && actual.id_marca == version.id_marca
                && actual.id_unidad_medida == version.id_unidad_medida && actual.precio_unitario == version.precio_unitario
                && actual.punto_reposicion == version.punto_reposicion && actual.activo == version.activo
                && (actual.deposito_ubicacion ?? string.Empty) == (version.deposito_ubicacion ?? string.Empty))
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("HIS_ERR_SIN_CAMBIOS"));
            }

            if (mpArticulo.BuscarArticulos(null, null, null, version.codigo, false)
                    .Any(a => a.id_articulo != version.id_articulo))
            {
                throw new InvalidOperationException(
                    string.Format(GestorDeIdioma.GetInstance.Traducir("ART_ERR_CODIGO_EXISTE"),
                        version.codigo));
            }

            mpArticulo.RestaurarArticulo(version.id_historial, usuario);

            PersistirDigito(ObtenerPorId(version.id_articulo));
            historialBLL.PersistirDigitosPendientes();
            bitacora.RegistrarBitacora(usuario, TipoAccion.RestauracionArticulo);
        }

        #endregion

        #region IExistencias_575_AV

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

        public bool VerificarIntegridad()
        {
            List<ArticuloBE_575_AV> articulos = mpArticulo.ListarArticulos()
                .OrderBy(a => a.id_articulo)
                .ToList();

            return verificador.VerificarIntegridad(articulos, TABLA);
        }

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

        private void PersistirDigito(ArticuloBE_575_AV articulo)
        {
            articulo.dvh = VerificadorIntegridad.CalcularDVH(articulo);
            mpArticulo.ActualizarDVH(articulo.id_articulo, articulo.dvh);
            verificador.ActualizarDVV(TABLA);
        }

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
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("ART_ERR_CODIGO"));
            }
            if (string.IsNullOrWhiteSpace(articulo.descripcion))
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("ART_ERR_DESCRIPCION"));
            }
            if (articulo.id_categoria <= 0)
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("ART_ERR_CATEGORIA"));
            }
            if (articulo.id_marca <= 0)
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("ART_ERR_MARCA"));
            }
            if (articulo.id_unidad_medida <= 0)
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("ART_ERR_UNIDAD"));
            }
            if (articulo.precio_unitario < 0)
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("ART_ERR_PRECIO"));
            }
            if (articulo.cantidad_disponible < 0)
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("ART_ERR_EXISTENCIAS"));
            }
            if (articulo.punto_reposicion < 0)
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("ART_ERR_REPOSICION"));
            }
        }

        #endregion
    }
}
