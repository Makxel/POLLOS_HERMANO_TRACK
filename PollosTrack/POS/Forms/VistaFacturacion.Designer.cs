namespace PollosTrack.POS.Forms
{
    partial class VistaFacturacion
    {
        // Diseño fijo editable desde Visual Studio; la lógica de cobro está en el otro archivo parcial.
        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            distribucion = new TableLayoutPanel();
            etiquetaTitulo = new Label();
            botonVolverAMesas = new Button();
            selectorCuenta = new ComboBox();
            botonActualizar = new Button();
            tablaProductos = new DataGridView();
            etiquetaTotales = new Label();
            panelPago = new FlowLayoutPanel();
            etiquetaMetodo = new Label();
            selectorPago = new ComboBox();
            etiquetaRecibido = new Label();
            montoRecibido = new NumericUpDown();
            etiquetaCambio = new Label();
            botonVerRecibo = new Button();
            botonFacturar = new Button();
            etiquetaAviso = new Label();
            distribucion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tablaProductos).BeginInit();
            panelPago.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)montoRecibido).BeginInit();
            SuspendLayout();
            // 
            // distribucion
            // 
            distribucion.ColumnCount = 2;
            distribucion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            distribucion.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170F));
            distribucion.Controls.Add(etiquetaTitulo, 0, 0);
            distribucion.Controls.Add(botonVolverAMesas, 1, 0);
            distribucion.Controls.Add(selectorCuenta, 0, 1);
            distribucion.Controls.Add(botonActualizar, 1, 1);
            distribucion.Controls.Add(tablaProductos, 0, 2);
            distribucion.Controls.Add(etiquetaTotales, 0, 3);
            distribucion.Controls.Add(panelPago, 0, 4);
            distribucion.Controls.Add(botonFacturar, 1, 4);
            distribucion.Controls.Add(etiquetaAviso, 0, 5);
            distribucion.Dock = DockStyle.Fill;
            distribucion.Location = new Point(20, 20);
            distribucion.Name = "distribucion";
            distribucion.RowCount = 6;
            distribucion.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
            distribucion.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            distribucion.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            distribucion.RowStyles.Add(new RowStyle(SizeType.Absolute, 82F));
            distribucion.RowStyles.Add(new RowStyle(SizeType.Absolute, 110F));
            distribucion.RowStyles.Add(new RowStyle(SizeType.Absolute, 65F));
            distribucion.Size = new Size(860, 660);
            distribucion.TabIndex = 0;
            // 
            // etiquetaTitulo
            // 
            etiquetaTitulo.Dock = DockStyle.Fill;
            etiquetaTitulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            etiquetaTitulo.Location = new Point(3, 0);
            etiquetaTitulo.Name = "etiquetaTitulo";
            etiquetaTitulo.Size = new Size(684, 58);
            etiquetaTitulo.TabIndex = 0;
            etiquetaTitulo.Text = "Facturación y cierre";
            // 
            // botonVolverAMesas
            // 
            botonVolverAMesas.Dock = DockStyle.Fill;
            botonVolverAMesas.Location = new Point(693, 3);
            botonVolverAMesas.Name = "botonVolverAMesas";
            botonVolverAMesas.Size = new Size(164, 52);
            botonVolverAMesas.TabIndex = 1;
            botonVolverAMesas.Text = "Volver a mesas";
            botonVolverAMesas.Click += AlVolverAMesas;
            // 
            // selectorCuenta
            // 
            selectorCuenta.AccessibleName = "Cuenta a facturar";
            selectorCuenta.Dock = DockStyle.Fill;
            selectorCuenta.DropDownStyle = ComboBoxStyle.DropDownList;
            selectorCuenta.Location = new Point(3, 61);
            selectorCuenta.Name = "selectorCuenta";
            selectorCuenta.Size = new Size(684, 28);
            selectorCuenta.TabIndex = 2;
            selectorCuenta.SelectedIndexChanged += AlSeleccionarCuenta;
            // 
            // botonActualizar
            // 
            botonActualizar.Dock = DockStyle.Fill;
            botonActualizar.Location = new Point(693, 61);
            botonActualizar.Name = "botonActualizar";
            botonActualizar.Size = new Size(164, 44);
            botonActualizar.TabIndex = 3;
            botonActualizar.Text = "Actualizar cuentas";
            botonActualizar.Click += AlActualizarCuentas;
            // 
            // tablaProductos
            // 
            tablaProductos.AllowUserToAddRows = false;
            tablaProductos.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(246, 248, 251);
            tablaProductos.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            tablaProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            tablaProductos.BackgroundColor = Color.White;
            tablaProductos.BorderStyle = BorderStyle.None;
            tablaProductos.ColumnHeadersHeight = 29;
            distribucion.SetColumnSpan(tablaProductos, 2);
            tablaProductos.Dock = DockStyle.Fill;
            tablaProductos.Location = new Point(3, 111);
            tablaProductos.Name = "tablaProductos";
            tablaProductos.ReadOnly = true;
            tablaProductos.RowHeadersVisible = false;
            tablaProductos.RowHeadersWidth = 51;
            tablaProductos.Size = new Size(854, 289);
            tablaProductos.TabIndex = 4;
            // 
            // etiquetaTotales
            // 
            etiquetaTotales.BackColor = Color.FromArgb(255, 245, 212);
            distribucion.SetColumnSpan(etiquetaTotales, 2);
            etiquetaTotales.Dock = DockStyle.Fill;
            etiquetaTotales.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            etiquetaTotales.Location = new Point(3, 403);
            etiquetaTotales.Name = "etiquetaTotales";
            etiquetaTotales.Padding = new Padding(12);
            etiquetaTotales.Size = new Size(854, 82);
            etiquetaTotales.TabIndex = 5;
            etiquetaTotales.Text = "Subtotal: C$ 0.00    IVA: C$ 0.00\r\nTOTAL: C$ 0.00";
            // 
            // panelPago
            // 
            panelPago.AutoScroll = true;
            panelPago.Controls.Add(etiquetaMetodo);
            panelPago.Controls.Add(selectorPago);
            panelPago.Controls.Add(etiquetaRecibido);
            panelPago.Controls.Add(montoRecibido);
            panelPago.Controls.Add(etiquetaCambio);
            panelPago.Controls.Add(botonVerRecibo);
            panelPago.Dock = DockStyle.Fill;
            panelPago.Location = new Point(3, 488);
            panelPago.Name = "panelPago";
            panelPago.Size = new Size(684, 104);
            panelPago.TabIndex = 6;
            // 
            // etiquetaMetodo
            // 
            etiquetaMetodo.AutoSize = true;
            etiquetaMetodo.Location = new Point(3, 8);
            etiquetaMetodo.Margin = new Padding(3, 8, 3, 3);
            etiquetaMetodo.Name = "etiquetaMetodo";
            etiquetaMetodo.Size = new Size(122, 20);
            etiquetaMetodo.TabIndex = 0;
            etiquetaMetodo.Text = "Método de pago";
            // 
            // selectorPago
            // 
            selectorPago.DropDownStyle = ComboBoxStyle.DropDownList;
            selectorPago.Items.AddRange(new object[] { "Efectivo", "Tarjeta", "Transferencia" });
            selectorPago.Location = new Point(131, 3);
            selectorPago.Name = "selectorPago";
            selectorPago.Size = new Size(145, 28);
            selectorPago.TabIndex = 1;
            selectorPago.SelectedIndexChanged += AlCambiarPago;
            // 
            // etiquetaRecibido
            // 
            etiquetaRecibido.AutoSize = true;
            etiquetaRecibido.Location = new Point(291, 8);
            etiquetaRecibido.Margin = new Padding(12, 8, 3, 3);
            etiquetaRecibido.Name = "etiquetaRecibido";
            etiquetaRecibido.Size = new Size(89, 20);
            etiquetaRecibido.TabIndex = 2;
            etiquetaRecibido.Text = "Recibido C$";
            // 
            // montoRecibido
            // 
            montoRecibido.DecimalPlaces = 2;
            montoRecibido.Location = new Point(386, 3);
            montoRecibido.Maximum = new decimal(new int[] { 1410065407, 2, 0, 131072 });
            montoRecibido.Name = "montoRecibido";
            montoRecibido.Size = new Size(150, 27);
            montoRecibido.TabIndex = 3;
            montoRecibido.ThousandsSeparator = true;
            montoRecibido.TextChanged += AlCambiarPago;
            montoRecibido.ValueChanged += AlCambiarPago;
            // 
            // etiquetaCambio
            // 
            etiquetaCambio.AutoSize = true;
            etiquetaCambio.Location = new Point(551, 8);
            etiquetaCambio.Margin = new Padding(12, 8, 3, 3);
            etiquetaCambio.Name = "etiquetaCambio";
            etiquetaCambio.Size = new Size(116, 20);
            etiquetaCambio.TabIndex = 4;
            etiquetaCambio.Text = "Cambio: C$ 0.00";
            // 
            // botonVerRecibo
            // 
            botonVerRecibo.Enabled = false;
            botonVerRecibo.Location = new Point(3, 37);
            botonVerRecibo.Name = "botonVerRecibo";
            botonVerRecibo.Size = new Size(160, 34);
            botonVerRecibo.TabIndex = 5;
            botonVerRecibo.Text = "Ver último recibo";
            botonVerRecibo.Click += AlVerRecibo;
            // 
            // botonFacturar
            // 
            botonFacturar.BackColor = Color.FromArgb(255, 209, 79);
            botonFacturar.Dock = DockStyle.Fill;
            botonFacturar.Enabled = false;
            botonFacturar.FlatAppearance.BorderSize = 0;
            botonFacturar.FlatStyle = FlatStyle.Flat;
            botonFacturar.Location = new Point(693, 488);
            botonFacturar.Name = "botonFacturar";
            botonFacturar.Size = new Size(164, 104);
            botonFacturar.TabIndex = 7;
            botonFacturar.Text = "Confirmar cobro";
            botonFacturar.UseVisualStyleBackColor = false;
            botonFacturar.Click += AlFacturar;
            // 
            // etiquetaAviso
            // 
            distribucion.SetColumnSpan(etiquetaAviso, 2);
            etiquetaAviso.Dock = DockStyle.Fill;
            etiquetaAviso.ForeColor = Color.FromArgb(85, 99, 116);
            etiquetaAviso.Location = new Point(3, 595);
            etiquetaAviso.Name = "etiquetaAviso";
            etiquetaAviso.Size = new Size(854, 65);
            etiquetaAviso.TabIndex = 8;
            etiquetaAviso.Text = "Selecciona una cuenta para comenzar.";
            // 
            // VistaFacturacion
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(246, 248, 251);
            Controls.Add(distribucion);
            ForeColor = Color.FromArgb(24, 49, 83);
            Name = "VistaFacturacion";
            Padding = new Padding(20);
            Size = new Size(900, 700);
            Load += AlCargarFacturacion;
            distribucion.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)tablaProductos).EndInit();
            panelPago.ResumeLayout(false);
            panelPago.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)montoRecibido).EndInit();
            ResumeLayout(false);
        }

        private TableLayoutPanel distribucion;
        private Label etiquetaTitulo;
        private Button botonVolverAMesas;
        private ComboBox selectorCuenta;
        private Button botonActualizar;
        private DataGridView tablaProductos;
        private Label etiquetaTotales;
        private FlowLayoutPanel panelPago;
        private Label etiquetaMetodo;
        private ComboBox selectorPago;
        private Label etiquetaRecibido;
        private NumericUpDown montoRecibido;
        private Label etiquetaCambio;
        private Button botonFacturar;
        private Button botonVerRecibo;
        private Label etiquetaAviso;
    }
}
