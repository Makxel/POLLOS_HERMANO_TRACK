namespace PollosTrack.POS.Forms
{
    partial class VistaMesa
    {
        // Este método conserva el nombre requerido por el diseñador de WinForms.
        private void InitializeComponent()
        {
            distribucionPrincipal = new TableLayoutPanel();
            etiquetaDescripcion = new Label();
            panelResumen = new FlowLayoutPanel();
            panelHerramientas = new TableLayoutPanel();
            selectorEstado = new ComboBox();
            botonActualizar = new Button();
            distribucionContenido = new TableLayoutPanel();
            panelMesas = new FlowLayoutPanel();
            panelDetalle = new TableLayoutPanel();
            etiquetaTituloDetalle = new Label();
            etiquetaDatosMesa = new Label();
            etiquetaEstadoMesa = new Label();
            botonVerComandas = new Button();
            botonNuevaCuenta = new Button();
            botonIrAFacturacion = new Button();
            etiquetaAyuda = new Label();
            etiquetaEstado = new Label();
            distribucionPrincipal.SuspendLayout();
            panelHerramientas.SuspendLayout();
            distribucionContenido.SuspendLayout();
            panelDetalle.SuspendLayout();
            SuspendLayout();
            // 
            // distribucionPrincipal
            // 
            distribucionPrincipal.ColumnCount = 1;
            distribucionPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            distribucionPrincipal.Controls.Add(etiquetaDescripcion, 0, 0);
            distribucionPrincipal.Controls.Add(panelResumen, 0, 1);
            distribucionPrincipal.Controls.Add(panelHerramientas, 0, 2);
            distribucionPrincipal.Controls.Add(distribucionContenido, 0, 3);
            distribucionPrincipal.Controls.Add(etiquetaEstado, 0, 4);
            distribucionPrincipal.Dock = DockStyle.Fill;
            distribucionPrincipal.Location = new Point(0, 0);
            distribucionPrincipal.Name = "distribucionPrincipal";
            distribucionPrincipal.RowCount = 5;
            distribucionPrincipal.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
            distribucionPrincipal.RowStyles.Add(new RowStyle(SizeType.Absolute, 72F));
            distribucionPrincipal.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
            distribucionPrincipal.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            distribucionPrincipal.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F));
            distribucionPrincipal.Size = new Size(880, 620);
            distribucionPrincipal.TabIndex = 0;
            // 
            // etiquetaDescripcion
            // 
            etiquetaDescripcion.Dock = DockStyle.Fill;
            etiquetaDescripcion.ForeColor = Color.FromArgb(24, 49, 83);
            etiquetaDescripcion.Location = new Point(3, 0);
            etiquetaDescripcion.Name = "etiquetaDescripcion";
            etiquetaDescripcion.Size = new Size(874, 52);
            etiquetaDescripcion.TabIndex = 0;
            etiquetaDescripcion.Text = "\r\nSelecciona la mesa a gestionar.";
            // 
            // panelResumen
            // 
            panelResumen.AutoScroll = true;
            panelResumen.BackColor = Color.White;
            panelResumen.Dock = DockStyle.Fill;
            panelResumen.Location = new Point(3, 55);
            panelResumen.Name = "panelResumen";
            panelResumen.Padding = new Padding(6);
            panelResumen.Size = new Size(874, 66);
            panelResumen.TabIndex = 1;
            panelResumen.WrapContents = false;
            // 
            // panelHerramientas
            // 
            panelHerramientas.ColumnCount = 2;
            panelHerramientas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            panelHerramientas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            panelHerramientas.Controls.Add(selectorEstado, 0, 0);
            panelHerramientas.Controls.Add(botonActualizar, 1, 0);
            panelHerramientas.Dock = DockStyle.Fill;
            panelHerramientas.Location = new Point(3, 127);
            panelHerramientas.Name = "panelHerramientas";
            panelHerramientas.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            panelHerramientas.Size = new Size(874, 48);
            panelHerramientas.TabIndex = 2;
            // 
            // selectorEstado
            // 
            selectorEstado.AccessibleName = "Filtrar mesas por estado";
            selectorEstado.Anchor = AnchorStyles.Left;
            selectorEstado.DropDownStyle = ComboBoxStyle.DropDownList;
            selectorEstado.Location = new Point(3, 10);
            selectorEstado.Name = "selectorEstado";
            selectorEstado.Size = new Size(230, 28);
            selectorEstado.TabIndex = 0;
            selectorEstado.SelectedIndexChanged += AlCambiarFiltroEstado;
            // 
            // botonActualizar
            // 
            botonActualizar.BackColor = Color.FromArgb(24, 49, 83);
            botonActualizar.Dock = DockStyle.Fill;
            botonActualizar.FlatAppearance.BorderSize = 0;
            botonActualizar.FlatStyle = FlatStyle.Flat;
            botonActualizar.ForeColor = Color.White;
            botonActualizar.Location = new Point(738, 6);
            botonActualizar.Margin = new Padding(4, 6, 0, 6);
            botonActualizar.Name = "botonActualizar";
            botonActualizar.Size = new Size(136, 36);
            botonActualizar.TabIndex = 1;
            botonActualizar.Text = "Actualizar";
            botonActualizar.UseVisualStyleBackColor = false;
            botonActualizar.Click += AlPulsarActualizar;
            // 
            // distribucionContenido
            // 
            distribucionContenido.ColumnCount = 2;
            distribucionContenido.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            distribucionContenido.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 225F));
            distribucionContenido.Controls.Add(panelMesas, 0, 0);
            distribucionContenido.Controls.Add(panelDetalle, 1, 0);
            distribucionContenido.Dock = DockStyle.Fill;
            distribucionContenido.Location = new Point(3, 181);
            distribucionContenido.Name = "distribucionContenido";
            distribucionContenido.RowCount = 1;
            distribucionContenido.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            distribucionContenido.Size = new Size(874, 372);
            distribucionContenido.TabIndex = 3;
            // 
            // panelMesas
            // 
            panelMesas.AutoScroll = true;
            panelMesas.BackColor = Color.FromArgb(235, 240, 247);
            panelMesas.Dock = DockStyle.Fill;
            panelMesas.Location = new Point(3, 3);
            panelMesas.Name = "panelMesas";
            panelMesas.Padding = new Padding(10);
            panelMesas.Size = new Size(643, 366);
            panelMesas.TabIndex = 0;
            // 
            // panelDetalle
            // 
            panelDetalle.AutoScroll = true;
            panelDetalle.BackColor = Color.White;
            panelDetalle.ColumnCount = 1;
            panelDetalle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            panelDetalle.Controls.Add(etiquetaTituloDetalle, 0, 0);
            panelDetalle.Controls.Add(etiquetaDatosMesa, 0, 1);
            panelDetalle.Controls.Add(etiquetaEstadoMesa, 0, 2);
            panelDetalle.Controls.Add(botonVerComandas, 0, 3);
            panelDetalle.Controls.Add(botonNuevaCuenta, 0, 4);
            panelDetalle.Controls.Add(botonIrAFacturacion, 0, 5);
            panelDetalle.Controls.Add(etiquetaAyuda, 0, 6);
            panelDetalle.Dock = DockStyle.Fill;
            panelDetalle.Location = new Point(661, 3);
            panelDetalle.Margin = new Padding(12, 3, 0, 3);
            panelDetalle.Name = "panelDetalle";
            panelDetalle.Padding = new Padding(16);
            panelDetalle.RowCount = 7;
            panelDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            panelDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F));
            panelDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            panelDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            panelDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            panelDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            panelDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
            panelDetalle.Size = new Size(213, 366);
            panelDetalle.TabIndex = 1;
            // 
            // etiquetaTituloDetalle
            // 
            etiquetaTituloDetalle.Dock = DockStyle.Fill;
            etiquetaTituloDetalle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            etiquetaTituloDetalle.Location = new Point(19, 16);
            etiquetaTituloDetalle.Name = "etiquetaTituloDetalle";
            etiquetaTituloDetalle.Size = new Size(154, 60);
            etiquetaTituloDetalle.TabIndex = 0;
            etiquetaTituloDetalle.Text = "Selecciona una mesa";
            // 
            // etiquetaDatosMesa
            // 
            etiquetaDatosMesa.Dock = DockStyle.Fill;
            etiquetaDatosMesa.Location = new Point(19, 76);
            etiquetaDatosMesa.Name = "etiquetaDatosMesa";
            etiquetaDatosMesa.Size = new Size(154, 64);
            etiquetaDatosMesa.TabIndex = 1;
            etiquetaDatosMesa.Text = "Aquí verás su capacidad y estado actual.";
            // 
            // etiquetaEstadoMesa
            // 
            etiquetaEstadoMesa.Dock = DockStyle.Fill;
            etiquetaEstadoMesa.Location = new Point(19, 140);
            etiquetaEstadoMesa.Name = "etiquetaEstadoMesa";
            etiquetaEstadoMesa.Size = new Size(154, 48);
            etiquetaEstadoMesa.TabIndex = 2;
            etiquetaEstadoMesa.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // botonVerComandas
            // 
            botonVerComandas.BackColor = Color.FromArgb(255, 209, 79);
            botonVerComandas.Dock = DockStyle.Fill;
            botonVerComandas.FlatAppearance.BorderSize = 0;
            botonVerComandas.FlatStyle = FlatStyle.Flat;
            botonVerComandas.ForeColor = Color.Black;
            botonVerComandas.Location = new Point(19, 191);
            botonVerComandas.Name = "botonVerComandas";
            botonVerComandas.Size = new Size(154, 42);
            botonVerComandas.TabIndex = 3;
            botonVerComandas.Text = "Ver comandas";
            botonVerComandas.UseVisualStyleBackColor = false;
            botonVerComandas.Visible = false;
            botonVerComandas.Click += AlPulsarVerComandas;
            // 
            // botonNuevaCuenta
            // 
            botonNuevaCuenta.BackColor = Color.FromArgb(255, 209, 79);
            botonNuevaCuenta.Dock = DockStyle.Fill;
            botonNuevaCuenta.FlatAppearance.BorderSize = 0;
            botonNuevaCuenta.FlatStyle = FlatStyle.Flat;
            botonNuevaCuenta.ForeColor = Color.Black;
            botonNuevaCuenta.Location = new Point(19, 239);
            botonNuevaCuenta.Name = "botonNuevaCuenta";
            botonNuevaCuenta.Size = new Size(154, 42);
            botonNuevaCuenta.TabIndex = 4;
            botonNuevaCuenta.Text = "Nueva cuenta separada";
            botonNuevaCuenta.UseVisualStyleBackColor = false;
            botonNuevaCuenta.Visible = false;
            botonNuevaCuenta.Click += AlPulsarNuevaCuenta;
            // 
            // botonIrAFacturacion
            // 
            botonIrAFacturacion.BackColor = Color.FromArgb(255, 209, 79);
            botonIrAFacturacion.Dock = DockStyle.Fill;
            botonIrAFacturacion.FlatAppearance.BorderSize = 0;
            botonIrAFacturacion.FlatStyle = FlatStyle.Flat;
            botonIrAFacturacion.ForeColor = Color.Black;
            botonIrAFacturacion.Location = new Point(19, 287);
            botonIrAFacturacion.Name = "botonIrAFacturacion";
            botonIrAFacturacion.Size = new Size(154, 42);
            botonIrAFacturacion.TabIndex = 5;
            botonIrAFacturacion.Text = "Ir a facturación";
            botonIrAFacturacion.UseVisualStyleBackColor = false;
            botonIrAFacturacion.Visible = false;
            botonIrAFacturacion.Click += AlPulsarFacturacion;
            // 
            // etiquetaAyuda
            // 
            etiquetaAyuda.Dock = DockStyle.Fill;
            etiquetaAyuda.ForeColor = Color.FromArgb(85, 99, 116);
            etiquetaAyuda.Location = new Point(19, 332);
            etiquetaAyuda.Name = "etiquetaAyuda";
            etiquetaAyuda.Padding = new Padding(0, 16, 0, 0);
            etiquetaAyuda.Size = new Size(154, 100);
            etiquetaAyuda.TabIndex = 6;
            etiquetaAyuda.Text = "Pulsa Actualizar para consultar los estados guardados en la base de datos.";
            // 
            // etiquetaEstado
            // 
            etiquetaEstado.Dock = DockStyle.Fill;
            etiquetaEstado.ForeColor = Color.FromArgb(85, 99, 116);
            etiquetaEstado.Location = new Point(3, 556);
            etiquetaEstado.Name = "etiquetaEstado";
            etiquetaEstado.Size = new Size(874, 64);
            etiquetaEstado.TabIndex = 4;
            etiquetaEstado.Text = "Esperando conexión…";
            etiquetaEstado.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // VistaMesa
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(246, 248, 251);
            Controls.Add(distribucionPrincipal);
            ForeColor = Color.FromArgb(24, 49, 83);
            Name = "VistaMesa";
            Size = new Size(880, 620);
            Load += AlCargarVista;
            distribucionPrincipal.ResumeLayout(false);
            panelHerramientas.ResumeLayout(false);
            distribucionContenido.ResumeLayout(false);
            panelDetalle.ResumeLayout(false);
            ResumeLayout(false);
        }

        private TableLayoutPanel distribucionPrincipal;
        private Label etiquetaDescripcion;
        private FlowLayoutPanel panelResumen;
        private TableLayoutPanel panelHerramientas;
        private ComboBox selectorEstado;
        private Button botonActualizar;
        private TableLayoutPanel distribucionContenido;
        private FlowLayoutPanel panelMesas;
        private TableLayoutPanel panelDetalle;
        private Label etiquetaTituloDetalle;
        private Label etiquetaDatosMesa;
        private Label etiquetaEstadoMesa;
        private Button botonVerComandas;
        private Button botonNuevaCuenta;
        private Button botonIrAFacturacion;
        private Label etiquetaAyuda;
        private Label etiquetaEstado;
    }
}
