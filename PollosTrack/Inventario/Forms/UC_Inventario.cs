using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace PollosTrack.Inventario.Forms
{
    public partial class UC_Inventario : UserControl
    {
        private string conexion = "Server=localhost;Database=POLLOS_HERMANOS_TRACK;Trusted_Connection=True;TrustServerCertificate=True;";

        public UC_Inventario()
        {
            InitializeComponent();
            CargarStock();
            CargarProductos();
            btnRegistrarIngreso.Click += BtnRegistrarIngreso_Click;
            btnBuscarKardex.Click += btnBuscarKardex_Click;
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
                            i.STOCK_ACTUAL AS Stock,
                            i.STOCK_MINIMO AS Mínimo,
                            CASE 
                                WHEN i.STOCK_ACTUAL = 0 THEN 'Agotado'
                                WHEN i.STOCK_ACTUAL <= i.STOCK_MINIMO THEN 'Crítico'
                                ELSE 'Normal'
                            END AS Estado
                        FROM INV.INVENTARIO i
                        INNER JOIN CAT.PRODUCTOS p ON i.PRODUCTO_ID = p.PRODUCTO_ID
                        ORDER BY p.NOMBRE";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvStock.AutoGenerateColumns = true;
                    dgvStock.DataSource = dt;

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

            try
            {
                int productoId = Convert.ToInt32(cmbProducto.SelectedValue);
                int cantidad = (int)numCantidad.Value;
                string motivo = string.IsNullOrWhiteSpace(txtMotivo.Text) ? "Ingreso de mercancía" : txtMotivo.Text;

                using (SqlConnection con = new SqlConnection(conexion))
                {
                    con.Open();

                    string update = @"
                        UPDATE INV.INVENTARIO 
                        SET STOCK_ACTUAL = STOCK_ACTUAL + @cantidad,
                            ULTIMA_ACTUALIZACION = GETDATE()
                        WHERE PRODUCTO_ID = @productoId";

                    using (SqlCommand cmd = new SqlCommand(update, con))
                    {
                        cmd.Parameters.AddWithValue("@cantidad", cantidad);
                        cmd.Parameters.AddWithValue("@productoId", productoId);
                        cmd.ExecuteNonQuery();
                    }

                    string insert = @"
                        INSERT INTO INV.MOVIMIENTOS_INVENTARIO 
                            (PRODUCTO_ID, USUARIO_ID, TIPO_MOVIMIENTO, CANTIDAD, MOTIVO)
                        VALUES 
                            (@productoId, 1, 'ENTRADA', @cantidad, @motivo)";

                    using (SqlCommand cmd = new SqlCommand(insert, con))
                    {
                        cmd.Parameters.AddWithValue("@productoId", productoId);
                        cmd.Parameters.AddWithValue("@cantidad", cantidad);
                        cmd.Parameters.AddWithValue("@motivo", motivo);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Ingreso registrado correctamente.");
                txtMotivo.Clear();
                numCantidad.Value = 1;
                CargarStock();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar ingreso:\n" + ex.Message);
            }
        }

        private void btnBuscarKardex_Click(object sender, EventArgs e)
        {
            if (cmbProductoKardex.SelectedValue == null)
            {
                MessageBox.Show("Selecciona un producto.");
                return;
            }

            try
            {
                int productoId = Convert.ToInt32(cmbProductoKardex.SelectedValue);

                using (SqlConnection con = new SqlConnection(conexion))
                {
                    string query = @"
                        SELECT 
                            m.MOVIMIENTO_ID AS ID,
                            m.TIPO_MOVIMIENTO AS Tipo,
                            m.CANTIDAD AS Cantidad,
                            m.MOTIVO AS Motivo,
                            m.FECHA_REGISTRO AS Fecha
                        FROM INV.MOVIMIENTOS_INVENTARIO m
                        WHERE m.PRODUCTO_ID = @productoId
                        ORDER BY m.FECHA_REGISTRO DESC";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    da.SelectCommand.Parameters.AddWithValue("@productoId", productoId);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvKardex.AutoGenerateColumns = true;
                    dgvKardex.DataSource = dt;

                    if (dt.Rows.Count == 0)
                        MessageBox.Show("No hay movimientos para este producto.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el Kardex:\n" + ex.Message);
            }
        }

        private void tabPage1_Click(object sender, EventArgs e) { }
        private void lblProducto_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void numCantidad_ValueChanged(object sender, EventArgs e) { }
        private void cmbProductoKardex_SelectedIndexChanged(object sender, EventArgs e) { }

        private void dgvStock_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvKardex_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void lblProductoKardex_Click(object sender, EventArgs e)
        {

        }
    }
}