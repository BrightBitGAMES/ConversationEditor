using System.Text.RegularExpressions;

namespace BrightBit
{

public static class StringExtensions
{
    static Regex titleCaseSeparator  = new Regex("(?<=[a-z])([A-Z0-9])|(?<=[A-Z])([A-Z0-9][a-z])", RegexOptions.None);
    static Regex pascalCaseSeparator = new Regex("(?<=[a-z])([A-Z])|(?<=[A-Z])([A-Z][a-z])", RegexOptions.None);

    static string pascalCaseSeparatorSubstitution = " $1$2";
    static string titleCaseSeparatorSubstitution  = " $1$2";

    public static string ToTitleCase(this string s)
    {
        string result = titleCaseSeparator.Replace(s, titleCaseSeparatorSubstitution);

        return result.ToFirstLetterUp();
    }

    public static string ToSeparatedPascalCase(this string s)
    {
        string result = pascalCaseSeparator.Replace(s, pascalCaseSeparatorSubstitution);

        return result.ToFirstLetterUp();
    }

    public static string ToFirstLetterUp(this string s)
    {
        if (s.IsNullOrEmpty()) return s;

        if (s.Length > 1) return char.ToUpper(s[0]) + s.Substring(1);

        return s.ToUpper(); // s.Length == 1
    }
}

} // of namespace BrightBit
