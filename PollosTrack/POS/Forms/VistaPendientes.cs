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
            if (Convert.ToString(detalle["CODIGO"]) == "PENDIENTE")
            {
                Button botonCancelar = new Button();
                botonCancelar.Text = "Cancelar producto";
                botonCancelar.Width = LogicalToDeviceUnits(265);
                botonCancelar.Height = LogicalToDeviceUnits(34);
                botonCancelar.FlatStyle = FlatStyle.Flat;
                botonCancelar.BackColor = Color.FromArgb(247, 214, 214);
                botonCancelar.Tag = detalle;
                botonCancelar.Click += AlCancelarDetalle;
                tarjeta.Controls.Add(botonCancelar);
            }
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

        // Solicita la transición apropiada a partir de la tarjeta que se está mostrando.
        private void AlCambiarEstadoDetalle(object? origen, EventArgs argumentos)
        {
            if (origen is not Button boton || boton.Tag is not DataRow detalle) return;
            string anterior = Convert.ToString(detalle["CODIGO"])!;
            string siguiente = "EN_PREPARACION";
            if (anterior == "EN_PREPARACION") siguiente = "ENTREGADO";
            ProcesarCambioDetalle(boton, detalle, siguiente);
        }

        // Cancelar antes de iniciar libera la reserva sin modificar las existencias físicas.
        private void AlCancelarDetalle(object? origen, EventArgs argumentos)
        {
            if (origen is not Button boton || boton.Tag is not DataRow detalle) return;
            if (MessageBox.Show(this, "¿Cancelar este producto pendiente?", "Cancelar producto",
                MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            ProcesarCambioDetalle(boton, detalle, "CANCELADO");
        }

        // Confirma estado y salida dentro de la misma transacción y luego recarga la lista.
        private void ProcesarCambioDetalle(Button boton, DataRow detalle, string siguiente)
        {
            boton.Enabled = false;
            try
            {
                using SqlConnection conexion = new conexion_sql_server().AbrirConexion();
                using SqlTransaction transaccion = conexion.BeginTransaction();
                int detalleId = Convert.ToInt32(detalle["DETALLE_ID"]);
                string anterior = Convert.ToString(detalle["CODIGO"])!;
                CambiarEstadoDetalle(conexion, transaccion, detalleId, anterior, siguiente);
                transaccion.Commit();
            }
            catch (Exception excepcion) { MessageBox.Show(this, excepcion.Message, "No se pudo cambiar el estado"); }
            finally { boton.Enabled = true; }
            CargarPedidos();
        }

        // Lee los datos actuales bajo bloqueo: no confía en una tarjeta que puede estar desactualizada.
        private void CambiarEstadoDetalle(SqlConnection conexion, SqlTransaction transaccion,
            int detalleId, string anterior, string siguiente)
        {
            bool transicionValida = anterior == "PENDIENTE" && (siguiente == "EN_PREPARACION" || siguiente == "CANCELADO");
            if (anterior == "EN_PREPARACION" && siguiente == "ENTREGADO") transicionValida = true;
            if (!transicionValida) throw new InvalidOperationException("El cambio de estado no está permitido.");

            // Bloquea primero el encabezado, siguiendo el mismo orden que al agregar productos.
            using SqlCommand bloquearPedido = new SqlCommand("""
                SELECT estado.CODIGO FROM POS.ENCABEZADO_PEDIDO pedido WITH (UPDLOCK,HOLDLOCK)
                JOIN CAT.ESTADOS estado ON estado.ESTADO_ID=pedido.ESTADO_ID
                WHERE pedido.PEDIDO_ID=(SELECT PEDIDO_ID FROM POS.DETALLE_PEDIDO WHERE DETALLE_ID=@Detalle)
                    AND estado.ENTIDAD='ENCABEZADO_PEDIDO';
                """, conexion, transaccion);
            bloquearPedido.Parameters.Add("@Detalle", SqlDbType.Int).Value = detalleId;
            string estadoPedido = Convert.ToString(bloquearPedido.ExecuteScalar())!;
            if (estadoPedido != "ABIERTO" && estadoPedido != "POR_COBRAR" && estadoPedido != "POR COBRAR")
                throw new InvalidOperationException("La cuenta ya no permite modificar productos.");

            int productoId;
            int cantidad;
            using SqlCommand leerDetalle = new SqlCommand("""
                SELECT detalle.PRODUCTO_ID,detalle.CANTIDAD,estado.CODIGO
                FROM POS.DETALLE_PEDIDO detalle WITH (UPDLOCK,HOLDLOCK)
                JOIN CAT.ESTADOS estado ON estado.ESTADO_ID=detalle.ESTADO_ID
                WHERE detalle.DETALLE_ID=@Detalle AND estado.ENTIDAD='DETALLE_PEDIDO';
                """, conexion, transaccion);
            leerDetalle.Parameters.Add("@Detalle", SqlDbType.Int).Value = detalleId;
            using (SqlDataReader lector = leerDetalle.ExecuteReader())
            {
                if (!lector.Read() || lector.GetString(2) != anterior)
                    throw new InvalidOperationException("El producto cambió de estado. Actualiza el monitor.");
                productoId = lector.GetInt32(0);
                cantidad = lector.GetInt32(1);
            }
            using SqlCommand buscarEstado = new SqlCommand(
                "SELECT ESTADO_ID FROM CAT.ESTADOS WHERE ENTIDAD='DETALLE_PEDIDO' AND CODIGO=@Codigo", conexion, transaccion);
            buscarEstado.Parameters.Add("@Codigo", SqlDbType.VarChar, 20).Value = siguiente;
            object? estadoId = buscarEstado.ExecuteScalar();
            if (estadoId == null) throw new InvalidOperationException("Falta configurar el estado " + siguiente);

            // Solo entregar produce una salida. Iniciar y cancelar no descuentan stock.
            if (siguiente == "ENTREGADO")
                RegistrarSalidaEntrega(conexion, transaccion, detalleId, productoId, cantidad);

            using SqlCommand actualizar = new SqlCommand("""
                UPDATE POS.DETALLE_PEDIDO SET ESTADO_ID=@Estado,
                    MINUTOS_PREPARACION=CASE
                        WHEN @Finalizar=0 THEN MINUTOS_PREPARACION
                        WHEN FECHA_REGISTRO IS NULL THEN NULL
                        WHEN FECHA_REGISTRO>GETDATE() THEN 0
                        ELSE DATEDIFF(SECOND,FECHA_REGISTRO,GETDATE())/60 END
                WHERE DETALLE_ID=@Detalle;
                """, conexion, transaccion);
            actualizar.Parameters.Add("@Finalizar", SqlDbType.Bit).Value = siguiente == "ENTREGADO";
            actualizar.Parameters.Add("@Estado", SqlDbType.Int).Value = Convert.ToInt32(estadoId);
            actualizar.Parameters.Add("@Detalle", SqlDbType.Int).Value = detalleId;
            actualizar.ExecuteNonQuery();
        }

        // Descuenta unidades y registra una salida referenciada al detalle, no a la factura.
        private void RegistrarSalidaEntrega(SqlConnection conexion, SqlTransaction transaccion,
            int detalleId, int productoId, int cantidad)
        {
            using SqlCommand inventario = new SqlCommand(
                "SELECT CONTROLA_STOCK FROM INV.INVENTARIO WITH (UPDLOCK,HOLDLOCK) WHERE PRODUCTO_ID=@Producto", conexion, transaccion);
            inventario.Parameters.Add("@Producto", SqlDbType.Int).Value = productoId;
            object? controlaStock = inventario.ExecuteScalar();
            if (controlaStock == null) throw new InvalidOperationException("Falta configurar el producto en Inventario.");
            if (!Convert.ToBoolean(controlaStock)) return;

            // La comprobación protege ante ajustes de inventario posteriores a la reserva.
            using SqlCommand descontar = new SqlCommand("""
                UPDATE INV.INVENTARIO SET STOCK_ACTUAL=STOCK_ACTUAL-@Cantidad,ULTIMA_ACTUALIZACION=GETDATE()
                WHERE PRODUCTO_ID=@Producto AND STOCK_ACTUAL>=@Cantidad;
                """, conexion, transaccion);
            descontar.Parameters.Add("@Producto", SqlDbType.Int).Value = productoId;
            descontar.Parameters.Add("@Cantidad", SqlDbType.Int).Value = cantidad;
            if (descontar.ExecuteNonQuery() != 1)
                throw new InvalidOperationException("Las existencias cambiaron y no alcanzan para entregar. Revisa Inventario.");

            // Hasta incorporar sesiones, las operaciones se atribuyen al usuario Sistema existente.
            using SqlCommand usuario = new SqlCommand(
                "SELECT USUARIO_ID FROM SEG.USUARIOS WHERE USERNAME='Sistema' AND ACTIVO=1", conexion, transaccion);
            object? usuarioId = usuario.ExecuteScalar();
            if (usuarioId == null) throw new InvalidOperationException("No existe el usuario Sistema activo.");
            using SqlCommand movimiento = new SqlCommand("""
                INSERT INTO INV.MOVIMIENTOS_INVENTARIO
                    (PRODUCTO_ID,USUARIO_ID,TIPO_MOVIMIENTO,CANTIDAD,MOTIVO,REFERENCIA_ID)
                VALUES (@Producto,@Usuario,'SALIDA',@Cantidad,'ENTREGA_PEDIDO',@Detalle);
                """, conexion, transaccion);
            movimiento.Parameters.Add("@Producto", SqlDbType.Int).Value = productoId;
            movimiento.Parameters.Add("@Usuario", SqlDbType.Int).Value = Convert.ToInt32(usuarioId);
            movimiento.Parameters.Add("@Cantidad", SqlDbType.Int).Value = cantidad;
            movimiento.Parameters.Add("@Detalle", SqlDbType.Int).Value = detalleId;
            movimiento.ExecuteNonQuery();
        }
    }
}
