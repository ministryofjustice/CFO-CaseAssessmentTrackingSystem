using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Domain.Entities.Participants;

public class EnrolmentQueueEntryNote : Note
{
    public bool IsExternal { get; set; }
}
