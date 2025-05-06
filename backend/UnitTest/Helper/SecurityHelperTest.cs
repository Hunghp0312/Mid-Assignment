using System.Security.Cryptography;
using Common.Utilities;

namespace UnitTest.Helper;

public class SecurityHelperTest
{
    public (string hash, string salt) HashPassword(string password)
    {
        return SecurityHelper.HashPassword(password);
    }
    public static bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        return SecurityHelper.VerifyPassword(password, storedHash, storedSalt);
    }
}
