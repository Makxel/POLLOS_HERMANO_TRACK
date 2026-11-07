using PollosTrack.Catalogos.Models;
using PollosTrack.Catalogos.Repositories;
using PollosTrack.Utils;

namespace PollosTrack.Catalogos.Forms
{
    public class UC_Productos : UserControl
    {
        private readonly ProductoRepository _repo = new();
        private readonly CategoriaRepository _repoCategorias = new();
        private readonly DataGridView grid = new();
        private readonly Label lblModo = new();
        private readonly ComboBox cmbCategoria = new() { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly TextBox txtNombre = new() { MaxLength = 100 };
        private readonly TextBox txtDescripcion = new() { MaxLength = 250, Multiline = true };
        private readonly NumericUpDown numPrecio = new() { DecimalPlaces = 2, Minimum = 0, Maximum = 100000 };
        private readonly CheckBox chkIva = new() { Text = "Aplica IVA", Checked = true, AutoSize = true };
        private readonly CheckBox chkActivo = new() { Text = "Activo", Checked = true, AutoSize = true };
        private readonly CheckBox chkInventario = new() { Text = "Controla inventario", Checked = true, AutoSize = true };
        private readonly NumericUpDown numMinimo = new() { Minimum = 0, Maximum = 100000, Value = 5 };

        private List<Producto> _productos = new();
        private List<Categoria> _categorias = new();
        private Producto? _seleccionado;

        public UC_Productos()
        {
            Dock = DockStyle.Fill;
            BackColor = EstiloUI.Crema;
            Font = new Font("Segoe UI", 10F);

            var panel = new Panel { Dock = DockStyle.Right, Width = 310, BackColor = Color.White, AutoScroll = true };

            lblModo.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblModo.ForeColor = EstiloUI.Azul;
            lblModo.Location = new Point(15, 12);
            panel.Controls.Add(lblModo);

            int y = 50;
            y = EstiloUI.AgregarCampo(panel, "Categoría", cmbCategoria, y);
            y = EstiloUI.AgregarCampo(panel, "Nombre", txtNombre, y);
            y = EstiloUI.AgregarCampo(panel, "Descripción", txtDescripcion, y, 60);
            y = EstiloUI.AgregarCampo(panel, "Precio unitario", numPrecio, y);

            chkIva.Location = new Point(15, y);
            chkActivo.Location = new Point(160, y);
            y += 32;
            chkInventario.Location = new Point(15, y);
            y += 32;
            panel.Controls.AddRange(new Control[] { chkIva, chkActivo, chkInventario });
            y = EstiloUI.AgregarCampo(panel, "Stock mínimo", numMinimo, y);
            chkInventario.CheckedChanged += (s, e) => numMinimo.Enabled = chkInventario.Checked;

            var btnGuardar = new Button { Text = "Guardar", Bounds = new Rectangle(15, y, 135, 38) };
            var btnNuevo = new Button { Text = "Nuevo", Bounds = new Rectangle(160, y, 135, 38) };
            EstiloUI.EstilizarBoton(btnGuardar, true);
            EstiloUI.EstilizarBoton(btnNuevo, false);
            btnGuardar.Click += Guardar;
            btnNuevo.Click += (s, e) => Nuevo();
            panel.Controls.Add(btnGuardar);
            panel.Controls.Add(btnNuevo);

            panel.Controls.Add(new Label
            {
                Text = "El inventario y el stock mínimo se definen al crear el producto; después se gestionan en Inventario. Para quitar un producto, desmarque \"Activo\" y guarde.",
                ForeColor = Color.DimGray,
                Bounds = new Rectangle(15, y + 50, 280, 90)
            });

            EstiloUI.ConfigurarGrid(grid);
            grid.CellClick += AlHacerClic;

            Controls.Add(grid);
            Controls.Add(panel);

            Recargar();
        }

        // Se llama al abrir la pestaña, por si se crearon categorías nuevas
        public void Recargar()
        {
            try
            {
                _categorias = _repoCategorias.ObtenerTodas();
                cmbCategoria.DataSource = null;
                cmbCategoria.DisplayMember = "Nombre";
                cmbCategoria.ValueMember = "CategoriaId";
                cmbCategoria.DataSource = _categorias;
                _productos = _repo.ObtenerTodos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar los productos:\n" + ex.Message,
                    "Productos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            grid.DataSource = _productos.Select(p => new
            {
                Id = p.ProductoId,
                Producto = p.Nombre,
                Categoria = _categorias.FirstOrDefault(c => c.CategoriaId == p.CategoriaId)?.Nombre ?? "",
                Precio = p.PrecioUnitario,
                IVA = p.AplicaIva ? "Sí" : "No",
                Estado = p.Activo ? "Activo" : "Inactivo"
            }).ToList();

            grid.Columns["Id"]!.Visible = false;
            grid.Columns["Categoria"]!.HeaderText = "Categoría";
            grid.Columns["Precio"]!.DefaultCellStyle.Format = "N2";
            Nuevo();
        }

        private void AlHacerClic(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (grid.Rows[e.RowIndex].Cells["Id"].Value is not int id) return;
            var p = _productos.FirstOrDefault(x => x.ProductoId == id);
            if (p == null) return;

            _seleccionado = p;
            lblModo.Text = "Editando producto";
            cmbCategoria.SelectedValue = p.CategoriaId;
            txtNombre.Text = p.Nombre;
            txtDescripcion.Text = p.Descripcion ?? string.Empty;
            numPrecio.Value = Math.Min(p.PrecioUnitario, numPrecio.Maximum);
            chkIva.Checked = p.AplicaIva;
            chkActivo.Checked = p.Activo;
            chkInventario.Enabled = false;
            numMinimo.Enabled = false;
        }

        private void Nuevo()
        {
            _seleccionado = null;
            lblModo.Text = "Nuevo producto";
            if (cmbCategoria.Items.Count > 0) cmbCategoria.SelectedIndex = 0;
            txtNombre.Clear();
            txtDescripcion.Clear();
            numPrecio.Value = 0;
            chkIva.Checked = true;
            chkActivo.Checked = true;
            chkInventario.Enabled = true;
            chkInventario.Checked = true;
            numMinimo.Value = 5;
            numMinimo.Enabled = true;
            grid.ClearSelection();
            txtNombre.Focus();
        }

        private static void Aviso(string texto) =>
            MessageBox.Show(texto, "Productos", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        private void Guardar(object? sender, EventArgs e)
        {
            if (cmbCategoria.SelectedValue is not int categoriaId)
            {
                Aviso("Primero cree una categoría y selecciónela.");
                return;
            }

            string nombre = txtNombre.Text.Trim();
            if (nombre.Length == 0)
            {
                Aviso("Ingrese el nombre del producto.");
                txtNombre.Focus();
                return;
            }

            var p = _seleccionado ?? new Producto();
            p.CategoriaId = categoriaId;
            p.Nombre = nombre;
            p.Descripcion = string.IsNullOrWhiteSpace(txtDescripcion.Text) ? null : txtDescripcion.Text.Trim();
            p.PrecioUnitario = numPrecio.Value;
            p.AplicaIva = chkIva.Checked;
            p.Activo = chkActivo.Checked;

            try
            {
                if (_seleccionado == null)
                    _repo.Insertar(p, chkInventario.Checked, (int)numMinimo.Value);
                else
                    _repo.Actualizar(p);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo guardar el producto:\n" + ex.Message, "Productos",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Recargar();
        }
    }
}