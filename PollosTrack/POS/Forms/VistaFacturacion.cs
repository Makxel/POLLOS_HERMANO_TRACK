using Microsoft.Data.SqlClient;
using System.Data;
using PollosTrack.POS.Models;
using PollosTrack.Utils;

namespace PollosTrack.POS.Forms
{
    public partial class VistaFacturacion : UserControl
    {
        public event EventHandler? VolverAMesasSolicitado;
        public Mesa? MesaActual { get; private set; }
        // Recibe desde FormPrincipal el identificador del usuario autenticado.
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int? UsuarioSesionId { get; set; }
        private int ultimaFacturaId;
        private bool cargandoCuentas;
        private int pedidoActualId;
        private decimal subtotal;
        private decimal montoIva;
        private decimal total;
        private bool cuentaCobrable;

        // El constructor solo inicializa controles, para conservar el diseñador de WinForms.
        public VistaFacturacion()
        {
            InitializeComponent();
        }

        // Filtra las cuentas cuando se llega desde el mapa de mesas.
        public VistaFacturacion(Mesa mesa) : this()
        {
            MesaActual = mesa;
        }

        // Consulta al abrir la vista, excepto dentro del diseñador.
        private void AlCargarFacturacion(object? origen, EventArgs argumentos)
        {
            if (DesignMode || System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            CargarCuentas();
        }

        // Actualización manual: no hay consultas periódicas.
        private void AlActualizarCuentas(object? origen, EventArgs argumentos)
        {
            CargarCuentas();
        }

        // Lista cuentas sin factura, distinguiendo mesa, cliente y referencia.
        private void CargarCuentas()
        {
            cargandoCuentas = true;
            LimpiarCuenta();
            try
            {
                using SqlConnection conexion = new conexion_sql_server().AbrirConexion();
                string consulta = """
                    SELECT pedido.PEDIDO_ID,
                        CONCAT('Mesa ',mesa.NUMERO,' · Cuenta ',pedido.PEDIDO_ID,' · ',cliente.NOMBRE,' · ',pedido.NOMBRE_REFERENCIA) AS DESCRIPCION
                    FROM POS.ENCABEZADO_PEDIDO pedido
                    JOIN CAT.MESAS mesa ON mesa.MESA_ID=pedido.MESA_ID
                    JOIN CAT.CLIENTES cliente ON cliente.CLIENTE_ID=pedido.CLIENTE_ID
                    JOIN CAT.ESTADOS estado ON estado.ESTADO_ID=pedido.ESTADO_ID
                    WHERE estado.ENTIDAD='ENCABEZADO_PEDIDO' AND estado.CODIGO IN ('ABIERTO','POR_COBRAR','POR COBRAR')
                        AND NOT EXISTS (SELECT 1 FROM POS.FACTURAS factura WHERE factura.PEDIDO_ID=pedido.PEDIDO_ID)
                    """;
                if (MesaActual != null) consulta += " AND pedido.MESA_ID=@Mesa";
                consulta += " ORDER BY pedido.FECHA_REGISTRO,pedido.PEDIDO_ID;";
                using SqlCommand comando = new SqlCommand(consulta, conexion);
                if (MesaActual != null) comando.Parameters.Add("@Mesa", SqlDbType.Int).Value = MesaActual.MesaId;
                using SqlDataReader lector = comando.ExecuteReader();
                DataTable cuentas = new DataTable();
                cuentas.Load(lector);
                selectorCuenta.DisplayMember = "DESCRIPCION";
                selectorCuenta.ValueMember = "PEDIDO_ID";
                selectorCuenta.DataSource = cuentas;
                selectorCuenta.SelectedIndex = -1;
                etiquetaAviso.Text = "Selecciona una cuenta para revisar los productos y registrar el pago.";
                if (cuentas.Rows.Count == 0) etiquetaAviso.Text = "No hay cuentas pendientes de facturar.";
            }
            catch (Exception excepcion) { MessageBox.Show(this, excepcion.Message, "No se pudieron cargar las cuentas"); }
            finally { cargandoCuentas = false; }
        }

        // Limpia datos de la cuenta anterior para evitar cobrar una selección desactualizada.
        private void LimpiarCuenta()
        {
            pedidoActualId = 0;
            cuentaCobrable = false;
            subtotal = 0;
            montoIva = 0;
            total = 0;
            tablaProductos.DataSource = null;
            montoRecibido.Value = 0;
            MostrarTotales();
        }

        // Lee el detalle y muestra importes antes de confirmar el cobro.
        private void AlSeleccionarCuenta(object? origen, EventArgs argumentos)
        {
            if (cargandoCuentas) return;
            LimpiarCuenta();
            if (selectorCuenta.SelectedItem is not DataRowView cuenta) return;
            try
            {
                int pedidoId = Convert.ToInt32(cuenta["PEDIDO_ID"]);
                using SqlConnection conexion = new conexion_sql_server().AbrirConexion();
                using SqlTransaction transaccion = conexion.BeginTransaction();
                DataTable lineas = ConsultarLineas(conexion, transaccion, pedidoId);
                CalcularTotales(lineas, out subtotal, out montoIva, out total);
                transaccion.Commit();
                pedidoActualId = pedidoId;
                tablaProductos.DataSource = lineas;
                tablaProductos.Columns["PRODUCTO_ID"]!.Visible = false;
                tablaProductos.Columns["CODIGO"]!.Visible = false;
                cuentaCobrable = EstanProductosEntregados(lineas);
                etiquetaAviso.Text = "Revisa el pago y confirma la factura. Los productos cancelados no se cobran.";
                if (!cuentaCobrable) etiquetaAviso.Text = "Para cobrar debe existir al menos un producto y todos deben estar entregados.";
                MostrarTotales();
            }
            catch (Exception excepcion)
            {
                LimpiarCuenta();
                MessageBox.Show(this, excepcion.Message, "No se pudo consultar la cuenta");
            }
        }

        // Lee solamente líneas no canceladas, con el precio histórico del pedido.
        private DataTable ConsultarLineas(SqlConnection conexion, SqlTransaction transaccion, int pedidoId)
        {
            using SqlCommand comando = new SqlCommand("""
                SELECT detalle.PRODUCTO_ID,producto.NOMBRE AS Producto,detalle.CANTIDAD AS Cantidad,
                    detalle.PRECIO_UNITARIO AS Precio,detalle.CANTIDAD*detalle.PRECIO_UNITARIO AS Importe,
                    producto.APLICA_IVA AS AplicaIVA,estado.NOMBRE AS Estado,estado.CODIGO
                FROM POS.DETALLE_PEDIDO detalle
                JOIN CAT.PRODUCTOS producto ON producto.PRODUCTO_ID=detalle.PRODUCTO_ID
                JOIN CAT.ESTADOS estado ON estado.ESTADO_ID=detalle.ESTADO_ID AND estado.ENTIDAD='DETALLE_PEDIDO'
                WHERE detalle.PEDIDO_ID=@Pedido AND estado.CODIGO<>'CANCELADO' ORDER BY detalle.DETALLE_ID;
                """, conexion, transaccion);
            comando.Parameters.Add("@Pedido", SqlDbType.Int).Value = pedidoId;
            using SqlDataReader lector = comando.ExecuteReader();
            DataTable lineas = new DataTable();
            lineas.Load(lector);
            return lineas;
        }

        // Suma importes e IVA por línea, con redondeo comercial a dos decimales.
        private void CalcularTotales(DataTable lineas, out decimal subtotalCalculado, out decimal ivaCalculado, out decimal totalCalculado)
        {
            subtotalCalculado = 0;
            ivaCalculado = 0;
            foreach (DataRow linea in lineas.Rows)
            {
                decimal importe = Convert.ToDecimal(linea["Importe"]);
                subtotalCalculado += importe;
                if (Convert.ToBoolean(linea["AplicaIVA"]))
                    ivaCalculado += decimal.Round(importe * 0.15m, 2, MidpointRounding.AwayFromZero);
            }
            totalCalculado = subtotalCalculado + ivaCalculado;
        }

        // Impide cerrar una cuenta vacía o con productos que cocina aún no ha terminado.
        private bool EstanProductosEntregados(DataTable lineas)
        {
            if (lineas.Rows.Count == 0) return false;
            foreach (DataRow linea in lineas.Rows)
            {
                if (Convert.ToString(linea["CODIGO"]) != "ENTREGADO") return false;
            }
            return true;
        }

        // Actualiza el cambio y los controles según el método de pago.
        private void AlCambiarPago(object? origen, EventArgs argumentos)
        {
            MostrarTotales();
        }

        // Lee el texto mientras se escribe: Value puede conservar el importe anterior hasta validar el control.
        private bool ObtenerMontoRecibido(out decimal recibido)
        {
            bool esNumero = decimal.TryParse(montoRecibido.Text,
                System.Globalization.NumberStyles.Number,
                System.Globalization.CultureInfo.CurrentCulture, out recibido);

            if (!esNumero || recibido < montoRecibido.Minimum || recibido > montoRecibido.Maximum)
            {
                return false;
            }

            // No modifica el texto ni mueve el cursor durante la escritura.
            recibido = decimal.Round(recibido, 2, MidpointRounding.AwayFromZero);
            return true;
        }

        // El efectivo exige un monto suficiente; tarjeta y transferencia usan el total exacto.
        private void MostrarTotales()
        {
            etiquetaTotales.Text = $"Subtotal: C$ {subtotal:N2}     IVA: C$ {montoIva:N2}\r\nTOTAL: C$ {total:N2}";
            bool efectivo = Convert.ToString(selectorPago.SelectedItem) == "Efectivo";
            montoRecibido.Enabled = efectivo;
            bool recibidoValido = ObtenerMontoRecibido(out decimal recibido);
            decimal cambio = 0;
            if (efectivo && recibidoValido && recibido >= total) cambio = recibido - total;
            etiquetaCambio.Text = $"Cambio: C$ {cambio:N2}";
            botonFacturar.Enabled = cuentaCobrable && pedidoActualId > 0 && selectorPago.SelectedIndex >= 0;
            if (efectivo && (!recibidoValido || recibido < total)) botonFacturar.Enabled = false;
        }

        // Confirma la operación y separa el éxito del cobro de posibles fallos posteriores de recarga.
        private void AlFacturar(object? origen, EventArgs argumentos)
        {
            if (!cuentaCobrable || pedidoActualId == 0) return;
            string metodo = Convert.ToString(selectorPago.SelectedItem)!;
            decimal recibido = total;
            if (metodo == "Efectivo" && !ObtenerMontoRecibido(out recibido))
            {
                MessageBox.Show(this, "Escribe un monto recibido válido.");
                return;
            }
            if (recibido < total) { MessageBox.Show(this, "El efectivo recibido es insuficiente."); return; }
            if (MessageBox.Show(this, $"¿Confirmar el cobro de C$ {total:N2} de la cuenta {pedidoActualId}?", "Facturar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            botonFacturar.Enabled = false;
            int facturaId;
            try
            {
                using SqlConnection conexion = new conexion_sql_server().AbrirConexion();
                using SqlTransaction transaccion = conexion.BeginTransaction(IsolationLevel.Serializable);
                facturaId = RegistrarFactura(conexion, transaccion, pedidoActualId, metodo, recibido);
                transaccion.Commit();
            }
            catch (Exception excepcion)
            {
                // Dispose revierte la transacción completa si no llegó a confirmarse.
                MessageBox.Show(this, excepcion.Message, "No se realizó el cobro");
                botonFacturar.Enabled = false;
                etiquetaAviso.Text = "Actualiza y vuelve a seleccionar la cuenta antes de reintentar.";
                return;
            }
            MessageBox.Show(this, $"Factura {facturaId} registrada correctamente. Cambio: C$ {recibido - total:N2}", "Cobro completado");
            ultimaFacturaId = facturaId;
            botonVerRecibo.Enabled = true;
            MostrarRecibo(facturaId);
            CargarCuentas();
        }

        // Registra factura y cierre como una única operación; no vuelve a descontar inventario.
        private int RegistrarFactura(SqlConnection conexion, SqlTransaction transaccion, int pedidoId, string metodo, decimal recibido)
        {
            // Bloquea primero la mesa, igual que al crear una comanda, para serializar cuentas separadas.
            using SqlCommand buscarMesa = new SqlCommand("SELECT MESA_ID FROM POS.ENCABEZADO_PEDIDO WHERE PEDIDO_ID=@Pedido", conexion, transaccion);
            buscarMesa.Parameters.Add("@Pedido", SqlDbType.Int).Value = pedidoId;
            object? resultadoMesa = buscarMesa.ExecuteScalar();
            if (resultadoMesa == null) throw new InvalidOperationException("El pedido ya no existe.");
            int mesaId = Convert.ToInt32(resultadoMesa);
            using SqlCommand bloquearMesa = new SqlCommand("SELECT MESA_ID FROM CAT.MESAS WITH (UPDLOCK,HOLDLOCK) WHERE MESA_ID=@Mesa", conexion, transaccion);
            bloquearMesa.Parameters.Add("@Mesa", SqlDbType.Int).Value = mesaId;
            if (bloquearMesa.ExecuteScalar() == null) throw new InvalidOperationException("La mesa ya no existe.");
            using SqlCommand comprobar = new SqlCommand("""
                SELECT estado.CODIGO FROM POS.ENCABEZADO_PEDIDO pedido WITH (UPDLOCK,HOLDLOCK)
                JOIN CAT.ESTADOS estado ON estado.ESTADO_ID=pedido.ESTADO_ID
                WHERE pedido.PEDIDO_ID=@Pedido AND estado.ENTIDAD='ENCABEZADO_PEDIDO'
                    AND NOT EXISTS (SELECT 1 FROM POS.FACTURAS WHERE PEDIDO_ID=@Pedido);
                """, conexion, transaccion);
            comprobar.Parameters.Add("@Pedido", SqlDbType.Int).Value = pedidoId;
            string estadoPedido = Convert.ToString(comprobar.ExecuteScalar())!;
            if (estadoPedido != "ABIERTO" && estadoPedido != "POR_COBRAR" && estadoPedido != "POR COBRAR")
                throw new InvalidOperationException("El pedido ya fue facturado o no permite cobro.");
            DataTable lineas = ConsultarLineas(conexion, transaccion, pedidoId);
            if (!EstanProductosEntregados(lineas)) throw new InvalidOperationException("Existen productos sin entregar o la cuenta está vacía.");
            CalcularTotales(lineas, out decimal nuevoSubtotal, out decimal nuevoIva, out decimal nuevoTotal);
            if (nuevoSubtotal != subtotal || nuevoIva != montoIva || nuevoTotal != total)
                throw new InvalidOperationException("Los importes cambiaron. Actualiza la cuenta y revisa el pago.");
            if (recibido < nuevoTotal) throw new InvalidOperationException("El monto recibido es insuficiente.");
            int usuarioId = ObtenerUsuario(conexion, transaccion);
            int estadoCerradoId = ObtenerEstado(conexion, transaccion, "ENCABEZADO_PEDIDO", "CERRADO");

            using SqlCommand insertar = new SqlCommand("""
                INSERT INTO POS.FACTURAS (PEDIDO_ID,USUARIO_ID,SUBTOTAL,MONTO_IVA,TOTAL,METODO_PAGO,MONTO_RECIBIDO,CAMBIO_DEVUELTO)
                OUTPUT INSERTED.FACTURA_ID VALUES (@Pedido,@Usuario,@Subtotal,@IVA,@Total,@Metodo,@Recibido,@Cambio);
                """, conexion, transaccion);
            insertar.Parameters.Add("@Pedido", SqlDbType.Int).Value = pedidoId;
            insertar.Parameters.Add("@Usuario", SqlDbType.Int).Value = usuarioId;
            AgregarImporte(insertar, "@Subtotal", nuevoSubtotal);
            AgregarImporte(insertar, "@IVA", nuevoIva);
            AgregarImporte(insertar, "@Total", nuevoTotal);
            AgregarImporte(insertar, "@Recibido", recibido);
            AgregarImporte(insertar, "@Cambio", recibido - nuevoTotal);
            insertar.Parameters.Add("@Metodo", SqlDbType.VarChar, 20).Value = metodo;
            int facturaId = Convert.ToInt32(insertar.ExecuteScalar());
            // Las unidades ya se descontaron al entregar cada detalle desde el monitor.
            using SqlCommand cerrar = new SqlCommand("UPDATE POS.ENCABEZADO_PEDIDO SET ESTADO_ID=@Estado,FECHA_FINALIZADO=GETDATE() WHERE PEDIDO_ID=@Pedido", conexion, transaccion);
            cerrar.Parameters.Add("@Estado", SqlDbType.Int).Value = estadoCerradoId;
            cerrar.Parameters.Add("@Pedido", SqlDbType.Int).Value = pedidoId;
            cerrar.ExecuteNonQuery();
            ActualizarMesa(conexion, transaccion, mesaId);
            return facturaId;
        }

        // Libera la mesa solo cuando no quedan cuentas abiertas; en otro caso conserva su estado operativo.
        private void ActualizarMesa(SqlConnection conexion, SqlTransaction transaccion, int mesaId)
        {
            using SqlCommand comando = new SqlCommand("""
                SELECT estado.CODIGO FROM POS.ENCABEZADO_PEDIDO pedido
                JOIN CAT.ESTADOS estado ON estado.ESTADO_ID=pedido.ESTADO_ID
                WHERE pedido.MESA_ID=@Mesa AND estado.ENTIDAD='ENCABEZADO_PEDIDO'
                    AND estado.CODIGO IN ('ABIERTO','POR_COBRAR','POR COBRAR');
                """, conexion, transaccion);
            comando.Parameters.Add("@Mesa", SqlDbType.Int).Value = mesaId;
            bool quedanCuentas = false;
            bool quedanAbiertas = false;
            using (SqlDataReader lector = comando.ExecuteReader())
            {
                while (lector.Read())
                {
                    quedanCuentas = true;
                    if (lector.GetString(0) == "ABIERTO") quedanAbiertas = true;
                }
            }
            string codigo = "DISPONIBLE";
            if (quedanCuentas) codigo = "POR_COBRAR";
            if (quedanAbiertas) codigo = "OCUPADA";
            int estadoId = ObtenerEstado(conexion, transaccion, "MESA", codigo);
            using SqlCommand actualizar = new SqlCommand("UPDATE CAT.MESAS SET ESTADO_ID=@Estado WHERE MESA_ID=@Mesa", conexion, transaccion);
            actualizar.Parameters.Add("@Estado", SqlDbType.Int).Value = estadoId;
            actualizar.Parameters.Add("@Mesa", SqlDbType.Int).Value = mesaId;
            actualizar.ExecuteNonQuery();
        }

        // Resuelve los estados por código; no depende de IDs fijos del catálogo.
        private int ObtenerEstado(SqlConnection conexion, SqlTransaction transaccion, string entidad, string codigo)
        {
            using SqlCommand comando = new SqlCommand("SELECT ESTADO_ID FROM CAT.ESTADOS WHERE ENTIDAD=@Entidad AND REPLACE(CODIGO,' ','_')=@Codigo", conexion, transaccion);
            comando.Parameters.Add("@Entidad", SqlDbType.VarChar, 25).Value = entidad;
            comando.Parameters.Add("@Codigo", SqlDbType.VarChar, 20).Value = codigo;
            object? resultado = comando.ExecuteScalar();
            if (resultado == null) throw new InvalidOperationException("Falta configurar el estado " + codigo);
            return Convert.ToInt32(resultado);
        }

        // Mantiene Sistema como usuario temporal hasta incorporar sesiones.
        private int ObtenerUsuario(SqlConnection conexion, SqlTransaction transaccion)
        {
            string consulta = "SELECT USUARIO_ID FROM SEG.USUARIOS WHERE USERNAME='Sistema' AND ACTIVO=1";
            if (UsuarioSesionId.HasValue) consulta = "SELECT USUARIO_ID FROM SEG.USUARIOS WHERE USUARIO_ID=@Usuario AND ACTIVO=1";
            using SqlCommand comando = new SqlCommand(consulta, conexion, transaccion);
            if (UsuarioSesionId.HasValue) comando.Parameters.Add("@Usuario", SqlDbType.Int).Value = UsuarioSesionId.Value;
            object? resultado = comando.ExecuteScalar();
            if (resultado == null) throw new InvalidOperationException("No existe el usuario responsable activo (Sistema o usuario de sesión).");
            return Convert.ToInt32(resultado);
        }

        // Declara importes como DECIMAL(10,2), igual que las columnas de facturas.
        private void AgregarImporte(SqlCommand comando, string nombre, decimal valor)
        {
            SqlParameter parametro = comando.Parameters.Add(nombre, SqlDbType.Decimal);
            parametro.Precision = 10;
            parametro.Scale = 2;
            parametro.Value = valor;
        }

        // Permite volver a abrir el último recibo sin registrar otro pago.
        private void AlVerRecibo(object? origen, EventArgs argumentos)
        {
            if (ultimaFacturaId > 0) MostrarRecibo(ultimaFacturaId);
        }

        // La factura ya está confirmada: un error de PDF no revierte ni repite el cobro.
        private void MostrarRecibo(int facturaId)
        {
            try
            {
                ReciboFactura.Mostrar(this, facturaId);
            }
            catch (Exception excepcion)
            {
                MessageBox.Show(this, $"La factura {facturaId} está guardada, pero no se pudo abrir el recibo.\r\nUsa Ver último recibo para reintentar.\r\n\r\n{excepcion.Message}", "Previsualización del recibo");
            }
        }

        // Regresa al mapa para consultar la ocupación resultante.
        private void AlVolverAMesas(object? origen, EventArgs argumentos)
        {
            if (VolverAMesasSolicitado != null) VolverAMesasSolicitado(this, EventArgs.Empty);
        }
    }
}
