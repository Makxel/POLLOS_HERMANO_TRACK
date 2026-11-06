using Microsoft.Data.SqlClient;
using PollosTrack.Catalogos.Models;
using PollosTrack.Utils;

namespace PollosTrack.Catalogos.Repositories
{
    public class UsuarioRepository
    {
        private const string Columnas =
            "USUARIO_ID, ROL_ID, NOMBRE_COMPLETO, USERNAME, PASSWORD_HASH, ACTIVO, FECHA_REGISTRO";

        private static Usuario Mapear(SqlDataReader r) => new Usuario
        {
            UsuarioId = r.GetInt32(0),
            RolId = r.GetInt32(1),
            NombreCompleto = r.GetString(2),
            Username = r.GetString(3),
            PasswordHash = r.GetString(4),
            Activo = r.GetBoolean(5),
            FechaRegistro = r.IsDBNull(6) ? default : r.GetDateTime(6)
        };

        public List<Usuario> ObtenerTodos()
        {
            var lista = new List<Usuario>();
            using var conn = Conexion.ObtenerConexion();
            using var cmd = new SqlCommand(
                $"SELECT {Columnas} FROM SEG.USUARIOS ORDER BY NOMBRE_COMPLETO", conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) lista.Add(Mapear(reader));
            return lista;
        }

        public Usuario? ObtenerPorUsername(string username)
        {
            using var conn = Conexion.ObtenerConexion();
            using var cmd = new SqlCommand(
                $"SELECT {Columnas} FROM SEG.USUARIOS WHERE USERNAME = @username", conn);
            cmd.Parameters.AddWithValue("@username", username);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Mapear(reader) : null;
        }

        // Para el login: devuelve el usuario si las credenciales son correctas y está activo; si no, null
        public Usuario? Autenticar(string username, string password)
        {
            var usuario = ObtenerPorUsername(username);
            if (usuario == null || !usuario.Activo) return null;
            return SeguridadHelper.Verificar(password, usuario.PasswordHash) ? usuario : null;
        }

        public void Insertar(Usuario usuario, string passwordPlano)
        {
            using var conn = Conexion.ObtenerConexion();
            using var cmd = new SqlCommand(
                @"INSERT INTO SEG.USUARIOS (ROL_ID, NOMBRE_COMPLETO, USERNAME, PASSWORD_HASH, ACTIVO)
                  VALUES (@rol, @nombre, @username, @hash, @activo)", conn);
            cmd.Parameters.AddWithValue("@rol", usuario.RolId);
            cmd.Parameters.AddWithValue("@nombre", usuario.NombreCompleto);
            cmd.Parameters.AddWithValue("@username", usuario.Username);
            cmd.Parameters.AddWithValue("@hash", SeguridadHelper.GenerarHash(passwordPlano));
            cmd.Parameters.AddWithValue("@activo", usuario.Activo);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        // No toca la contraseña; para eso está CambiarPassword
        public void Actualizar(Usuario usuario)
        {
            using var conn = Conexion.ObtenerConexion();
            using var cmd = new SqlCommand(
                @"UPDATE SEG.USUARIOS
                  SET ROL_ID = @rol, NOMBRE_COMPLETO = @nombre, USERNAME = @username, ACTIVO = @activo
                  WHERE USUARIO_ID = @id", conn);
            cmd.Parameters.AddWithValue("@id", usuario.UsuarioId);
            cmd.Parameters.AddWithValue("@rol", usuario.RolId);
            cmd.Parameters.AddWithValue("@nombre", usuario.NombreCompleto);
            cmd.Parameters.AddWithValue("@username", usuario.Username);
            cmd.Parameters.AddWithValue("@activo", usuario.Activo);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void CambiarPassword(int usuarioId, string nuevaPassword)
        {
            using var conn = Conexion.ObtenerConexion();
            using var cmd = new SqlCommand(
                "UPDATE SEG.USUARIOS SET PASSWORD_HASH = @hash WHERE USUARIO_ID = @id", conn);
            cmd.Parameters.AddWithValue("@id", usuarioId);
            cmd.Parameters.AddWithValue("@hash", SeguridadHelper.GenerarHash(nuevaPassword));
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        // "Eliminar" = desactivar, porque los pedidos y facturas referencian al usuario
        public void CambiarEstado(int usuarioId, bool activo)
        {
            using var conn = Conexion.ObtenerConexion();
            using var cmd = new SqlCommand(
                "UPDATE SEG.USUARIOS SET ACTIVO = @activo WHERE USUARIO_ID = @id", conn);
            cmd.Parameters.AddWithValue("@id", usuarioId);
            cmd.Parameters.AddWithValue("@activo", activo);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        // Crea los roles base y el usuario admin si la tabla de usuarios está vacía
        public void AsegurarAdminInicial()
        {
            using var conn = Conexion.ObtenerConexion();
            conn.Open();

            using (var roles = new SqlCommand(
                @"INSERT INTO SEG.ROLES (NOMBRE, DESCRIPCION)
                  SELECT v.NOMBRE, v.DESCRIPCION
                  FROM (VALUES
                      ('Administrador', 'Acceso total al sistema'),
                      ('Mesero', 'Toma pedidos y atiende mesas'),
                      ('Cajero', 'Cobra y emite facturas')) AS v(NOMBRE, DESCRIPCION)
                  WHERE NOT EXISTS (SELECT 1 FROM SEG.ROLES r WHERE r.NOMBRE = v.NOMBRE)", conn))
            {
                roles.ExecuteNonQuery();
            }

            using (var cuenta = new SqlCommand("SELECT COUNT(*) FROM SEG.USUARIOS", conn))
            {
                if ((int)cuenta.ExecuteScalar()! > 0) return;
            }

            using var admin = new SqlCommand(
                @"INSERT INTO SEG.USUARIOS (ROL_ID, NOMBRE_COMPLETO, USERNAME, PASSWORD_HASH)
                  SELECT ROL_ID, 'Administrador', 'admin', @hash FROM SEG.ROLES WHERE NOMBRE = 'Administrador'", conn);
            admin.Parameters.AddWithValue("@hash", SeguridadHelper.GenerarHash("Admin123*"));
            admin.ExecuteNonQuery();
        }
    }
}