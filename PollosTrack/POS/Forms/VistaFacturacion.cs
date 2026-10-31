using PollosTrack.POS.Models;

namespace PollosTrack.POS.Forms
{
    public partial class VistaFacturacion : UserControl
    {
        public event EventHandler? VolverAMesasSolicitado;
        public Mesa? MesaActual { get; private set; }

        // Mantiene la apertura desde el menú y el diseñador sin una mesa seleccionada.
        public VistaFacturacion()
        {
            InitializeComponent();
            etiquetaDescripcion.Text = "Facturación y cierre\r\n\r\nSelecciona una mesa por cobrar en el mapa para continuar.";
        }

        // Recibe la mesa sin elegir automáticamente una cuenta ni registrar un cobro.
        public VistaFacturacion(Mesa mesa) : this()
        {
            MesaActual = mesa;
            etiquetaDescripcion.Text = $"Mesa {mesa.Numero} · Facturación\r\n\r\nAquí se elegirá la cuenta que se desea cobrar.\r\nLa facturación está pendiente de implementar.";
        }

        // Solicita regresar al mapa sin modificar el estado de la mesa.
        private void AlVolverAMesas(object? origen, EventArgs argumentos)
        {
            if (VolverAMesasSolicitado != null)
            {
                VolverAMesasSolicitado(this, EventArgs.Empty);
            }
        }
    }
}
