namespace Cfo.Cats.Server.UI.Extensions;

public static class DateExtensions
{
    public static string ToShortDateOrEmptyString(this DateTime? datetime)
        => ToShortDateOrDefault(datetime, string.Empty);

    public static string ToShortDateOrDefault(this DateTime? dateTime, string defaultText)
    {
        if(dateTime.HasValue is false)
        {
            return defaultText;
        }
        return dateTime.Value.ToShortDateString();
    }
}