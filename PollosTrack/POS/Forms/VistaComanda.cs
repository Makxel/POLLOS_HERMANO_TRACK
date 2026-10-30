using PollosTrack.POS.Models;

namespace PollosTrack.POS.Forms
{
    public partial class VistaComanda : UserControl
    {
        public event EventHandler? VolverAMesasSolicitado;
        public Mesa? MesaActual { get; private set; }
        public bool EsNuevaCuenta { get; private set; }

        // Permite abrir la vista desde el menú y desde el diseñador, sin una mesa elegida.
        public VistaComanda()
        {
            InitializeComponent();
            etiquetaDescripcion.Text = "Gestión de comandas\r\n\r\nSelecciona una mesa en el mapa para continuar.";
        }

        // Recibe el contexto de navegación. Todavía no crea ni consulta pedidos.
        public VistaComanda(Mesa mesa, bool esNuevaCuenta) : this()
        {
            MesaActual = mesa;
            EsNuevaCuenta = esNuevaCuenta;

            if (esNuevaCuenta)
            {
                etiquetaDescripcion.Text = $"Mesa {mesa.Numero} · Nueva cuenta\r\n\r\nAquí se preparará una comanda independiente para esta mesa.\r\nTodavía no se ha creado ningún pedido.";
            }
            else
            {
                etiquetaDescripcion.Text = $"Mesa {mesa.Numero} · Ver comandas\r\n\r\nAquí se mostrarán las cuentas abiertas de esta mesa.\r\nLa selección y edición de pedidos está pendiente de implementar.";
            }
        }

        // Delega el regreso al formulario principal para conservar el layout común.
        private void AlVolverAMesas(object? origen, EventArgs argumentos)
        {
            if (VolverAMesasSolicitado != null)
            {
                VolverAMesasSolicitado(this, EventArgs.Empty);
            }
        }
    }
}
