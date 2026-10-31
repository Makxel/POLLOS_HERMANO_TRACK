namespace PollosTrack.POS.Forms
{
    partial class VistaPendientes
    {
        private void InitializeComponent()
        {
            etiquetaDescripcion = new Label();
            SuspendLayout();
            etiquetaDescripcion.Dock = DockStyle.Fill;
            etiquetaDescripcion.Name = "etiquetaDescripcion";
            etiquetaDescripcion.Text = "Monitor de pendientes · Pantalla pendiente de implementar";
            etiquetaDescripcion.TextAlign = ContentAlignment.MiddleCenter;
            Controls.Add(etiquetaDescripcion);
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Size = new Size(700, 500);
            Name = "VistaPendientes";
            ResumeLayout(false);
        }

        private Label etiquetaDescripcion;
    }
}
