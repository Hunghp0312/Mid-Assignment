using System.Security.Cryptography;

namespace Common.Utilities;

public static class SecurityHelper
{
    public static (string hash, string salt) HashPassword(string password)
    {
        byte[] saltBytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes); // Generate random salt
        }

        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000, HashAlgorithmName.SHA256);
        byte[] hashBytes = pbkdf2.GetBytes(32); // 256-bit hash
        string hash = Convert.ToBase64String(hashBytes);
        string salt = Convert.ToBase64String(saltBytes);
        return (hash, salt);
    }
    public static bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        byte[] saltBytes = Convert.FromBase64String(storedSalt);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000, HashAlgorithmName.SHA256);
        byte[] hashBytes = pbkdf2.GetBytes(32);
        string computedHash = Convert.ToBase64String(hashBytes);
        return computedHash == storedHash;
    }
}
