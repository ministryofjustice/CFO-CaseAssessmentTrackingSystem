namespace Cfo.Cats.Server.UI.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Returns the left most characters of the given string, upto a maximum length of length.
    /// </summary>
    /// <param name="value">The string to parse</param>
    /// <param name="length">The maximum length of the string to return.</param>
    /// <returns>
    ///     The string trimmed to length (or the length of the given string if shorter)
    /// </returns>
    public static string Left(this string value, int length)
    {
        int maxLength = value.Length;

        return value.Substring(0, Math.Min(maxLength, length));
    }
}