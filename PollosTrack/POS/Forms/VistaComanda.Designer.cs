namespace PollosTrack.POS.Forms
{
    partial class VistaComanda
    {
        // El diseño se mantiene aquí para editar los controles desde Visual Studio.
        private void InitializeComponent()
        {
            panelDetalle = new TableLayoutPanel();
            accionesDetalle = new FlowLayoutPanel();
            botonAgregarProducto = new Button();
            botonActualizarDetalle = new Button();
            tablaDetalle = new DataGridView();
            etiquetaSubtotal = new Label();
            etiquetaCuenta = new Label();
            selectorCuenta = new ComboBox();
            etiquetaCategoriaDetalle = new Label();
            selectorCategoria = new ComboBox();
            etiquetaProductoDetalle = new Label();
            filaProducto = new TableLayoutPanel();
            textoProducto = new TextBox();
            botonBuscarProducto = new Button();
            etiquetaCantidadDetalle = new Label();
            cantidadProducto = new NumericUpDown();
            etiquetaNotasDetalle = new Label();
            textoNotas = new TextBox();
            etiquetaImporteDetalle = new Label();
            etiquetaPrecio = new Label();
            distribucionPrincipal = new TableLayoutPanel();
            etiquetaTitulo = new Label();
            botonVolverAMesas = new Button();
            etiquetaDescripcion = new Label();
            panelEncabezado = new TableLayoutPanel();
            etiquetaSeccion = new Label();
            etiquetaAyuda = new Label();
            etiquetaCliente = new Label();
            filaCliente = new TableLayoutPanel();
            textoCliente = new TextBox();
            botonBuscar = new Button();
            etiquetaReferencia = new Label();
            textoReferencia = new TextBox();
            etiquetaNota = new Label();
            botonGuardar = new Button();
            panelDetalle.SuspendLayout();
            accionesDetalle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tablaDetalle).BeginInit();
            filaProducto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)cantidadProducto).BeginInit();
            distribucionPrincipal.SuspendLayout();
            panelEncabezado.SuspendLayout();
            filaCliente.SuspendLayout();
            SuspendLayout();
            // 
            // panelDetalle
            // 
            panelDetalle.AutoSize = true;
            panelDetalle.BackColor = Color.White;
            panelDetalle.ColumnCount = 2;
            distribucionPrincipal.SetColumnSpan(panelDetalle, 2);
            panelDetalle.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
            panelDetalle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            panelDetalle.Controls.Add(accionesDetalle, 0, 6);
            panelDetalle.Controls.Add(tablaDetalle, 0, 7);
            panelDetalle.Controls.Add(etiquetaSubtotal, 0, 8);
            panelDetalle.Controls.Add(etiquetaCuenta, 0, 0);
            panelDetalle.Controls.Add(selectorCuenta, 1, 0);
            panelDetalle.Controls.Add(etiquetaCategoriaDetalle, 0, 1);
            panelDetalle.Controls.Add(selectorCategoria, 1, 1);
            panelDetalle.Controls.Add(etiquetaProductoDetalle, 0, 2);
            panelDetalle.Controls.Add(filaProducto, 1, 2);
            panelDetalle.Controls.Add(etiquetaCantidadDetalle, 0, 3);
            panelDetalle.Controls.Add(cantidadProducto, 1, 3);
            panelDetalle.Controls.Add(etiquetaNotasDetalle, 0, 4);
            panelDetalle.Controls.Add(textoNotas, 1, 4);
            panelDetalle.Controls.Add(etiquetaImporteDetalle, 0, 5);
            panelDetalle.Controls.Add(etiquetaPrecio, 1, 5);
            panelDetalle.Dock = DockStyle.Top;
            panelDetalle.Location = new Point(3, 610);
            panelDetalle.Name = "panelDetalle";
            panelDetalle.Padding = new Padding(20);
            panelDetalle.RowCount = 9;
            panelDetalle.RowStyles.Add(new RowStyle());
            panelDetalle.RowStyles.Add(new RowStyle());
            panelDetalle.RowStyles.Add(new RowStyle());
            panelDetalle.RowStyles.Add(new RowStyle());
            panelDetalle.RowStyles.Add(new RowStyle());
            panelDetalle.RowStyles.Add(new RowStyle());
            panelDetalle.RowStyles.Add(new RowStyle());
            panelDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 240F));
            panelDetalle.RowStyles.Add(new RowStyle());
            panelDetalle.Size = new Size(812, 646);
            panelDetalle.TabIndex = 4;
            // 
            // accionesDetalle
            // 
            accionesDetalle.AutoSize = true;
            panelDetalle.SetColumnSpan(accionesDetalle, 2);
            accionesDetalle.Controls.Add(botonAgregarProducto);
            accionesDetalle.Controls.Add(botonActualizarDetalle);
            accionesDetalle.Dock = DockStyle.Fill;
            accionesDetalle.Location = new Point(23, 314);
            accionesDetalle.Name = "accionesDetalle";
            accionesDetalle.Size = new Size(766, 46);
            accionesDetalle.TabIndex = 0;
            // 
            // botonAgregarProducto
            // 
            botonAgregarProducto.BackColor = Color.FromArgb(255, 209, 79);
            botonAgregarProducto.Enabled = false;
            botonAgregarProducto.FlatAppearance.BorderSize = 0;
            botonAgregarProducto.FlatStyle = FlatStyle.Flat;
            botonAgregarProducto.Location = new Point(3, 3);
            botonAgregarProducto.Name = "botonAgregarProducto";
            botonAgregarProducto.Size = new Size(190, 40);
            botonAgregarProducto.TabIndex = 0;
            botonAgregarProducto.Text = "Agregar producto";
            botonAgregarProducto.UseVisualStyleBackColor = false;
            botonAgregarProducto.Click += AlAgregarProducto;
            // 
            // botonActualizarDetalle
            // 
            botonActualizarDetalle.Location = new Point(199, 3);
            botonActualizarDetalle.Name = "botonActualizarDetalle";
            botonActualizarDetalle.Size = new Size(140, 40);
            botonActualizarDetalle.TabIndex = 1;
            botonActualizarDetalle.Text = "Actualizar";
            botonActualizarDetalle.Click += AlActualizarDetalle;
            // 
            // tablaDetalle
            // 
            tablaDetalle.AllowUserToAddRows = false;
            tablaDetalle.AllowUserToDeleteRows = false;
            tablaDetalle.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            tablaDetalle.BackgroundColor = Color.White;
            tablaDetalle.ColumnHeadersHeight = 29;
            panelDetalle.SetColumnSpan(tablaDetalle, 2);
            tablaDetalle.Dock = DockStyle.Fill;
            tablaDetalle.Location = new Point(23, 366);
            tablaDetalle.Name = "tablaDetalle";
            tablaDetalle.ReadOnly = true;
            tablaDetalle.RowHeadersVisible = false;
            tablaDetalle.RowHeadersWidth = 51;
            tablaDetalle.Size = new Size(766, 234);
            tablaDetalle.TabIndex = 1;
            // 
            // etiquetaSubtotal
            // 
            etiquetaSubtotal.AutoSize = true;
            panelDetalle.SetColumnSpan(etiquetaSubtotal, 2);
            etiquetaSubtotal.Location = new Point(23, 603);
            etiquetaSubtotal.Name = "etiquetaSubtotal";
            etiquetaSubtotal.Size = new Size(74, 23);
            etiquetaSubtotal.TabIndex = 2;
            etiquetaSubtotal.Text = "Subtotal";
            // 
            // etiquetaCuenta
            // 
            etiquetaCuenta.AutoSize = true;
            etiquetaCuenta.Location = new Point(20, 28);
            etiquetaCuenta.Margin = new Padding(0, 8, 12, 12);
            etiquetaCuenta.Name = "etiquetaCuenta";
            etiquetaCuenta.Size = new Size(65, 23);
            etiquetaCuenta.TabIndex = 3;
            etiquetaCuenta.Text = "Cuenta";
            // 
            // selectorCuenta
            // 
            selectorCuenta.Dock = DockStyle.Fill;
            selectorCuenta.DropDownStyle = ComboBoxStyle.DropDownList;
            selectorCuenta.Location = new Point(180, 26);
            selectorCuenta.Margin = new Padding(0, 6, 0, 12);
            selectorCuenta.Name = "selectorCuenta";
            selectorCuenta.Size = new Size(612, 31);
            selectorCuenta.TabIndex = 4;
            selectorCuenta.SelectedIndexChanged += AlElegirCuenta;
            // 
            // etiquetaCategoriaDetalle
            // 
            etiquetaCategoriaDetalle.AutoSize = true;
            etiquetaCategoriaDetalle.Location = new Point(20, 77);
            etiquetaCategoriaDetalle.Margin = new Padding(0, 8, 12, 12);
            etiquetaCategoriaDetalle.Name = "etiquetaCategoriaDetalle";
            etiquetaCategoriaDetalle.Size = new Size(84, 23);
            etiquetaCategoriaDetalle.TabIndex = 5;
            etiquetaCategoriaDetalle.Text = "Categoría";
            // 
            // selectorCategoria
            // 
            selectorCategoria.Dock = DockStyle.Fill;
            selectorCategoria.DropDownStyle = ComboBoxStyle.DropDownList;
            selectorCategoria.Location = new Point(180, 75);
            selectorCategoria.Margin = new Padding(0, 6, 0, 12);
            selectorCategoria.Name = "selectorCategoria";
            selectorCategoria.Size = new Size(612, 31);
            selectorCategoria.TabIndex = 6;
            selectorCategoria.SelectedIndexChanged += AlCambiarCategoria;
            // 
            // etiquetaProductoDetalle
            // 
            etiquetaProductoDetalle.AutoSize = true;
            etiquetaProductoDetalle.Location = new Point(20, 126);
            etiquetaProductoDetalle.Margin = new Padding(0, 8, 12, 12);
            etiquetaProductoDetalle.Name = "etiquetaProductoDetalle";
            etiquetaProductoDetalle.Size = new Size(80, 23);
            etiquetaProductoDetalle.TabIndex = 7;
            etiquetaProductoDetalle.Text = "Producto";
            // 
            // filaProducto
            // 
            filaProducto.AutoSize = true;
            filaProducto.ColumnCount = 2;
            filaProducto.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            filaProducto.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
            filaProducto.Controls.Add(textoProducto, 0, 0);
            filaProducto.Controls.Add(botonBuscarProducto, 1, 0);
            filaProducto.Dock = DockStyle.Fill;
            filaProducto.Location = new Point(183, 121);
            filaProducto.Name = "filaProducto";
            filaProducto.RowCount = 1;
            filaProducto.RowStyles.Add(new RowStyle());
            filaProducto.Size = new Size(606, 48);
            filaProducto.TabIndex = 8;
            // 
            // textoProducto
            // 
            textoProducto.BackColor = Color.FromArgb(241, 244, 248);
            textoProducto.Dock = DockStyle.Fill;
            textoProducto.Location = new Point(0, 6);
            textoProducto.Margin = new Padding(0, 6, 0, 12);
            textoProducto.Name = "textoProducto";
            textoProducto.PlaceholderText = "Selecciona un producto";
            textoProducto.ReadOnly = true;
            textoProducto.Size = new Size(446, 30);
            textoProducto.TabIndex = 8;
            // 
            // botonBuscarProducto
            // 
            botonBuscarProducto.BackColor = Color.FromArgb(24, 49, 83);
            botonBuscarProducto.Dock = DockStyle.Fill;
            botonBuscarProducto.FlatAppearance.BorderSize = 0;
            botonBuscarProducto.FlatStyle = FlatStyle.Flat;
            botonBuscarProducto.ForeColor = Color.White;
            botonBuscarProducto.Location = new Point(449, 3);
            botonBuscarProducto.MinimumSize = new Size(0, 36);
            botonBuscarProducto.Name = "botonBuscarProducto";
            botonBuscarProducto.Size = new Size(154, 42);
            botonBuscarProducto.TabIndex = 9;
            botonBuscarProducto.Text = "Buscar producto";
            botonBuscarProducto.UseVisualStyleBackColor = false;
            botonBuscarProducto.Click += AlBuscarProducto;
            // 
            // etiquetaCantidadDetalle
            // 
            etiquetaCantidadDetalle.AutoSize = true;
            etiquetaCantidadDetalle.Location = new Point(20, 180);
            etiquetaCantidadDetalle.Margin = new Padding(0, 8, 12, 12);
            etiquetaCantidadDetalle.Name = "etiquetaCantidadDetalle";
            etiquetaCantidadDetalle.Size = new Size(79, 23);
            etiquetaCantidadDetalle.TabIndex = 9;
            etiquetaCantidadDetalle.Text = "Cantidad";
            // 
            // cantidadProducto
            // 
            cantidadProducto.Dock = DockStyle.Fill;
            cantidadProducto.Location = new Point(180, 178);
            cantidadProducto.Margin = new Padding(0, 6, 0, 12);
            cantidadProducto.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            cantidadProducto.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            cantidadProducto.Name = "cantidadProducto";
            cantidadProducto.Size = new Size(612, 30);
            cantidadProducto.TabIndex = 10;
            cantidadProducto.Value = new decimal(new int[] { 1, 0, 0, 0 });
            cantidadProducto.ValueChanged += AlCambiarImporte;
            // 
            // etiquetaNotasDetalle
            // 
            etiquetaNotasDetalle.AutoSize = true;
            etiquetaNotasDetalle.Location = new Point(20, 228);
            etiquetaNotasDetalle.Margin = new Padding(0, 8, 12, 12);
            etiquetaNotasDetalle.Name = "etiquetaNotasDetalle";
            etiquetaNotasDetalle.Size = new Size(55, 23);
            etiquetaNotasDetalle.TabIndex = 11;
            etiquetaNotasDetalle.Text = "Notas";
            // 
            // textoNotas
            // 
            textoNotas.Dock = DockStyle.Fill;
            textoNotas.Location = new Point(180, 226);
            textoNotas.Margin = new Padding(0, 6, 0, 12);
            textoNotas.MaxLength = 150;
            textoNotas.Name = "textoNotas";
            textoNotas.PlaceholderText = "Ej.: sin salsa (máximo 150 caracteres)";
            textoNotas.Size = new Size(612, 30);
            textoNotas.TabIndex = 12;
            // 
            // etiquetaImporteDetalle
            // 
            etiquetaImporteDetalle.AutoSize = true;
            etiquetaImporteDetalle.Location = new Point(20, 276);
            etiquetaImporteDetalle.Margin = new Padding(0, 8, 12, 12);
            etiquetaImporteDetalle.Name = "etiquetaImporteDetalle";
            etiquetaImporteDetalle.Size = new Size(71, 23);
            etiquetaImporteDetalle.TabIndex = 13;
            etiquetaImporteDetalle.Text = "Importe";
            // 
            // etiquetaPrecio
            // 
            etiquetaPrecio.AutoSize = true;
            etiquetaPrecio.Dock = DockStyle.Fill;
            etiquetaPrecio.Location = new Point(180, 274);
            etiquetaPrecio.Margin = new Padding(0, 6, 0, 12);
            etiquetaPrecio.Name = "etiquetaPrecio";
            etiquetaPrecio.Size = new Size(612, 25);
            etiquetaPrecio.TabIndex = 14;
            etiquetaPrecio.Text = "Selecciona un producto para ver su importe.";
            // 
            // distribucionPrincipal
            // 
            distribucionPrincipal.AutoSize = true;
            distribucionPrincipal.ColumnCount = 2;
            distribucionPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            distribucionPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 169F));
            distribucionPrincipal.Controls.Add(etiquetaTitulo, 0, 0);
            distribucionPrincipal.Controls.Add(botonVolverAMesas, 1, 0);
            distribucionPrincipal.Controls.Add(etiquetaDescripcion, 0, 1);
            distribucionPrincipal.Controls.Add(panelEncabezado, 0, 2);
            distribucionPrincipal.Controls.Add(panelDetalle, 0, 3);
            distribucionPrincipal.Dock = DockStyle.Top;
            distribucionPrincipal.Location = new Point(27, 28);
            distribucionPrincipal.Name = "distribucionPrincipal";
            distribucionPrincipal.RowCount = 4;
            distribucionPrincipal.RowStyles.Add(new RowStyle(SizeType.Absolute, 55F));
            distribucionPrincipal.RowStyles.Add(new RowStyle(SizeType.Absolute, 104F));
            distribucionPrincipal.RowStyles.Add(new RowStyle());
            distribucionPrincipal.RowStyles.Add(new RowStyle());
            distribucionPrincipal.Size = new Size(818, 1259);
            distribucionPrincipal.TabIndex = 0;
            // 
            // etiquetaTitulo
            // 
            etiquetaTitulo.Dock = DockStyle.Fill;
            etiquetaTitulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            etiquetaTitulo.Location = new Point(3, 0);
            etiquetaTitulo.Name = "etiquetaTitulo";
            etiquetaTitulo.Size = new Size(643, 55);
            etiquetaTitulo.TabIndex = 0;
            etiquetaTitulo.Text = "Gestión de comandas";
            etiquetaTitulo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // botonVolverAMesas
            // 
            botonVolverAMesas.BackColor = Color.FromArgb(232, 238, 246);
            botonVolverAMesas.Dock = DockStyle.Fill;
            botonVolverAMesas.FlatAppearance.BorderSize = 0;
            botonVolverAMesas.FlatStyle = FlatStyle.Flat;
            botonVolverAMesas.Location = new Point(649, 7);
            botonVolverAMesas.Margin = new Padding(0, 7, 0, 7);
            botonVolverAMesas.Name = "botonVolverAMesas";
            botonVolverAMesas.Size = new Size(169, 41);
            botonVolverAMesas.TabIndex = 1;
            botonVolverAMesas.Text = "← Volver a mesas";
            botonVolverAMesas.UseVisualStyleBackColor = false;
            botonVolverAMesas.Click += AlVolverAMesas;
            // 
            // etiquetaDescripcion
            // 
            etiquetaDescripcion.BackColor = Color.FromArgb(255, 245, 212);
            distribucionPrincipal.SetColumnSpan(etiquetaDescripcion, 2);
            etiquetaDescripcion.Dock = DockStyle.Fill;
            etiquetaDescripcion.Location = new Point(0, 69);
            etiquetaDescripcion.Margin = new Padding(0, 14, 0, 18);
            etiquetaDescripcion.Name = "etiquetaDescripcion";
            etiquetaDescripcion.Padding = new Padding(18, 9, 18, 9);
            etiquetaDescripcion.Size = new Size(818, 72);
            etiquetaDescripcion.TabIndex = 2;
            etiquetaDescripcion.Text = "Selecciona una mesa desde el mapa para comenzar.";
            etiquetaDescripcion.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panelEncabezado
            // 
            panelEncabezado.AutoSize = true;
            panelEncabezado.BackColor = Color.White;
            panelEncabezado.ColumnCount = 1;
            distribucionPrincipal.SetColumnSpan(panelEncabezado, 2);
            panelEncabezado.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            panelEncabezado.Controls.Add(etiquetaSeccion, 0, 0);
            panelEncabezado.Controls.Add(etiquetaAyuda, 0, 1);
            panelEncabezado.Controls.Add(etiquetaCliente, 0, 2);
            panelEncabezado.Controls.Add(filaCliente, 0, 3);
            panelEncabezado.Controls.Add(etiquetaReferencia, 0, 4);
            panelEncabezado.Controls.Add(textoReferencia, 0, 5);
            panelEncabezado.Controls.Add(etiquetaNota, 0, 6);
            panelEncabezado.Controls.Add(botonGuardar, 0, 7);
            panelEncabezado.Dock = DockStyle.Top;
            panelEncabezado.Location = new Point(0, 159);
            panelEncabezado.Margin = new Padding(0);
            panelEncabezado.Name = "panelEncabezado";
            panelEncabezado.Padding = new Padding(27, 28, 27, 28);
            panelEncabezado.RowCount = 8;
            panelEncabezado.RowStyles.Add(new RowStyle());
            panelEncabezado.RowStyles.Add(new RowStyle());
            panelEncabezado.RowStyles.Add(new RowStyle());
            panelEncabezado.RowStyles.Add(new RowStyle(SizeType.Absolute, 55F));
            panelEncabezado.RowStyles.Add(new RowStyle());
            panelEncabezado.RowStyles.Add(new RowStyle());
            panelEncabezado.RowStyles.Add(new RowStyle());
            panelEncabezado.RowStyles.Add(new RowStyle(SizeType.Absolute, 67F));
            panelEncabezado.Size = new Size(818, 448);
            panelEncabezado.TabIndex = 3;
            // 
            // etiquetaSeccion
            // 
            etiquetaSeccion.AutoSize = true;
            etiquetaSeccion.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            etiquetaSeccion.Location = new Point(27, 28);
            etiquetaSeccion.Margin = new Padding(0, 0, 0, 9);
            etiquetaSeccion.Name = "etiquetaSeccion";
            etiquetaSeccion.Size = new Size(205, 30);
            etiquetaSeccion.TabIndex = 0;
            etiquetaSeccion.Text = "Datos de la cuenta";
            // 
            // etiquetaAyuda
            // 
            etiquetaAyuda.AutoSize = true;
            etiquetaAyuda.ForeColor = Color.FromArgb(95, 110, 130);
            etiquetaAyuda.Location = new Point(27, 67);
            etiquetaAyuda.Margin = new Padding(0, 0, 0, 28);
            etiquetaAyuda.Name = "etiquetaAyuda";
            etiquetaAyuda.Size = new Size(419, 23);
            etiquetaAyuda.TabIndex = 1;
            etiquetaAyuda.Text = "Selecciona el cliente al que se asociará esta comanda.";
            // 
            // etiquetaCliente
            // 
            etiquetaCliente.AutoSize = true;
            etiquetaCliente.Location = new Point(27, 118);
            etiquetaCliente.Margin = new Padding(0, 0, 0, 9);
            etiquetaCliente.Name = "etiquetaCliente";
            etiquetaCliente.Size = new Size(63, 23);
            etiquetaCliente.TabIndex = 2;
            etiquetaCliente.Text = "Cliente";
            // 
            // filaCliente
            // 
            filaCliente.ColumnCount = 2;
            filaCliente.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            filaCliente.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 163F));
            filaCliente.Controls.Add(textoCliente, 0, 0);
            filaCliente.Controls.Add(botonBuscar, 1, 0);
            filaCliente.Dock = DockStyle.Fill;
            filaCliente.Location = new Point(27, 150);
            filaCliente.Margin = new Padding(0);
            filaCliente.Name = "filaCliente";
            filaCliente.RowCount = 1;
            filaCliente.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            filaCliente.Size = new Size(764, 55);
            filaCliente.TabIndex = 3;
            // 
            // textoCliente
            // 
            textoCliente.Dock = DockStyle.Fill;
            textoCliente.Location = new Point(0, 6);
            textoCliente.Margin = new Padding(0, 6, 14, 0);
            textoCliente.Name = "textoCliente";
            textoCliente.PlaceholderText = "Nombre, identificación o teléfono";
            textoCliente.Size = new Size(587, 30);
            textoCliente.TabIndex = 0;
            // 
            // botonBuscar
            // 
            botonBuscar.BackColor = Color.FromArgb(24, 49, 83);
            botonBuscar.Dock = DockStyle.Fill;
            botonBuscar.FlatAppearance.BorderSize = 0;
            botonBuscar.FlatStyle = FlatStyle.Flat;
            botonBuscar.ForeColor = Color.White;
            botonBuscar.Location = new Point(601, 0);
            botonBuscar.Margin = new Padding(0, 0, 0, 9);
            botonBuscar.Name = "botonBuscar";
            botonBuscar.Size = new Size(163, 46);
            botonBuscar.TabIndex = 1;
            botonBuscar.Text = "Buscar cliente";
            botonBuscar.UseVisualStyleBackColor = false;
            botonBuscar.Click += AlBuscarCliente;
            // 
            // etiquetaReferencia
            // 
            etiquetaReferencia.AutoSize = true;
            etiquetaReferencia.Location = new Point(27, 223);
            etiquetaReferencia.Margin = new Padding(0, 18, 0, 9);
            etiquetaReferencia.Name = "etiquetaReferencia";
            etiquetaReferencia.Size = new Size(370, 23);
            etiquetaReferencia.TabIndex = 4;
            etiquetaReferencia.Text = "Nombre de referencia * (máximo 50 caracteres)";
            etiquetaReferencia.Visible = false;
            // 
            // textoReferencia
            // 
            textoReferencia.Dock = DockStyle.Top;
            textoReferencia.Location = new Point(27, 255);
            textoReferencia.Margin = new Padding(0, 0, 0, 9);
            textoReferencia.MaxLength = 50;
            textoReferencia.Name = "textoReferencia";
            textoReferencia.PlaceholderText = "Nombre de quien solicita la cuenta";
            textoReferencia.Size = new Size(764, 30);
            textoReferencia.TabIndex = 5;
            textoReferencia.Visible = false;
            // 
            // etiquetaNota
            // 
            etiquetaNota.AutoSize = true;
            etiquetaNota.ForeColor = Color.FromArgb(95, 110, 130);
            etiquetaNota.Location = new Point(27, 312);
            etiquetaNota.Margin = new Padding(0, 18, 0, 18);
            etiquetaNota.Name = "etiquetaNota";
            etiquetaNota.Size = new Size(505, 23);
            etiquetaNota.TabIndex = 6;
            etiquetaNota.Text = "Para Consumidor Generico se solicitará un nombre de referencia.";
            // 
            // botonGuardar
            // 
            botonGuardar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            botonGuardar.BackColor = Color.FromArgb(255, 209, 79);
            botonGuardar.FlatAppearance.BorderSize = 0;
            botonGuardar.FlatStyle = FlatStyle.Flat;
            botonGuardar.ForeColor = Color.Black;
            botonGuardar.Location = new Point(624, 356);
            botonGuardar.Name = "botonGuardar";
            botonGuardar.Size = new Size(164, 48);
            botonGuardar.TabIndex = 7;
            botonGuardar.Text = "Crear comanda";
            botonGuardar.UseVisualStyleBackColor = false;
            botonGuardar.Click += AlGuardarEncabezado;
            // 
            // VistaComanda
            // 
            AutoScaleDimensions = new SizeF(9F, 23F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            BackColor = Color.FromArgb(246, 248, 251);
            Controls.Add(distribucionPrincipal);
            Font = new Font("Segoe UI", 10F);
            ForeColor = Color.FromArgb(24, 49, 83);
            Name = "VistaComanda";
            Padding = new Padding(27, 28, 27, 28);
            Size = new Size(872, 713);
            Load += AlCargarComanda;
            panelDetalle.ResumeLayout(false);
            panelDetalle.PerformLayout();
            accionesDetalle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)tablaDetalle).EndInit();
            filaProducto.ResumeLayout(false);
            filaProducto.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)cantidadProducto).EndInit();
            distribucionPrincipal.ResumeLayout(false);
            distribucionPrincipal.PerformLayout();
            panelEncabezado.ResumeLayout(false);
            panelEncabezado.PerformLayout();
            filaCliente.ResumeLayout(false);
            filaCliente.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private TableLayoutPanel panelDetalle;
        private ComboBox selectorCuenta;
        private ComboBox selectorCategoria;
        private TextBox textoProducto;
        private TableLayoutPanel filaProducto;
        private Button botonBuscarProducto;
        private NumericUpDown cantidadProducto;
        private TextBox textoNotas;
        private Label etiquetaPrecio;
        private Label etiquetaSubtotal;
        private Button botonAgregarProducto;
        private Button botonActualizarDetalle;
        private DataGridView tablaDetalle;
        private FlowLayoutPanel accionesDetalle;
        private Label etiquetaCuenta;
        private Label etiquetaCategoriaDetalle;
        private Label etiquetaProductoDetalle;
        private Label etiquetaCantidadDetalle;
        private Label etiquetaNotasDetalle;
        private Label etiquetaImporteDetalle;
        private TableLayoutPanel distribucionPrincipal;
        private TableLayoutPanel panelEncabezado;
        private TableLayoutPanel filaCliente;
        private Label etiquetaTitulo;
        private Label etiquetaDescripcion;
        private Label etiquetaSeccion;
        private Label etiquetaAyuda;
        private Label etiquetaCliente;
        private Label etiquetaReferencia;
        private Label etiquetaNota;
        private TextBox textoCliente;
        private TextBox textoReferencia;
        private Button botonVolverAMesas;
        private Button botonBuscar;
        private Button botonGuardar;
    }
}
