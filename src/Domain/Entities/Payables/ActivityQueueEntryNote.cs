using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Domain.Entities.Payables
{
    public class ActivityQueueEntryNote: Note
    {
        public bool IsExternal { get; set; }
    }
}