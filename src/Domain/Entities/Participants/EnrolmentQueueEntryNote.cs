using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Domain.Entities.Participants;

public class EnrolmentQueueEntryNote : Note, IShallowAuditable
{
    public bool IsExternal { get; set; }
    public Cfo.Cats.Domain.Common.Enums.FeedbackType? FeedbackType { get; set; }
    public string? ReturnReason { get; set; }
}
