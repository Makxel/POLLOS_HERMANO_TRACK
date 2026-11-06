using PollosTrack.POS.Forms;

namespace PollosTrack
{
    public partial class FormPrincipal : Form
    {
        // Crea los controles y conecta los eventos definidos en el diseñador.
        public FormPrincipal()
        {
            InitializeComponent();
            // La imagen va incluida en el ejecutable y conserva sus proporciones con Zoom.
            using Stream? archivoLogo = typeof(FormPrincipal).Assembly.GetManifestResourceStream("PollosTrack.LogoEmpresa");
            if (archivoLogo != null)
            {
                using Image logo = Image.FromStream(archivoLogo);
                imagenLogo.Image = new Bitmap(logo);
            }
        }

        // Muestra u oculta las opciones del módulo POS.
        private void AlPulsarMenuPOS(object? origen, EventArgs argumentos)
        {
            panelSubmenuPOS.Visible = !panelSubmenuPOS.Visible;
        }

        // Abre el panel de mesas dentro del layout principal.
        private void AlAbrirMesas(object? origen, EventArgs argumentos)
        {
            VistaMesa vistaMesas = new VistaMesa();
            vistaMesas.AbrirComandasSolicitado += AlVerComandasDeMesa;
            vistaMesas.NuevaCuentaSolicitada += AlAbrirNuevaCuenta;
            vistaMesas.FacturacionSolicitada += AlFacturarMesa;
            MostrarVista(vistaMesas, "POS · Mesas", botonMesas);
        }

        // Abre la pantalla de comandas.
        private void AlAbrirComandas(object? origen, EventArgs argumentos)
        {
            VistaComanda vistaComanda = new VistaComanda();
            vistaComanda.VolverAMesasSolicitado += AlAbrirMesas;
            MostrarVista(vistaComanda, "POS · Comandas", botonComandas);
        }

        // Abre la pantalla de pedidos pendientes.
        private void AlAbrirPendientes(object? origen, EventArgs argumentos)
        {
            MostrarVista(new VistaPendientes(), "POS · Pendientes", botonPendientes);
        }

        // Abre la pantalla de facturación.
        private void AlAbrirFacturacion(object? origen, EventArgs argumentos)
        {
            VistaFacturacion vistaFacturacion = new VistaFacturacion();
            vistaFacturacion.VolverAMesasSolicitado += AlAbrirMesas;
            MostrarVista(vistaFacturacion, "POS · Facturación", botonFacturacion);
        }

        // Consulta pedidos históricos y permite volver a visualizar sus recibos.
        private void AlAbrirHistorial(object? origen, EventArgs argumentos)
        {
            MostrarVista(new VistaHistorial(), "POS · Historial de pedidos", botonHistorial);
        }
        // Abre Comandas con la mesa elegida, sin asumir que tiene un único pedido.
        private void AlVerComandasDeMesa(object? origen, EventArgs argumentos)
        {
            if (origen is VistaMesa vistaMesas && vistaMesas.MesaSeleccionada != null)
            {
                VistaComanda vistaComanda = new VistaComanda(vistaMesas.MesaSeleccionada, false);
                vistaComanda.VolverAMesasSolicitado += AlAbrirMesas;
                MostrarVista(vistaComanda, "POS · Comandas", botonComandas);
            }
        }

        // Pasa la intención de abrir una cuenta independiente; no crea ningún pedido.
        private void AlAbrirNuevaCuenta(object? origen, EventArgs argumentos)
        {
            if (origen is VistaMesa vistaMesas && vistaMesas.MesaSeleccionada != null)
            {
                VistaComanda vistaComanda = new VistaComanda(vistaMesas.MesaSeleccionada, true);
                vistaComanda.VolverAMesasSolicitado += AlAbrirMesas;
                MostrarVista(vistaComanda, "POS · Nueva comanda", botonComandas);
            }
        }

        // Abre Facturación con la mesa, dejando la elección de cuenta para la siguiente etapa.
        private void AlFacturarMesa(object? origen, EventArgs argumentos)
        {
            if (origen is VistaMesa vistaMesas && vistaMesas.MesaSeleccionada != null)
            {
                VistaFacturacion vistaFacturacion = new VistaFacturacion(vistaMesas.MesaSeleccionada);
                vistaFacturacion.VolverAMesasSolicitado += AlAbrirMesas;
                MostrarVista(vistaFacturacion, "POS · Facturación", botonFacturacion);
            }
        }

        // Limpia el contenido y vuelve al inicio.
        private void AlVolverAlInicio(object? origen, EventArgs argumentos)
        {
            LimpiarContenido();
            etiquetaTitulo.Text = "Inicio";
            panelSubmenuPOS.Visible = false;
            MarcarBotonSeleccionado(botonInicio);
        }

        // Todas las pantallas se cargan en el mismo contenedor, conservando el menú.
        private void MostrarVista(UserControl nuevaVista, string titulo, Button botonSeleccionado)
        {
            panelContenido.SuspendLayout();
            try
            {
                LimpiarContenido();
                nuevaVista.Dock = DockStyle.Fill;
                panelContenido.Controls.Add(nuevaVista);
                etiquetaTitulo.Text = titulo;
                MarcarBotonSeleccionado(botonSeleccionado);
            }
            finally
            {
                panelContenido.ResumeLayout(true);
            }
        }

        // Libera la vista anterior antes de cargar otra pantalla.
        private void LimpiarContenido()
        {
            // Quitar un control no libera sus recursos: también hay que disponerlo.
            while (panelContenido.Controls.Count > 0)
            {
                Control vistaAnterior = panelContenido.Controls[0];
                panelContenido.Controls.Remove(vistaAnterior);
                vistaAnterior.Dispose();
            }
        }

        // Resalta la opción elegida y restaura los colores de las demás.
        private void MarcarBotonSeleccionado(Button botonSeleccionado)
        {
            Button[] botonesNavegacion =
                { botonInicio, botonMesas, botonComandas, botonPendientes, botonFacturacion, botonHistorial };

            foreach (Button boton in botonesNavegacion)
            {
                if (boton == botonSeleccionado)
                {
                    boton.BackColor = Color.FromArgb(255, 209, 79);
                    boton.ForeColor = Color.FromArgb(24, 49, 83);
                }
                else
                {
                    boton.BackColor = Color.FromArgb(24, 49, 83);
                    boton.ForeColor = Color.White;
                }
            }
        }
    }
}
