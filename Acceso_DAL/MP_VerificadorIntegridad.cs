using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acceso_DAL
{
    public class MP_VerificadorIntegridad
    {
        AccesoDatos conexDB = new AccesoDatos();
        
        public List<string> ExtraerDVH(string tabla)
        {
            List<string> dvh = new List<string>();
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@tabla", tabla);
            DataTable dt = conexDB.LeerTabla("SP_ExtraerDVH", param);
            foreach (DataRow dr in dt.Rows)
            {
                dvh.Add(dr[0].ToString());
            }
            return dvh;
        }

        public void ActualizarDVV(string dvv, string tabla)
        {
            SqlParameter[] param = new SqlParameter[2];
            param[0] = new SqlParameter("dvv",dvv);
            param[1] = new SqlParameter("tabla", tabla);
            conexDB.Escribir("SP_ActualizarDVV",param);
        }

        public string ExtraerDVV(string tabla)
        { 
            SqlParameter[] param = new SqlParameter[2];
            param[0] = new SqlParameter("@tabla", "DigitoVertical");
            param[1] = new SqlParameter("@tablaVerif", tabla);
            object dvv = conexDB.ExtraerDato("SP_ExtraerDVV", param);
            if(dvv != null)
            {
                return dvv.ToString();
            }
            return "";
        }
    }
}
