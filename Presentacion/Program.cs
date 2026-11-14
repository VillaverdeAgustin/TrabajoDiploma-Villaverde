using Negocio_BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Presentacion_575_AV
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!new ConexionBLL_575_AV().VerificarConexionActual())
            {
                using (frmConexion_575_AV conexion = new frmConexion_575_AV())
                {
                    if (conexion.ShowDialog() != DialogResult.OK) { return; }
                }
            }

            Application.Run(new frmLogin());
        }
    }
}
