namespace Cfo.Cats.Application.Outbox;

public abstract record IntegrationEvent(DateTime OccurredOn)
{
    public Guid MessageId { get; set; } = Guid.CreateVersion7();
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime OccurredOn { get; set; } = OccurredOn;
};
