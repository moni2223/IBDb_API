using System.Security.Cryptography;
using System.Text;

namespace IBDb.Helpers
{
    public class HelperFunctions
    {
        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return System.BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
        public bool VerifyPasswordHash(string password, string storedHash)
        {
            string hashedInput = HashPassword(password);
            return string.Equals(hashedInput, storedHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}
