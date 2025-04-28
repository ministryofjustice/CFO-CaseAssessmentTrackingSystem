namespace Cfo.Cats.Application.Common.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Converts the first character of the string to uppercase and the remaining characters to lowercase.
    /// Returns <c>null</c> if the input string is <c>null</c>.
    /// </summary>
    /// <param name="str">The input string to convert.</param>
    /// <returns>
    /// A string with the first character capitalized and the rest in lowercase, 
    /// or <c>null</c> if the input was <c>null</c>.
    /// </returns>
    public static string ToTitleCase(this string str)
    {
        if(string.IsNullOrEmpty(str))
        {
            return str;
        }

        return char.ToUpper(str[0]) + str.Substring(1).ToLower();
    }
}
