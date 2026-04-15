using System.Globalization;
using System.Text.RegularExpressions;

namespace GroceryPromoApi.Application.Services;

public static class QuantityNormalizer
{
    private static readonly Dictionary<string, string> UnitMap = new()
    {
        { "мл", "ml" }, { "л", "l" },
        { "кг", "kg" }, { "кр", "kg" },
        { "гр", "g"  }, { "г",  "g" },
        { "броя", "br" }, { "бр", "br" },
        { "комплект", "kit" }, { "комплекта", "kit" },
        { "чифта", "pair" },
        { "опаковки", "pcs" }, { "опаковка", "pcs" }
    };

    private static readonly Regex PackagingSuffix =
        new(@"(/опак\w*| в оп\w*).*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static string? Normalize(string? quantity)
    {
        if (string.IsNullOrWhiteSpace(quantity))
            return null;

        var normalized = quantity.ToLower().Trim()
            .Replace(',', '.')
            .TrimStart('~')
            .TrimEnd('.');

        normalized = PackagingSuffix.Replace(normalized, "").Trim();

        if (string.IsNullOrWhiteSpace(normalized))
            return null;

        var parts = normalized.Split(['x', 'х', '×'], StringSplitOptions.TrimEntries);
        return string.Join("x", parts.Select(NormalizeToken));
    }

    private static string NormalizeToken(string token)
    {
        int unitStart = 0;
        while (unitStart < token.Length && (char.IsDigit(token[unitStart]) || token[unitStart] == '.' || token[unitStart] == ' '))
            unitStart++;

        if (unitStart == 0)
            return token; // no leading number — return as-is

        var numberStr = token[..unitStart].Trim();
        var unitStr   = token[unitStart..].Trim().TrimEnd('.');

        if (!decimal.TryParse(numberStr, NumberStyles.Number, CultureInfo.InvariantCulture, out var number))
            return token;

        if (string.IsNullOrEmpty(unitStr))
            return number.ToString("0.##");

        var latinUnit = UnitMap.TryGetValue(unitStr, out var latin) ? latin : unitStr;

        return latinUnit switch
        {
            "kg" => (number * 1000).ToString("0.##") + "g",
            "l"  => (number * 1000).ToString("0.##") + "ml",
            _    => number.ToString("0.##") + latinUnit
        };
    }
}
