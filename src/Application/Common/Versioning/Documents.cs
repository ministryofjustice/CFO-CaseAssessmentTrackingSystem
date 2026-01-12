namespace Cfo.Cats.Infrastructure.Constants;

public static class Documents
{
    public static class Consent
    {
        public const double MaximumSizeInMegabytes = 1;

        public static IReadOnlyList<string> Versions { get; set; } =
        [
            "1.1",
            "1.0",
        ];
    }

    public static class RightToWork
    {
        public const double MaximumSizeInMegabytes = 1;
    }

    public static class ActivityTemplate
    {
        public const double MaximumSizeInMegabytes = 1;
    }

}
