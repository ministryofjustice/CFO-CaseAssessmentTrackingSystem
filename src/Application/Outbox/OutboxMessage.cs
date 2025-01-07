namespace Cfo.Cats.Application.Outbox;

public sealed class OutboxMessage 
{
    public Guid Id { get; set; }
    public required string Type { get; set; }
    public required string Content { get; set; }
    public DateTime OccurredOnUtc { get; set; }
    public DateTime? ProcessedOnUtc { get; set; }
    public string? Error { get; set; }

    /// <summary>
    /// The Id of the outbox message that this was copied from.
    /// This being set means this outbox message was recreated.
    /// </summary>
    public Guid? ParentId { get;set; }

    public OutboxMessage Replicate() => new()
    {
        Id = Guid.CreateVersion7(),
        Content = this.Content,
        Type = this.Type,
        Error = null,
        OccurredOnUtc = this.OccurredOnUtc,
        ParentId = this.Id,
        ProcessedOnUtc = null
    };

}