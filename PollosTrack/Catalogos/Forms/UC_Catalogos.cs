using PollosTrack.Utils;

namespace PollosTrack.Catalogos.Forms
{
    public class UC_Catalogos : UserControl
    {
        public UC_Catalogos()
        {
            Dock = DockStyle.Fill;
            BackColor = EstiloUI.Crema;

            var tabs = new TabControl { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 11F) };
            tabs.TabPages.Add(CrearPestana("Categorías", new UC_Categorias()));
            tabs.TabPages.Add(CrearPestana("Productos", new UC_Productos()));

            // Al abrir Productos se recargan las categorías por si hubo cambios
            tabs.SelectedIndexChanged += (s, e) =>
            {
                if (tabs.SelectedTab?.Tag is UC_Productos productos) productos.Recargar();
            };

            Controls.Add(tabs);
        }

        private static TabPage CrearPestana(string titulo, Control contenido)
        {
            contenido.Dock = DockStyle.Fill;
            var pestana = new TabPage(titulo) { Tag = contenido };
            pestana.Controls.Add(contenido);
            return pestana;
        }
    }
}