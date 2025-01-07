using Cfo.Cats.Application.Outbox;

namespace Cfo.Cats.Application.Features.Outbox.DTOs;

[Description("Outbox Messages")]
public class OutboxMessageDto
{
    public Guid Id { get; set; }

    [Description("Type")]
    public string Type { get; set; } = default!;

    [Description("Content")]
    public string Content { get; set; } = default!;

    [Description("Occurred On")]
    public DateTime OccurredOnUtc { get; set; }

    [Description("Processed On")]
    public DateTime? ProcessedOnUtc { get; set; }

    [Description("Error")]
    public string? Error { get; set; }

    [Description]
    public Guid? ParentId { get;set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<OutboxMessage, OutboxMessageDto>();
        }
    }

}