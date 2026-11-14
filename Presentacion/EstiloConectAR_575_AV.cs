using System.Drawing;
using System.Windows.Forms;

namespace Presentacion_575_AV
{
    public static class EstiloConectAR_575_AV
    {
        #region Paleta institucional (tomada del logotipo)

        public static readonly Color Navy = Color.FromArgb(13, 59, 92);
        public static readonly Color Cian = Color.FromArgb(0, 169, 224);
        public static readonly Color NavySuave = Color.FromArgb(49, 88, 116);

        public static readonly Color Fondo = Color.FromArgb(242, 246, 249);
        public static readonly Color Superficie = Color.White;
        public static readonly Color Borde = Color.FromArgb(214, 225, 232);
        public static readonly Color TextoSuave = Color.FromArgb(90, 113, 131);
        public static readonly Color Peligro = Color.FromArgb(192, 57, 43);
        public static readonly Color Exito = Color.FromArgb(30, 142, 90);

        #endregion

        #region Tipografia

        public const string Familia = "Segoe UI";

        public static readonly Font FuenteBase = new Font(Familia, 9.75F, FontStyle.Regular);
        public static readonly Font FuenteEtiqueta = new Font(Familia, 9.75F, FontStyle.Bold);
        public static readonly Font FuenteCampo = new Font(Familia, 10.5F, FontStyle.Regular);
        public static readonly Font FuenteBoton = new Font(Familia, 9.75F, FontStyle.Bold);
        public static readonly Font FuenteTitulo = new Font(Familia, 15F, FontStyle.Bold);
        public static readonly Font FuenteBajada = new Font(Familia, 8.25F, FontStyle.Regular);
        public static readonly Font FuenteEnlace = new Font(Familia, 9.75F, FontStyle.Underline);
        public static readonly Font FuenteEnlaceActivo = new Font(Familia, 9.75F, FontStyle.Underline | FontStyle.Bold);

        #endregion

        #region Aplicacion

        public static void AplicarFormulario(Form frm)
        {
            if (frm == null) { return; }

            frm.BackColor = Fondo;
            frm.Font = FuenteBase;
            try { frm.Icon = Properties.Resources.IconoConectAR; }
            catch {  }

            AplicarControles(frm.Controls);
        }

        public static void AplicarControles(Control.ControlCollection controles)
        {
            foreach (Control ctrl in controles)
            {
                if (ctrl is ucAlfaNum)
                {
                    ((ucAlfaNum)ctrl).AplicarEstilo();
                    continue;
                }

                if (ctrl is Button) { AplicarBoton((Button)ctrl); }
                else if (ctrl is Label) { AplicarEtiqueta((Label)ctrl); }
                else if (ctrl is TextBox) { AplicarCampo((TextBox)ctrl); }
                else if (ctrl is ComboBox) { AplicarCombo((ComboBox)ctrl); }
                else if (ctrl is DataGridView) { AplicarGrilla((DataGridView)ctrl); }
                else if (ctrl is TreeView) { AplicarArbol((TreeView)ctrl); }
                else if (ctrl is GroupBox) { AplicarGrupo((GroupBox)ctrl); }
                else if (ctrl is MenuStrip) { AplicarMenu((MenuStrip)ctrl); }
                else if (ctrl is DateTimePicker) { ctrl.Font = FuenteBase; }

                if (ctrl.HasChildren) { AplicarControles(ctrl.Controls); }
            }
        }

        public static void AplicarBoton(Button btn)
        {
            bool destructivo = btn.BackColor == Color.LightCoral
                            || btn.BackColor == Color.Firebrick
                            || btn.BackColor == Color.Red
                            || btn.ForeColor == Color.Firebrick;

            if (destructivo) { BotonPeligro(btn); } else { BotonPrimario(btn); }
        }

        public static void BotonPrimario(Button btn)
        {
            EstiloBoton(btn, Navy, Color.White, Navy);
            btn.FlatAppearance.MouseOverBackColor = Cian;
            btn.FlatAppearance.MouseDownBackColor = NavySuave;
        }

        public static void BotonSecundario(Button btn)
        {
            EstiloBoton(btn, Superficie, Navy, Borde);
            btn.FlatAppearance.MouseOverBackColor = Fondo;
            btn.FlatAppearance.MouseDownBackColor = Borde;
        }

        public static void BotonPeligro(Button btn)
        {
            EstiloBoton(btn, Superficie, Peligro, Peligro);
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(250, 235, 233);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(244, 214, 210);
        }

        private static void EstiloBoton(Button btn, Color fondo, Color texto, Color borde)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = borde;
            btn.BackColor = fondo;
            btn.ForeColor = texto;
            btn.Font = FuenteBoton;
            btn.Cursor = Cursors.Hand;
            btn.UseVisualStyleBackColor = false;
        }

        public static void EstadoBoton(Button btn, bool habilitado)
        {
            btn.Enabled = habilitado;
            btn.Font = FuenteBoton;
            btn.BackColor = habilitado ? Navy : Fondo;
            btn.ForeColor = habilitado ? Color.White : TextoSuave;
            btn.FlatAppearance.BorderColor = habilitado ? Navy : Borde;
        }

        public static void BotonSeleccionado(Button btn)
        {
            btn.BackColor = Cian;
            btn.ForeColor = Color.White;
            btn.FlatAppearance.BorderColor = Cian;
        }

        public static void CampoEditable(TextBox txt)
        {
            txt.BackColor = Superficie;
            txt.ForeColor = Navy;
        }

        public static void CampoBloqueado(TextBox txt)
        {
            txt.BackColor = Fondo;
            txt.ForeColor = TextoSuave;
        }

        public static void CampoConError(TextBox txt)
        {
            txt.BackColor = Color.FromArgb(250, 235, 233);
            txt.ForeColor = Peligro;
        }

        public static void AplicarEtiqueta(Label lbl)
        {
            if (lbl.ForeColor == Color.Red || lbl.ForeColor == Color.Firebrick)
            {
                lbl.ForeColor = Peligro;
                lbl.Font = FuenteEtiqueta;
                return;
            }

            if (lbl.Font != null && lbl.Font.Underline)
            {
                lbl.Font = FuenteEnlace;
                lbl.ForeColor = NavySuave;
                return;
            }

            bool destacada = lbl.Font != null && lbl.Font.Bold;
            lbl.Font = destacada ? FuenteEtiqueta : FuenteBase;
            lbl.ForeColor = destacada ? Navy : TextoSuave;
        }

        public static void AplicarCampo(TextBox txt)
        {
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.BackColor = Superficie;
            txt.ForeColor = Navy;
            txt.Font = FuenteCampo;
        }

        public static void AplicarCombo(ComboBox cmb)
        {
            cmb.FlatStyle = FlatStyle.Flat;
            cmb.BackColor = Superficie;
            cmb.ForeColor = Navy;
            cmb.Font = FuenteCampo;
        }

        public static void AplicarGrupo(GroupBox grp)
        {
            grp.ForeColor = Navy;
            grp.Font = FuenteEtiqueta;
        }

        public static void AplicarArbol(TreeView arbol)
        {
            arbol.BackColor = Superficie;
            arbol.ForeColor = Navy;
            arbol.BorderStyle = BorderStyle.FixedSingle;
            arbol.Font = FuenteBase;
            arbol.LineColor = Borde;
        }

        public static void AplicarGrilla(DataGridView grilla)
        {
            grilla.BorderStyle = BorderStyle.None;
            grilla.BackgroundColor = Superficie;
            grilla.GridColor = Borde;
            grilla.EnableHeadersVisualStyles = false;
            grilla.RowHeadersVisible = false;
            grilla.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grilla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grilla.ColumnHeadersHeight = 34;

            grilla.ColumnHeadersDefaultCellStyle.BackColor = Navy;
            grilla.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grilla.ColumnHeadersDefaultCellStyle.Font = FuenteEtiqueta;
            grilla.ColumnHeadersDefaultCellStyle.SelectionBackColor = Navy;
            grilla.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
            grilla.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 6, 0);

            grilla.DefaultCellStyle.BackColor = Superficie;
            grilla.DefaultCellStyle.ForeColor = Navy;
            grilla.DefaultCellStyle.Font = FuenteBase;
            grilla.DefaultCellStyle.SelectionBackColor = Cian;
            grilla.DefaultCellStyle.SelectionForeColor = Color.White;
            grilla.DefaultCellStyle.Padding = new Padding(6, 4, 6, 4);

            grilla.AlternatingRowsDefaultCellStyle.BackColor = Fondo;
            grilla.AlternatingRowsDefaultCellStyle.ForeColor = Navy;
            grilla.AlternatingRowsDefaultCellStyle.SelectionBackColor = Cian;
            grilla.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

            grilla.RowsDefaultCellStyle.Font = FuenteBase;
            grilla.RowTemplate.Height = 28;
        }

        public static void MostrarColumnas(DataGridView grilla, params string[] columnas)
        {
            if (grilla.Columns.Count == 0) { return; }

            foreach (DataGridViewColumn columna in grilla.Columns)
            {
                columna.Visible = false;
            }

            for (int i = 0; i < columnas.Length; i++)
            {
                DataGridViewColumn columna = grilla.Columns[columnas[i]];
                if (columna == null) { continue; }

                columna.Visible = true;
                columna.DisplayIndex = i;
            }
        }

        public static void FormatoNumero(DataGridView grilla, params string[] columnas)
        {
            foreach (string nombre in columnas)
            {
                DataGridViewColumn columna = grilla.Columns[nombre];
                if (columna == null) { continue; }

                columna.DefaultCellStyle.Format = "N2";
                columna.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        public static void ColumnaElastica(DataGridView grilla, string columna)
        {
            foreach (DataGridViewColumn col in grilla.Columns)
            {
                if (!col.Visible || col.Name == columna) { continue; }

                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                if (col.ValueType == typeof(System.DateTime) && string.IsNullOrEmpty(col.DefaultCellStyle.Format))
                {
                    col.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                }
            }

            if (grilla.Columns[columna] != null)
            {
                grilla.Columns[columna].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        public static void AplicarMenu(MenuStrip menu)
        {
            menu.Renderer = new RenderizadorMenuConectAR_575_AV();
            menu.BackColor = Navy;
            menu.ForeColor = Color.White;
            menu.Font = FuenteEtiqueta;
            menu.Padding = new Padding(8, 2, 0, 2);
        }

        #endregion
    }

    public class ColoresMenuConectAR_575_AV : ProfessionalColorTable
    {
        public override Color MenuStripGradientBegin { get { return EstiloConectAR_575_AV.Navy; } }
        public override Color MenuStripGradientEnd { get { return EstiloConectAR_575_AV.Navy; } }

        public override Color MenuItemSelected { get { return EstiloConectAR_575_AV.Cian; } }
        public override Color MenuItemSelectedGradientBegin { get { return EstiloConectAR_575_AV.Cian; } }
        public override Color MenuItemSelectedGradientEnd { get { return EstiloConectAR_575_AV.Cian; } }
        public override Color MenuItemPressedGradientBegin { get { return EstiloConectAR_575_AV.Navy; } }
        public override Color MenuItemPressedGradientEnd { get { return EstiloConectAR_575_AV.Navy; } }
        public override Color MenuItemBorder { get { return EstiloConectAR_575_AV.Cian; } }

        public override Color ToolStripDropDownBackground { get { return EstiloConectAR_575_AV.Superficie; } }
        public override Color ImageMarginGradientBegin { get { return EstiloConectAR_575_AV.Superficie; } }
        public override Color ImageMarginGradientMiddle { get { return EstiloConectAR_575_AV.Superficie; } }
        public override Color ImageMarginGradientEnd { get { return EstiloConectAR_575_AV.Superficie; } }
        public override Color MenuBorder { get { return EstiloConectAR_575_AV.Borde; } }
        public override Color SeparatorDark { get { return EstiloConectAR_575_AV.Borde; } }
        public override Color SeparatorLight { get { return EstiloConectAR_575_AV.Borde; } }
    }

    public class RenderizadorMenuConectAR_575_AV : ToolStripProfessionalRenderer
    {
        public RenderizadorMenuConectAR_575_AV() : base(new ColoresMenuConectAR_575_AV()) { }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            bool primerNivel = e.Item.OwnerItem == null;

            if (!e.Item.Enabled)
            {
                e.TextColor = primerNivel ? EstiloConectAR_575_AV.NavySuave : EstiloConectAR_575_AV.Borde;
            }
            else if (primerNivel)
            {
                e.TextColor = Color.White;
            }
            else
            {
                e.TextColor = e.Item.Selected ? Color.White : EstiloConectAR_575_AV.Navy;
            }

            base.OnRenderItemText(e);
        }
    }
}
