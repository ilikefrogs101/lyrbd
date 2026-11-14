using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ilikefrogs101.CustomExtensions;

public static class StringExtensions {
    public static string Standardise(this string input) {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        input = input.Normalize(NormalizationForm.FormKC);
        input = Regex.Replace(input, @"\s+", " ").Trim();

        return input;
    }
    public static string StandardiseForSearch(this string input) {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        input = input.Standardise();
        input = input.StandardisePunctuation();

        input = input.ToLowerInvariant();
        input = input.RemoveDiacritics();
        input = input.RemoveSymbols();
        //input = input.RemovePunctuation();

        return input;
    }
    public static string StandardisePunctuation(this string input) {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        input = Regex.Replace(input, @"[\u2010-\u2015\u2212\uFE58\uFE63\uFF0D]", "-");
        input = Regex.Replace(
            input, 
            @"[\u201C\u201D\u201E\u201F\u00AB\u00BB\u301D\u301E\u301F\u2018\u2019\u201A\u201B\u2032\u2035\u0060\u00B4]", 
            "\""
        );

        return input;
    }
    public static string RemoveDiacritics(this string input) {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        string normalised = input.Normalize(NormalizationForm.FormKD);
        StringBuilder stringBuilder = new StringBuilder(normalised.Length);

        for (int i = 0; i < normalised.Length; i++) {
            char c = normalised[i];
            UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark
                && unicodeCategory != UnicodeCategory.SpacingCombiningMark
                && unicodeCategory != UnicodeCategory.EnclosingMark) {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder
            .ToString()
            .Normalize(NormalizationForm.FormKC);
    }
    public static string RemoveSymbols(this string input) {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        return Regex.Replace(input, @"\p{S}", "");
    }
    public static string RemovePunctuation(this string input) {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        return Regex.Replace(input, @"\p{P}", "");
    }
    public static string RemoveNumbers(this string input) {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        return Regex.Replace(input, @"\d+", "");
    }
}