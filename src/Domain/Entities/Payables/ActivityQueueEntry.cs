using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Domain.Entities.Payables
{
    public abstract class ActivityQueueEntry : OwnerPropertyEntity<Guid>, IMustHaveTenant
    {
        private readonly List<ActivityQueueEntryNote> _notes = [];

        public bool IsAccepted { get; protected set; }
        public bool IsCompleted { get; protected set; }
        public Guid ActivityId { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private ActivityQueueEntry()
        {

        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.    

        protected ActivityQueueEntry(Guid activityId)
        {
            Id = Guid.CreateVersion7();
            ActivityId = activityId;
        }

        public virtual Activity? Activity { get; private set; }

        public virtual Tenant? Tenant { get; private set; }

        public virtual Participant? Participant { get; private set; }

        public IReadOnlyCollection<ActivityQueueEntryNote> Notes => _notes.AsReadOnly();

        public abstract ActivityQueueEntry Accept();

        public abstract ActivityQueueEntry Return();

        public ActivityQueueEntry AddNote(string? message, bool isExternal = false)
        {
            if (string.IsNullOrWhiteSpace(message) == false)
            {
                _notes.Add(new ActivityQueueEntryNote()
                {
                    TenantId = TenantId,
                    Message = message,
                    IsExternal = isExternal
                });
            }
            return this;
        }

        public string TenantId { get; set; } = default!;
    }
}