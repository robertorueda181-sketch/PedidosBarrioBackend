using System.Security.Cryptography;

namespace PedidosBarrio.Application.Utilities
{
    public class PasswordHasher
    {
        private const int SaltSize = 32; // 256 bits
        private const int HashSize = 32; // 256 bits
        private const int Iterations = 100000; // Iteraciones PBKDF2

        public static (string hash, string salt) HashPassword(string password)
        {
            // Generar salt aleatorio
            byte[] saltBytes = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            // Generar hash usando PBKDF2
            using (var pbkdf2 = new Rfc2898DeriveBytes(
                password, 
                saltBytes, 
                Iterations, 
                HashAlgorithmName.SHA256))
            {
                byte[] hashBytes = pbkdf2.GetBytes(HashSize);
                
                // Convertir a Base64 para almacenar
                string hash = Convert.ToBase64String(hashBytes);
                string salt = Convert.ToBase64String(saltBytes);
                
                return (hash, salt);
            }
        }

        public static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            byte[] saltBytes = Convert.FromBase64String(storedSalt);
            
            // Generar hash con la misma sal
            using (var pbkdf2 = new Rfc2898DeriveBytes(
                password, 
                saltBytes, 
                Iterations, 
                HashAlgorithmName.SHA256))
            {
                byte[] hashBytes = pbkdf2.GetBytes(HashSize);
                string newHash = Convert.ToBase64String(hashBytes);
                
                // Comparación segura contra timing attacks
                return CryptographicOperations.FixedTimeEquals(
                    Convert.FromBase64String(storedHash),
                    hashBytes
                );
            }
        }
    }
}
