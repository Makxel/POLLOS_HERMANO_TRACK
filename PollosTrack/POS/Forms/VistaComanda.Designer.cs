namespace PollosTrack.POS.Forms
{
    partial class VistaComanda
    {
        private void InitializeComponent()
        {
            etiquetaDescripcion = new Label();
            botonVolverAMesas = new Button();
            SuspendLayout();
            etiquetaDescripcion.Dock = DockStyle.Fill;
            etiquetaDescripcion.Name = "etiquetaDescripcion";
            etiquetaDescripcion.Text = "Gestión de comandas · Pantalla pendiente de implementar";
            etiquetaDescripcion.TextAlign = ContentAlignment.MiddleCenter;
            // Acceso de regreso al mapa; la navegación la resuelve FormPrincipal.
            botonVolverAMesas.Name = "botonVolverAMesas";
            botonVolverAMesas.Text = "Volver a mesas";
            botonVolverAMesas.Dock = DockStyle.Top;
            botonVolverAMesas.Height = 44;
            botonVolverAMesas.BackColor = Color.FromArgb(255, 209, 79);
            botonVolverAMesas.ForeColor = Color.Black;
            botonVolverAMesas.FlatStyle = FlatStyle.Flat;
            botonVolverAMesas.FlatAppearance.BorderSize = 0;
            botonVolverAMesas.Click += AlVolverAMesas;
            Controls.Add(etiquetaDescripcion);
            Controls.Add(botonVolverAMesas);
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Size = new Size(700, 500);
            Name = "VistaComanda";
            ResumeLayout(false);
        }

        private Label etiquetaDescripcion;
        private Button botonVolverAMesas;
    }
}
