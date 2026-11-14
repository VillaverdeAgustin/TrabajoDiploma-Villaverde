using Entidad_BE;
using Servicios;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Negocio_BLL
{
    // A03 - Serializacion de una orden de compra a XML y verificaciones de ida y vuelta.
    // El archivo lo escribe y lo lee Serializador_575_AV (Servicios); aca se arma el documento y se verifica.
    public class SerializacionOrdenCompraBLL_575_AV
    {
        public const string VERSION = "1.0";

        private Serializador_575_AV serializador = new Serializador_575_AV();
        private OrdenCompraBLL_575_AV ordenBLL = new OrdenCompraBLL_575_AV();
        private ProveedorBLL_575_AV proveedorBLL = new ProveedorBLL_575_AV();
        private BitacoraBLL bitacora = new BitacoraBLL();

        public List<OrdenCompraBE_575_AV> ListarOrdenes()
        {
            return ordenBLL.ListarEmitidas().OrderByDescending(o => o.fecha_emision).ThenByDescending(o => o.id_orden_compra).ToList();
        }

        #region Serializar

        public ResultadoSerializacion_575_AV Serializar(int id_orden_compra, string ruta, string usuario)
        {
            if (string.IsNullOrWhiteSpace(ruta))
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("SER_ERR_RUTA"));
            }

            OrdenCompraBE_575_AV orden = ordenBLL.ObtenerOrden(id_orden_compra);
            if (orden == null || orden.estado == EstadoOrdenCompra_575_AV.Borrador)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("SER_ERR_NO_EXISTE"));
            }

            DocumentoOrdenCompra_575_AV documento = new DocumentoOrdenCompra_575_AV
            {
                version = VERSION,
                fecha_generacion = DateTime.Now,
                usuario = usuario,
                orden = orden,
                proveedor = proveedorBLL.ListarProveedores().FirstOrDefault(p => p.id_proveedor == orden.id_proveedor)
            };

            try
            {
                serializador.Serializar(documento, ruta);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is ArgumentException || ex is NotSupportedException)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("SER_ERR_ESCRITURA"), ex);
            }

            // Verificacion: releer el archivo recien escrito y compararlo campo por campo con el objeto
            DocumentoOrdenCompra_575_AV releido = Leer(ruta);
            ResultadoSerializacion_575_AV resultado = new ResultadoSerializacion_575_AV
            {
                ruta = ruta,
                hash = serializador.CalcularHash(ruta),
                documento = releido
            };

            resultado.verificaciones.Add(Verificacion("SER_VER_RELEIDO", releido != null && releido.version == VERSION,
                new FileInfo(ruta).Length.ToString("N0") + " bytes", Path.GetFileName(ruta)));
            resultado.verificaciones.Add(CompararCampos("SER_VER_CABECERA", documento.orden, releido.orden, documento.orden.nro_orden_compra));
            resultado.verificaciones.Add(Verificacion("SER_VER_CANT_RENGLONES", documento.orden.detalle.Count == releido.orden.detalle.Count,
                Igualdad(documento.orden.detalle.Count, releido.orden.detalle.Count)));
            for (int i = 0; i < Math.Min(documento.orden.detalle.Count, releido.orden.detalle.Count); i++)
            {
                resultado.verificaciones.Add(CompararCampos("SER_VER_RENGLON", documento.orden.detalle[i], releido.orden.detalle[i],
                    (i + 1).ToString(), documento.orden.detalle[i].codigo_articulo));
            }
            resultado.verificaciones.Add(CompararCampos("SER_VER_PROVEEDOR", documento.proveedor, releido.proveedor,
                documento.proveedor == null ? string.Empty : documento.proveedor.razon_social));
            resultado.verificaciones.Add(Verificacion("SER_VER_HASH", true, resultado.hash));

            bitacora.RegistrarBitacora(usuario, TipoAccion.SerializacionOrdenCompra);
            return resultado;
        }

        #endregion

        #region Deserializar

        public ResultadoSerializacion_575_AV Deserializar(string ruta, string usuario)
        {
            if (string.IsNullOrWhiteSpace(ruta))
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("SER_ERR_RUTA"));
            }

            DocumentoOrdenCompra_575_AV documento = Leer(ruta);
            ResultadoSerializacion_575_AV resultado = new ResultadoSerializacion_575_AV
            {
                ruta = ruta,
                hash = serializador.CalcularHash(ruta),
                documento = documento
            };
            List<ResultadoVerificacion_575_AV> v = resultado.verificaciones;
            OrdenCompraBE_575_AV delArchivo = documento.orden;

            v.Add(Verificacion("SER_VER_HASH", true, resultado.hash));
            v.Add(VerificarDigitos(delArchivo));

            // Contra la base: la orden tiene que existir con el mismo numero
            OrdenCompraBE_575_AV enBase = ordenBLL.ObtenerOrden(delArchivo.id_orden_compra);
            bool existe = enBase != null && enBase.estado != EstadoOrdenCompra_575_AV.Borrador
                          && enBase.nro_orden_compra == delArchivo.nro_orden_compra;
            v.Add(Verificacion("SER_VER_EXISTE", existe,
                existe ? enBase.nro_orden_compra : "id_orden_compra " + delArchivo.id_orden_compra, delArchivo.nro_orden_compra));

            if (existe)
            {
                ProveedorBE_575_AV proveedor = proveedorBLL.ListarProveedores().FirstOrDefault(p => p.id_proveedor == enBase.id_proveedor);
                bool mismoProveedor = proveedor != null && delArchivo.id_proveedor == enBase.id_proveedor
                                      && documento.proveedor.id_proveedor == enBase.id_proveedor && documento.proveedor.cuit == proveedor.cuit;
                v.Add(Verificacion("SER_VER_PROVEEDOR_BD", mismoProveedor,
                    Igualdad(documento.proveedor.cuit, proveedor == null ? null : proveedor.cuit), documento.proveedor.razon_social));

                v.Add(Verificacion("SER_VER_CANT_RENGLONES", delArchivo.detalle.Count == enBase.detalle.Count,
                    Igualdad(delArchivo.detalle.Count, enBase.detalle.Count)));

                for (int i = 0; i < delArchivo.detalle.Count; i++)
                {
                    DetalleOrdenCompraBE_575_AV renglon = delArchivo.detalle[i];
                    DetalleOrdenCompraBE_575_AV guardado = enBase.detalle.FirstOrDefault(d => d.id_detalle_orden_compra == renglon.id_detalle_orden_compra);
                    v.Add(CompararRenglon(renglon, guardado, i + 1));
                }

                decimal sumaArchivo = delArchivo.detalle.Sum(d => d.subtotal);
                v.Add(Verificacion("SER_VER_TOTAL", delArchivo.total == enBase.total && sumaArchivo == delArchivo.total,
                    Igualdad(delArchivo.total.ToString("N2"), enBase.total.ToString("N2"))));
            }

            bitacora.RegistrarBitacora(usuario, TipoAccion.DeserializacionOrdenCompra);
            return resultado;
        }

        // Si se edita a mano un campo que entra en el digito verificador, el DVH guardado en el archivo deja de coincidir
        private static ResultadoVerificacion_575_AV VerificarDigitos(OrdenCompraBE_575_AV orden)
        {
            List<string> distintos = new List<string>();
            if (VerificadorIntegridad.CalcularDVH(orden) != orden.dvh) { distintos.Add(orden.nro_orden_compra); }
            for (int i = 0; i < orden.detalle.Count; i++)
            {
                if (VerificadorIntegridad.CalcularDVH(orden.detalle[i]) != orden.detalle[i].dvh)
                {
                    distintos.Add("#" + (i + 1) + " " + orden.detalle[i].codigo_articulo);
                }
            }

            int total = orden.detalle.Count + 1;
            return Verificacion("SER_VER_DIGITO", distintos.Count == 0,
                distintos.Count == 0 ? total + "/" + total : string.Join(", ", distintos));
        }

        private static ResultadoVerificacion_575_AV CompararRenglon(DetalleOrdenCompraBE_575_AV renglon, DetalleOrdenCompraBE_575_AV guardado, int numero)
        {
            if (guardado == null)
            {
                return Verificacion("SER_VER_RENGLON_BD", false, "id_detalle_orden_compra " + renglon.id_detalle_orden_compra + " ∉",
                    numero.ToString(), renglon.codigo_articulo);
            }

            List<string> distintos = new List<string>();
            if (renglon.id_articulo != guardado.id_articulo) { distintos.Add(Diferencia("id_articulo", renglon.id_articulo, guardado.id_articulo)); }
            if (renglon.cantidad != guardado.cantidad) { distintos.Add(Diferencia("cantidad", renglon.cantidad, guardado.cantidad)); }
            if (renglon.precio_unitario != guardado.precio_unitario) { distintos.Add(Diferencia("precio_unitario", renglon.precio_unitario, guardado.precio_unitario)); }
            if (renglon.subtotal != guardado.subtotal) { distintos.Add(Diferencia("subtotal", renglon.subtotal, guardado.subtotal)); }

            return Verificacion("SER_VER_RENGLON_BD", distintos.Count == 0,
                distintos.Count == 0 ? "4/4" : string.Join("; ", distintos), numero.ToString(), renglon.codigo_articulo);
        }

        #endregion

        #region Apoyo

        private DocumentoOrdenCompra_575_AV Leer(string ruta)
        {
            DocumentoOrdenCompra_575_AV documento;
            try
            {
                documento = serializador.Deserializar<DocumentoOrdenCompra_575_AV>(ruta);
            }
            catch (FormatException ex)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir(ex.Message), ex);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is ArgumentException || ex is NotSupportedException)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("SER_ERR_LECTURA"), ex);
            }

            if (documento == null || documento.orden == null || documento.proveedor == null || documento.orden.detalle == null
                || string.IsNullOrEmpty(documento.orden.nro_orden_compra))
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir(Serializador_575_AV.ERROR_FORMATO));
            }
            return documento;
        }

        // Compara una por una las propiedades simples (no las listas) de dos objetos del mismo tipo
        private static ResultadoVerificacion_575_AV CompararCampos(string clave, object esperado, object obtenido, params string[] argumentos)
        {
            if (esperado == null || obtenido == null)
            {
                return Verificacion(clave, esperado == null && obtenido == null, Igualdad(esperado, obtenido), argumentos);
            }

            List<PropertyInfo> campos = esperado.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite
                            && (p.PropertyType == typeof(string) || !typeof(IEnumerable).IsAssignableFrom(p.PropertyType)))
                .ToList();

            List<string> distintos = new List<string>();
            foreach (PropertyInfo campo in campos)
            {
                object a = campo.GetValue(esperado);
                object b = campo.GetValue(obtenido);
                if (!Equals(a, b)) { distintos.Add(Diferencia(campo.Name, a, b)); }
            }

            return Verificacion(clave, distintos.Count == 0,
                distintos.Count == 0 ? campos.Count + "/" + campos.Count : string.Join("; ", distintos), argumentos);
        }

        private static ResultadoVerificacion_575_AV Verificacion(string clave, bool correcto, string detalle, params string[] argumentos)
        {
            return new ResultadoVerificacion_575_AV
            {
                clave = clave,
                correcto = correcto,
                detalle = detalle,
                argumentos = argumentos ?? new string[0]
            };
        }

        private static string Diferencia(string campo, object a, object b)
        {
            return campo + ": " + Valor(a) + " ≠ " + Valor(b);
        }

        private static string Igualdad(object a, object b)
        {
            return Valor(a) + (Equals(a, b) ? " = " : " ≠ ") + Valor(b);
        }

        private static string Valor(object valor)
        {
            if (valor == null) { return "∅"; }
            if (valor is decimal) { return ((decimal)valor).ToString("N2"); }
            if (valor is DateTime) { return ((DateTime)valor).ToString("dd/MM/yyyy HH:mm:ss"); }
            return valor.ToString();
        }

        #endregion
    }
}
