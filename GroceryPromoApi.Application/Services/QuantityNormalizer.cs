using System.Text.RegularExpressions;

namespace GroceryPromoApi.Application.Services;

internal static class QuantityNormalizer
{
    private static readonly (Regex Pattern, string Replacement)[] UnitReplacements =
    [
        (new Regex(@"(?<!\p{L})мл\.?(?!\p{L})", RegexOptions.IgnoreCase), "ml"),
        (new Regex(@"(?<!\p{L})л\.?(?!\p{L})",  RegexOptions.IgnoreCase), "l"),
        (new Regex(@"(?<!\p{L})кг\.?(?!\p{L})", RegexOptions.IgnoreCase), "kg"),
        (new Regex(@"(?<!\p{L})кр\.?(?!\p{L})", RegexOptions.IgnoreCase), "kg"),
        (new Regex(@"(?<!\p{L})гр\.?(?!\p{L})", RegexOptions.IgnoreCase), "g"),
        (new Regex(@"(?<!\p{L})г\.?(?!\p{L})",  RegexOptions.IgnoreCase), "g"),
        (new Regex(@"(?<!\p{L})броя(?!\p{L})",  RegexOptions.IgnoreCase), "br"),
        (new Regex(@"(?<!\p{L})бр\.?(?!\p{L})", RegexOptions.IgnoreCase), "br"),
        (new Regex(@"(?<!\p{L})комплекта?(?!\p{L})", RegexOptions.IgnoreCase), "kit"),
        (new Regex(@"(?<!\p{L})чифта(?!\p{L})",      RegexOptions.IgnoreCase), "pair"),
    ];

    private static readonly Regex SpacesAroundX    = new(@"\s*[xх×]\s*", RegexOptions.IgnoreCase);
    private static readonly Regex NumberUnitSpace   = new(@"(\d)\s+([a-z]+)");
    private static readonly Regex PackagingSuffix   = new(@"(/опак\w*| в оп\w*).*$", RegexOptions.IgnoreCase);
    private static readonly Regex TrailingSlashData = new(@"/опаковка.*$", RegexOptions.IgnoreCase);

    public static string? Normalize(string? quantity)
    {
        if (string.IsNullOrWhiteSpace(quantity))
            return null;

        var q = quantity.ToLower().Trim();

        // decimal separator
        q = q.Replace(',', '.');

        // Cyrillic units → Latin
        foreach (var (pattern, replacement) in UnitReplacements)
            q = pattern.Replace(q, replacement);

        // multiplication sign (Cyrillic х and ×) → Latin x
        q = SpacesAroundX.Replace(q, "x");

        // remove space between number and unit: "250 g" → "250g"
        q = NumberUnitSpace.Replace(q, "$1$2");

        // strip packaging suffixes
        q = PackagingSuffix.Replace(q, "");
        q = TrailingSlashData.Replace(q, "");

        // strip approximate prefix
        q = q.TrimStart('~').Trim();

        // strip trailing period
        q = q.TrimEnd('.').Trim();

        return string.IsNullOrWhiteSpace(q) ? null : q;
    }
}
