namespace Cfo.Cats.Application.Common.Extensions;

public static class MathExtensions
{
    public static double PowerRound(this double value, double valueToRaiseBy, int roundTo = 5)
    {
        double result = Math.Pow(value, valueToRaiseBy);
        double rounded = Math.Round(result, roundTo);
        return rounded;
    }
}
