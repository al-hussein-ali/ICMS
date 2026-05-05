using BCrypt.Net;

namespace ICMS.Application.Utilities
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(hashedPassword)) return false;

            try
            {
                // BCrypt hashes start with $2a$, $2b$ or $2y$
                if (hashedPassword.StartsWith("$2"))
                {
                    return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
                }

                // Legacy SHA256 Fallback
                using var sha256 = System.Security.Cryptography.SHA256.Create();
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                var sha256Hash = Convert.ToBase64String(hashedBytes);
                
                return sha256Hash == hashedPassword;
            }
            catch
            {
                return false;
            }
        }

        public static string GenerateSimplePassword(int length = 6)
        {
            const string chars = "0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
