using Microsoft.Data.SqlClient;
using System.Data;
using PollosTrack.Utils;

namespace PollosTrack.POS.Forms
{
    public partial class VistaHistorial : UserControl
    {
        private const int registrosPorPagina = 20;
        private int paginaActual = 1;
        private int cantidadRegistros;
        private int cantidadPaginas = 1;

        // Inicializa controles. Las consultas se ejecutan únicamente al cargar en la aplicación.
        public VistaHistorial()
        {
            InitializeComponent();
        }

        private void AlCargarHistorial(object? origen, EventArgs argumentos)
        {
            if (DesignMode || System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            fechaDesde.Value = DateTime.Today.AddDays(-30);
            fechaHasta.Value = DateTime.Today;
            selectorEstado.SelectedIndex = 0;
            CargarHistorial();
        }

        // Buscar comienza siempre por la primera página del resultado filtrado.
        private void AlBuscar(object? origen, EventArgs argumentos)
        {
            paginaActual = 1;
            CargarHistorial();
        }

        private void AlPaginaAnterior(object? origen, EventArgs argumentos)
        {
            if (paginaActual > 1)
            {
                paginaActual--;
                CargarHistorial();
            }
        }

        private void AlPaginaSiguiente(object? origen, EventArgs argumentos)
        {
            if (paginaActual < cantidadPaginas)
            {
                paginaActual++;
                CargarHistorial();
            }
        }

        // Prioriza la fecha de factura; para cuentas sin factura usa la fecha del pedido.
        private void CargarHistorial()
        {
            if (fechaDesde.Checked && fechaHasta.Checked && fechaDesde.Value.Date > fechaHasta.Value.Date)
            {
                MessageBox.Show(this, "La fecha inicial no puede ser posterior a la final.");
                return;
            }
            botonBuscar.Enabled = false;
            try
            {
                string condiciones = """
                    FROM POS.ENCABEZADO_PEDIDO pedido
                    JOIN CAT.MESAS mesa ON mesa.MESA_ID=pedido.MESA_ID
                    JOIN CAT.CLIENTES cliente ON cliente.CLIENTE_ID=pedido.CLIENTE_ID
                    JOIN CAT.ESTADOS estado ON estado.ESTADO_ID=pedido.ESTADO_ID AND estado.ENTIDAD='ENCABEZADO_PEDIDO'
                    LEFT JOIN POS.FACTURAS factura ON factura.PEDIDO_ID=pedido.PEDIDO_ID
                    WHERE (@Desde IS NULL OR COALESCE(factura.FECHA_REGISTRO,pedido.FECHA_REGISTRO)>=@Desde)
                      AND (@Hasta IS NULL OR COALESCE(factura.FECHA_REGISTRO,pedido.FECHA_REGISTRO)<@Hasta)
                      AND (@Texto='' OR cliente.NOMBRE LIKE @Busqueda OR cliente.IDENTIFICACION LIKE @Busqueda
                           OR pedido.NOMBRE_REFERENCIA LIKE @Busqueda
                           OR pedido.PEDIDO_ID=TRY_CONVERT(INT,@Texto) OR factura.FACTURA_ID=TRY_CONVERT(INT,@Texto))
                      AND (@Estado='' OR REPLACE(estado.CODIGO,' ','_')=@Estado)
                    """;
                string consulta = "SELECT COUNT(*) AS CANTIDAD,COALESCE(SUM(factura.TOTAL),0) AS TOTAL " + condiciones + ";";
                consulta += """
                    SELECT pedido.PEDIDO_ID AS Pedido,factura.FACTURA_ID AS Recibo,
                        COALESCE(factura.FECHA_REGISTRO,pedido.FECHA_REGISTRO) AS Fecha,
                        mesa.NUMERO AS Mesa,cliente.NOMBRE AS Cliente,pedido.NOMBRE_REFERENCIA AS Referencia,
                        estado.NOMBRE AS Estado,factura.METODO_PAGO AS Pago,factura.TOTAL AS Total
                    """;
                consulta += "\n" + condiciones;
                consulta += " ORDER BY COALESCE(factura.FECHA_REGISTRO,pedido.FECHA_REGISTRO) DESC,pedido.PEDIDO_ID DESC OFFSET @Omitir ROWS FETCH NEXT @Cantidad ROWS ONLY;";
                using SqlConnection conexion = new conexion_sql_server().AbrirConexion();
                using SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.Add("@Desde", SqlDbType.DateTime).Value = DBNull.Value;
                comando.Parameters.Add("@Hasta", SqlDbType.DateTime).Value = DBNull.Value;
                if (fechaDesde.Checked) comando.Parameters["@Desde"].Value = fechaDesde.Value.Date;
                if (fechaHasta.Checked) comando.Parameters["@Hasta"].Value = fechaHasta.Value.Date.AddDays(1);
                string texto = textoBusqueda.Text.Trim();
                // Escapa comodines para que el filtro busque lo escrito literalmente.
                string busqueda = texto.Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]");
                comando.Parameters.Add("@Texto", SqlDbType.NVarChar, 150).Value = texto;
                comando.Parameters.Add("@Busqueda", SqlDbType.NVarChar, 450).Value = "%" + busqueda + "%";
                string codigoEstado = string.Empty;
                if (selectorEstado.SelectedIndex == 1) codigoEstado = "ABIERTO";
                if (selectorEstado.SelectedIndex == 2) codigoEstado = "POR_COBRAR";
                if (selectorEstado.SelectedIndex == 3) codigoEstado = "CERRADO";
                if (selectorEstado.SelectedIndex == 4) codigoEstado = "CANCELADO";
                comando.Parameters.Add("@Estado", SqlDbType.VarChar, 20).Value = codigoEstado;
                comando.Parameters.Add("@Omitir", SqlDbType.Int).Value = (paginaActual - 1) * registrosPorPagina;
                comando.Parameters.Add("@Cantidad", SqlDbType.Int).Value = registrosPorPagina;
                using SqlDataAdapter adaptador = new SqlDataAdapter(comando);
                DataSet resultado = new DataSet();
                adaptador.Fill(resultado);
                cantidadRegistros = Convert.ToInt32(resultado.Tables[0].Rows[0]["CANTIDAD"]);
                cantidadPaginas = Math.Max(1, (cantidadRegistros + registrosPorPagina - 1) / registrosPorPagina);
                if (paginaActual > cantidadPaginas)
                {
                    paginaActual = cantidadPaginas;
                    CargarHistorial();
                    return;
                }
                tablaPedidos.DataSource = resultado.Tables[1];
                tablaPedidos.ClearSelection();
                tablaPedidos.CurrentCell = null;
                foreach (DataGridViewColumn columna in tablaPedidos.Columns)
                {
                    if (columna.Name == "Fecha") columna.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                    if (columna.Name == "Total") columna.DefaultCellStyle.Format = "N2";
                }
                decimal totalFacturado = Convert.ToDecimal(resultado.Tables[0].Rows[0]["TOTAL"]);
                etiquetaPie.Text = $"Página {paginaActual} de {cantidadPaginas} · {cantidadRegistros} pedidos · Total facturado del filtro: C$ {totalFacturado:N2}";
                botonAnterior.Enabled = paginaActual > 1;
                botonSiguiente.Enabled = paginaActual < cantidadPaginas;
                AlSeleccionarPedido(this, EventArgs.Empty);
            }
            catch (Exception excepcion)
            {
                tablaPedidos.DataSource = null;
                botonRecibo.Enabled = false;
                botonAnterior.Enabled = false;
                botonSiguiente.Enabled = false;
                etiquetaPie.Text = "No se pudo consultar el historial. Pulsa Buscar para reintentar.";
                MessageBox.Show(this, excepcion.Message, "Historial de pedidos");
            }
            finally { botonBuscar.Enabled = true; }
        }

        // Solo las cuentas facturadas tienen un recibo que pueda regenerarse.
        private void AlSeleccionarPedido(object? origen, EventArgs argumentos)
        {
            botonRecibo.Enabled = false;
            if (tablaPedidos.CurrentRow != null && tablaPedidos.CurrentRow.DataBoundItem is DataRowView pedido)
                botonRecibo.Enabled = !pedido.Row.IsNull("Recibo");
        }

        // Reutiliza exactamente la misma plantilla que Facturación, sin repetir el cobro.
        private void AlVerRecibo(object? origen, EventArgs argumentos)
        {
            if (tablaPedidos.CurrentRow == null || tablaPedidos.CurrentRow.DataBoundItem is not DataRowView pedido) return;
            if (pedido.Row.IsNull("Recibo")) return;
            try { ReciboFactura.Mostrar(this, Convert.ToInt32(pedido["Recibo"])); }
            catch (Exception excepcion) { MessageBox.Show(this, excepcion.Message, "No se pudo abrir el recibo"); }
        }
    }
}
