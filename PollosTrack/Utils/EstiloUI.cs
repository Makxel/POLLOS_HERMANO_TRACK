namespace PollosTrack.Utils
{
    public static class EstiloUI
    {
        public static readonly Color Azul = Color.FromArgb(24, 49, 83);
        public static readonly Color Amarillo = Color.FromArgb(255, 209, 79);
        public static readonly Color Crema = Color.FromArgb(255, 248, 229);

        public static void ConfigurarGrid(DataGridView grid)
        {
            grid.Dock = DockStyle.Fill;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.RowHeadersVisible = false;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Azul;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = 36;
            grid.RowTemplate.Height = 30;
            grid.DefaultCellStyle.SelectionBackColor = Amarillo;
            grid.DefaultCellStyle.SelectionForeColor = Azul;
        }

        public static void EstilizarBoton(Button boton, bool principal)
        {
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = principal ? 0 : 1;
            boton.FlatAppearance.BorderColor = Azul;
            boton.BackColor = principal ? Amarillo : Color.White;
            boton.ForeColor = Azul;
            boton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            boton.Cursor = Cursors.Hand;
        }

        // Agrega una etiqueta y su campo debajo; devuelve la siguiente posición vertical
        public static int AgregarCampo(Control panel, string etiqueta, Control campo, int y, int alto = 28)
        {
            int ancho = panel.Width - 30;
            panel.Controls.Add(new Label { Text = etiqueta, ForeColor = Azul, Location = new Point(15, y) });
            campo.Bounds = new Rectangle(15, y + 22, ancho, alto);
            panel.Controls.Add(campo);
            return y + 22 + alto + 12;
        }
    }
}