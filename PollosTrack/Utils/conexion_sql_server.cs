using Microsoft.Data.SqlClient;

namespace PollosTrack.Utils
{
    internal class conexion_sql_server
    {
        // Conexión local con la cuenta de Windows. TrustServerCertificate es para desarrollo local.
        private readonly string cadenaConexion =
            "Server=localhost;Database=POLLOS_HERMANOS_TRACK;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;Connect Timeout=5;";

        // Abre la conexión sin bloquear la interfaz mientras SQL Server responde.
        public async Task<SqlConnection> AbrirConexionAsincrona()
        {
            SqlConnection conexion = new SqlConnection(cadenaConexion);
            try
            {
                await conexion.OpenAsync();
                return conexion;
            }
            catch
            {
                conexion.Dispose();
                throw;
            }
        }

        // Abre una conexión de forma síncrona para operaciones que no necesitan espera asíncrona.
        public SqlConnection AbrirConexion()
        {
            SqlConnection conexion = new SqlConnection(cadenaConexion);
            try
            {
                conexion.Open();
                return conexion;
            }
            catch
            {
                conexion.Dispose();
                throw;
            }
        }
    }
}
