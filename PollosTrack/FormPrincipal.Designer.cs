namespace PollosTrack
{
    partial class FormPrincipal
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código del diseñador de Windows Forms

        private void InitializeComponent()
        {
            panelBarraLateral = new Panel();
            etiquetaPieBarra = new Label();
            botonCatalogos = new Button();
            botonInventario = new Button();
            botonPOS = new Button();
            botonInicio = new Button();
            panelEmpresa = new Panel();
            etiquetaDescripcion = new Label();
            etiquetaNombreEmpresa = new Label();
            etiquetaLogo = new Label();
            panelEncabezado = new Panel();
            etiquetaTitulo = new Label();
            panelContenido = new Panel();
            panelBarraLateral.SuspendLayout();
            panelEmpresa.SuspendLayout();
            panelEncabezado.SuspendLayout();
            SuspendLayout();
            // 
            // panelBarraLateral
            // 
            panelBarraLateral.BackColor = Color.FromArgb(24, 49, 83);
            panelBarraLateral.Controls.Add(etiquetaPieBarra);
            panelBarraLateral.Controls.Add(botonCatalogos);
            panelBarraLateral.Controls.Add(botonInventario);
            panelBarraLateral.Controls.Add(botonPOS);
            panelBarraLateral.Controls.Add(botonInicio);
            panelBarraLateral.Controls.Add(panelEmpresa);
            panelBarraLateral.Dock = DockStyle.Left;
            panelBarraLateral.Location = new Point(0, 0);
            panelBarraLateral.Margin = new Padding(3, 4, 3, 4);
            panelBarraLateral.Name = "panelBarraLateral";
            panelBarraLateral.Size = new Size(274, 933);
            panelBarraLateral.TabIndex = 0;
            // 
            // etiquetaPieBarra
            // 
            etiquetaPieBarra.Dock = DockStyle.Bottom;
            etiquetaPieBarra.ForeColor = Color.FromArgb(204, 215, 231);
            etiquetaPieBarra.Location = new Point(0, 848);
            etiquetaPieBarra.Name = "etiquetaPieBarra";
            etiquetaPieBarra.Padding = new Padding(32, 0, 0, 0);
            etiquetaPieBarra.Size = new Size(274, 85);
            etiquetaPieBarra.TabIndex = 0;
            etiquetaPieBarra.Text = "Sistema de gestión";
            etiquetaPieBarra.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // botonCatalogos
            // 
            botonCatalogos.BackColor = Color.FromArgb(24, 49, 83);
            botonCatalogos.Dock = DockStyle.Top;
            botonCatalogos.FlatAppearance.BorderSize = 0;
            botonCatalogos.FlatAppearance.MouseDownBackColor = Color.FromArgb(83, 117, 157);
            botonCatalogos.FlatAppearance.MouseOverBackColor = Color.FromArgb(69, 100, 137);
            botonCatalogos.FlatStyle = FlatStyle.Flat;
            botonCatalogos.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            botonCatalogos.ForeColor = Color.White;
            botonCatalogos.Location = new Point(0, 516);
            botonCatalogos.Margin = new Padding(3, 4, 3, 4);
            botonCatalogos.Name = "botonCatalogos";
            botonCatalogos.Padding = new Padding(27, 0, 9, 0);
            botonCatalogos.Size = new Size(274, 75);
            botonCatalogos.TabIndex = 3;
            botonCatalogos.Text = "Catálogos";
            botonCatalogos.TextAlign = ContentAlignment.MiddleLeft;
            botonCatalogos.UseVisualStyleBackColor = false;
            // 
            // botonInventario
            // 
            botonInventario.BackColor = Color.FromArgb(24, 49, 83);
            botonInventario.Dock = DockStyle.Top;
            botonInventario.FlatAppearance.BorderSize = 0;
            botonInventario.FlatAppearance.MouseDownBackColor = Color.FromArgb(83, 117, 157);
            botonInventario.FlatAppearance.MouseOverBackColor = Color.FromArgb(69, 100, 137);
            botonInventario.FlatStyle = FlatStyle.Flat;
            botonInventario.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            botonInventario.ForeColor = Color.White;
            botonInventario.Location = new Point(0, 441);
            botonInventario.Margin = new Padding(3, 4, 3, 4);
            botonInventario.Name = "botonInventario";
            botonInventario.Padding = new Padding(27, 0, 9, 0);
            botonInventario.Size = new Size(274, 75);
            botonInventario.TabIndex = 2;
            botonInventario.Text = "Inventario";
            botonInventario.TextAlign = ContentAlignment.MiddleLeft;
            botonInventario.UseVisualStyleBackColor = false;
            // 
            // botonPOS
            // 
            botonPOS.BackColor = Color.FromArgb(24, 49, 83);
            botonPOS.Dock = DockStyle.Top;
            botonPOS.FlatAppearance.BorderSize = 0;
            botonPOS.FlatAppearance.MouseDownBackColor = Color.FromArgb(83, 117, 157);
            botonPOS.FlatAppearance.MouseOverBackColor = Color.FromArgb(69, 100, 137);
            botonPOS.FlatStyle = FlatStyle.Flat;
            botonPOS.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            botonPOS.ForeColor = Color.White;
            botonPOS.Location = new Point(0, 366);
            botonPOS.Margin = new Padding(3, 4, 3, 4);
            botonPOS.Name = "botonPOS";
            botonPOS.Padding = new Padding(27, 0, 9, 0);
            botonPOS.Size = new Size(274, 75);
            botonPOS.TabIndex = 1;
            botonPOS.Text = "POS · Mesas y comandas";
            botonPOS.TextAlign = ContentAlignment.MiddleLeft;
            botonPOS.UseVisualStyleBackColor = false;
            // 
            // botonInicio
            // 
            botonInicio.BackColor = Color.FromArgb(255, 209, 79);
            botonInicio.Dock = DockStyle.Top;
            botonInicio.FlatAppearance.BorderSize = 0;
            botonInicio.FlatAppearance.MouseDownBackColor = Color.FromArgb(83, 117, 157);
            botonInicio.FlatAppearance.MouseOverBackColor = Color.FromArgb(69, 100, 137);
            botonInicio.FlatStyle = FlatStyle.Flat;
            botonInicio.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            botonInicio.ForeColor = Color.FromArgb(24, 49, 83);
            botonInicio.Location = new Point(0, 291);
            botonInicio.Margin = new Padding(3, 4, 3, 4);
            botonInicio.Name = "botonInicio";
            botonInicio.Padding = new Padding(27, 0, 9, 0);
            botonInicio.Size = new Size(274, 75);
            botonInicio.TabIndex = 0;
            botonInicio.Text = "Inicio";
            botonInicio.TextAlign = ContentAlignment.MiddleLeft;
            botonInicio.UseVisualStyleBackColor = false;
            // 
            // panelEmpresa
            // 
            panelEmpresa.Controls.Add(etiquetaDescripcion);
            panelEmpresa.Controls.Add(etiquetaNombreEmpresa);
            panelEmpresa.Controls.Add(etiquetaLogo);
            panelEmpresa.Dock = DockStyle.Top;
            panelEmpresa.Location = new Point(0, 0);
            panelEmpresa.Margin = new Padding(3, 4, 3, 4);
            panelEmpresa.Name = "panelEmpresa";
            panelEmpresa.Size = new Size(274, 291);
            panelEmpresa.TabIndex = 4;
            // 
            // etiquetaDescripcion
            // 
            etiquetaDescripcion.AutoSize = true;
            etiquetaDescripcion.ForeColor = Color.FromArgb(204, 215, 231);
            etiquetaDescripcion.Location = new Point(64, 210);
            etiquetaDescripcion.Name = "etiquetaDescripcion";
            etiquetaDescripcion.Size = new Size(133, 20);
            etiquetaDescripcion.TabIndex = 0;
            etiquetaDescripcion.Text = "Mesas y comandas";
            // 
            // etiquetaNombreEmpresa
            // 
            etiquetaNombreEmpresa.AutoSize = true;
            etiquetaNombreEmpresa.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            etiquetaNombreEmpresa.ForeColor = Color.White;
            etiquetaNombreEmpresa.Location = new Point(12, 147);
            etiquetaNombreEmpresa.Name = "etiquetaNombreEmpresa";
            etiquetaNombreEmpresa.Size = new Size(253, 46);
            etiquetaNombreEmpresa.TabIndex = 1;
            etiquetaNombreEmpresa.Text = "POLLOSTRACK";
            // 
            // etiquetaLogo
            // 
            etiquetaLogo.BackColor = Color.FromArgb(255, 209, 79);
            etiquetaLogo.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            etiquetaLogo.ForeColor = Color.FromArgb(24, 49, 83);
            etiquetaLogo.Location = new Point(83, 35);
            etiquetaLogo.Name = "etiquetaLogo";
            etiquetaLogo.Size = new Size(82, 96);
            etiquetaLogo.TabIndex = 2;
            etiquetaLogo.Text = "LOGO";
            etiquetaLogo.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panelEncabezado
            // 
            panelEncabezado.BackColor = Color.FromArgb(255, 209, 79);
            panelEncabezado.Controls.Add(etiquetaTitulo);
            panelEncabezado.Dock = DockStyle.Top;
            panelEncabezado.Location = new Point(274, 0);
            panelEncabezado.Margin = new Padding(3, 4, 3, 4);
            panelEncabezado.Name = "panelEncabezado";
            panelEncabezado.Size = new Size(983, 117);
            panelEncabezado.TabIndex = 2;
            // 
            // etiquetaTitulo
            // 
            etiquetaTitulo.Dock = DockStyle.Fill;
            etiquetaTitulo.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            etiquetaTitulo.ForeColor = Color.FromArgb(24, 49, 83);
            etiquetaTitulo.Location = new Point(0, 0);
            etiquetaTitulo.Name = "etiquetaTitulo";
            etiquetaTitulo.Padding = new Padding(37, 0, 0, 0);
            etiquetaTitulo.Size = new Size(983, 117);
            etiquetaTitulo.TabIndex = 0;
            etiquetaTitulo.Text = "Inicio";
            etiquetaTitulo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panelContenido
            // 
            panelContenido.BackColor = Color.FromArgb(255, 249, 229);
            panelContenido.Dock = DockStyle.Fill;
            panelContenido.Location = new Point(274, 117);
            panelContenido.Margin = new Padding(3, 4, 3, 4);
            panelContenido.Name = "panelContenido";
            panelContenido.Padding = new Padding(37, 43, 37, 43);
            panelContenido.Size = new Size(983, 816);
            panelContenido.TabIndex = 1;
            // 
            // FormPrincipal
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1257, 933);
            Controls.Add(panelContenido);
            Controls.Add(panelEncabezado);
            Controls.Add(panelBarraLateral);
            Margin = new Padding(3, 4, 3, 4);
            MinimumSize = new Size(1026, 784);
            Name = "FormPrincipal";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "PollosTrack | Mesas y comandas";
            panelBarraLateral.ResumeLayout(false);
            panelEmpresa.ResumeLayout(false);
            panelEmpresa.PerformLayout();
            panelEncabezado.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panelBarraLateral;
        private Panel panelEmpresa;
        private Label etiquetaLogo;
        private Label etiquetaNombreEmpresa;
        private Label etiquetaDescripcion;
        private Button botonInicio;
        private Button botonPOS;
        private Button botonInventario;
        private Button botonCatalogos;
        private Label etiquetaPieBarra;
        private Panel panelEncabezado;
        private Label etiquetaTitulo;
        private Panel panelContenido;
    }
}
