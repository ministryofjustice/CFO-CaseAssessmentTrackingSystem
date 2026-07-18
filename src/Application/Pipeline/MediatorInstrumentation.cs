using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Cfo.Cats.Application.Pipeline;

/// <summary>
///     Holds the <see cref="ActivitySource" /> and <see cref="Meter" /> used by the mediator
///     pipeline to emit OpenTelemetry traces and metrics. These are surfaced through the Aspire
///     dashboard (and any OTLP-compatible backend such as Grafana / Prometheus).
///     The <see cref="ActivitySourceName" /> and <see cref="MeterName" /> must be registered with
///     OpenTelemetry via <c>AddSource</c> / <c>AddMeter</c> for the signals to be collected
///     (see <c>Cats.ServiceDefaults.Extensions.ConfigureOpenTelemetry</c>).
/// </summary>
public static class MediatorInstrumentation
{
    public const string ActivitySourceName = "Cfo.Cats.Mediator";
    public const string MeterName = "Cfo.Cats.Mediator";

    public static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    private static readonly Meter Meter = new(MeterName);

    /// <summary>Total number of mediator requests handled, dimensioned by request, kind and outcome.</summary>
    public static readonly Counter<long> RequestCount = Meter.CreateCounter<long>(
        "cats.mediator.requests",
        unit: "{request}",
        description: "Number of mediator requests handled.");

    /// <summary>Duration of mediator request handling in milliseconds.</summary>
    public static readonly Histogram<double> RequestDuration = Meter.CreateHistogram<double>(
        "cats.mediator.request.duration",
        unit: "ms",
        description: "Duration of mediator request handling.");

    /// <summary>Number of mediator requests currently in flight.</summary>
    public static readonly UpDownCounter<long> ActiveRequests = Meter.CreateUpDownCounter<long>(
        "cats.mediator.active_requests",
        unit: "{request}",
        description: "Number of mediator requests currently being handled.");
}
