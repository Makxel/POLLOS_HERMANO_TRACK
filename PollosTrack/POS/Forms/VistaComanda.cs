using Microsoft.Data.SqlClient;
using System.Data;
using PollosTrack.POS.Models;
using PollosTrack.Catalogos.Models;
using PollosTrack.Utils;
using Mesa = PollosTrack.POS.Models.Mesa;

namespace PollosTrack.POS.Forms
{
    public partial class VistaComanda : UserControl
    {
        public event EventHandler? VolverAMesasSolicitado;
        public Mesa? MesaActual { get; private set; }
        public bool EsNuevaCuenta { get; private set; }
        private Cliente? clienteSeleccionado;
        private bool clienteGenerico;
        private bool pedidoGuardado;
        private int pedidoActualId;
        private bool cargandoCuentas;
        private Producto? productoSeleccionado;
        private List<Producto> productos = new List<Producto>();

        // Cuando exista inicio de sesión, asignar aquí el ID del usuario autenticado.
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        
        public int? UsuarioSesionId { get; set; }

        // Conserva la apertura desde el diseñador y desde el menú sin mesa seleccionada.
        public VistaComanda()
        {
            InitializeComponent();


        }

        // Recibe la mesa y la intención de crear una cuenta independiente.
        public VistaComanda(Mesa mesa, bool esNuevaCuenta) : this()
        {
            MesaActual = mesa;
            EsNuevaCuenta = esNuevaCuenta;
            etiquetaDescripcion.Text = $"Mesa {mesa.Numero} · Capacidad: {mesa.Capacidad} personas";
            panelEncabezado.Visible = esNuevaCuenta;
            if (!esNuevaCuenta)
                etiquetaDescripcion.Text += "\r\nSelecciona una cuenta para consultar sus productos.";
        }

        // Busca un usuario activo ya registrado; la sesión tendrá prioridad cuando exista.
        private int ObtenerUsuarioResponsable(SqlConnection conexion, SqlTransaction transaccion)
        {
            string consulta = "SELECT USUARIO_ID FROM SEG.USUARIOS WHERE USERNAME=@Nombre AND ACTIVO=1";
            if (UsuarioSesionId.HasValue)
            {
                consulta = "SELECT USUARIO_ID FROM SEG.USUARIOS WHERE USUARIO_ID=@Usuario AND ACTIVO=1";
            }
            using SqlCommand comando = new SqlCommand(consulta, conexion, transaccion);
            comando.Parameters.Add("@Nombre", SqlDbType.VarChar, 50).Value = "Sistema";
            if (UsuarioSesionId.HasValue)
            {
                comando.Parameters.Add("@Usuario", SqlDbType.Int).Value = UsuarioSesionId.Value;
            }
            object? resultado = comando.ExecuteScalar();
            if (resultado == null)
            {
                throw new InvalidOperationException("No existe el usuario responsable activo. Revisa el registro Sistema o la sesión.");
            }
            return Convert.ToInt32(resultado);
        }

        // Obtiene la clave del estado mediante su entidad y código.
        private int ObtenerIdentificadorEstado(SqlConnection conexion, SqlTransaction transaccion, string entidad, string codigo)
        {
            using SqlCommand comando = new SqlCommand(
                "SELECT ESTADO_ID FROM CAT.ESTADOS WHERE ENTIDAD=@Entidad AND CODIGO=@Codigo", conexion, transaccion);
            comando.Parameters.Add("@Entidad", SqlDbType.VarChar, 25).Value = entidad;
            comando.Parameters.Add("@Codigo", SqlDbType.VarChar, 20).Value = codigo;
            object? resultado = comando.ExecuteScalar();
            if (resultado == null)
            {
                throw new InvalidOperationException($"No está configurado el estado {codigo} para {entidad}.");
            }
            return Convert.ToInt32(resultado);
        }
        // Abre el buscador modal reutilizando el modelo Cliente de Catálogos.
        private void AlBuscarCliente(object? origen, EventArgs argumentos)
        {
            using BuscadorClientes buscador = new BuscadorClientes(textoCliente.Text);
            if (buscador.ShowDialog(this) != DialogResult.OK || buscador.ClienteSeleccionado == null) return;
            clienteSeleccionado = buscador.ClienteSeleccionado;
            clienteGenerico = buscador.EsClienteGenerico;
            textoCliente.Text = clienteSeleccionado.Nombre;
            textoCliente.ReadOnly = true;
            etiquetaReferencia.Visible = clienteGenerico;
            textoReferencia.Visible = clienteGenerico;
            textoReferencia.Clear();
            if (clienteGenerico) textoReferencia.Focus();
        }

        // Guarda encabezado y ocupación de mesa en una sola transacción.
        private void AlGuardarEncabezado(object? origen, EventArgs argumentos)
        {
            if (pedidoGuardado || MesaActual == null || !EsNuevaCuenta) return;
            if (clienteSeleccionado == null) { MessageBox.Show(this, "Selecciona un cliente con el buscador."); return; }
            string referencia = textoReferencia.Text.Trim();
            if (clienteGenerico && (referencia.Length == 0 || referencia.Length > 50))
            { MessageBox.Show(this, "Escribe la referencia del cliente, entre 1 y 50 caracteres."); textoReferencia.Focus(); return; }
            botonGuardar.Enabled = false;
            try
            {
                using SqlConnection conexion = new conexion_sql_server().AbrirConexion();
                using SqlTransaction transaccion = conexion.BeginTransaction();
                int usuarioResponsableId = ObtenerUsuarioResponsable(conexion, transaccion);
                int estadoPedidoId = ObtenerIdentificadorEstado(conexion, transaccion, "ENCABEZADO_PEDIDO", "ABIERTO");
                int estadoMesaId = ObtenerIdentificadorEstado(conexion, transaccion, "MESA", "OCUPADA");

                // Comprueba y ocupa la mesa dentro de la misma transacción del encabezado.
                const string consultaMesa = """
                    UPDATE CAT.MESAS SET ESTADO_ID=@Estado
                    WHERE MESA_ID=@Mesa AND ESTADO_ID IN
                    (SELECT ESTADO_ID FROM CAT.ESTADOS WHERE ENTIDAD='MESA'
                     AND CODIGO IN ('DISPONIBLE','OCUPADA','POR_COBRAR','POR COBRAR'));
                    """;
                using SqlCommand comandoMesa = new SqlCommand(consultaMesa, conexion, transaccion);
                comandoMesa.Parameters.Add("@Estado", SqlDbType.Int).Value = estadoMesaId;
                comandoMesa.Parameters.Add("@Mesa", SqlDbType.Int).Value = MesaActual.MesaId;
                int mesasActualizadas = comandoMesa.ExecuteNonQuery();
                if (mesasActualizadas != 1)
                {
                    throw new InvalidOperationException("La mesa no existe o su estado no permite abrir cuentas.");
                }

                const string consulta = """
                    INSERT INTO POS.ENCABEZADO_PEDIDO (MESA_ID,USUARIO_ID,CLIENTE_ID,NOMBRE_REFERENCIA,ESTADO_ID)
                    OUTPUT INSERTED.PEDIDO_ID VALUES (@Mesa,@Usuario,@Cliente,@Referencia,@EstadoPedido);
                    """;
                using SqlCommand comando = new SqlCommand(consulta, conexion, transaccion);
                comando.Parameters.Add("@EstadoPedido", SqlDbType.Int).Value = estadoPedidoId;
                comando.Parameters.Add("@Mesa", SqlDbType.Int).Value = MesaActual.MesaId;
                comando.Parameters.Add("@Usuario", SqlDbType.Int).Value = usuarioResponsableId;
                comando.Parameters.Add("@Cliente", SqlDbType.Int).Value = clienteSeleccionado.ClienteId;
                comando.Parameters.Add("@Referencia", SqlDbType.VarChar, 50).Value = DBNull.Value;
                if (clienteGenerico) comando.Parameters["@Referencia"].Value = referencia;
                int pedidoId = Convert.ToInt32(comando.ExecuteScalar());
                transaccion.Commit();
                pedidoGuardado = true;
                panelEncabezado.Visible = false;
                pedidoActualId = pedidoId;
                etiquetaDescripcion.Text = $"Mesa {MesaActual.Numero} · Pedido {pedidoId} creado\r\nAgrega los productos de esta cuenta.";
            }
            catch (Exception excepcion) { MessageBox.Show(this, excepcion.Message, "No se pudo crear el pedido"); }
            finally { if (!pedidoGuardado) botonGuardar.Enabled = true; }
            if (pedidoGuardado) MostrarDetallePedido();
        }

        // La carga se omite en el diseñador: allí todos los controles quedan editables.
        private void AlCargarComanda(object? origen, EventArgs argumentos)
        {
            if (DesignMode || System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
            {
                return;
            }
            panelEncabezado.Visible = EsNuevaCuenta;
            panelDetalle.Visible = false;
            selectorCuenta.Visible = false;
            etiquetaCuenta.Visible = false;
            if (MesaActual == null || EsNuevaCuenta) return;
            try
            {
                using SqlConnection conexion = new conexion_sql_server().AbrirConexion();
                using SqlCommand comando = new SqlCommand("""
                    SELECT pedido.PEDIDO_ID,CONCAT('Cuenta ',pedido.PEDIDO_ID,' · ',cliente.NOMBRE,' · ',pedido.NOMBRE_REFERENCIA) AS Nombre
                    FROM POS.ENCABEZADO_PEDIDO pedido JOIN CAT.CLIENTES cliente ON cliente.CLIENTE_ID=pedido.CLIENTE_ID
                    JOIN CAT.ESTADOS estado ON estado.ESTADO_ID=pedido.ESTADO_ID
                    WHERE pedido.MESA_ID=@Mesa AND estado.ENTIDAD='ENCABEZADO_PEDIDO'
                    AND estado.CODIGO IN ('ABIERTO','POR_COBRAR','POR COBRAR') ORDER BY pedido.PEDIDO_ID;
                    """, conexion);
                comando.Parameters.Add("@Mesa", SqlDbType.Int).Value = MesaActual.MesaId;
                using SqlDataReader lector = comando.ExecuteReader();
                DataTable cuentas = new DataTable();
                cuentas.Load(lector);
                cargandoCuentas = true;
                selectorCuenta.DisplayMember = "Nombre";
                selectorCuenta.ValueMember = "PEDIDO_ID";
                selectorCuenta.DataSource = cuentas;
                selectorCuenta.SelectedIndex = -1;
                panelDetalle.Visible = true;
                selectorCuenta.Visible = true;
                etiquetaCuenta.Visible = true;
                etiquetaSubtotal.Text = "Selecciona una cuenta. Si no aparecen opciones, no hay cuentas abiertas.";
            }
            catch (Exception excepcion) { MessageBox.Show(this, excepcion.Message, "No se pudieron cargar las cuentas"); }
            finally { cargandoCuentas = false; }
        }

        // Identifica la cuenta elegida sin asumir que una mesa tiene un solo pedido.
        private void AlElegirCuenta(object? origen, EventArgs argumentos)
        {
            if (!cargandoCuentas && selectorCuenta.SelectedItem is DataRowView cuenta)
            {
                pedidoActualId = Convert.ToInt32(cuenta["PEDIDO_ID"]);
                MostrarDetallePedido();
            }
        }

        // Recupera productos y categorías activos antes de consultar las líneas guardadas.
        private void MostrarDetallePedido()
        {
            panelDetalle.Visible = true;
            try
            {
                using SqlConnection conexion = new conexion_sql_server().AbrirConexion();
                using SqlCommand comando = new SqlCommand("""
                    SELECT CATEGORIA_ID,NOMBRE FROM CAT.CATEGORIAS WHERE ACTIVO=1 ORDER BY NOMBRE;
                    SELECT producto.PRODUCTO_ID,producto.CATEGORIA_ID,producto.NOMBRE,producto.PRECIO_UNITARIO,producto.DESCRIPCION
                    FROM CAT.PRODUCTOS producto JOIN CAT.CATEGORIAS categoria ON categoria.CATEGORIA_ID=producto.CATEGORIA_ID
                    WHERE producto.ACTIVO=1 AND categoria.ACTIVO=1 ORDER BY producto.NOMBRE;
                    """, conexion);
                using SqlDataReader lector = comando.ExecuteReader();
                List<Categoria> categorias = new List<Categoria>();
                categorias.Add(new Categoria { Nombre = "Todas las categorías" });
                while (lector.Read())
                {
                    categorias.Add(new Categoria { CategoriaId = lector.GetInt32(0), Nombre = lector.GetString(1) });
                }
                lector.NextResult();
                productos.Clear();
                while (lector.Read())
                {
                    productos.Add(new Producto { ProductoId = lector.GetInt32(0), CategoriaId = lector.GetInt32(1), Nombre = lector.GetString(2), PrecioUnitario = lector.GetDecimal(3), Descripcion = Convert.ToString(lector.GetValue(4)) });
                }
                selectorCategoria.DisplayMember = nameof(Categoria.Nombre);
                selectorCategoria.DataSource = categorias;
                AlCambiarCategoria(this, EventArgs.Empty);
                CargarLineasPedido();
            }
            catch (Exception excepcion)
            {
                botonAgregarProducto.Enabled = false;
                MessageBox.Show(this, excepcion.Message, "No se pudo cargar el detalle. Pulsa Actualizar para reintentar.");
            }
        }

        // Filtra el catálogo en memoria según la categoría elegida.
        private void AlCambiarCategoria(object? origen, EventArgs argumentos)
        {
            productoSeleccionado = null;
            textoProducto.Clear();
            AlCambiarImporte(this, EventArgs.Empty);
        }

        // Calcula la vista previa del importe; al guardar se usa el precio vigente del catálogo.
        private void AlCambiarImporte(object? origen, EventArgs argumentos)
        {
            etiquetaPrecio.Text = "Selecciona un producto con el botón Buscar producto.";
            botonAgregarProducto.Enabled = false;
            if (productoSeleccionado is Producto producto && pedidoActualId > 0)
            {
                etiquetaPrecio.Text = $"Precio: C$ {producto.PrecioUnitario:N2} · Importe: C$ {producto.PrecioUnitario * cantidadProducto.Value:N2}";
                botonAgregarProducto.Enabled = true;
            }
        }

        // Guarda una línea pendiente dentro de una transacción, sin descontar inventario todavía.
        private void AlAgregarProducto(object? origen, EventArgs argumentos)
        {
            if (pedidoActualId == 0 || MesaActual == null || productoSeleccionado is not Producto producto) return;
            botonAgregarProducto.Enabled = false;
            bool guardado = false;
            try
            {
                using SqlConnection conexion = new conexion_sql_server().AbrirConexion();
                using SqlTransaction transaccion = conexion.BeginTransaction();
                using SqlCommand validar = new SqlCommand("""
                    SELECT estado.CODIGO FROM POS.ENCABEZADO_PEDIDO pedido WITH (UPDLOCK,HOLDLOCK)
                    JOIN CAT.ESTADOS estado ON estado.ESTADO_ID=pedido.ESTADO_ID
                    WHERE pedido.PEDIDO_ID=@Pedido AND pedido.MESA_ID=@Mesa AND estado.ENTIDAD='ENCABEZADO_PEDIDO';
                    """, conexion, transaccion);
                validar.Parameters.Add("@Pedido", SqlDbType.Int).Value = pedidoActualId;
                validar.Parameters.Add("@Mesa", SqlDbType.Int).Value = MesaActual.MesaId;
                if (Convert.ToString(validar.ExecuteScalar()) != "ABIERTO")
                {
                    throw new InvalidOperationException("Solo se pueden agregar productos a una cuenta abierta.");
                }
                int estadoId = ObtenerIdentificadorEstado(conexion, transaccion, "DETALLE_PEDIDO", "PENDIENTE");
                using SqlCommand insertar = new SqlCommand("""
                    INSERT INTO POS.DETALLE_PEDIDO (PEDIDO_ID,PRODUCTO_ID,CANTIDAD,PRECIO_UNITARIO,NOTAS,ESTADO_ID,MINUTOS_PREPARACION)
                    SELECT @Pedido,producto.PRODUCTO_ID,@Cantidad,producto.PRECIO_UNITARIO,@Notas,@Estado,@Minutos
                    FROM CAT.PRODUCTOS producto JOIN CAT.CATEGORIAS categoria ON categoria.CATEGORIA_ID=producto.CATEGORIA_ID
                    WHERE producto.PRODUCTO_ID=@Producto AND producto.ACTIVO=1 AND categoria.ACTIVO=1;
                    """, conexion, transaccion);
                insertar.Parameters.Add("@Pedido", SqlDbType.Int).Value = pedidoActualId;
                insertar.Parameters.Add("@Producto", SqlDbType.Int).Value = producto.ProductoId;
                insertar.Parameters.Add("@Cantidad", SqlDbType.Int).Value = Convert.ToInt32(cantidadProducto.Value);
                insertar.Parameters.Add("@Notas", SqlDbType.VarChar, 150).Value = textoNotas.Text.Trim();
                insertar.Parameters.Add("@Estado", SqlDbType.Int).Value = estadoId;
                insertar.Parameters.Add("@Minutos", SqlDbType.Int).Value = DBNull.Value;

                if (insertar.ExecuteNonQuery() != 1)
                {
                    throw new InvalidOperationException("El producto o la categoría ya no están activos.");
                }
                transaccion.Commit();
                guardado = true;
                cantidadProducto.Value = 1;

                textoNotas.Clear();
            }
            catch (Exception excepcion) { MessageBox.Show(this, excepcion.Message, "No se pudo agregar el producto"); }
            finally { AlCambiarImporte(this, EventArgs.Empty); }
            // Un fallo de lectura posterior no debe provocar una segunda inserción.
            if (guardado)
            {
                try { CargarLineasPedido(); }
                catch (Exception excepcion) { MessageBox.Show(this, "El producto se guardó. Pulsa Actualizar para verlo.\r\n" + excepcion.Message); }
            }
        }

        // Muestra precios históricos y calcula el subtotal sin las líneas canceladas.
        private void CargarLineasPedido()
        {
            using SqlConnection conexion = new conexion_sql_server().AbrirConexion();
            using SqlCommand comando = new SqlCommand("""
                SELECT producto.NOMBRE AS Producto,detalle.CANTIDAD AS Cantidad,detalle.PRECIO_UNITARIO AS Precio,
                    detalle.CANTIDAD*detalle.PRECIO_UNITARIO AS Importe,detalle.NOTAS AS Notas,
                    detalle.MINUTOS_PREPARACION AS Minutos,estado.NOMBRE AS Estado,estado.CODIGO AS CodigoEstado
                FROM POS.DETALLE_PEDIDO detalle JOIN CAT.PRODUCTOS producto ON producto.PRODUCTO_ID=detalle.PRODUCTO_ID
                JOIN CAT.ESTADOS estado ON estado.ESTADO_ID=detalle.ESTADO_ID
                WHERE detalle.PEDIDO_ID=@Pedido ORDER BY detalle.DETALLE_ID;
                """, conexion);
            comando.Parameters.Add("@Pedido", SqlDbType.Int).Value = pedidoActualId;
            using SqlDataReader lector = comando.ExecuteReader();
            DataTable detalles = new DataTable();
            detalles.Load(lector);
            tablaDetalle.DataSource = detalles;
            foreach (DataGridViewColumn columna in tablaDetalle.Columns)
            {
                if (columna.Name == "CodigoEstado") columna.Visible = false;
                if (columna.Name == "Precio" || columna.Name == "Importe") columna.DefaultCellStyle.Format = "N2";
            }
            decimal subtotal = 0;
            foreach (DataRow detalle in detalles.Rows)
            {
                if (Convert.ToString(detalle["CodigoEstado"]) != "CANCELADO")
                {
                    subtotal += Convert.ToDecimal(detalle["Importe"]);
                }
            }
            etiquetaSubtotal.Text = $"Cuenta {pedidoActualId} · Subtotal: C$ {subtotal:N2} · IVA y cobro en Facturación";
        }

        // Recarga el detalle únicamente cuando el usuario lo solicita.
        private void AlActualizarDetalle(object? origen, EventArgs argumentos)
        {
            if (pedidoActualId > 0) MostrarDetallePedido();
        }
        // Regresa al mapa para consultar la ocupación actualizada.
        private void AlVolverAMesas(object? origen, EventArgs argumentos)
        {
            if (VolverAMesasSolicitado != null) VolverAMesasSolicitado(this, EventArgs.Empty);
        }

        // Filtra el catálogo cargado y abre una selección modal sin consultas adicionales.
        private void AlBuscarProducto(object? origen, EventArgs argumentos)
        {
            List<Producto> disponibles = new List<Producto>();
            if (selectorCategoria.SelectedItem is Categoria categoria)
            {
                foreach (Producto producto in productos)
                {
                    if (categoria.CategoriaId == 0 || producto.CategoriaId == categoria.CategoriaId)
                    {
                        disponibles.Add(producto);
                    }
                }
            }
            if (disponibles.Count == 0)
            {
                MessageBox.Show(this, "No hay productos activos en esta categoría. Revisa Catálogos y pulsa Actualizar.");
                return;
            }
            using BuscadorProductos buscador = new BuscadorProductos(disponibles);
            if (buscador.ShowDialog(this) == DialogResult.OK && buscador.ProductoSeleccionado != null)
            {
                productoSeleccionado = buscador.ProductoSeleccionado;
                textoProducto.Text = productoSeleccionado.Nombre;
                if (!string.IsNullOrWhiteSpace(productoSeleccionado.Descripcion))
                {
                    textoProducto.Text += " · " + productoSeleccionado.Descripcion;
                }
                AlCambiarImporte(this, EventArgs.Empty);
            }
        }

        // Ventana sencilla de selección. Devuelve el objeto Producto de Catálogos.
        private class BuscadorProductos : Form
        {
            private DataGridView tablaProductos = new DataGridView();
            public Producto? ProductoSeleccionado { get; private set; }

            public BuscadorProductos(List<Producto> productos)
            {
                Text = "Seleccionar producto";
                StartPosition = FormStartPosition.CenterParent;
                Size = new Size(820, 460);
                MinimumSize = new Size(600, 360);
                Padding = new Padding(16);
                BackColor = Color.White;
                Font = new Font("Segoe UI", 10F);
                tablaProductos.Dock = DockStyle.Fill;
                tablaProductos.ReadOnly = true;
                tablaProductos.AllowUserToAddRows = false;
                tablaProductos.AllowUserToDeleteRows = false;
                tablaProductos.MultiSelect = false;
                tablaProductos.RowHeadersVisible = false;
                tablaProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                tablaProductos.AutoGenerateColumns = false;
                tablaProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                tablaProductos.BackgroundColor = Color.White;
                tablaProductos.BorderStyle = BorderStyle.None;
                tablaProductos.EnableHeadersVisualStyles = false;
                tablaProductos.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(24, 49, 83);
                tablaProductos.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                tablaProductos.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(246, 248, 251);
                tablaProductos.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 235, 171);
                tablaProductos.DefaultCellStyle.SelectionForeColor = Color.Black;
                tablaProductos.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Producto", DataPropertyName = nameof(Producto.Nombre) });
                tablaProductos.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Descripción", DataPropertyName = nameof(Producto.Descripcion), FillWeight = 160 });
                DataGridViewTextBoxColumn columnaPrecio = new DataGridViewTextBoxColumn();
                columnaPrecio.HeaderText = "Precio C$";
                columnaPrecio.DataPropertyName = nameof(Producto.PrecioUnitario);
                columnaPrecio.DefaultCellStyle.Format = "N2";
                columnaPrecio.FillWeight = 60;
                tablaProductos.Columns.Add(columnaPrecio);
                tablaProductos.DataSource = productos;
                tablaProductos.CellDoubleClick += AlPulsarFila;
                FlowLayoutPanel acciones = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 55, Padding = new Padding(0, 12, 0, 0) };
                Button seleccionar = new Button { Text = "Seleccionar", Width = 140, Height = 34, BackColor = Color.FromArgb(255, 209, 79), FlatStyle = FlatStyle.Flat };
                seleccionar.Click += AlSeleccionarProducto;
                Button cancelar = new Button { Text = "Cancelar", Width = 120, Height = 34, DialogResult = DialogResult.Cancel };
                acciones.Controls.Add(seleccionar);
                acciones.Controls.Add(cancelar);
                Controls.Add(tablaProductos);
                Controls.Add(acciones);
                AcceptButton = seleccionar;
                CancelButton = cancelar;
            }

            // Permite confirmar también con doble clic sobre una fila de datos.
            private void AlPulsarFila(object? origen, DataGridViewCellEventArgs argumentos)
            {
                if (argumentos.RowIndex >= 0) AlSeleccionarProducto(this, EventArgs.Empty);
            }

            // Cierra el modal únicamente con una selección válida.
            private void AlSeleccionarProducto(object? origen, EventArgs argumentos)
            {
                if (tablaProductos.CurrentRow != null && tablaProductos.CurrentRow.DataBoundItem is Producto producto)
                {
                    ProductoSeleccionado = producto;
                    DialogResult = DialogResult.OK;
                }
            }
        }
        // Modal local de esta vista: evita agregar archivos o carpetas auxiliares.
        private class BuscadorClientes : Form
        {
            private TextBox textoBusqueda = new TextBox();
            private DataGridView tablaClientes = new DataGridView();
            private List<Cliente> clientes = new List<Cliente>();
            private int identificadorGenerico;
            public Cliente? ClienteSeleccionado { get; private set; }
            public bool EsClienteGenerico { get; private set; }

            // Construye la tabla y un único campo de búsqueda para los tres datos.
            public BuscadorClientes(string busqueda)
            {
                Text = "Seleccionar cliente";
                Size = new Size(820, 520); MinimumSize = new Size(650, 420); BackColor = Color.White; Font = new Font("Segoe UI", 10F); Padding = new Padding(20);
                StartPosition = FormStartPosition.CenterParent;
                // El contenedor agrega espacio interior y separa el buscador de la tabla.
                Panel panelBusqueda = new Panel();
                panelBusqueda.Dock = DockStyle.Top;
                panelBusqueda.Height = textoBusqueda.PreferredHeight + 32;
                panelBusqueda.Padding = new Padding(0, 0, 0, 12);

                Panel marcoBusqueda = new Panel();
                marcoBusqueda.Dock = DockStyle.Fill;
                marcoBusqueda.BackColor = Color.FromArgb(241, 244, 248);
                marcoBusqueda.BorderStyle = BorderStyle.FixedSingle;
                marcoBusqueda.Padding = new Padding(10, 8, 10, 8);

                textoBusqueda.Dock = DockStyle.Top;
                textoBusqueda.BorderStyle = BorderStyle.None;
                textoBusqueda.BackColor = marcoBusqueda.BackColor;
                textoBusqueda.ForeColor = Color.FromArgb(24, 49, 83);
                marcoBusqueda.Controls.Add(textoBusqueda);
                panelBusqueda.Controls.Add(marcoBusqueda);
                textoBusqueda.PlaceholderText = "Buscar por nombre, identificación o teléfono";
                textoBusqueda.Text = busqueda;
                textoBusqueda.TextChanged += AlFiltrarClientes;
                tablaClientes.Dock = DockStyle.Fill;
                tablaClientes.BackgroundColor = Color.White;
                tablaClientes.BorderStyle = BorderStyle.None;
                tablaClientes.RowHeadersVisible = false;
                tablaClientes.EnableHeadersVisualStyles = false;
                tablaClientes.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(24, 49, 83);
                tablaClientes.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                tablaClientes.ColumnHeadersHeight = 40;
                tablaClientes.RowTemplate.Height = 36;
                tablaClientes.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(246, 248, 251);
                tablaClientes.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 235, 171);
                tablaClientes.DefaultCellStyle.SelectionForeColor = Color.Black;
                tablaClientes.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                tablaClientes.GridColor = Color.FromArgb(232, 236, 242);
                tablaClientes.ReadOnly = true;
                tablaClientes.AllowUserToAddRows = false;
                tablaClientes.AllowUserToDeleteRows = false;
                tablaClientes.MultiSelect = false;
                tablaClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                tablaClientes.AutoGenerateColumns = false;
                tablaClientes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                tablaClientes.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Identificación", DataPropertyName = nameof(Cliente.Identificacion) });
                tablaClientes.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nombre", DataPropertyName = nameof(Cliente.Nombre) });
                tablaClientes.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Teléfono", DataPropertyName = nameof(Cliente.Telefono) });
                FlowLayoutPanel acciones = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 60, Padding = new Padding(0, 12, 0, 0) };
                Button seleccionar = new Button { Text = "Seleccionar", Width = 130 };
                seleccionar.Click += AlSeleccionarCliente;

                Button cancelar = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel };
                acciones.Controls.Add(seleccionar); acciones.Controls.Add(cancelar);
                Controls.Add(tablaClientes); Controls.Add(panelBusqueda); Controls.Add(acciones);
                // Botones planos y consistentes con la paleta de la aplicación.
                foreach (Control control in acciones.Controls)
                {
                    if (control is Button boton)
                    {
                        boton.Height = 36;
                        boton.FlatStyle = FlatStyle.Flat;
                        boton.FlatAppearance.BorderSize = 0;
                        boton.BackColor = Color.FromArgb(239, 243, 248);
                        boton.ForeColor = Color.FromArgb(24, 49, 83);
                    }
                }
                seleccionar.BackColor = Color.FromArgb(255, 209, 79);
                CancelButton = cancelar;
                Load += AlCargarClientes;
            }

            // Consulta clientes una vez; las búsquedas posteriores se realizan en memoria.
            private void AlCargarClientes(object? origen, EventArgs argumentos)
            {
                try
                {
                    using SqlConnection conexion = new conexion_sql_server().AbrirConexion();
                    using SqlCommand comando = new SqlCommand("SELECT CLIENTE_ID,IDENTIFICACION,NOMBRE,TELEFONO FROM CAT.CLIENTES ORDER BY NOMBRE", conexion);
                    using SqlDataReader lector = comando.ExecuteReader();
                    while (lector.Read())
                    {
                        Cliente cliente = new Cliente { ClienteId = lector.GetInt32(0), Nombre = lector.GetString(2) };
                        if (!lector.IsDBNull(1)) cliente.Identificacion = lector.GetString(1);
                        if (!lector.IsDBNull(3)) cliente.Telefono = lector.GetString(3);
                        clientes.Add(cliente);
                        // Convención inicial: Consumidor Generico es el cliente genérico.
                        if (string.Equals(cliente.Nombre.Trim(), "Consumidor Generico", StringComparison.OrdinalIgnoreCase))
                            identificadorGenerico = cliente.ClienteId;
                    }
                    AlFiltrarClientes(this, EventArgs.Empty);
                }
                catch (Exception excepcion) { MessageBox.Show(this, excepcion.Message, "Error al consultar clientes"); }
            }

            // Busca el mismo texto en nombre, identificación o teléfono.
            private void AlFiltrarClientes(object? origen, EventArgs argumentos)
            {
                string busqueda = textoBusqueda.Text.Trim();
                List<Cliente> resultado = new List<Cliente>();
                foreach (Cliente cliente in clientes)
                {
                    bool coincide = cliente.Nombre.Contains(busqueda, StringComparison.OrdinalIgnoreCase);
                    if (cliente.Identificacion != null && cliente.Identificacion.Contains(busqueda, StringComparison.OrdinalIgnoreCase)) coincide = true;
                    if (cliente.Telefono != null && cliente.Telefono.Contains(busqueda, StringComparison.OrdinalIgnoreCase)) coincide = true;
                    if (coincide) resultado.Add(cliente);
                }
                tablaClientes.DataSource = resultado;
            }

            // Devuelve el cliente elegido a la comanda.
            private void AlSeleccionarCliente(object? origen, EventArgs argumentos)
            {
                if (tablaClientes.CurrentRow == null) return;
                if (tablaClientes.CurrentRow.DataBoundItem is Cliente cliente)
                {
                    ClienteSeleccionado = cliente;
                    EsClienteGenerico = cliente.ClienteId == identificadorGenerico;
                    DialogResult = DialogResult.OK;
                }
            }

        }
    }
}
