using PollosTrack.Catalogos.Models;

namespace PollosTrack.Utils
{
    public static class SesionActual
    {
        public static Usuario? Usuario { get; private set; }

        public static bool HaIniciadoSesion => Usuario != null;

        public static void Iniciar(Usuario usuario)
        {
            usuario.PasswordHash = string.Empty;
            Usuario = usuario;
        }

        public static void Cerrar() => Usuario = null;
    }
}