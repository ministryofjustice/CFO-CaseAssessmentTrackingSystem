using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Domain.Entities.Activities;

public class ActivityQueueEntryNote : Note
{
    public bool IsExternal { get; set; }
    public Cfo.Cats.Domain.Common.Enums.FeedbackType? FeedbackType { get; set; }
}