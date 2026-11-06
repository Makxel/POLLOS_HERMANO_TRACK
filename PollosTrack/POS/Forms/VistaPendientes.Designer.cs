namespace PollosTrack.POS.Forms
{
    partial class VistaPendientes
    {
        // Controles fijos del monitor; las tarjetas se construyen al leer los pedidos.
        private void InitializeComponent()
        {
            panelCabecera = new Panel();
            etiquetaTitulo = new Label();
            etiquetaDescripcion = new Label();
            botonActualizar = new Button();
            panelPedidos = new FlowLayoutPanel();
            etiquetaResumen = new Label();
            SuspendLayout();
            panelCabecera.Dock = DockStyle.Top;
            panelCabecera.Height = 105;
            etiquetaTitulo.Text = "Cola de cocina";
            etiquetaTitulo.Font = new Font("Segoe UI", 19F, FontStyle.Bold);
            etiquetaTitulo.Dock = DockStyle.Top;
            etiquetaTitulo.Height = 44;
            etiquetaDescripcion.Text = "Primero los productos más antiguos · Pendiente → En preparación → Entregado";
            etiquetaDescripcion.Dock = DockStyle.Fill;
            botonActualizar.Text = "Actualizar";
            botonActualizar.Dock = DockStyle.Right;
            botonActualizar.Width = 125;
            botonActualizar.BackColor = Color.FromArgb(255, 209, 79);
            botonActualizar.FlatStyle = FlatStyle.Flat;
            botonActualizar.FlatAppearance.BorderSize = 0;
            botonActualizar.Click += AlActualizarMonitor;
            panelCabecera.Controls.Add(etiquetaDescripcion);
            panelCabecera.Controls.Add(etiquetaTitulo);
            panelCabecera.Controls.Add(botonActualizar);
            panelPedidos.Dock = DockStyle.Fill;
            panelPedidos.AutoScroll = true;
            panelPedidos.BackColor = Color.FromArgb(235, 240, 247);
            panelPedidos.Padding = new Padding(8);
            etiquetaResumen.Dock = DockStyle.Bottom;
            etiquetaResumen.Height = 48;
            etiquetaResumen.TextAlign = ContentAlignment.MiddleLeft;
            etiquetaResumen.Text = "Actualiza para consultar los pedidos.";
            Controls.Add(panelPedidos);
            Controls.Add(etiquetaResumen);
            Controls.Add(panelCabecera);
            BackColor = Color.FromArgb(246, 248, 251);
            ForeColor = Color.FromArgb(24, 49, 83);
            Padding = new Padding(16);
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Size = new Size(900, 650);
            Name = "VistaPendientes";
            Load += AlCargarMonitor;
            ResumeLayout(false);
        }

        private Panel panelCabecera;
        private Label etiquetaTitulo;
        private Label etiquetaDescripcion;
        private Button botonActualizar;
        private FlowLayoutPanel panelPedidos;
        private Label etiquetaResumen;
    }
}
