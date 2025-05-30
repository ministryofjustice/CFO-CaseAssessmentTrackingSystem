using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Events;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Domain.Entities.Assessments
{
    public class ParticipantAssessment : OwnerPropertyEntity<Guid>, IMayHaveTenant, IAuditTrial
    {

 #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private ParticipantAssessment()
 
        {
        }
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        private readonly List<PathwayScore> _scores = new();

        public string ParticipantId {get; private set;}
        public string AssessmentJson {get; private set;}
        public DateTime? Completed { get; private set; }
        public string? CompletedBy { get; private set; }
        public int LocationId { get; private set; }

        public IReadOnlyCollection<PathwayScore> Scores => _scores.AsReadOnly();

        private ParticipantAssessment(Guid id, string participantId, string assessmentJson, string tenantId, int locationId)
        {
            Id = id;
            ParticipantId = participantId;
            AssessmentJson = assessmentJson;
            TenantId = tenantId;
            LocationId = locationId;
            AddDomainEvent(new AssessmentCreatedDomainEvent(this));
        }

        public ParticipantAssessment UpdateJson(string json)
        {
            //TODO: Add events for update, and logic to stop locked assessments being updated
            this.AssessmentJson = json;
            return this;
        }

        public ParticipantAssessment SetPathwayScore(string pathwayName, double score)
        {
            PathwayScore? pathwayScore = _scores.FirstOrDefault(p => p.Pathway == pathwayName );
            
            if(pathwayScore is not null)
            {
                // remove rather than update
                _scores.Remove(pathwayScore);
            }

            _scores.Add(new PathwayScore(pathwayName, score));
            return this;
        }

        public ParticipantAssessment Submit(string completedBy)
        {
            Completed = DateTime.UtcNow;
            CompletedBy = completedBy;
            AddDomainEvent(new AssessmentScoredDomainEvent(this));
            return this;
        }


        public static ParticipantAssessment Create(Guid id, string participantId, string assessmentJson, string tenantId, int locationId)
        {
            return new ParticipantAssessment(id, participantId, assessmentJson, tenantId, locationId);
        }

        public string? TenantId {get; set;}

        public bool IsCompleted => Completed is not null;
        
    }
}