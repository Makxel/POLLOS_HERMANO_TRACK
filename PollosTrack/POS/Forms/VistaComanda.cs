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

        // Cuando exista inicio de sesión, asignar aquí el ID del usuario autenticado.
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        
        public int? UsuarioSesionId { get; set; }

        // Conserva la apertura desde el diseñador y desde el menú sin mesa seleccionada.
        public VistaComanda()
        {
            InitializeComponent();
            panelEncabezado.Visible = false;

        }

        // Recibe la mesa y la intención de crear una cuenta independiente.
        public VistaComanda(Mesa mesa, bool esNuevaCuenta) : this()
        {
            MesaActual = mesa;
            EsNuevaCuenta = esNuevaCuenta;
            etiquetaDescripcion.Text = $"Mesa {mesa.Numero} · Capacidad: {mesa.Capacidad} personas";
            panelEncabezado.Visible = esNuevaCuenta;
            if (!esNuevaCuenta)
                etiquetaDescripcion.Text += "\r\nLa consulta de cuentas existentes está pendiente.";
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
                panelEncabezado.Enabled = false;
                etiquetaDescripcion.Text = $"Mesa {MesaActual.Numero} · Pedido {pedidoId} creado\r\nLa asociación de productos se implementará en la siguiente etapa.";
            }
            catch (Exception excepcion) { MessageBox.Show(this, excepcion.Message, "No se pudo crear el pedido"); }
            finally { if (!pedidoGuardado) botonGuardar.Enabled = true; }
        }

        // Regresa al mapa para consultar la ocupación actualizada.
        private void AlVolverAMesas(object? origen, EventArgs argumentos)
        {
            if (VolverAMesasSolicitado != null) VolverAMesasSolicitado(this, EventArgs.Empty);
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
