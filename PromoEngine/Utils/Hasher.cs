using System.Security.Cryptography;
using System.Text;

namespace PromoEngine.Utils;

public static class Hasher
{
    public static string HashString(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        { 
            return string.Empty;
        }

        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input.Trim().ToLower());
        var hash = sha.ComputeHash(bytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
}
