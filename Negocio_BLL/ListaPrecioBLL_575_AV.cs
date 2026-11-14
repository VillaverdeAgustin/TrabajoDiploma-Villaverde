using Acceso_DAL;
using Entidad_BE;
using Servicios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Negocio_BLL
{
    public class ListaPrecioBLL_575_AV
    {
        private const string TABLA = "ListaPrecio";
        private const string TABLA_ITEM = "ItemListaPrecio";

        public const int DIAS_VIGENCIA = 30;

        private MP_ListaPrecio_575_AV mpLista = new MP_ListaPrecio_575_AV();
        private ArticuloBLL_575_AV articuloBLL = new ArticuloBLL_575_AV();
        private BitacoraBLL bitacora = new BitacoraBLL();
        private VerificadorIntegridadBLL verificador = new VerificadorIntegridadBLL();

        #region Importacion (CUN-008)

        public ResumenImportacion_575_AV Previsualizar(string ruta, int id_proveedor)
        {
            if (id_proveedor <= 0)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("LPR_ERR_PROVEEDOR"));
            }
            if (string.IsNullOrWhiteSpace(ruta))
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("LPR_ERR_ARCHIVO"));
            }

            List<ItemListaPrecioBE_575_AV> items;
            try
            {
                items = new LectorListaPrecios_575_AV().Leer(ruta);
            }
            catch (FormatException ex)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir(ex.Message), ex);
            }
            catch (IOException ex)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("LPR_ERR_LECTURA"), ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("LPR_ERR_LECTURA"), ex);
            }

            HashSet<string> codigos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (ItemListaPrecioBE_575_AV item in items)
            {
                if (item.resultado != ResultadoImportacion_575_AV.Rechazado && !codigos.Add(item.codigo_proveedor))
                {
                    item.resultado = ResultadoImportacion_575_AV.Rechazado;
                    item.motivo = "LPR_MOT_DUPLICADO";
                }

                if (item.resultado == ResultadoImportacion_575_AV.Rechazado)
                {
                    item.motivo = GestorDeIdioma.GetInstance.Traducir(item.motivo);
                    continue;
                }

                ArticuloBE_575_AV articulo = articuloBLL.BuscarArticulos(null, null, null, item.codigo_proveedor)
                    .FirstOrDefault(a => string.Equals(a.codigo, item.codigo_proveedor, StringComparison.OrdinalIgnoreCase));

                if (articulo != null)
                {
                    item.id_articulo = articulo.id_articulo;
                    item.descripcion_articulo = articulo.descripcion;
                    item.resultado = ResultadoImportacion_575_AV.Actualizado;
                }
                else
                {
                    item.id_articulo = null;
                    item.resultado = ResultadoImportacion_575_AV.Incorporado;
                }
            }

            return new ResumenImportacion_575_AV { archivo = Path.GetFileName(ruta), renglones = items };
        }

        public ResumenImportacion_575_AV Importar(ListaPrecioBE_575_AV lista, List<ItemListaPrecioBE_575_AV> items, string usuario)
        {
            if (lista == null || lista.id_proveedor <= 0)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("LPR_ERR_PROVEEDOR"));
            }
            if (string.IsNullOrWhiteSpace(lista.archivo))
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("LPR_ERR_ARCHIVO"));
            }
            if (lista.moneda != "ARS" && lista.moneda != "USD")
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("LPR_ERR_MONEDA"));
            }

            List<ItemListaPrecioBE_575_AV> validos = (items ?? new List<ItemListaPrecioBE_575_AV>())
                .Where(i => i.resultado != ResultadoImportacion_575_AV.Rechazado)
                .ToList();
            if (validos.Count == 0)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("LPR_ERR_SIN_RENGLONES"));
            }

            lista.archivo = Path.GetFileName(lista.archivo);
            lista.fecha_vigencia = lista.fecha_vigencia.Date;
            lista.fecha_importacion = DateTime.Now;
            lista.usuario_importacion = usuario;
            lista.activo = true;

            lista.id_lista_precio = mpLista.CrearLista(lista);

            ListaPrecioBE_575_AV guardada = mpLista.ListarListas()
                .FirstOrDefault(l => l.id_lista_precio == lista.id_lista_precio);
            if (guardada != null) { lista.fecha_importacion = guardada.fecha_importacion; }

            PersistirDigito(lista, TABLA);

            try
            {
                foreach (ItemListaPrecioBE_575_AV item in validos)
                {
                    item.id_lista_precio = lista.id_lista_precio;
                    item.id_item_lista = mpLista.AgregarItem(item);
                    PersistirDVH(item, TABLA_ITEM);
                }
                verificador.ActualizarDVV(TABLA_ITEM);
            }
            catch (Exception ex)
            {
                Revertir(lista, usuario);
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("LPR_ERR_IMPORTACION_REVERTIDA"), ex);
            }

            bitacora.RegistrarBitacora(usuario, TipoAccion.ImportacionListaPrecios);

            return new ResumenImportacion_575_AV
            {
                id_lista_precio = lista.id_lista_precio,
                archivo = lista.archivo,
                renglones = items.ToList()
            };
        }

        private void Revertir(ListaPrecioBE_575_AV lista, string usuario)
        {
            verificador.ActualizarDVV(TABLA_ITEM);
            mpLista.DesactivarLista(lista.id_lista_precio);

            ListaPrecioBE_575_AV guardada = mpLista.ListarListas()
                .FirstOrDefault(l => l.id_lista_precio == lista.id_lista_precio) ?? lista;
            guardada.activo = false;

            PersistirDigito(guardada, TABLA);
            bitacora.RegistrarBitacora(usuario, TipoAccion.ImportacionListaRevertida);
        }

        #endregion

        #region Comparacion y precio vigente (CUN-009)

        public List<ComparacionPrecio_575_AV> CompararPrecios(int id_articulo, string moneda)
        {
            if (moneda != "ARS" && moneda != "USD")
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("LPR_ERR_MONEDA"));
            }

            DateTime limite = DateTime.Today.AddDays(-DIAS_VIGENCIA);

            List<ComparacionPrecio_575_AV> precios = mpLista.ListarPreciosPorArticulo(id_articulo, moneda);
            foreach (ComparacionPrecio_575_AV precio in precios)
            {
                precio.vencida = precio.fecha_vigencia.Date < limite;
            }

            return precios.OrderBy(p => p.precio_lista).ThenBy(p => p.razon_social).ToList();
        }

        public ItemListaPrecioBE_575_AV ObtenerPrecioVigente(int id_proveedor, int id_articulo)
        {
            return mpLista.BuscarPrecioVigente(id_proveedor, id_articulo);
        }

        #endregion

        #region Integridad

        public bool VerificarIntegridad()
        {
            bool listasOk = verificador.VerificarIntegridad(
                mpLista.ListarListas().OrderBy(l => l.id_lista_precio).ToList(), TABLA);
            bool itemsOk = verificador.VerificarIntegridad(
                mpLista.ListarItems().OrderBy(i => i.id_item_lista).ToList(), TABLA_ITEM);

            return listasOk && itemsOk;
        }

        public void RecalcularDV()
        {
            foreach (ListaPrecioBE_575_AV lista in mpLista.ListarListas().OrderBy(l => l.id_lista_precio))
            {
                lista.dvh = VerificadorIntegridad.CalcularDVH(lista);
                mpLista.ActualizarDVH(lista.id_lista_precio, lista.dvh);
            }
            verificador.ActualizarDVV(TABLA);

            foreach (ItemListaPrecioBE_575_AV item in mpLista.ListarItems().OrderBy(i => i.id_item_lista))
            {
                item.dvh = VerificadorIntegridad.CalcularDVH(item);
                mpLista.ActualizarDVHItem(item.id_item_lista, item.dvh);
            }
            verificador.ActualizarDVV(TABLA_ITEM);
        }

        private void PersistirDigito(IVerificable entidad, string tabla)
        {
            PersistirDVH(entidad, tabla);
            verificador.ActualizarDVV(tabla);
        }

        private void PersistirDVH(IVerificable entidad, string tabla)
        {
            string dvh = VerificadorIntegridad.CalcularDVH(entidad);

            switch (tabla)
            {
                case TABLA:
                    ListaPrecioBE_575_AV lista = (ListaPrecioBE_575_AV)entidad;
                    lista.dvh = dvh;
                    mpLista.ActualizarDVH(lista.id_lista_precio, dvh);
                    break;

                case TABLA_ITEM:
                    ItemListaPrecioBE_575_AV item = (ItemListaPrecioBE_575_AV)entidad;
                    item.dvh = dvh;
                    mpLista.ActualizarDVHItem(item.id_item_lista, dvh);
                    break;

                default:
                    throw new ArgumentException("Tabla no contemplada: " + tabla);
            }
        }

        #endregion
    }
}
