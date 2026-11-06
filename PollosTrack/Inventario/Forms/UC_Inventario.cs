using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace PollosTrack.Inventario.Forms
{
    public partial class UC_Inventario : UserControl
    {
        private string conexion = "Server=localhost;Database=POLLOS_HERMANOS_TRACK;Trusted_Connection=True;TrustServerCertificate=True;";

        // La descripción del ingreso es el motivo del movimiento
        private TextBox CampoDescripcion => textBox3;

        // Campos que se localizan por el texto de su etiqueta
        private TextBox CampoCodigo;
        private TextBox CampoPrecio;
        private TextBox CampoTotal;

        // Se leen de la base de datos al iniciar (valores por defecto si no hay conexión)
        private int largoMotivo = 50;
        private bool hayColumnaPrecio = false;
        private bool hayColumnaCodigo = false;

        public UC_Inventario()
        {
            InitializeComponent();

            // Crea las columnas de precio y código si faltan, y detecta el esquema real
            AsegurarColumnas();
            DetectarEsquema();
            CampoDescripcion.MaxLength = largoMotivo;

            // Corrige ":Producto" -> "Producto:" y la alineación a la derecha
            this.RightToLeft = RightToLeft.No;
            AplicarRightToLeftNo(this);

            // Configuración visual del grid de stock
            dgvStock.AllowUserToAddRows = false;
            dgvStock.RowHeadersVisible = false;
            dgvStock.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            ConfigurarLayout();
            ConfigurarCamposIngreso();

            CargarStock();
            CargarProductos();
            btnRegistrarIngreso.Click += BtnRegistrarIngreso_Click;
            btnBuscarKardex.Click += btnBuscarKardex_Click;

            // Colorear filas de forma segura cuando se termina de enlazar
            dgvStock.DataBindingComplete += (s, e) =>
            {
                foreach (DataGridViewRow fila in dgvStock.Rows)
                {
                    if (fila.Cells["Estado"].Value == null) continue;

                    string estado = fila.Cells["Estado"].Value.ToString();

                    if (estado == "Agotado")
                        fila.DefaultCellStyle.BackColor = Color.LightCoral;
                    else if (estado == "Crítico")
                        fila.DefaultCellStyle.BackColor = Color.LightYellow;
                    else
                        fila.DefaultCellStyle.BackColor = Color.LightGreen;
                }
            };
        }

        // ===== Esquema de la base de datos =====

        // Agrega las columnas nuevas si no existen y deja el código igual al ID en todos los movimientos.
        // Necesita permiso ALTER/UPDATE sobre la tabla; si no lo hay, se ignora y la app sigue funcionando.
        private void AsegurarColumnas()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conexion))
                {
                    con.Open();

                    // Lote 1: crear las columnas si faltan
                    const string crear = @"
                        IF COL_LENGTH('INV.MOVIMIENTOS_INVENTARIO', 'PRECIO_COSTO') IS NULL
                            ALTER TABLE INV.MOVIMIENTOS_INVENTARIO ADD PRECIO_COSTO DECIMAL(18,2) NULL;

                        IF COL_LENGTH('INV.MOVIMIENTOS_INVENTARIO', 'CODIGO_MERCADERIA') IS NULL
                            ALTER TABLE INV.MOVIMIENTOS_INVENTARIO ADD CODIGO_MERCADERIA NVARCHAR(50) NULL;";

                    using (SqlCommand cmd = new SqlCommand(crear, con))
                        cmd.ExecuteNonQuery();

                    // Lote 2 (aparte, porque la columna debe existir al compilar la consulta):
                    // el código de cada movimiento es igual a su ID
                    const string igualar = @"
                        UPDATE INV.MOVIMIENTOS_INVENTARIO
                        SET CODIGO_MERCADERIA = CAST(MOVIMIENTO_ID AS NVARCHAR(50))
                        WHERE CODIGO_MERCADERIA IS NULL
                           OR CODIGO_MERCADERIA <> CAST(MOVIMIENTO_ID AS NVARCHAR(50));";

                    using (SqlCommand cmd = new SqlCommand(igualar, con))
                        cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                // Sin permiso o sin conexión: se continúa
            }
        }

        private void DetectarEsquema()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conexion))
                {
                    con.Open();

                    const string sql = @"
                        SELECT COLUMN_NAME, CHARACTER_MAXIMUM_LENGTH
                        FROM INFORMATION_SCHEMA.COLUMNS
                        WHERE TABLE_SCHEMA = 'INV' AND TABLE_NAME = 'MOVIMIENTOS_INVENTARIO'";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            string col = rd.GetString(0).ToUpperInvariant();

                            if (col == "MOTIVO" && !rd.IsDBNull(1))
                            {
                                int len = rd.GetInt32(1);
                                largoMotivo = len > 0 ? len : 4000; // -1 = MAX
                            }
                            else if (col == "PRECIO_COSTO")
                                hayColumnaPrecio = true;
                            else if (col == "CODIGO_MERCADERIA")
                                hayColumnaCodigo = true;
                        }
                    }
                }
            }
            catch
            {
                // Sin conexión: se usan los valores por defecto
            }
        }

        // Recorre todos los controles y fuerza RightToLeft = No
        private void AplicarRightToLeftNo(Control contenedor)
        {
            foreach (Control c in contenedor.Controls)
            {
                c.RightToLeft = RightToLeft.No;
                if (c.HasChildren)
                    AplicarRightToLeftNo(c);
            }
        }

        // ===== Layout adaptable a pantalla grande =====
        private void ConfigurarLayout()
        {
            // Sin márgenes: que llegue hasta las esquinas
            this.Padding = Padding.Empty;
            this.Margin = Padding.Empty;

            TabControl tabs = this.Controls.OfType<TabControl>().FirstOrDefault();
            if (tabs == null && dgvStock.Parent != null)
                tabs = dgvStock.Parent.Parent as TabControl;

            if (tabs != null)
            {
                tabs.Dock = DockStyle.Fill;
                tabs.Margin = Padding.Empty;
                tabs.Font = new Font("Segoe UI", 10.5f);

                // Pestañas que se reparten todo el ancho
                tabs.SizeMode = TabSizeMode.Fixed;

                bool ajustando = false;
                Action ajustarPestanas = () =>
                {
                    if (ajustando || tabs.TabCount == 0) return;

                    int ancho = (tabs.ClientSize.Width - 2) / tabs.TabCount;
                    if (ancho <= 0 || tabs.ItemSize.Width == ancho) return;

                    ajustando = true;
                    try
                    {
                        tabs.ItemSize = new Size(ancho, 40);
                    }
                    finally
                    {
                        ajustando = false;
                    }
                };

                tabs.Resize += (s, e) => ajustarPestanas();
                this.Load += (s, e) => ajustarPestanas();
            }

            // --- Pestaña Monitor de Stock: la tabla ocupa todo ---
            if (dgvStock.Parent is TabPage tabStock)
                tabStock.Padding = new Padding(15);

            dgvStock.Dock = DockStyle.Fill;
            dgvStock.Font = new Font("Segoe UI", 11f);
            dgvStock.RowTemplate.Height = 36;
            dgvStock.ColumnHeadersHeight = 40;
            dgvStock.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvStock.AllowUserToResizeRows = false;

            // --- Pestaña Kardex: la tabla crece hacia abajo y a la derecha ---
            dgvKardex.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvKardex.Font = new Font("Segoe UI", 10.5f);
            dgvKardex.RowTemplate.Height = 32;
            dgvKardex.AllowUserToAddRows = false;
            dgvKardex.RowHeadersVisible = false;
            dgvKardex.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvKardex.ShowCellToolTips = true;

            if (dgvKardex.Parent is TabPage tabKardex)
            {
                tabKardex.Padding = new Padding(15);
                tabKardex.Resize += (s, e) => AjustarKardex(tabKardex);
                AjustarKardex(tabKardex);
            }

            // --- Pestaña Ingreso de Mercancía ---
            if (cmbProducto.Parent is TabPage tabIngreso)
            {
                // Importante: localizar los campos ANTES de reordenarlos en la cuadrícula
                CampoCodigo = CampoDebajoDe(tabIngreso, "digo") as TextBox;
                CampoPrecio = CampoDebajoDe(tabIngreso, "Precio") as TextBox;
                CampoTotal = CampoDebajoDe(tabIngreso, "Total") as TextBox;

                AjustarIngresoComoDiseno(tabIngreso);
            }
        }

        // Busca el campo que está justo debajo de la etiqueta que contiene el texto indicado
        private Control CampoDebajoDe(TabPage page, string textoEtiqueta)
        {
            Label lbl = page.Controls.OfType<Label>().FirstOrDefault(l =>
                l.Text.IndexOf(textoEtiqueta, StringComparison.OrdinalIgnoreCase) >= 0);

            if (lbl == null) return null;

            int centro = lbl.Left + lbl.Width / 2;

            return page.Controls.Cast<Control>()
                .Where(c => !(c is Label) && c.Top >= lbl.Top && c.Left <= centro && c.Right >= centro)
                .OrderBy(c => c.Top)
                .FirstOrDefault();
        }

        // ===== Comportamiento de los campos del ingreso =====
        private void ConfigurarCamposIngreso()
        {
            // Total: solo lectura
            if (CampoTotal != null)
            {
                CampoTotal.ReadOnly = true;
                CampoTotal.TabStop = false;
                CampoTotal.BackColor = Color.WhiteSmoke;
            }

            // Código: automático, igual al ID del movimiento (solo lectura)
            if (CampoCodigo != null)
            {
                CampoCodigo.ReadOnly = true;
                CampoCodigo.TabStop = false;
                CampoCodigo.BackColor = Color.WhiteSmoke;
                CampoCodigo.PlaceholderText = "Automático (igual al ID)";
            }

            // Precio: solo números y un separador decimal
            if (CampoPrecio != null)
            {
                CampoPrecio.KeyPress += (s, e) =>
                {
                    if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar)) return;

                    bool esSeparador = e.KeyChar == '.' || e.KeyChar == ',';
                    if (esSeparador && !CampoPrecio.Text.Contains(".") && !CampoPrecio.Text.Contains(","))
                        return;

                    e.Handled = true;
                };

                CampoPrecio.TextChanged += (s, e) => RecalcularTotal();
            }

            numCantidad.Minimum = 1;
            numCantidad.ValueChanged += (s, e) => RecalcularTotal();

            RecalcularTotal();
        }

        // Lee un decimal sin depender de la configuración regional (acepta "12.5" o "12,5")
        private static bool TryLeerDecimal(string texto, out decimal valor)
        {
            return decimal.TryParse(texto.Replace(',', '.'),
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out valor);
        }

        // Total = cantidad x precio de costo
        private void RecalcularTotal()
        {
            if (CampoTotal == null) return;

            decimal precio = 0;
            if (CampoPrecio != null && !string.IsNullOrWhiteSpace(CampoPrecio.Text))
                TryLeerDecimal(CampoPrecio.Text.Trim(), out precio);

            decimal total = numCantidad.Value * precio;
            CampoTotal.Text = total.ToString("N2", CultureInfo.CurrentCulture);
        }

        // Estira el grid del Kardex hasta el borde de la pestaña
        private void AjustarKardex(TabPage page)
        {
            int margen = 20;
            int ancho = page.ClientSize.Width - dgvKardex.Left - margen;
            int alto = page.ClientSize.Height - dgvKardex.Top - margen;

            if (ancho > 100) dgvKardex.Width = ancho;
            if (alto > 100) dgvKardex.Height = alto;
        }

        // Mantiene la distribución del diseñador (mismos espacios verticales y mismas
        // proporciones horizontales) y solo estira el formulario al ancho disponible.
        private void AjustarIngresoComoDiseno(TabPage page)
        {
            var boton = page.Controls.OfType<Button>().FirstOrDefault();
            var controles = page.Controls.Cast<Control>().ToList();
            if (controles.Count == 0) return;

            // Textos corregidos de las etiquetas
            foreach (var lbl in controles.OfType<Label>())
            {
                lbl.Text = lbl.Text.Trim().TrimEnd(':')
                    .Replace("Provedor", "Proveedor")
                    .Replace("Codigo", "Código")
                    .Replace("Descripcion", "Descripción")
                    .Replace("mercaderia", "mercadería")
                    .Replace("ingreso mercadería", "ingreso de mercadería")
                    .Replace("costo(unidad)", "costo (unidad)");
            }

            // Posiciones originales del diseñador (se guardan antes de tocar nada)
            var original = controles.ToDictionary(c => c, c => c.Bounds);
            int minLeft = controles.Min(c => original[c].Left);
            int maxRight = controles.Max(c => original[c].Right);
            int margen = minLeft;          // mismo margen izquierdo y derecho
            const int margenInferior = 30;

            // Anclas simples: la posición la calculamos nosotros
            foreach (var c in controles)
                c.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            Action ajustar = () =>
            {
                int disponible = page.ClientSize.Width - 2 * margen;
                int total = maxRight - minLeft;
                if (disponible <= 100 || total <= 0) return;

                double k = (double)disponible / total;
                int abajoDeCampos = 0;

                page.SuspendLayout();
                foreach (var c in controles)
                {
                    Rectangle o = original[c];
                    int left = margen + (int)Math.Round((o.Left - minLeft) * k);
                    int right = margen + (int)Math.Round((o.Right - minLeft) * k);

                    if (c == boton)
                        continue; // se coloca al final
                    if (c is Label)
                        c.Left = left; // las etiquetas conservan su tamaño
                    else
                        c.SetBounds(left, o.Top, right - left, c.Height);

                    abajoDeCampos = Math.Max(abajoDeCampos, c.Bottom);
                }

                // Botón abajo a la derecha, alineado con el borde derecho del formulario
                if (boton != null)
                {
                    int top = page.ClientSize.Height - boton.Height - margenInferior;
                    boton.Top = Math.Max(top, abajoDeCampos + 15);
                    boton.Left = page.ClientSize.Width - margen - boton.Width;
                }
                page.ResumeLayout();
            };

            page.Resize += (s, e) => ajustar();
            page.VisibleChanged += (s, e) => { if (page.Visible) ajustar(); };
            this.Load += (s, e) => ajustar();
            ajustar();
        }

        private void CargarStock()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conexion))
                {
                    string query = @"
                SELECT 
                    p.NOMBRE AS Producto,
                    CASE 
                        WHEN i.STOCK_ACTUAL = 0 THEN 'Agotado'
                        WHEN i.STOCK_ACTUAL <= i.STOCK_MINIMO THEN 'Crítico'
                        ELSE 'Normal'
                    END AS Estado,
                    i.STOCK_ACTUAL AS Stock,
                    i.STOCK_MINIMO AS Mínimo
                FROM INV.INVENTARIO i
                INNER JOIN CAT.PRODUCTOS p ON i.PRODUCTO_ID = p.PRODUCTO_ID
                ORDER BY p.NOMBRE";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvStock.DataSource = null;
                    dgvStock.Columns.Clear();
                    dgvStock.AutoGenerateColumns = false;

                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        Name = "Producto",
                        HeaderText = "Producto",
                        DataPropertyName = "Producto",
                        FillWeight = 40,
                        DefaultCellStyle = new DataGridViewCellStyle
                        {
                            Alignment = DataGridViewContentAlignment.MiddleLeft
                        }
                    });
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        Name = "Estado",
                        HeaderText = "Estado",
                        DataPropertyName = "Estado",
                        FillWeight = 20,
                        DefaultCellStyle = new DataGridViewCellStyle
                        {
                            Alignment = DataGridViewContentAlignment.MiddleCenter
                        }
                    });
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        Name = "Stock",
                        HeaderText = "Stock",
                        DataPropertyName = "Stock",
                        FillWeight = 20,
                        DefaultCellStyle = new DataGridViewCellStyle
                        {
                            Alignment = DataGridViewContentAlignment.MiddleCenter
                        }
                    });
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        Name = "Mínimo",
                        HeaderText = "Mínimo",
                        DataPropertyName = "Mínimo",
                        FillWeight = 20,
                        DefaultCellStyle = new DataGridViewCellStyle
                        {
                            Alignment = DataGridViewContentAlignment.MiddleCenter
                        }
                    });

                    dgvStock.DataSource = dt;

                    dgvStock.Columns["Producto"].DisplayIndex = 0;
                    dgvStock.Columns["Estado"].DisplayIndex = 1;
                    dgvStock.Columns["Stock"].DisplayIndex = 2;
                    dgvStock.Columns["Mínimo"].DisplayIndex = 3;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el stock:\n" + ex.Message);
            }
        }

        private void CargarProductos()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conexion))
                {
                    string query = "SELECT PRODUCTO_ID, NOMBRE FROM CAT.PRODUCTOS ORDER BY NOMBRE";
                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cmbProducto.DataSource = dt.Copy();
                    cmbProducto.DisplayMember = "NOMBRE";
                    cmbProducto.ValueMember = "PRODUCTO_ID";

                    cmbProductoKardex.DataSource = dt.Copy();
                    cmbProductoKardex.DisplayMember = "NOMBRE";
                    cmbProductoKardex.ValueMember = "PRODUCTO_ID";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar productos:\n" + ex.Message);
            }
        }

        // ===== Registrar ingreso =====
        private void BtnRegistrarIngreso_Click(object sender, EventArgs e)
        {
            if (cmbProducto.SelectedValue == null)
            {
                MessageBox.Show("Selecciona un producto.");
                return;
            }

            if (numCantidad.Value <= 0)
            {
                MessageBox.Show("La cantidad debe ser mayor a 0.");
                return;
            }

            // Precio de costo (opcional, pero si se escribe debe ser válido)
            decimal? precio = null;
            string textoPrecio = CampoPrecio != null ? CampoPrecio.Text.Trim() : "";
            if (!string.IsNullOrEmpty(textoPrecio))
            {
                if (!TryLeerDecimal(textoPrecio, out decimal p) || p < 0)
                {
                    MessageBox.Show("El precio de costo no es válido.");
                    CampoPrecio.Focus();
                    return;
                }
                precio = p;
            }

            try
            {
                int productoId = Convert.ToInt32(cmbProducto.SelectedValue);
                int cantidad = (int)numCantidad.Value;
                int? idMovimiento = null;

                // La descripción del ingreso se guarda como motivo del movimiento
                string motivo = string.IsNullOrWhiteSpace(CampoDescripcion.Text)
                    ? "Ingreso de mercancía"
                    : CampoDescripcion.Text.Trim();

                if (motivo.Length > largoMotivo)
                    motivo = motivo.Substring(0, largoMotivo);

                // El INSERT incluye el precio solo si la tabla tiene esa columna.
                // El código no se escribe aquí: se iguala al ID justo después de insertar.
                var columnas = new List<string> { "PRODUCTO_ID", "USUARIO_ID", "TIPO_MOVIMIENTO", "CANTIDAD", "MOTIVO" };
                var valores = new List<string> { "@productoId", "1", "'ENTRADA'", "@cantidad", "@motivo" };

                if (hayColumnaPrecio)
                {
                    columnas.Add("PRECIO_COSTO");
                    valores.Add("@precio");
                }

                string insert = "INSERT INTO INV.MOVIMIENTOS_INVENTARIO (" + string.Join(", ", columnas) + ") " +
                                "VALUES (" + string.Join(", ", valores) + "); " +
                                "SELECT CAST(SCOPE_IDENTITY() AS INT);";

                using (SqlConnection con = new SqlConnection(conexion))
                {
                    con.Open();

                    // Transacción: o se guardan todas las operaciones o ninguna
                    using (SqlTransaction tx = con.BeginTransaction())
                    {
                        try
                        {
                            string update = @"
                                UPDATE INV.INVENTARIO 
                                SET STOCK_ACTUAL = STOCK_ACTUAL + @cantidad,
                                    ULTIMA_ACTUALIZACION = GETDATE()
                                WHERE PRODUCTO_ID = @productoId";

                            using (SqlCommand cmd = new SqlCommand(update, con, tx))
                            {
                                cmd.Parameters.AddWithValue("@cantidad", cantidad);
                                cmd.Parameters.AddWithValue("@productoId", productoId);

                                if (cmd.ExecuteNonQuery() == 0)
                                    throw new InvalidOperationException(
                                        "El producto no tiene registro en el inventario (INV.INVENTARIO).");
                            }

                            object nuevoId;
                            using (SqlCommand cmd = new SqlCommand(insert, con, tx))
                            {
                                cmd.Parameters.AddWithValue("@productoId", productoId);
                                cmd.Parameters.AddWithValue("@cantidad", cantidad);
                                cmd.Parameters.AddWithValue("@motivo", motivo);

                                if (hayColumnaPrecio)
                                    cmd.Parameters.AddWithValue("@precio",
                                        precio.HasValue ? (object)precio.Value : DBNull.Value);

                                nuevoId = cmd.ExecuteScalar();
                            }

                            if (nuevoId != null && nuevoId != DBNull.Value)
                            {
                                idMovimiento = Convert.ToInt32(nuevoId);

                                // Código = ID del movimiento
                                if (hayColumnaCodigo)
                                {
                                    const string igualar = @"
                                        UPDATE INV.MOVIMIENTOS_INVENTARIO
                                        SET CODIGO_MERCADERIA = CAST(@id AS NVARCHAR(50))
                                        WHERE MOVIMIENTO_ID = @id";

                                    using (SqlCommand cmd = new SqlCommand(igualar, con, tx))
                                    {
                                        cmd.Parameters.AddWithValue("@id", idMovimiento.Value);
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }

                            tx.Commit();
                        }
                        catch
                        {
                            tx.Rollback();
                            throw;
                        }
                    }
                }

                // Mensaje de éxito, con el código asignado
                string aviso = "Ingreso registrado correctamente.";
                if (idMovimiento.HasValue)
                    aviso += "\nCódigo / ID: " + idMovimiento.Value;

                // Aviso solo si la tabla no pudo guardar algún dato
                if (precio.HasValue && !hayColumnaPrecio)
                {
                    aviso += "\n\nNota: el precio de costo no se guardó porque la tabla " +
                             "INV.MOVIMIENTOS_INVENTARIO no tiene la columna PRECIO_COSTO " +
                             "(la aplicación no pudo crearla, probablemente por falta de permisos).";
                }
                if (!hayColumnaCodigo)
                {
                    aviso += "\n\nNota: el código no se asignó porque la tabla no tiene la columna CODIGO_MERCADERIA.";
                }

                MessageBox.Show(aviso);

                LimpiarFormularioIngreso();
                CargarStock();

                // Si el Kardex está mostrando este mismo producto, se actualiza
                if (dgvKardex.DataSource != null &&
                    cmbProductoKardex.SelectedValue != null &&
                    Convert.ToInt32(cmbProductoKardex.SelectedValue) == productoId)
                {
                    CargarKardex(false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar ingreso:\n" + ex.Message);
            }
        }

        private void LimpiarFormularioIngreso()
        {
            CampoDescripcion.Clear();
            if (CampoCodigo != null) CampoCodigo.Clear();
            if (CampoPrecio != null) CampoPrecio.Clear();
            numCantidad.Value = 1;
            RecalcularTotal();
        }

        // ===== Kardex =====
        private void btnBuscarKardex_Click(object sender, EventArgs e)
        {
            CargarKardex(true);
        }

        private void CargarKardex(bool avisarSiVacio)
        {
            if (cmbProductoKardex.SelectedValue == null)
            {
                MessageBox.Show("Selecciona un producto.");
                return;
            }

            try
            {
                int productoId = Convert.ToInt32(cmbProductoKardex.SelectedValue);

                // Muestra precio y código solo si existen en la tabla
                string extra = "";
                if (hayColumnaPrecio) extra += ", m.PRECIO_COSTO AS [Precio costo]";
                if (hayColumnaCodigo) extra += ", m.CODIGO_MERCADERIA AS [Código]";

                using (SqlConnection con = new SqlConnection(conexion))
                {
                    string query = @"
                        SELECT 
                            m.MOVIMIENTO_ID AS ID,
                            m.TIPO_MOVIMIENTO AS Tipo,
                            m.CANTIDAD AS Cantidad,
                            m.MOTIVO AS Motivo,
                            m.FECHA_REGISTRO AS Fecha" + extra + @"
                        FROM INV.MOVIMIENTOS_INVENTARIO m
                        WHERE m.PRODUCTO_ID = @productoId
                        ORDER BY m.FECHA_REGISTRO DESC";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    da.SelectCommand.Parameters.AddWithValue("@productoId", productoId);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvKardex.AutoGenerateColumns = true;
                    dgvKardex.DataSource = dt;

                    FormatearKardex();

                    if (avisarSiVacio && dt.Rows.Count == 0)
                        MessageBox.Show("No hay movimientos para este producto.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el Kardex:\n" + ex.Message);
            }
        }

        // Aplica anchos, formatos y tooltips al grid del Kardex
        private void FormatearKardex()
        {
            // Código == ID: se oculta para no repetir (sigue existiendo en la BD).
            // Si prefieres mostrar el Código y ocultar el ID, cambia el nombre de la columna aquí.
            if (dgvKardex.Columns.Contains("Código"))
                dgvKardex.Columns["Código"].Visible = false;

            // Anchos proporcionales: el Motivo recibe más espacio
            void Peso(string col, float w)
            {
                if (dgvKardex.Columns.Contains(col))
                    dgvKardex.Columns[col].FillWeight = w;
            }
            Peso("ID", 8);
            Peso("Tipo", 14);
            Peso("Cantidad", 12);
            Peso("Motivo", 40);
            Peso("Fecha", 20);
            Peso("Precio costo", 14);

            // Formatos
            if (dgvKardex.Columns.Contains("Fecha"))
                dgvKardex.Columns["Fecha"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";

            if (dgvKardex.Columns.Contains("Precio costo"))
            {
                var c = dgvKardex.Columns["Precio costo"];
                c.DefaultCellStyle.Format = "N2";
                c.DefaultCellStyle.NullValue = "—";
                c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (dgvKardex.Columns.Contains("Cantidad"))
                dgvKardex.Columns["Cantidad"].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleRight;

            // Para leer el motivo completo al pasar el mouse
            dgvKardex.ShowCellToolTips = true;
            if (dgvKardex.Columns.Contains("Motivo"))
            {
                foreach (DataGridViewRow r in dgvKardex.Rows)
                    r.Cells["Motivo"].ToolTipText = Convert.ToString(r.Cells["Motivo"].Value);
            }
        }

        private void btnRegistrarIngreso_Click_1(object sender, EventArgs e)
        {

        }
    }
}