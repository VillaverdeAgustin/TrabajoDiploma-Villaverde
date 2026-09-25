using Entidad_BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Acceso_DAL
{
    /// <summary>
    /// Mapeador de Cliente. El correo viaja tal cual esta en la base, cifrado:
    /// descifrarlo es tarea de la BLL, no de esta capa.
    /// </summary>
    public class MP_Cliente_575_AV
    {
        AccesoDatos conexDB = new AccesoDatos();

        public List<ClienteBE_575_AV> ListarClientes()
        {
            return Proyectar(conexDB.LeerTabla("SP_ExtCliente", null));
        }

        /// <summary>Entrada del circuito de caja. Devuelve nulo si no existe.</summary>
        public ClienteBE_575_AV BuscarPorDni(string dni)
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@dni", dni);

            return Proyectar(conexDB.LeerTabla("SP_BuscarClientePorDni", parametros))
                .FirstOrDefault();
        }

        /// <summary>Devuelve nulo si no existe.</summary>
        public ClienteBE_575_AV BuscarPorCuit(string cuit)
        {
            SqlParameter[] parametros = new SqlParameter[3];
            parametros[0] = new SqlParameter("@dni", DBNull.Value);
            parametros[1] = new SqlParameter("@cuit", cuit);
            parametros[2] = new SqlParameter("@razon_social", DBNull.Value);

            return Proyectar(conexDB.LeerTabla("SP_BuscarCliente", parametros))
                .FirstOrDefault();
        }

        /// <summary>Alta. Devuelve el id_cliente generado para calcular el dvh.</summary>
        public int CrearCliente(ClienteBE_575_AV cliente)
        {
            SqlParameter[] parametros = new SqlParameter[10];
            parametros[0] = new SqlParameter("@razon_social", Texto(cliente.razon_social));
            parametros[1] = new SqlParameter("@nombre", cliente.nombre);
            parametros[2] = new SqlParameter("@apellido", cliente.apellido);
            parametros[3] = new SqlParameter("@dni", cliente.dni);
            parametros[4] = new SqlParameter("@cuit", Texto(cliente.cuit));
            parametros[5] = new SqlParameter("@condicion_iva", cliente.condicion_iva.ToString());
            parametros[6] = new SqlParameter("@direccion", Texto(cliente.direccion));
            parametros[7] = new SqlParameter("@telefono", Texto(cliente.telefono));
            parametros[8] = new SqlParameter("@correo_electronico", Texto(cliente.correo_electronico));
            parametros[9] = new SqlParameter("@fecha_actualizacion", cliente.fecha_actualizacion);

            object id = conexDB.EscribirRetornar("SP_CrearCliente", parametros);
            return id == null ? 0 : Convert.ToInt32(id);
        }

        public void ActualizarDVH(int id_cliente, string dvh)
        {
            SqlParameter[] parametros = new SqlParameter[3];
            parametros[0] = new SqlParameter("@tabla", "Cliente");
            parametros[1] = new SqlParameter("@id", id_cliente);
            parametros[2] = new SqlParameter("@dvh", dvh);

            conexDB.Escribir("SP_ActualizarDVHNegocio", parametros);
        }

        private static object Texto(string valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? (object)DBNull.Value : valor;
        }

        private List<ClienteBE_575_AV> Proyectar(DataTable dt)
        {
            List<ClienteBE_575_AV> clientes = new List<ClienteBE_575_AV>();
            foreach (DataRow dr in dt.Rows)
            {
                CondicionIva_575_AV condicion;
                if (!Enum.TryParse(dr["condicion_iva"].ToString(), out condicion))
                {
                    condicion = CondicionIva_575_AV.ConsumidorFinal;
                }

                clientes.Add(new ClienteBE_575_AV
                {
                    id_cliente = Convert.ToInt32(dr["id_cliente"]),
                    razon_social = Leer(dr["razon_social"]),
                    nombre = dr["nombre"].ToString(),
                    apellido = dr["apellido"].ToString(),
                    dni = dr["dni"].ToString(),
                    cuit = Leer(dr["cuit"]),
                    condicion_iva = condicion,
                    direccion = Leer(dr["direccion"]),
                    telefono = Leer(dr["telefono"]),
                    correo_electronico = Leer(dr["correo_electronico"]),
                    fecha_actualizacion = Convert.ToDateTime(dr["fecha_actualizacion"]),
                    activo = Convert.ToBoolean(dr["activo"]),
                    dvh = Leer(dr["dvh"])
                });
            }
            return clientes;
        }

        private static string Leer(object valor)
        {
            return valor == DBNull.Value ? null : valor.ToString();
        }
    }
}
