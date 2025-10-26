using System.Text;

namespace PromoEngine.Utils;

public static class PromoCodeGenerator
{
    private static readonly char[] AllowedChars =
        "ABCDEFGHJKLMNPQRSTUVWX13456789".ToCharArray();

    public static string GenerateCode()
    {
        var random = new Random();
        var sb = new StringBuilder(5);

        for (int i = 0; i < 5; i++)
            sb.Append(AllowedChars[random.Next(AllowedChars.Length)]);

        return sb.ToString();
    }

    public static List<string> GenerateUniqueCodes(int count)
    {
        var codes = new HashSet<string>();
        while (codes.Count < count)
        {
            codes.Add(GenerateCode());
        }
        return codes.ToList();
    }
}
