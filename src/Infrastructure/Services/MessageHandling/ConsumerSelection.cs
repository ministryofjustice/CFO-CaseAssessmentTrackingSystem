using Microsoft.Extensions.Hosting;

namespace Cfo.Cats.Infrastructure.Services.MessageHandling;

/// <summary>
/// Selects which Rebus consumer services run in this process.
/// An empty <see cref="Include"/> means all services; <see cref="Exclude"/> is applied last.
/// </summary>
public class ConsumerSelectionOptions
{
    public const string Key = "Consumers";

    public string[] Include { get; set; } = [];
    public string[] Exclude { get; set; } = [];
}

public static class ConsumerNames
{
    public const string Overnight = nameof(Overnight);
    public const string Tasks = nameof(Tasks);
    public const string Payment = nameof(Payment);
    public const string Documents = nameof(Documents);
    public const string Telemetry = nameof(Telemetry);

    public static readonly IReadOnlyList<string> All = [Overnight, Tasks, Payment, Documents, Telemetry];
}

public static class ConsumerSelection
{
    /// <summary>
    /// Resolves the enabled consumer names. Throws on unknown names so a typo cannot silently disable a consumer.
    /// </summary>
    public static IReadOnlySet<string> Resolve(ConsumerSelectionOptions options)
    {
        var include = Normalise(options.Include, nameof(options.Include));
        var exclude = Normalise(options.Exclude, nameof(options.Exclude));

        var selected = include.Count == 0
            ? new HashSet<string>(ConsumerNames.All, StringComparer.OrdinalIgnoreCase)
            : include;

        selected.ExceptWith(exclude);

        return selected;
    }

    private static HashSet<string> Normalise(IEnumerable<string> names, string optionName)
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var name in names.Where(n => !string.IsNullOrWhiteSpace(n)))
        {
            var canonical = ConsumerNames.All.FirstOrDefault(
                n => string.Equals(n, name.Trim(), StringComparison.OrdinalIgnoreCase));

            if (canonical is null)
            {
                throw new InvalidOperationException(
                    $"Unknown consumer '{name}' in {ConsumerSelectionOptions.Key}:{optionName}. " +
                    $"Valid names: {string.Join(", ", ConsumerNames.All)}.");
            }

            result.Add(canonical);
        }

        return result;
    }
}

internal sealed class ConsumerSelectionLogger(ILogger<ConsumerSelectionLogger> logger, IReadOnlySet<string> enabled) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Message consumers enabled: {Enabled}; disabled: {Disabled}",
            string.Join(", ", ConsumerNames.All.Where(enabled.Contains)),
            string.Join(", ", ConsumerNames.All.Where(n => !enabled.Contains(n))));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
