using Amazon.SecurityToken.Model;

namespace Cfo.Cats.Infrastructure.Services.MessageHandling;

public class RabbitSettings
{
    public string DocumentService { get; set; } = "document-service";
    public string PaymentService { get; set; } = "payment-service";
    public string TasksService { get; set; } = "tasks-service";
    public string OvernightService { get; set; } = "overnight-service";

    public string TelemetryService { get; set; } = "telemetry-service";

    public string CatsService { get; set; } = "cats-service";
}