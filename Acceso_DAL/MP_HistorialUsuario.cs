using Entidad_BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acceso_DAL
{
    public class MP_HistorialUsuario
    {
        AccesoDatos conexDB = new AccesoDatos();


        public List<HistorialUsuarioBE> ListarHistorial()
        {
            List<HistorialUsuarioBE> historial = new List<HistorialUsuarioBE>();
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("tabla", "HistorialUsuario");
            DataTable dt = conexDB.LeerTabla("SP_ExtTabla", parametros);
            foreach (DataRow dr in dt.Rows)
            {
                historial.Add(MapearFila(dr));
            }
            return historial;
        }

        public List<HistorialUsuarioBE> ListarHistorial(TipoAccion? accion, DateTime fIni, DateTime fFin)
        {
            List<HistorialUsuarioBE> historial = new List<HistorialUsuarioBE>();
            SqlParameter[] parametros = new SqlParameter[3];
            if (accion == null) { parametros[0] = new SqlParameter("@accion", DBNull.Value); }
            else { parametros[0] = new SqlParameter("@accion", (int)accion); }
            parametros[1] = new SqlParameter("@fechaDesde", fIni);
            parametros[2] = new SqlParameter("@fechaHasta", fFin);

            DataTable dt = conexDB.LeerTabla("SP_ListarHistorialUsuario", parametros);
            foreach (DataRow dr in dt.Rows)
            {
                historial.Add(MapearFila(dr));
            }
            return historial;
        }

        public HistorialUsuarioBE ObtenerVersion(int idHistorial)
        {
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@idHistorial", idHistorial);

            DataTable dt = conexDB.LeerTabla("SP_ExtHistorialUsuario", parametros);
            if (dt.Rows.Count > 0) { return MapearFila(dt.Rows[0]); }
            return null;
        }

        private HistorialUsuarioBE MapearFila(DataRow dr)
        {
            HistorialUsuarioBE h = new HistorialUsuarioBE();
            h.idHistorial = Convert.ToInt32(dr[0].ToString());
            h.usuarioId = Convert.ToInt32(dr[1].ToString());
            h.dni = dr[2].ToString();
            h.nombre = dr[3].ToString();
            h.apellido = dr[4].ToString();
            h.usuario = dr[5].ToString();
            h.rol = dr[6].ToString();
            h.direccion = dr[7] == DBNull.Value ? null : dr[7].ToString();
            h.telefono = dr[8] == DBNull.Value ? null : dr[8].ToString();
            h.email = dr[9] == DBNull.Value ? null : dr[9].ToString();
            h.activo = Convert.ToBoolean(dr[10]);
            h.bloqueado = Convert.ToBoolean(dr[11]);
            h.accion = (TipoAccion)Convert.ToInt32(dr[12].ToString());
            h.usuarioResponsable = dr[13].ToString();
            h.fecha = Convert.ToDateTime(dr[14].ToString());
            return h;
        }
    }
}
