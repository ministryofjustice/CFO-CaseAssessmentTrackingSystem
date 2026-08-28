using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Domain.Entities.Activities;

public class ActivityQueueEntryNote : Note, IShallowAuditable
{
    public bool IsExternal { get; set; }
    public Cfo.Cats.Domain.Common.Enums.FeedbackType? FeedbackType { get; set; }
    public string? ReturnReason { get; set; }
}