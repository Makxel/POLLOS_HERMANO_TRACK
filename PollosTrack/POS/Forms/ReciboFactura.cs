using Microsoft.Data.SqlClient;
using System.Data;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using PollosTrack.Utils;
using Size = System.Drawing.Size;
using Color = System.Drawing.Color;

namespace PollosTrack.POS.Forms
{
    // Generación y previsualización compartidas entre Facturación e Historial.
    internal static class ReciboFactura
    {
        // Solo consulta y muestra: nunca registra pagos ni mueve inventario.
        public static void Mostrar(IWin32Window propietario, int facturaId)
        {
            byte[] archivo = GenerarRecibo(facturaId);
            using VentanaRecibo ventana = new VentanaRecibo(archivo, facturaId);
            ventana.ShowDialog(propietario);
        }
        // Consulta los datos persistidos; no usa los importes temporales de la pantalla.
        public static byte[] GenerarRecibo(int facturaId)
        {
            using SqlConnection conexion = new conexion_sql_server().AbrirConexion();
            using SqlCommand comando = new SqlCommand("""
                SELECT factura.FACTURA_ID,factura.PEDIDO_ID,factura.FECHA_REGISTRO,
                    factura.SUBTOTAL,factura.MONTO_IVA,factura.TOTAL,factura.METODO_PAGO,
                    factura.MONTO_RECIBIDO,factura.CAMBIO_DEVUELTO,
                    mesa.NUMERO AS MESA,cliente.NOMBRE AS CLIENTE,cliente.IDENTIFICACION,
                    pedido.NOMBRE_REFERENCIA,usuario.NOMBRE_COMPLETO AS USUARIO
                FROM POS.FACTURAS factura
                JOIN POS.ENCABEZADO_PEDIDO pedido ON pedido.PEDIDO_ID=factura.PEDIDO_ID
                JOIN CAT.MESAS mesa ON mesa.MESA_ID=pedido.MESA_ID
                JOIN CAT.CLIENTES cliente ON cliente.CLIENTE_ID=pedido.CLIENTE_ID
                JOIN SEG.USUARIOS usuario ON usuario.USUARIO_ID=factura.USUARIO_ID
                WHERE factura.FACTURA_ID=@Factura;

                SELECT producto.NOMBRE AS PRODUCTO,detalle.CANTIDAD,detalle.PRECIO_UNITARIO,
                    detalle.CANTIDAD*detalle.PRECIO_UNITARIO AS IMPORTE
                FROM POS.DETALLE_PEDIDO detalle
                JOIN POS.FACTURAS factura ON factura.PEDIDO_ID=detalle.PEDIDO_ID
                JOIN CAT.PRODUCTOS producto ON producto.PRODUCTO_ID=detalle.PRODUCTO_ID
                JOIN CAT.ESTADOS estado ON estado.ESTADO_ID=detalle.ESTADO_ID
                WHERE factura.FACTURA_ID=@Factura AND estado.ENTIDAD='DETALLE_PEDIDO'
                    AND estado.CODIGO<>'CANCELADO'
                ORDER BY detalle.DETALLE_ID;
                """, conexion);
            comando.Parameters.Add("@Factura", SqlDbType.Int).Value = facturaId;
            using SqlDataAdapter adaptador = new SqlDataAdapter(comando);
            DataSet datos = new DataSet();
            adaptador.Fill(datos);
            if (datos.Tables.Count != 2 || datos.Tables[0].Rows.Count != 1)
                throw new InvalidOperationException("No se encontraron los datos de la factura.");
            // Licencia Community para este proyecto.
            QuestPDF.Settings.License = LicenseType.Community;
            DocumentoRecibo recibo = new DocumentoRecibo(datos.Tables[0].Rows[0], datos.Tables[1]);
            return recibo.GeneratePdf();
        }

        // Plantilla local de ticket: métodos separados y sin funciones flecha.
        private class DocumentoRecibo : IDocument
        {
            private readonly DataRow factura;
            private readonly DataTable productos;
            private readonly System.Globalization.CultureInfo cultura = System.Globalization.CultureInfo.GetCultureInfo("es-NI");

            public DocumentoRecibo(DataRow factura, DataTable productos)
            {
                this.factura = factura;
                this.productos = productos;
            }

            // QuestPDF solicita aquí la estructura del documento.
            public void Compose(IDocumentContainer contenedor)
            {
                contenedor.Page(ConfigurarPagina);
            }

            // Ancho de ticket de 80 mm; el alto se adapta a los productos.
            private void ConfigurarPagina(PageDescriptor pagina)
            {
                pagina.ContinuousSize(80, Unit.Millimetre);
                pagina.Margin(5, Unit.Millimetre);
                pagina.DefaultTextStyle(TextStyle.Default.FontSize(9));
                pagina.Content().Column(ComponerContenido);
            }

            // Agrega identificación, productos y totales leídos desde la factura.
            private void ComponerContenido(ColumnDescriptor columna)
            {
                columna.Spacing(5);
                columna.Item().AlignCenter().Text("POLLOS HERMANOS").FontSize(16).Bold();
                columna.Item().AlignCenter().Text("RECIBO DE COMPRA").FontSize(10);
                columna.Item().AlignCenter().Text("PollosTrack").FontSize(8);
                columna.Item().LineHorizontal(0.6f);
                int identificador = Convert.ToInt32(factura["FACTURA_ID"]);
                columna.Item().Text($"Recibo N.º {identificador:D6}").Bold();
                columna.Item().Table(ComponerDatosPedido);
                columna.Item().LineHorizontal(0.6f);
                columna.Item().Table(ComponerProductos);
                columna.Item().LineHorizontal(0.6f);
                columna.Item().Text($"Subtotal: {FormatearDinero(factura["SUBTOTAL"])}").AlignRight();
                columna.Item().Text($"IVA: {FormatearDinero(factura["MONTO_IVA"])}").AlignRight();
                columna.Item().Text($"TOTAL: {FormatearDinero(factura["TOTAL"])}").FontSize(13).Bold().AlignRight();
                columna.Item().LineHorizontal(0.6f);
                columna.Item().Table(ComponerDatosPago);
                columna.Item().PaddingTop(6).AlignCenter().Text("¡Gracias por su visita!").Bold();
            }

            // Etiquetas a la izquierda y datos alineados a la derecha, con ajuste para nombres largos.
            private void ComponerDatosPedido(TableDescriptor tabla)
            {
                tabla.ColumnsDefinition(DefinirColumnasDatos);
                AgregarDato(tabla, "Fecha", Convert.ToDateTime(factura["FECHA_REGISTRO"]).ToString("dd/MM/yyyy HH:mm"));
                AgregarDato(tabla, "Mesa", Convert.ToString(factura["MESA"])!);
                AgregarDato(tabla, "Cuenta", Convert.ToString(factura["PEDIDO_ID"])!);
                AgregarDato(tabla, "Cliente", Convert.ToString(factura["CLIENTE"])!);
                if (!string.IsNullOrWhiteSpace(Convert.ToString(factura["IDENTIFICACION"])))
                    AgregarDato(tabla, "Identificación", Convert.ToString(factura["IDENTIFICACION"])!);
                if (!string.IsNullOrWhiteSpace(Convert.ToString(factura["NOMBRE_REFERENCIA"])))
                    AgregarDato(tabla, "Referencia", Convert.ToString(factura["NOMBRE_REFERENCIA"])!);
                AgregarDato(tabla, "Atendió", Convert.ToString(factura["USUARIO"])!);
            }

            // Conserva la misma alineación para el método de pago, recibido y cambio.
            private void ComponerDatosPago(TableDescriptor tabla)
            {
                tabla.ColumnsDefinition(DefinirColumnasDatos);
                AgregarDato(tabla, "Pago", Convert.ToString(factura["METODO_PAGO"])!);
                if (!factura.IsNull("MONTO_RECIBIDO"))
                    AgregarDato(tabla, "Recibido", FormatearDinero(factura["MONTO_RECIBIDO"]));
                if (!factura.IsNull("CAMBIO_DEVUELTO"))
                    AgregarDato(tabla, "Cambio", FormatearDinero(factura["CAMBIO_DEVUELTO"]));
            }

            private void DefinirColumnasDatos(TableColumnsDefinitionDescriptor columnas)
            {
                columnas.ConstantColumn(65);
                columnas.RelativeColumn();
            }

            // Una fila sencilla para reutilizar el espaciado y la alineación de cada dato.
            private void AgregarDato(TableDescriptor tabla, string campo, string valor)
            {
                tabla.Cell().PaddingVertical(2).PaddingRight(5).Text(campo + ":");
                tabla.Cell().PaddingVertical(2).AlignRight().Text(valor);
            }
            // Columnas simples para que los nombres largos se ajusten al ancho del papel.
            private void ComponerProductos(TableDescriptor tabla)
            {
                tabla.ColumnsDefinition(DefinirColumnas);
                tabla.Cell().Text("Producto").Bold();
                tabla.Cell().AlignRight().Text("Importe").Bold();
                foreach (DataRow producto in productos.Rows)
                {
                    tabla.Cell().PaddingTop(6).PaddingRight(5).Text($"{producto["PRODUCTO"]}\n{producto["CANTIDAD"]} × {FormatearDinero(producto["PRECIO_UNITARIO"])}");
                    tabla.Cell().PaddingTop(6).AlignRight().Text(FormatearDinero(producto["IMPORTE"]));
                }
            }

            // Reserva espacio para importes sin reducir el texto del producto.
            private void DefinirColumnas(TableColumnsDefinitionDescriptor columnas)
            {
                columnas.RelativeColumn();
                columnas.ConstantColumn(65);
            }

            // Usa el mismo formato monetario en todas las secciones del ticket.
            private string FormatearDinero(object importe)
            {
                return "C$ " + Convert.ToDecimal(importe).ToString("N2", cultura);
            }
        }
        // Ventana de lectura con ancho ajustable y controles simples de escala.
        private class VentanaRecibo : Form
        {
            private readonly MemoryStream contenido;
            private readonly PdfiumViewer.PdfDocument documento;
            private readonly Panel panelPaginas = new Panel();
            private readonly PictureBox imagenRecibo = new PictureBox();
            private readonly Label etiquetaEscala = new Label();
            private int escala = 100;

            public VentanaRecibo(byte[] archivo, int facturaId)
            {
                Text = $"Recibo {facturaId:D6} · PollosTrack";
                StartPosition = FormStartPosition.CenterParent;
                Size = new Size(650, 800);
                MinimumSize = new Size(420, 400);
                contenido = new MemoryStream(archivo);
                try { documento = PdfiumViewer.PdfDocument.Load(contenido); }
                catch { contenido.Dispose(); throw; }
                FlowLayoutPanel herramientas = new FlowLayoutPanel();
                herramientas.Dock = DockStyle.Top;
                herramientas.Height = 48;
                herramientas.Padding = new Padding(8);
                herramientas.BackColor = Color.White;
                Button reducir = new Button { Text = "−", Width = 45 };
                Button ampliar = new Button { Text = "+", Width = 45 };
                Button ajustar = new Button { Text = "Ajustar ancho", Width = 130 };
                reducir.Click += AlReducir;
                ampliar.Click += AlAmpliar;
                ajustar.Click += AlAjustar;
                etiquetaEscala.AutoSize = true;
                etiquetaEscala.Margin = new Padding(12, 6, 0, 0);
                herramientas.Controls.Add(reducir);
                herramientas.Controls.Add(ampliar);
                herramientas.Controls.Add(ajustar);
                herramientas.Controls.Add(etiquetaEscala);
                panelPaginas.Dock = DockStyle.Fill;
                panelPaginas.AutoScroll = true;
                panelPaginas.BackColor = Color.FromArgb(230, 234, 240);
                imagenRecibo.SizeMode = PictureBoxSizeMode.AutoSize;
                panelPaginas.Controls.Add(imagenRecibo);
                Controls.Add(panelPaginas);
                Controls.Add(herramientas);
                Shown += AlMostrar;
                ResizeEnd += AlCambiarTamano;
            }

            // Ajusta inicialmente el recibo al ancho disponible de la ventana.
            private void AlMostrar(object? origen, EventArgs argumentos)
            {
                DibujarRecibo();
            }

            private void AlCambiarTamano(object? origen, EventArgs argumentos)
            {
                DibujarRecibo();
            }

            private void AlReducir(object? origen, EventArgs argumentos)
            {
                escala = Math.Max(50, escala - 25);
                DibujarRecibo();
            }

            private void AlAmpliar(object? origen, EventArgs argumentos)
            {
                escala = Math.Min(200, escala + 25);
                DibujarRecibo();
            }

            private void AlAjustar(object? origen, EventArgs argumentos)
            {
                escala = 100;
                DibujarRecibo();
            }

            // El ticket tiene una página continua; se renderiza al ancho solicitado y se centra.
            private void DibujarRecibo()
            {
                try
                {
                    int ancho = Math.Max(200, panelPaginas.ClientSize.Width - 55) * escala / 100;
                    System.Drawing.SizeF dimensiones = documento.PageSizes[0];
                    int alto = Math.Max(1, (int)Math.Ceiling(ancho * dimensiones.Height / dimensiones.Width));
                    System.Drawing.Image nuevaImagen = documento.Render(0, ancho, alto, 96f, 96f, false);
                    System.Drawing.Image? anterior = imagenRecibo.Image;
                    panelPaginas.AutoScrollPosition = Point.Empty;
                    imagenRecibo.Image = nuevaImagen;
                    imagenRecibo.Location = new Point(Math.Max(20, (panelPaginas.ClientSize.Width - ancho) / 2), 20);
                    if (anterior != null) anterior.Dispose();
                    etiquetaEscala.Text = $"{escala}% del ancho";
                }
                catch (Exception excepcion)
                {
                    MessageBox.Show(this, excepcion.Message, "No se pudo mostrar el recibo");
                }
            }

            // Cierra el documento nativo y libera la imagen y el flujo al cerrar la ventana.
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (imagenRecibo.Image != null)
                    {
                        imagenRecibo.Image.Dispose();
                        imagenRecibo.Image = null;
                    }
                    documento.Dispose();
                    contenido.Dispose();
                }
                base.Dispose(disposing);
            }
        }    }
}
