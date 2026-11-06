using System.Security.Cryptography;

namespace PollosTrack.Utils
{
    public static class SeguridadHelper
    {
        private const int TamanoSalt = 16;
        private const int TamanoHash = 32;
        private const int Iteraciones = 100000;

        // Devuelve "iteraciones.salt.hash" (cabe de sobra en VARCHAR(256))
        public static string GenerarHash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(TamanoSalt);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password, salt, Iteraciones, HashAlgorithmName.SHA256, TamanoHash);
            return $"{Iteraciones}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public static bool Verificar(string password, string hashGuardado)
        {
            try
            {
                var partes = hashGuardado.Split('.');
                if (partes.Length != 3 || !int.TryParse(partes[0], out int iteraciones))
                    return false;

                byte[] salt = Convert.FromBase64String(partes[1]);
                byte[] esperado = Convert.FromBase64String(partes[2]);
                byte[] calculado = Rfc2898DeriveBytes.Pbkdf2(
                    password, salt, iteraciones, HashAlgorithmName.SHA256, esperado.Length);

                return CryptographicOperations.FixedTimeEquals(calculado, esperado);
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}