using Microsoft.Data.SqlClient;
using System.Data;
using PollosTrack.Utils;

namespace PollosTrack.POS.Forms
{
    public partial class VistaPendientes : UserControl
    {
        // Inicializa solamente los controles; el diseñador no consulta SQL Server.
        public VistaPendientes()
        {
            InitializeComponent();
        }

        // Consulta al entrar al monitor, sin temporizadores.
        private void AlCargarMonitor(object? origen, EventArgs argumentos)
        {
            if (DesignMode || System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            CargarPedidos();
        }

        // Actualiza manualmente los estados y tiempos visibles.
        private void AlActualizarMonitor(object? origen, EventArgs argumentos)
        {
            CargarPedidos();
        }

        // SQL entrega una cola FIFO: primero el pedido con el detalle activo más antiguo.
        private void CargarPedidos()
        {
            botonActualizar.Enabled = false;
            try
            {
                using SqlConnection conexion = new conexion_sql_server().AbrirConexion();
                using SqlCommand comando = new SqlCommand("""
                    SELECT detalle.DETALLE_ID,detalle.PEDIDO_ID,mesa.NUMERO,producto.NOMBRE AS PRODUCTO,
                        detalle.CANTIDAD,detalle.NOTAS,detalle.FECHA_REGISTRO,estado.CODIGO,estado.NOMBRE AS ESTADO,
                        estado.COLOR_HEX,pedido.NOMBRE_REFERENCIA,cliente.NOMBRE AS CLIENTE,
                        GETDATE() AS AHORA,
                        MIN(detalle.FECHA_REGISTRO) OVER (PARTITION BY detalle.PEDIDO_ID) AS PRIORIDAD
                    FROM POS.DETALLE_PEDIDO detalle
                    JOIN POS.ENCABEZADO_PEDIDO pedido ON pedido.PEDIDO_ID=detalle.PEDIDO_ID
                    JOIN CAT.MESAS mesa ON mesa.MESA_ID=pedido.MESA_ID
                    JOIN CAT.PRODUCTOS producto ON producto.PRODUCTO_ID=detalle.PRODUCTO_ID
                    JOIN CAT.CLIENTES cliente ON cliente.CLIENTE_ID=pedido.CLIENTE_ID
                    JOIN CAT.ESTADOS estado ON estado.ESTADO_ID=detalle.ESTADO_ID
                    JOIN CAT.ESTADOS estadoPedido ON estadoPedido.ESTADO_ID=pedido.ESTADO_ID
                    WHERE estado.ENTIDAD='DETALLE_PEDIDO' AND estado.CODIGO IN ('PENDIENTE','EN_PREPARACION')
                        AND estadoPedido.ENTIDAD='ENCABEZADO_PEDIDO'
                        AND estadoPedido.CODIGO IN ('ABIERTO','POR_COBRAR','POR COBRAR')
                    ORDER BY PRIORIDAD,detalle.PEDIDO_ID,detalle.FECHA_REGISTRO,detalle.DETALLE_ID;
                    """, conexion);
                using SqlDataReader lector = comando.ExecuteReader();
                DataTable detalles = new DataTable();
                detalles.Load(lector);
                MostrarPedidos(detalles);
            }
            catch (Exception excepcion)
            {
                // Conserva la última vista, pero bloquea acciones hasta conseguir una lectura válida.
                panelPedidos.Enabled = false;
                etiquetaResumen.Text = "No se pudo actualizar. Los datos pueden estar desactualizados. Pulsa Actualizar.";
                MessageBox.Show(this, excepcion.Message, "Monitor de pendientes");
            }
            finally { botonActualizar.Enabled = true; }
        }

        // Agrupa visualmente las líneas sin perder el orden recibido de SQL Server.
        private void MostrarPedidos(DataTable detalles)
        {
            panelPedidos.SuspendLayout();
            try
            {
                while (panelPedidos.Controls.Count > 0)
                {
                    Control tarjetaAnterior = panelPedidos.Controls[0];
                    panelPedidos.Controls.Remove(tarjetaAnterior);
                    tarjetaAnterior.Dispose();
                }
                int pedidoAnterior = -1;
                int cantidadPedidos = 0;
                FlowLayoutPanel? tarjeta = null;
                foreach (DataRow detalle in detalles.Rows)
                {
                    int pedidoId = Convert.ToInt32(detalle["PEDIDO_ID"]);
                    if (pedidoId != pedidoAnterior)
                    {
                        tarjeta = CrearTarjetaPedido(detalle);
                        panelPedidos.Controls.Add(tarjeta);
                        pedidoAnterior = pedidoId;
                        cantidadPedidos++;
                    }
                    if (tarjeta != null) AgregarProductoTarjeta(tarjeta, detalle);
                }
                if (detalles.Rows.Count == 0)
                {
                    panelPedidos.Controls.Add(new Label { AutoSize = true, Padding = new Padding(24),
                        Text = "Todo al día. No hay productos pendientes ni en preparación." });
                }
                panelPedidos.Enabled = true;
                etiquetaResumen.Text = $"{cantidadPedidos} cuentas · {detalles.Rows.Count} líneas por atender · Actualizado {DateTime.Now:HH:mm:ss}";
            }
            finally { panelPedidos.ResumeLayout(true); }
        }

        // Encabeza cada cuenta con la mesa y el cliente, incluyendo cuentas separadas.
        private FlowLayoutPanel CrearTarjetaPedido(DataRow detalle)
        {
            FlowLayoutPanel tarjeta = new FlowLayoutPanel();
            tarjeta.Width = LogicalToDeviceUnits(300);
            tarjeta.AutoSize = true;
            tarjeta.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tarjeta.MinimumSize = new Size(LogicalToDeviceUnits(300), 0);
            tarjeta.MaximumSize = new Size(LogicalToDeviceUnits(300), 0);
            tarjeta.FlowDirection = FlowDirection.TopDown;
            tarjeta.WrapContents = false;
            tarjeta.BackColor = Color.White;
            tarjeta.Padding = new Padding(12);
            tarjeta.Margin = new Padding(8);
            string cliente = Convert.ToString(detalle["CLIENTE"])!;
            if (!string.IsNullOrWhiteSpace(Convert.ToString(detalle["NOMBRE_REFERENCIA"])))
                cliente = Convert.ToString(detalle["NOMBRE_REFERENCIA"])!;
            Label titulo = CrearEtiqueta($"MESA {detalle["NUMERO"]} · Cuenta {detalle["PEDIDO_ID"]}\r\n{cliente}");
            titulo.BackColor = Color.FromArgb(255, 245, 212);
            titulo.Padding = new Padding(8);
            tarjeta.Controls.Add(titulo);
            return tarjeta;
        }

        // Presenta cada detalle con sus notas y la única transición permitida.
        private void AgregarProductoTarjeta(FlowLayoutPanel tarjeta, DataRow detalle)
        {
            tarjeta.Controls.Add(CrearEtiqueta($"{detalle["CANTIDAD"]} × {detalle["PRODUCTO"]}"));
            string notas = Convert.ToString(detalle["NOTAS"])!;
            if (!string.IsNullOrWhiteSpace(notas)) tarjeta.Controls.Add(CrearEtiqueta("Nota: " + notas));
            string tiempo = "Registro sin fecha";
            if (!detalle.IsNull("FECHA_REGISTRO"))
            {
                DateTime registro = Convert.ToDateTime(detalle["FECHA_REGISTRO"]);
                DateTime ahora = Convert.ToDateTime(detalle["AHORA"]);
                int minutos = Math.Max(0, (int)(ahora - registro).TotalMinutes);
                tiempo = $"Ingreso {registro:dd/MM HH:mm} · {minutos} min transcurridos";
            }
            tarjeta.Controls.Add(CrearEtiqueta(tiempo));
            Button botonAccion = new Button();
            botonAccion.Width = LogicalToDeviceUnits(265);
            botonAccion.Height = LogicalToDeviceUnits(48);
            botonAccion.Margin = new Padding(3, 4, 3, 16);
            botonAccion.FlatStyle = FlatStyle.Flat;
            botonAccion.FlatAppearance.BorderSize = 0;
            botonAccion.ForeColor = Color.Black;
            botonAccion.BackColor = Color.Gainsboro;
            string hexadecimal = Convert.ToString(detalle["COLOR_HEX"])!;
            if (System.Text.RegularExpressions.Regex.IsMatch(hexadecimal, "^#[0-9A-Fa-f]{6}$"))
                botonAccion.BackColor = ColorTranslator.FromHtml(hexadecimal);
            botonAccion.Text = "Pendiente · Iniciar";
            if (Convert.ToString(detalle["CODIGO"]) == "EN_PREPARACION")
                botonAccion.Text = "En preparación · Finalizar";
            botonAccion.Tag = detalle;
            botonAccion.Click += AlCambiarEstadoDetalle;
            tarjeta.Controls.Add(botonAccion);
        }

        // Mantiene el texto dentro del ancho de la tarjeta y permite notas largas.
        private Label CrearEtiqueta(string texto)
        {
            Label etiqueta = new Label();
            etiqueta.AutoSize = true;
            etiqueta.MaximumSize = new Size(LogicalToDeviceUnits(265), 0);
            etiqueta.Text = texto;
            etiqueta.Margin = new Padding(3, 6, 3, 6);
            return etiqueta;
        }

        // Actualiza solo si el detalle conserva su estado anterior: evita dobles finalizaciones.
        private void AlCambiarEstadoDetalle(object? origen, EventArgs argumentos)
        {
            if (origen is not Button boton || boton.Tag is not DataRow detalle) return;
            boton.Enabled = false;
            try
            {
                string anterior = Convert.ToString(detalle["CODIGO"])!;
                string siguiente = "EN_PREPARACION";
                if (anterior == "EN_PREPARACION") siguiente = "ENTREGADO";
                using SqlConnection conexion = new conexion_sql_server().AbrirConexion();
                using SqlCommand buscarEstado = new SqlCommand(
                    "SELECT ESTADO_ID FROM CAT.ESTADOS WHERE ENTIDAD='DETALLE_PEDIDO' AND CODIGO=@Codigo", conexion);
                buscarEstado.Parameters.Add("@Codigo", SqlDbType.VarChar, 20).Value = siguiente;
                object? resultado = buscarEstado.ExecuteScalar();
                if (resultado == null) throw new InvalidOperationException("No está configurado el estado " + siguiente);
                string consulta = """
                    UPDATE detalle SET ESTADO_ID=@Estado,
                        MINUTOS_PREPARACION=CASE
                            WHEN @Finalizar=0 THEN detalle.MINUTOS_PREPARACION
                            WHEN detalle.FECHA_REGISTRO IS NULL THEN NULL
                            WHEN detalle.FECHA_REGISTRO>GETDATE() THEN 0
                            ELSE DATEDIFF(SECOND,detalle.FECHA_REGISTRO,GETDATE())/60 END
                    FROM POS.DETALLE_PEDIDO detalle
                    JOIN CAT.ESTADOS estado ON estado.ESTADO_ID=detalle.ESTADO_ID
                    JOIN POS.ENCABEZADO_PEDIDO pedido ON pedido.PEDIDO_ID=detalle.PEDIDO_ID
                    JOIN CAT.ESTADOS estadoPedido ON estadoPedido.ESTADO_ID=pedido.ESTADO_ID
                    WHERE detalle.DETALLE_ID=@Detalle AND estado.ENTIDAD='DETALLE_PEDIDO' AND estado.CODIGO=@Anterior
                        AND estadoPedido.ENTIDAD='ENCABEZADO_PEDIDO'
                        AND estadoPedido.CODIGO IN ('ABIERTO','POR_COBRAR','POR COBRAR');
                    """;
                using SqlCommand actualizar = new SqlCommand(consulta, conexion);
                // SQL usa su reloj para guardar minutos completos desde el registro.
                actualizar.Parameters.Add("@Finalizar", SqlDbType.Bit).Value = siguiente == "ENTREGADO";
                actualizar.Parameters.Add("@Estado", SqlDbType.Int).Value = Convert.ToInt32(resultado);
                actualizar.Parameters.Add("@Detalle", SqlDbType.Int).Value = Convert.ToInt32(detalle["DETALLE_ID"]);
                actualizar.Parameters.Add("@Anterior", SqlDbType.VarChar, 20).Value = anterior;
                if (actualizar.ExecuteNonQuery() == 0)
                    MessageBox.Show(this, "El detalle o la cuenta cambió de estado. Se actualizará la lista.");
            }
            catch (Exception excepcion) { MessageBox.Show(this, excepcion.Message, "No se pudo cambiar el estado"); }
            finally { boton.Enabled = true; }
            CargarPedidos();
        }
    }
}
