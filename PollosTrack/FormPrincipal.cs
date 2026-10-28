using System;
using System.Windows.Forms;
using PollosTrack.Inventario.Forms;

namespace PollosTrack
{
    public partial class FormPrincipal : Form
    {
        public FormPrincipal()
        {
            InitializeComponent();

            // Conectar el botón de Inventario
            botonInventario.Click += BotonInventario_Click;
        }

        private void BotonInventario_Click(object sender, EventArgs e)
        {
            // Limpiar el panel central
            panelContenido.Controls.Clear();

            // Crear el módulo de inventario
            UC_Inventario modulo = new UC_Inventario();
            modulo.Dock = DockStyle.Fill;

            // Agregarlo al panel
            panelContenido.Controls.Add(modulo);

            // Cambiar el título
            etiquetaTitulo.Text = "Inventario";
        }
    }
}