using PollosTrack.Catalogos.Repositories;
using PollosTrack.Utils;

namespace PollosTrack.Catalogos.Forms
{
    public class FormLogin : Form
    {
        private readonly TextBox txtUsuario = new();
        private readonly TextBox txtPassword = new();
        private readonly Button btnIngresar = new();
        private readonly Label lblError = new();
        private readonly UsuarioRepository _usuarios = new();

        public FormLogin()
        {
            Text = "PollosTrack | Iniciar sesión";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(380, 340);
            BackColor = Color.FromArgb(255, 248, 229);
            Font = new Font("Segoe UI", 10F);

            Color azul = Color.FromArgb(27, 47, 82);
            Color amarillo = Color.FromArgb(255, 208, 79);

            var titulo = new Label
            {
                Text = "POLLOSTRACK",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = azul,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds = new Rectangle(0, 20, 380, 50)
            };

            var lblUsuario = new Label
            {
                Text = "Usuario",
                ForeColor = azul,
                Bounds = new Rectangle(40, 90, 300, 22)
            };
            txtUsuario.Bounds = new Rectangle(40, 115, 300, 30);

            var lblPassword = new Label
            {
                Text = "Contraseña",
                ForeColor = azul,
                Bounds = new Rectangle(40, 160, 300, 22)
            };
            txtPassword.Bounds = new Rectangle(40, 185, 300, 30);
            txtPassword.UseSystemPasswordChar = true;

            lblError.ForeColor = Color.Firebrick;
            lblError.Bounds = new Rectangle(40, 225, 300, 40);

            btnIngresar.Text = "Ingresar";
            btnIngresar.BackColor = amarillo;
            btnIngresar.ForeColor = azul;
            btnIngresar.FlatStyle = FlatStyle.Flat;
            btnIngresar.FlatAppearance.BorderSize = 0;
            btnIngresar.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnIngresar.Bounds = new Rectangle(40, 275, 300, 40);
            btnIngresar.Click += Ingresar;

            Controls.AddRange(new Control[]
            {
                titulo, lblUsuario, txtUsuario, lblPassword, txtPassword, lblError, btnIngresar
            });

            AcceptButton = btnIngresar;   // Enter = Ingresar
            Load += (s, e) => PrepararBaseDeDatos();
        }

        // Crea el usuario administrador la primera vez que se abre el sistema
        private void PrepararBaseDeDatos()
        {
            try
            {
                _usuarios.AsegurarAdminInicial();
            }
            catch (Exception)
            {
                lblError.Text = "No se pudo conectar con la base de datos. Verifique que SQL Server esté en ejecución.";
            }
        }

        private void Ingresar(object? sender, EventArgs e)
        {
            lblError.Text = string.Empty;
            string usuario = txtUsuario.Text.Trim();
            string password = txtPassword.Text;

            if (usuario.Length == 0 || password.Length == 0)
            {
                lblError.Text = "Ingrese su usuario y contraseña.";
                return;
            }

            try
            {
                var autenticado = _usuarios.Autenticar(usuario, password);
                if (autenticado == null)
                {
                    lblError.Text = "Usuario o contraseña incorrectos.";
                    txtPassword.Clear();
                    txtPassword.Focus();
                    return;
                }

                SesionActual.Iniciar(autenticado);
                DialogResult = DialogResult.OK;   // cierra el login
            }
            catch (Exception)
            {
                lblError.Text = "No se pudo conectar con la base de datos.";
            }
        }
    }
}