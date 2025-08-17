namespace Cfo.Cats.Application.Features.Payments.Extensions;

public static class PaymentExtensions
{
    public static decimal CalculateCappedPercentage(this int score, int target)
    {
        // Prevent division by zero
        if (target == 0)
        {
            return 0;
        }

        // Calculate the percentage
        decimal percentage = ((decimal)score / target) * 100;

        // Cap the result at 100%
        return Math.Min(Math.Round(percentage), 100);
    }

    public static decimal CalculatePercentage(this int achieved, int target)
    {
        if (target == 0)
        {
            return 0;
        }

        decimal percentage = (decimal)achieved / target * 100;
        return Math.Round(percentage);
    }

    public static string GetPerformanceClass(this int achieved, int target)
    {
        if (target == 0)
        {
            return "performance-error";
        }

        var percentage = (decimal)achieved / target * 100;

        return percentage switch
        {
            >= 100 => "performance-success",
            >= 86 => "performance-warning",
            _ => "performance-error"
        };
    }
}
