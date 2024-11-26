using System.Security.Cryptography;
using System.Text;

namespace Cart_It.Models.JWT
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool IsValidBase64String(string value);
        bool VerifyPassword(string password, string hashedPassword);
    }

    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16; // 128-bit
        private const int KeySize = 32; // 256-bit
        private const int Iterations = 10000; // PBKDF2 iterations

        public string HashPassword(string password)
        {
            // Generate a salt
            using var rng = new RNGCryptoServiceProvider();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);

       // Generate the hash
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(KeySize);

            // Combine salt and hash
            var hashBytes = new byte[SaltSize + KeySize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, KeySize);

            // Convert to base64 string
            return Convert.ToBase64String(hashBytes);
        }

        public bool IsValidBase64String(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            // Base64 string should have length multiple of 4
            if (value.Length % 4 != 0)
                return false;

            // Try to convert the string to bytes and check if it's valid Base64
            try
            {
                Convert.FromBase64String(value); // Will throw exception if it's invalid
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            // First check if the hashedPassword is a valid Base64 string
            if (!IsValidBase64String(hashedPassword))
            {
                throw new FormatException("The hashed password is not a valid Base64 string.");
            }

            // Decode the base64 encoded hash
            var hashBytes = Convert.FromBase64String(hashedPassword);

            // Extract the salt (assuming the salt size is known)
            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            // Extract the stored hash
            var storedHash = new byte[KeySize];
            Array.Copy(hashBytes, SaltSize, storedHash, 0, KeySize);

            // Hash the input password with the extracted salt
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var computedHash = pbkdf2.GetBytes(KeySize);

            // Compare the stored hash with the computed hash
            return storedHash.SequenceEqual(computedHash);
        }
    }
}
