using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Domain.Entities.Participants;

public class EnrolmentQueueEntryNote : Note
{
    public bool IsExternal { get; set; }
    public Cfo.Cats.Domain.Common.Enums.FeedbackType? FeedbackType { get; set; }
}
