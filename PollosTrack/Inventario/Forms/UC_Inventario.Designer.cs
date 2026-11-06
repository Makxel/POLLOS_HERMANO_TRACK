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
            label1 = new Label();
            textBox1 = new TextBox();
            lblProducto = new Label();
            cmbProducto = new ComboBox();
            label2 = new Label();
            numCantidad = new NumericUpDown();
            label3 = new Label();
            txtPrecioCosto = new TextBox();
            label5 = new Label();
            txtCodigo = new TextBox();
            label6 = new Label();
            textBox3 = new TextBox();
            btnRegistrarIngreso = new Button();
            tabPage3 = new TabPage();
            lblProductoKardex = new Label();
            cmbProductoKardex = new ComboBox();
            btnBuscarKardex = new Button();
            dgvKardex = new DataGridView();
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
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(855, 521);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(dgvStock);
            tabPage1.Location = new Point(4, 29);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(847, 488);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Monitor de Stock";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // dgvStock
            // 
            dgvStock.AllowUserToAddRows = false;
            dgvStock.AllowUserToDeleteRows = false;
            dgvStock.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvStock.Dock = DockStyle.Fill;
            dgvStock.Location = new Point(3, 3);
            dgvStock.Name = "dgvStock";
            dgvStock.ReadOnly = true;
            dgvStock.RowHeadersWidth = 62;
            dgvStock.Size = new Size(841, 482);
            dgvStock.TabIndex = 0;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(label1);
            tabPage2.Controls.Add(textBox1);
            tabPage2.Controls.Add(lblProducto);
            tabPage2.Controls.Add(cmbProducto);
            tabPage2.Controls.Add(label2);
            tabPage2.Controls.Add(numCantidad);
            tabPage2.Controls.Add(label3);
            tabPage2.Controls.Add(txtPrecioCosto);
            tabPage2.Controls.Add(label5);
            tabPage2.Controls.Add(txtCodigo);
            tabPage2.Controls.Add(label6);
            tabPage2.Controls.Add(textBox3);
            tabPage2.Controls.Add(btnRegistrarIngreso);
            tabPage2.Location = new Point(4, 29);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(847, 488);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Ingreso de Mercancía";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(571, 128);
            label1.Name = "label1";
            label1.Size = new Size(120, 20);
            label1.TabIndex = 15;
            label1.Text = "Total del ingreso";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(571, 156);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(204, 27);
            textBox1.TabIndex = 16;
            // 
            // lblProducto
            // 
            lblProducto.AutoSize = true;
            lblProducto.Location = new Point(40, 20);
            lblProducto.Name = "lblProducto";
            lblProducto.Size = new Size(69, 20);
            lblProducto.TabIndex = 0;
            lblProducto.Text = "Producto";
            // 
            // cmbProducto
            // 
            cmbProducto.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbProducto.FormattingEnabled = true;
            cmbProducto.Location = new Point(40, 48);
            cmbProducto.Name = "cmbProducto";
            cmbProducto.Size = new Size(494, 28);
            cmbProducto.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(40, 128);
            label2.Name = "label2";
            label2.Size = new Size(69, 20);
            label2.TabIndex = 2;
            label2.Text = "Cantidad";
            // 
            // numCantidad
            // 
            numCantidad.Location = new Point(40, 157);
            numCantidad.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numCantidad.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numCantidad.Name = "numCantidad";
            numCantidad.Size = new Size(243, 27);
            numCantidad.TabIndex = 3;
            numCantidad.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(335, 129);
            label3.Name = "label3";
            label3.Size = new Size(167, 20);
            label3.TabIndex = 4;
            label3.Text = "Precio de costo(unidad)";
            // 
            // txtPrecioCosto
            // 
            txtPrecioCosto.Location = new Point(335, 157);
            txtPrecioCosto.Name = "txtPrecioCosto";
            txtPrecioCosto.Size = new Size(199, 27);
            txtPrecioCosto.TabIndex = 5;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(571, 20);
            label5.Name = "label5";
            label5.Size = new Size(158, 20);
            label5.TabIndex = 6;
            label5.Text = "Código de mercadería";
            // 
            // txtCodigo
            // 
            txtCodigo.Location = new Point(571, 49);
            txtCodigo.Name = "txtCodigo";
            txtCodigo.Size = new Size(204, 27);
            txtCodigo.TabIndex = 7;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(40, 252);
            label6.Name = "label6";
            label6.Size = new Size(265, 20);
            label6.TabIndex = 12;
            label6.Text = "Descripción del ingreso de mercadería";
            // 
            // textBox3
            // 
            textBox3.Location = new Point(40, 297);
            textBox3.MinimumSize = new Size(730, 30);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(735, 30);
            textBox3.TabIndex = 13;
            // 
            // btnRegistrarIngreso
            // 
            btnRegistrarIngreso.Location = new Point(620, 420);
            btnRegistrarIngreso.Name = "btnRegistrarIngreso";
            btnRegistrarIngreso.Size = new Size(200, 42);
            btnRegistrarIngreso.TabIndex = 14;
            btnRegistrarIngreso.Text = "Registrar Ingreso";
            btnRegistrarIngreso.UseVisualStyleBackColor = true;
            btnRegistrarIngreso.Click += btnRegistrarIngreso_Click_1;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(lblProductoKardex);
            tabPage3.Controls.Add(cmbProductoKardex);
            tabPage3.Controls.Add(btnBuscarKardex);
            tabPage3.Controls.Add(dgvKardex);
            tabPage3.Location = new Point(4, 29);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(847, 488);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Kardex";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // lblProductoKardex
            // 
            lblProductoKardex.AutoSize = true;
            lblProductoKardex.Location = new Point(20, 22);
            lblProductoKardex.Name = "lblProductoKardex";
            lblProductoKardex.Size = new Size(72, 20);
            lblProductoKardex.TabIndex = 0;
            lblProductoKardex.Text = "Producto:";
            // 
            // cmbProductoKardex
            // 
            cmbProductoKardex.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbProductoKardex.FormattingEnabled = true;
            cmbProductoKardex.Location = new Point(115, 22);
            cmbProductoKardex.Name = "cmbProductoKardex";
            cmbProductoKardex.Size = new Size(250, 28);
            cmbProductoKardex.TabIndex = 1;
            // 
            // btnBuscarKardex
            // 
            btnBuscarKardex.Location = new Point(413, 22);
            btnBuscarKardex.Name = "btnBuscarKardex";
            btnBuscarKardex.Size = new Size(110, 34);
            btnBuscarKardex.TabIndex = 2;
            btnBuscarKardex.Text = "Buscar";
            btnBuscarKardex.UseVisualStyleBackColor = true;
            // 
            // dgvKardex
            // 
            dgvKardex.AllowUserToAddRows = false;
            dgvKardex.AllowUserToDeleteRows = false;
            dgvKardex.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvKardex.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvKardex.Location = new Point(15, 70);
            dgvKardex.Name = "dgvKardex";
            dgvKardex.ReadOnly = true;
            dgvKardex.RowHeadersWidth = 62;
            dgvKardex.Size = new Size(815, 400);
            dgvKardex.TabIndex = 3;
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

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DataGridView dgvStock;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label lblProducto;
        private System.Windows.Forms.ComboBox cmbProducto;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numCantidad;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPrecioCosto;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtCodigo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbBodega;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbProveedor;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Button btnRegistrarIngreso;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label lblProductoKardex;
        private System.Windows.Forms.ComboBox cmbProductoKardex;
        private System.Windows.Forms.Button btnBuscarKardex;
        private System.Windows.Forms.DataGridView dgvKardex;
        private TextBox textBox1;
    }
}