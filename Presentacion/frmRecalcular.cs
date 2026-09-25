using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Presentacion_575_AV
{
    public partial class frmRecalcular : Form
    {
        public frmRecalcular()
        {
            InitializeComponent();
            EstiloConectAR_575_AV.AplicarFormulario(this);
        }

        /// <summary>
        /// Nombra las tablas que no pasaron el control de integridad. Se dice
        /// que tabla esta afectada, sin exponer detalle tecnico alguno.
        /// </summary>
        public void IndicarTablasAfectadas(List<string> tablas)
        {
            if (tablas == null || tablas.Count == 0) { return; }

            label1.Text = "Se detectaron diferencias en: " + string.Join(", ", tablas) +
                          ". Desea recalcular los digitos verificadores?";

            AcomodarAlTexto();
        }

        /// <summary>Da lugar al texto mas largo corriendo los botones hacia abajo.</summary>
        private void AcomodarAlTexto()
        {
            int alto = TextRenderer.MeasureText(
                label1.Text, label1.Font,
                new Size(label1.Width, 0), TextFormatFlags.WordBreak).Height;

            int crecimiento = alto - label1.Height + 8;
            if (crecimiento <= 0) { return; }

            label1.Height += crecimiento;
            btnRecalcular.Top += crecimiento;
            btnCancelar.Top += crecimiento;
            this.Height += crecimiento;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
        }

        private void btnRecalcular_Click(object sender, EventArgs e)
        {
        }
    }
}
