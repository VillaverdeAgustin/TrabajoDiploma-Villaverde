using Negocio_BLL;
using System;
using System.Windows.Forms;

namespace Presentacion_575_AV
{
    // Textos de combos y grillas que se resuelven al dibujarse, para que sigan el idioma activo
    // sin cambiar el valor enlazado.
    public static class TextoTraducido_575_AV
    {
        // Un valor de enum se muestra con la clave prefijo + nombre del valor.
        public static void Enum(ComboBox combo, string prefijo)
        {
            combo.FormattingEnabled = true;
            combo.Format += (s, e) =>
            {
                if (e.ListItem is System.Enum) { e.Value = GestorDeIdioma.GetInstance.Traducir(prefijo + e.ListItem); }
            };
        }

        public static void Enum(DataGridView grilla, string columna, string prefijo)
        {
            grilla.CellFormatting += (s, e) =>
            {
                if (e.ColumnIndex >= 0 && grilla.Columns[e.ColumnIndex].Name == columna && e.Value is System.Enum)
                {
                    e.Value = GestorDeIdioma.GetInstance.Traducir(prefijo + e.Value);
                    e.FormattingApplied = true;
                }
            };
        }

        // El primer elemento del combo es la opcion "todos" que se agrega en pantalla.
        public static void PrimeraOpcion(ComboBox combo, string clave)
        {
            combo.FormattingEnabled = true;
            combo.Format += (s, e) =>
            {
                if (combo.Items.Count > 0 && ReferenceEquals(e.ListItem, combo.Items[0]))
                {
                    e.Value = GestorDeIdioma.GetInstance.Traducir(clave);
                }
            };
        }

        // Vuelve a enlazar el combo para que redibuje sus textos, conservando la seleccion.
        public static void Refrescar(ComboBox combo)
        {
            if (combo.IsDisposed || combo.DataSource == null) { return; }
            object origen = combo.DataSource;
            string mostrar = combo.DisplayMember;
            string valor = combo.ValueMember;
            int indice = combo.SelectedIndex;
            combo.DataSource = null;
            combo.DataSource = origen;
            combo.DisplayMember = mostrar;
            combo.ValueMember = valor;
            combo.SelectedIndex = indice;
        }
    }
}
