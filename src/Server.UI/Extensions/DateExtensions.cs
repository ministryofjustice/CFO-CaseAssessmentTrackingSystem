namespace Cfo.Cats.Server.UI.Extensions;

public static class DateExtensions
{
    public static string ToShortDateOrEmptyString(this DateTime? datetime)
    {
        if (datetime.HasValue is false)
        {
            return string.Empty;
        }

        return datetime.Value.ToShortDateString();
    }
}