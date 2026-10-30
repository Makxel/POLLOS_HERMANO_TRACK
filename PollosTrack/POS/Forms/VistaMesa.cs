using System.ComponentModel;
using System.Globalization;
using PollosTrack.POS.Models;
using Microsoft.Data.SqlClient;
using PollosTrack.Utils;

namespace PollosTrack.POS.Forms
{
    public partial class VistaMesa : UserControl
    {
        private List<Mesa> mesas = new List<Mesa>();
        private List<Estado> estados = new List<Estado>();
        // null indica que todavía no se ha seleccionado una mesa.
        private int? identificadorMesaSeleccionada;
        private bool consultaEnCurso;
        private bool actualizandoFiltro;
        private bool datosCargados;

        // El formulario principal escucha estas solicitudes y se encarga de navegar.
        public event EventHandler? AbrirComandasSolicitado;
        public event EventHandler? NuevaCuentaSolicitada;
        public event EventHandler? FacturacionSolicitada;
        public Mesa? MesaSeleccionada { get; private set; }

        // Crea los controles definidos en el diseñador de WinForms.
        public VistaMesa()
        {
            InitializeComponent();
        }

        // Consulta las mesas una vez al abrir la vista, fuera del diseñador.
        private async void AlCargarVista(object? origen, EventArgs argumentos)
        {
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return;
            }

            await CargarMesas();
        }

        // Vuelve a consultar los datos únicamente cuando se pulsa Actualizar.
        private async void AlPulsarActualizar(object? origen, EventArgs argumentos)
        {
            await CargarMesas();
        }

        // Carga los datos sin bloquear la ventana y reconstruye el panel completo.
        private async Task CargarMesas()
        {
            if (consultaEnCurso || IsDisposed)
            {
                return;
            }

            consultaEnCurso = true;
            botonActualizar.Enabled = false;
            etiquetaEstado.Text = "Cargando mesas…";

            try
            {
                await ConsultarMesasYEstados();

                // El usuario puede salir mientras SQL Server responde.
                if (IsDisposed)
                {
                    return;
                }

                datosCargados = true;

                ActualizarFiltros();
                MostrarResumen();
                MostrarMesas();
                MostrarDetalleMesa();

                etiquetaEstado.Text = $"Última consulta: {DateTime.Now:HH:mm:ss} · Mesas registradas: {mesas.Count}";
                etiquetaEstado.ForeColor = Color.FromArgb(85, 99, 116);
            }
            catch (Exception excepcion)
            {
                if (IsDisposed)
                {
                    return;
                }

                etiquetaEstado.ForeColor = Color.DarkRed;
                if (datosCargados)
                {
                    etiquetaEstado.Text = "No se pudo actualizar. Se muestran los datos anteriores. Pulsa Actualizar para reintentar.";
                }
                else
                {
                    etiquetaEstado.Text = "No se pudieron cargar las mesas. Revisa la conexión y pulsa Actualizar para reintentar.";
                }

                // El error completo aparece en la ventana Salida durante la depuración.
                System.Diagnostics.Debug.WriteLine(excepcion);
            }
            finally
            {
                consultaEnCurso = false;
                if (!IsDisposed)
                {
                    botonActualizar.Enabled = true;
                }
            }
        }

        // Lee las tablas con la conexión compartida
        private async Task ConsultarMesasYEstados()
        {
            conexion_sql_server proveedorConexion = new conexion_sql_server();
            using SqlConnection conexion = await proveedorConexion.AbrirConexionAsincrona();
            const string consulta = """
                SELECT mesa.MESA_ID, mesa.NUMERO, mesa.CAPACIDAD, mesa.ESTADO_ID, estado.CODIGO
                FROM CAT.MESAS AS mesa
                LEFT JOIN CAT.ESTADOS AS estado
                    ON estado.ESTADO_ID = mesa.ESTADO_ID AND estado.ENTIDAD = 'MESA'
                ORDER BY mesa.NUMERO;

                SELECT ESTADO_ID, ENTIDAD, CODIGO, NOMBRE, COLOR_HEX
                FROM CAT.ESTADOS
                WHERE ENTIDAD = 'MESA'
                ORDER BY ORDEN, NOMBRE;
                """;
            using SqlCommand comando = new SqlCommand(consulta, conexion);
            using SqlDataReader lector = await comando.ExecuteReaderAsync();
            List<Mesa> mesasConsultadas = new List<Mesa>();
            List<Estado> estadosConsultados = new List<Estado>();

            // El primer resultado contiene las mesas y el código de su estado.
            while (await lector.ReadAsync())
            {
                Mesa mesa = new Mesa();
                mesa.MesaId = lector.GetInt32(0);
                mesa.Numero = lector.GetInt32(1);
                mesa.Capacidad = lector.GetInt32(2);
                if (!lector.IsDBNull(3))
                {
                    mesa.EstadoId = lector.GetInt32(3);
                }
                if (!lector.IsDBNull(4))
                {
                    mesa.CodigoEstado = lector.GetString(4).Trim();
                }
                mesasConsultadas.Add(mesa);
            }

            // El segundo resultado contiene los estados del filtro y sus colores.
            await lector.NextResultAsync();
            while (await lector.ReadAsync())
            {
                Estado estado = new Estado();
                estado.EstadoId = lector.GetInt32(0);
                estado.Entidad = lector.GetString(1);
                estado.Codigo = lector.GetString(2);
                estado.Nombre = lector.GetString(3);
                estado.ColorHexadecimal = lector.GetString(4);
                estadosConsultados.Add(estado);
            }

            // Reemplaza los datos solo cuando ambas lecturas terminan correctamente.
            mesas = mesasConsultadas;
            estados = estadosConsultados;
        }

        // Obtiene el código del filtro; una cadena vacía representa todas las mesas.
        private string ObtenerCodigoFiltro()
        {
            if (selectorEstado.SelectedItem is Estado estadoSeleccionado)
            {
                return estadoSeleccionado.Codigo;
            }

            return string.Empty;
        }

        // Recarga las opciones del catálogo y conserva el filtro anterior si existe.
        private void ActualizarFiltros()
        {
            string codigoAnterior = ObtenerCodigoFiltro();
            List<Estado> opciones = new List<Estado>();
            opciones.Add(new Estado { Nombre = "Todas las mesas" });
            opciones.AddRange(estados);

            // Cambiar DataSource dispara el evento del selector; evitamos dibujar dos veces.
            actualizandoFiltro = true;
            try
            {
                selectorEstado.DisplayMember = nameof(Estado.Nombre);
                selectorEstado.DataSource = opciones;
                selectorEstado.SelectedIndex = 0;

                for (int indice = 0; indice < opciones.Count; indice++)
                {
                    if (CoincideEstado(opciones[indice].Codigo, codigoAnterior))
                    {
                        selectorEstado.SelectedIndex = indice;
                        break;
                    }
                }
            }
            finally
            {
                actualizandoFiltro = false;
            }
        }

        // Aplica el filtro a los datos ya cargados, sin consultar SQL Server.
        private void AlCambiarFiltroEstado(object? origen, EventArgs argumentos)
        {
            if (!actualizandoFiltro)
            {
                MostrarMesas();
            }
        }

        // Cuenta las mesas de cada estado y muestra su color del catálogo.
        private void MostrarResumen()
        {
            LiberarControles(panelResumen);
            foreach (Estado estado in estados)
            {
                int cantidad = 0;
                foreach (Mesa mesa in mesas)
                {
                    if (CoincideEstado(mesa.CodigoEstado, estado.Codigo))
                    {
                        cantidad++;
                    }
                }

                Label etiquetaResumen = new Label();
                etiquetaResumen.Size = new Size(LogicalToDeviceUnits(156), LogicalToDeviceUnits(46));
                etiquetaResumen.Text = $"{estado.Nombre}   {cantidad}";
                etiquetaResumen.TextAlign = ContentAlignment.MiddleCenter;
                etiquetaResumen.BackColor = ObtenerColorEstado(estado);
                etiquetaResumen.ForeColor = Color.Black;
                etiquetaResumen.Margin = new Padding(4);
                panelResumen.Controls.Add(etiquetaResumen);
            }
        }

        // Reconstruye las tarjetas que cumplen el filtro conservando el desplazamiento.
        private void MostrarMesas()
        {
            Point desplazamientoAnterior = panelMesas.AutoScrollPosition;
            string codigoFiltro = ObtenerCodigoFiltro();
            panelMesas.SuspendLayout();

            try
            {
                LiberarControles(panelMesas);
                foreach (Mesa mesa in mesas)
                {
                    if (codigoFiltro != string.Empty && !CoincideEstado(mesa.CodigoEstado, codigoFiltro))
                    {
                        continue;
                    }

                    Button botonMesa = CrearBotonMesa(mesa);
                    panelMesas.Controls.Add(botonMesa);
                }

                if (panelMesas.Controls.Count == 0)
                {
                    Label etiquetaSinMesas = new Label();
                    etiquetaSinMesas.AutoSize = true;
                    etiquetaSinMesas.MaximumSize = new Size(LogicalToDeviceUnits(260), 0);
                    etiquetaSinMesas.Padding = new Padding(12);

                    if (mesas.Count == 0)
                    {
                        etiquetaSinMesas.Text = "Todavía no hay mesas registradas en el catálogo.";
                    }
                    else
                    {
                        etiquetaSinMesas.Text = "No hay mesas con el estado seleccionado.";
                    }

                    panelMesas.Controls.Add(etiquetaSinMesas);
                }
            }
            finally
            {
                panelMesas.ResumeLayout(true);
                panelMesas.AutoScrollPosition = new Point(-desplazamientoAnterior.X, -desplazamientoAnterior.Y);
            }
        }

        // Crea una tarjeta y guarda su identificador en Tag para reconocerla al pulsarla.
        private Button CrearBotonMesa(Mesa mesa)
        {
            Estado estado = BuscarEstado(mesa.CodigoEstado);
            Button botonMesa = new Button();
            botonMesa.Name = $"botonMesa{mesa.MesaId}";
            botonMesa.Tag = mesa.MesaId;
            botonMesa.Size = new Size(LogicalToDeviceUnits(148), LogicalToDeviceUnits(118));
            botonMesa.Margin = new Padding(LogicalToDeviceUnits(10));
            botonMesa.Text = $"Mesa {mesa.Numero}\r\n\r\n{mesa.Capacidad} personas\r\n{estado.Nombre}";
            botonMesa.BackColor = ObtenerColorEstado(estado);
            botonMesa.ForeColor = Color.Black;
            botonMesa.FlatStyle = FlatStyle.Flat;
            botonMesa.UseVisualStyleBackColor = false;
            botonMesa.Cursor = Cursors.Hand;
            botonMesa.FlatAppearance.BorderColor = Color.FromArgb(24, 49, 83);
            botonMesa.FlatAppearance.BorderSize = 1;

            if (mesa.MesaId == identificadorMesaSeleccionada)
            {
                botonMesa.FlatAppearance.BorderSize = 3;
            }

            botonMesa.Click += AlSeleccionarMesa;
            return botonMesa;
        }

        // Marca la tarjeta seleccionada y muestra su detalle, sin modificar la base.
        private void AlSeleccionarMesa(object? origen, EventArgs argumentos)
        {
            if (origen is Button botonSeleccionado && botonSeleccionado.Tag is int identificadorMesa)
            {
                identificadorMesaSeleccionada = identificadorMesa;

                foreach (Control control in panelMesas.Controls)
                {
                    if (control is Button botonMesa)
                    {
                        botonMesa.FlatAppearance.BorderSize = 1;
                        if (botonMesa == botonSeleccionado)
                        {
                            botonMesa.FlatAppearance.BorderSize = 3;
                        }
                    }
                }

                MostrarDetalleMesa();
            }
        }

        // Busca la mesa seleccionada; si ya no existe, limpia el detalle lateral.
        private void MostrarDetalleMesa()
        {
            foreach (Mesa mesa in mesas)
            {
                if (mesa.MesaId == identificadorMesaSeleccionada)
                {
                    Estado estado = BuscarEstado(mesa.CodigoEstado);
                    MesaSeleccionada = mesa;
                    ActualizarAccionesMesa(mesa);
                    etiquetaTituloDetalle.Text = $"Mesa {mesa.Numero}";
                    etiquetaDatosMesa.Text = $"Capacidad: {mesa.Capacidad} personas";
                    etiquetaEstadoMesa.Text = estado.Nombre;
                    etiquetaEstadoMesa.BackColor = ObtenerColorEstado(estado);
                    etiquetaEstadoMesa.ForeColor = Color.Black;
                    return;
                }
            }

            MesaSeleccionada = null;
            botonVerComandas.Visible = false;
            botonNuevaCuenta.Visible = false;
            botonIrAFacturacion.Visible = false;
            identificadorMesaSeleccionada = null;
            etiquetaTituloDetalle.Text = "Selecciona una mesa";
            etiquetaDatosMesa.Text = "Aquí verás su capacidad y estado actual.";
            etiquetaEstadoMesa.Text = string.Empty;
            etiquetaEstadoMesa.BackColor = Color.Transparent;
        }

        // Habilita accesos según el estado. Navegar nunca cambia la mesa ni crea pedidos.
        private void ActualizarAccionesMesa(Mesa mesa)
        {
            bool disponible = CoincideEstado(mesa.CodigoEstado, "DISPONIBLE");
            bool ocupada = CoincideEstado(mesa.CodigoEstado, "OCUPADA");
            bool porCobrar = CoincideEstado(mesa.CodigoEstado, "POR COBRAR");

            botonVerComandas.Visible = ocupada || porCobrar;
            botonNuevaCuenta.Visible = disponible || ocupada || porCobrar;
            botonIrAFacturacion.Visible = porCobrar;
            botonNuevaCuenta.Text = "Nueva cuenta separada";
            if (disponible)
            {
                botonNuevaCuenta.Text = "Nueva comanda";
            }
        }

        // Solicita ver las comandas de la mesa; todavía no se elige un pedido específico.
        private void AlPulsarVerComandas(object? origen, EventArgs argumentos)
        {
            if (MesaSeleccionada != null && AbrirComandasSolicitado != null)
            {
                AbrirComandasSolicitado(this, EventArgs.Empty);
            }
        }

        // Indica la intención de abrir otra cuenta para la misma mesa, sin guardarla.
        private void AlPulsarNuevaCuenta(object? origen, EventArgs argumentos)
        {
            if (MesaSeleccionada != null && NuevaCuentaSolicitada != null)
            {
                NuevaCuentaSolicitada(this, EventArgs.Empty);
            }
        }

        // Solicita abrir Facturación con el contexto de la mesa seleccionada.
        private void AlPulsarFacturacion(object? origen, EventArgs argumentos)
        {
            if (MesaSeleccionada != null && FacturacionSolicitada != null)
            {
                FacturacionSolicitada(this, EventArgs.Empty);
            }
        }
        // Compara códigos ignorando mayúsculas y espacios externos.
        private static bool CoincideEstado(string codigoMesa, string codigoCatalogo)
        {
            return string.Equals(codigoMesa.Trim(), codigoCatalogo.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        // Devuelve el estado del catálogo o una descripción neutral si no está configurado.
        private Estado BuscarEstado(string codigo)
        {
            foreach (Estado estado in estados)
            {
                if (CoincideEstado(codigo, estado.Codigo))
                {
                    return estado;
                }
            }

            return new Estado { Nombre = "Sin estado configurado" };
        }

        // Convierte #RRGGBB a Color; usa gris si el valor está vacío o no es válido.
        private static Color ObtenerColorEstado(Estado estado)
        {
            string hexadecimal = estado.ColorHexadecimal;
            if (hexadecimal.Length != 7 || !hexadecimal.StartsWith("#"))
            {
                return Color.Gainsboro;
            }

            int componentesColor;
            bool colorValido = int.TryParse(hexadecimal.Substring(1), NumberStyles.HexNumber,
                CultureInfo.InvariantCulture, out componentesColor);

            if (!colorValido)
            {
                return Color.Gainsboro;
            }

            // Añade opacidad completa al color leído del catálogo.
            return Color.FromArgb(255, Color.FromArgb(componentesColor));
        }

        // Libera los controles anteriores antes de reconstruir el panel.
        private static void LiberarControles(Control contenedor)
        {
            while (contenedor.Controls.Count > 0)
            {
                Control control = contenedor.Controls[0];
                contenedor.Controls.Remove(control);
                control.Dispose();
            }
        }
    }
}
