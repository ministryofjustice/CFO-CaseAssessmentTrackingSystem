namespace Cfo.Cats.Application.Common.Extensions;

public static class DateExtensions
{
    public static int CalculateAge(this DateOnly date)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        return date.CalculateAge(today);
    }

    public static int CalculateAge(this DateOnly date, DateTime asOf)
    {
        return date.CalculateAge(DateOnly.FromDateTime(asOf));
    }

    public static int CalculateAge(this DateOnly date, DateOnly asOf)
    {
        var age = asOf.Year - date.Year;

        // Adjust if the birthday hasn't occurred yet this year
        if (date > asOf.AddYears(-age))
        {
            age--;
        }

        return age;
    }
}
