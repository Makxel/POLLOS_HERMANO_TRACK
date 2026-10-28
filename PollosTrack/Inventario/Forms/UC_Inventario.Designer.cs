namespace PollosTrack.Inventario.Forms
{
    partial class UC_Inventario
    {
 
        private System.ComponentModel.IContainer components = null;
    
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes
        private void InitializeComponent()
        {
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            dgvStock = new DataGridView();
            tabPage2 = new TabPage();
            btnRegistrarIngreso = new Button();
            txtMotivo = new TextBox();
            lblMotivo = new Label();
            numCantidad = new NumericUpDown();
            label2 = new Label();
            cmbProducto = new ComboBox();
            lblProducto = new Label();
            tabPage3 = new TabPage();
            dgvKardex = new DataGridView();
            btnBuscarKardex = new Button();
            cmbProductoKardex = new ComboBox();
            lblProductoKardex = new Label();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvStock).BeginInit();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numCantidad).BeginInit();
            tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvKardex).BeginInit();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.ItemSize = new Size(50, 30);
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.RightToLeft = RightToLeft.Yes;
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(856, 521);
            tabControl1.SizeMode = TabSizeMode.FillToRight;
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(dgvStock);
            tabPage1.Location = new Point(4, 34);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(848, 483);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Monitor de Stock";
            tabPage1.UseVisualStyleBackColor = true;
            tabPage1.Click += tabPage1_Click;
            // 
            // dgvStock
            // 
            dgvStock.AllowUserToAddRows = false;
            dgvStock.AllowUserToDeleteRows = false;
            dgvStock.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvStock.BackgroundColor = Color.White;
            dgvStock.BorderStyle = BorderStyle.None;
            dgvStock.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvStock.Dock = DockStyle.Fill;
            dgvStock.Location = new Point(3, 3);
            dgvStock.Name = "dgvStock";
            dgvStock.ReadOnly = true;
            dgvStock.RowHeadersWidth = 51;
            dgvStock.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvStock.Size = new Size(842, 477);
            dgvStock.TabIndex = 0;
            dgvStock.CellContentClick += dgvStock_CellContentClick;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(btnRegistrarIngreso);
            tabPage2.Controls.Add(txtMotivo);
            tabPage2.Controls.Add(lblMotivo);
            tabPage2.Controls.Add(numCantidad);
            tabPage2.Controls.Add(label2);
            tabPage2.Controls.Add(cmbProducto);
            tabPage2.Controls.Add(lblProducto);
            tabPage2.Location = new Point(4, 34);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(848, 483);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Ingreso de Mercancía";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnRegistrarIngreso
            // 
            btnRegistrarIngreso.BackColor = Color.LightBlue;
            btnRegistrarIngreso.ForeColor = SystemColors.ControlLightLight;
            btnRegistrarIngreso.Location = new Point(169, 189);
            btnRegistrarIngreso.Name = "btnRegistrarIngreso";
            btnRegistrarIngreso.Size = new Size(190, 29);
            btnRegistrarIngreso.TabIndex = 6;
            btnRegistrarIngreso.Text = "Registrar Ingreso";
            btnRegistrarIngreso.UseVisualStyleBackColor = false;
            // 
            // txtMotivo
            // 
            txtMotivo.Location = new Point(211, 133);
            txtMotivo.Name = "txtMotivo";
            txtMotivo.Size = new Size(125, 27);
            txtMotivo.TabIndex = 5;
            // 
            // lblMotivo
            // 
            lblMotivo.AutoSize = true;
            lblMotivo.Location = new Point(146, 133);
            lblMotivo.Name = "lblMotivo";
            lblMotivo.Size = new Size(59, 20);
            lblMotivo.TabIndex = 4;
            lblMotivo.Text = "Motivo:";
            // 
            // numCantidad
            // 
            numCantidad.Location = new Point(211, 74);
            numCantidad.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numCantidad.Name = "numCantidad";
            numCantidad.Size = new Size(150, 27);
            numCantidad.TabIndex = 3;
            numCantidad.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numCantidad.ValueChanged += numCantidad_ValueChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(133, 76);
            label2.Name = "label2";
            label2.Size = new Size(72, 20);
            label2.TabIndex = 2;
            label2.Text = "Cantidad:";
            label2.Click += label2_Click;
            // 
            // cmbProducto
            // 
            cmbProducto.FormattingEnabled = true;
            cmbProducto.Location = new Point(208, 21);
            cmbProducto.Name = "cmbProducto";
            cmbProducto.Size = new Size(151, 28);
            cmbProducto.TabIndex = 1;
            // 
            // lblProducto
            // 
            lblProducto.AutoSize = true;
            lblProducto.Location = new Point(130, 24);
            lblProducto.Name = "lblProducto";
            lblProducto.Size = new Size(72, 20);
            lblProducto.TabIndex = 0;
            lblProducto.Text = "Producto:";
            lblProducto.Click += lblProducto_Click;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(dgvKardex);
            tabPage3.Controls.Add(btnBuscarKardex);
            tabPage3.Controls.Add(cmbProductoKardex);
            tabPage3.Controls.Add(lblProductoKardex);
            tabPage3.Location = new Point(4, 34);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(848, 483);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Kardex";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // dgvKardex
            // 
            dgvKardex.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvKardex.BackgroundColor = Color.White;
            dgvKardex.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvKardex.Location = new Point(20, 160);
            dgvKardex.Name = "dgvKardex";
            dgvKardex.RowHeadersWidth = 51;
            dgvKardex.Size = new Size(801, 295);
            dgvKardex.TabIndex = 3;
            // 
            // btnBuscarKardex
            // 
            btnBuscarKardex.Location = new Point(264, 92);
            btnBuscarKardex.Name = "btnBuscarKardex";
            btnBuscarKardex.Size = new Size(149, 43);
            btnBuscarKardex.TabIndex = 2;
            btnBuscarKardex.Text = "Buscar";
            btnBuscarKardex.UseVisualStyleBackColor = true;
            btnBuscarKardex.Click += btnBuscarKardex_Click;
            // 
            // cmbProductoKardex
            // 
            cmbProductoKardex.FormattingEnabled = true;
            cmbProductoKardex.Location = new Point(346, 34);
            cmbProductoKardex.Name = "cmbProductoKardex";
            cmbProductoKardex.Size = new Size(151, 28);
            cmbProductoKardex.TabIndex = 1;
            cmbProductoKardex.Text = "Lista desplegable";
            cmbProductoKardex.SelectedIndexChanged += cmbProductoKardex_SelectedIndexChanged;
            // 
            // lblProductoKardex
            // 
            lblProductoKardex.AutoSize = true;
            lblProductoKardex.Location = new Point(229, 37);
            lblProductoKardex.Name = "lblProductoKardex";
            lblProductoKardex.Size = new Size(72, 20);
            lblProductoKardex.TabIndex = 0;
            lblProductoKardex.Text = "Producto:";
            lblProductoKardex.Click += lblProductoKardex_Click;
            // 
            // UC_Inventario
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tabControl1);
            Name = "UC_Inventario";
            Size = new Size(855, 521);
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvStock).EndInit();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numCantidad).EndInit();
            tabPage3.ResumeLayout(false);
            tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvKardex).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private TabPage tabPage1;
        private TabPage tabPage3;
        private DataGridView dgvStock;
        private TabPage tabPage2;
        private Button btnRegistrarIngreso;
        private TextBox txtMotivo;
        private Label lblMotivo;
        private NumericUpDown numCantidad;
        private Label label2;
        private ComboBox cmbProducto;
        private Label lblProducto;
        private DataGridView dgvKardex;
        private Button btnBuscarKardex;
        private ComboBox cmbProductoKardex;
        private Label lblProductoKardex;
        internal TabControl tabControl1;
    }
}
