using Microsoft.Data.SqlClient;
using PollosTrack.Catalogos.Models;
using PollosTrack.Catalogos.Repositories;
using PollosTrack.Utils;

namespace PollosTrack.Catalogos.Forms
{
    public class UC_Categorias : UserControl
    {
        private readonly CategoriaRepository _repo = new();
        private readonly DataGridView grid = new();
        private readonly Label lblModo = new();
        private readonly TextBox txtNombre = new() { MaxLength = 50 };
        private readonly CheckBox chkActivo = new() { Text = "Activa", Checked = true, AutoSize = true };
        private Categoria? _seleccionada;

        public UC_Categorias()
        {
            Dock = DockStyle.Fill;
            BackColor = EstiloUI.Crema;
            Font = new Font("Segoe UI", 10F);

            var panel = new Panel { Dock = DockStyle.Right, Width = 310, BackColor = Color.White };

            lblModo.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblModo.ForeColor = EstiloUI.Azul;
            lblModo.Location = new Point(15, 12);
            panel.Controls.Add(lblModo);

            int y = 50;
            y = EstiloUI.AgregarCampo(panel, "Nombre", txtNombre, y);
            chkActivo.Location = new Point(15, y);
            panel.Controls.Add(chkActivo);
            y += 45;

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
                Text = "Para quitar una categoría, desmarque \"Activa\" y guarde.",
                ForeColor = Color.DimGray,
                Bounds = new Rectangle(15, y + 50, 280, 40)
            });

            EstiloUI.ConfigurarGrid(grid);
            grid.CellClick += AlHacerClic;

            Controls.Add(grid);    // Fill primero
            Controls.Add(panel);   // luego el borde derecho

            Cargar();
            Nuevo();
        }

        private void Cargar()
        {
            try
            {
                grid.DataSource = _repo.ObtenerTodas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar las categorías:\n" + ex.Message,
                    "Categorías", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            grid.Columns["CategoriaId"]!.Visible = false;
            grid.Columns["Activo"]!.HeaderText = "Activa";
        }

        private void AlHacerClic(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (grid.Rows[e.RowIndex].DataBoundItem is not Categoria c) return;

            _seleccionada = c;
            lblModo.Text = "Editando categoría";
            txtNombre.Text = c.Nombre;
            chkActivo.Checked = c.Activo;
        }

        private void Nuevo()
        {
            _seleccionada = null;
            lblModo.Text = "Nueva categoría";
            txtNombre.Clear();
            chkActivo.Checked = true;
            grid.ClearSelection();
            txtNombre.Focus();
        }

        private void Guardar(object? sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            if (nombre.Length == 0)
            {
                MessageBox.Show("Ingrese el nombre de la categoría.", "Categorías",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (_seleccionada == null)
                {
                    _repo.Insertar(new Categoria { Nombre = nombre, Activo = chkActivo.Checked });
                }
                else
                {
                    _seleccionada.Nombre = nombre;
                    _seleccionada.Activo = chkActivo.Checked;
                    _repo.Actualizar(_seleccionada);
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                MessageBox.Show("Ya existe una categoría con ese nombre.", "Categorías",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo guardar:\n" + ex.Message, "Categorías",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Cargar();
            Nuevo();
        }
    }
}