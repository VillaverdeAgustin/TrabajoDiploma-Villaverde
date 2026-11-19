using Negocio_BLL;
using Servicios;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Presentacion_575_AV
{
    public partial class frmRecalcular : Form, IObservadorIdioma
    {
        private readonly GestorDeIdioma gestorIdioma = GestorDeIdioma.GetInstance;

        public frmRecalcular()
        {
            InitializeComponent();
            AyudaFormulario_575_AV.Asociar(this, "login");
            EstiloConectAR_575_AV.AplicarFormulario(this);
            EstiloConectAR_575_AV.BotonPrimario(btnRecalcular);
            EstiloConectAR_575_AV.BotonSecundario(btnRestaurar);
            EstiloConectAR_575_AV.BotonPeligro(btnSalir);

            lblRestaurar.ForeColor = EstiloConectAR_575_AV.TextoSuave;

            this.Load += (s, a) => gestorIdioma.Suscribir(this);
            this.FormClosed += (s, a) => gestorIdioma.Desuscribir(this);
        }

        public void ActualizarTextos()
        {
            this.Text = gestorIdioma.Traducir("REC_TITULO");
            lblTitulo.Text = gestorIdioma.Traducir("REC_TITULO");
            label1.Text = gestorIdioma.Traducir("REC_LBL_TABLAS");
            lblAyuda.Text = gestorIdioma.Traducir("REC_LBL_AYUDA");
            btnRecalcular.Text = gestorIdioma.Traducir("REC_BTN_RECALCULAR");
            btnRestaurar.Text = gestorIdioma.Traducir("REC_BTN_RESTAURAR");
            btnSalir.Text = gestorIdioma.Traducir("COMUN_SALIR");
            lblRestaurar.Text = gestorIdioma.Traducir("REC_LBL_RESTAURAR_T07");
        }

        public void IndicarTablasAfectadas(List<string> tablas)
        {
            lstTablas.Items.Clear();
            if (tablas == null) { return; }

            foreach (string tabla in tablas)
            {
                lstTablas.Items.Add(tabla);
            }
        }
    }
}
