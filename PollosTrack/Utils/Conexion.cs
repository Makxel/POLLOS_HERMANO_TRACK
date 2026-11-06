using Microsoft.Data.SqlClient;

namespace PollosTrack.Utils
{
    public static class Conexion
    {
        private const string CadenaConexion =
            "Server=localhost;Database=POLLOS_HERMANOS_TRACK;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;Connect Timeout=5;";

        public static SqlConnection ObtenerConexion() => new SqlConnection(CadenaConexion);
    }
}
