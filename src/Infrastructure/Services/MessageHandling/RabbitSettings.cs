namespace Cfo.Cats.Infrastructure.Services.MessageHandling;

public class RabbitSettings
{
    public required string TopicExchange { get; set; } = "CatsTopic";
    public required string DirectExchange { get; set; } = "CatsDirect";
    public required int Retries { get; set; } = 2;
    public required string DocumentService { get; set; } = "document-service";
    public required string PaymentService { get; set; } = "payment-service";
    public required string TasksService { get; set; } = "tasks-service";
    public required string OvernightService { get; set; } = "overnight-service";
}