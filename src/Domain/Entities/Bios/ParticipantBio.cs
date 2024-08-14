using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Events;
using System.Security.Cryptography;

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
        public string BioJson { get; private set; }
        public BioStatus Status { get; private set; } = BioStatus.NotStarted;
        private ParticipantBio(Guid id, string participantId, string bioJson, BioStatus status)
        {
            Id = id;
            ParticipantId = participantId;
            BioJson = bioJson;
            Status = status;
            AddDomainEvent(new BioCreatedDomainEvent(this));
        }

        public ParticipantBio UpdateJson(string json)
        {
            //TODO: Add events for update, and logic to stop locked assessments being updated
            this.BioJson = json;
            return this;
        }

        public ParticipantBio Submit()
        {
            // this does nothing except raise the event.
            AddDomainEvent(new BioCreatedDomainEvent(this));
            return this;
        }


        public static ParticipantBio Create(Guid id, string participantId, string bioJson, BioStatus status)
        {
            return new ParticipantBio(id, participantId, bioJson, status);
        }

        public ParticipantBio UpdateStatus(BioStatus status)
        {
            this.Status = status;
            return this;
        }
    }
}