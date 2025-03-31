using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.Bios
{
    public class ParticipantBio : BaseAuditableEntity<Guid>, IAuditTrial
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private ParticipantBio()
        {
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public string ParticipantId { get; private set; }
        public DateTime? Completed { get; private set; }
        public string? CompletedBy { get; private set; }
        public string BioJson { get; private set; }
        public BioStatus Status { get; private set; } = BioStatus.NotStarted;
        private ParticipantBio(Guid id, string participantId, string bioJson, BioStatus status)
        {
            Id = id;
            ParticipantId = participantId;
            BioJson = bioJson;
            Status = status;
            if (status == BioStatus.SkippedForNow)
            {
                AddDomainEvent(new BioSkippedDomainEvent(this));
            }
            else if (status == BioStatus.InProgress)
            {
                AddDomainEvent(new BioCreatedDomainEvent(this));
            }
        }

        public ParticipantBio UpdateJson(string json)
        {
            BioJson = json;
            return this;
        }

        public ParticipantBio Submit(string completedBy)
        {
            Completed = DateTime.Now;
            CompletedBy = completedBy;
            AddDomainEvent(new BioSubmittedDomainEvent(this));
            return this;
        }

        public static ParticipantBio Create(Guid id, string participantId, string bioJson)
        {
            return new ParticipantBio(id, participantId, bioJson, BioStatus.InProgress);
        }

        public static ParticipantBio Skip(Guid id, string participantId, string bioJson)
        {
            return new ParticipantBio(id, participantId, bioJson, BioStatus.SkippedForNow);
        }

        public ParticipantBio UpdateStatus(BioStatus status)
        {
            if(Status != BioStatus.Complete)
            {
                Status = status;
            }
            return this;
        }
    }
}