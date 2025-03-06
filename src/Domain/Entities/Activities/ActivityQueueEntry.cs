using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Domain.Entities.Activities;

public abstract class ActivityQueueEntry : OwnerPropertyEntity<Guid>
{
    private readonly List<ActivityQueueEntryNote> _notes = [];

    public bool IsAccepted { get; protected set; }
    public bool IsCompleted { get; protected set; }
    public Guid ActivityId { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected ActivityQueueEntry()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.    

    protected ActivityQueueEntry(Guid activityId, string tenantId, string supportWorkerId, DateTime originalPQASubmissionDate,string participantId)
    {
        Id = Guid.CreateVersion7();
        ActivityId = activityId;
        ParticipantId = participantId;
        TenantId = tenantId;
        SupportWorkerId = supportWorkerId;
        OriginalPQASubmissionDate = originalPQASubmissionDate;
    }

    public virtual Activity? Activity { get; private set; }

    public virtual Tenant? Tenant { get; private set; }

    public virtual Participant? Participant { get; protected set; }

    public string? ParticipantId { get; set; }

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

    public string SupportWorkerId { get; private set; }

    public DateTime OriginalPQASubmissionDate { get; private set; }
}