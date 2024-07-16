using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.Bios
{

    public class Bio : OwnerPropertyEntity<Guid>, IMayHaveTenant, IAuditTrial
    {

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private Bio()
        {
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.



        public string ParticipantId { get; private set; }
        public string BioJson { get; private set; }

        private Bio(Guid id, string participantId, string bioJson, string tenantId)
        {
            Id = id;
            ParticipantId = participantId;
            BioJson = bioJson;
            TenantId = tenantId;
            AddDomainEvent(new BioCreatedDomainEvent(this));
        }

        public Bio UpdateJson(string json)
        {
            //TODO: Add events for update, and logic to stop locked assessments being updated
            this.BioJson = json;
            return this;
        }

        public Bio Submit()
        {
            // this does nothing except raise the event.
            AddDomainEvent(new BioCreatedDomainEvent(this));
            return this;
        }


        public static Bio Create(Guid id, string participantId, string bioJson, string tenantId)
        {
            return new Bio(id, participantId, bioJson, tenantId);
        }

        public string? TenantId { get; set; }

    }
}