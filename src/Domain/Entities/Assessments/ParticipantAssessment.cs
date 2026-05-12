using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Events;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Domain.Entities.Assessments;

public class ParticipantAssessment : OwnerPropertyEntity<Guid>, IMayHaveTenant, IAuditTrial
{

 #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ParticipantAssessment()
 
    {
    }
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private readonly List<PathwayScore> _scores = new();
    private readonly List<AssessmentAnswer> _answers = new();

    public string ParticipantId {get; private set;}
    public DateTime? Completed { get; private set; }
    public string? CompletedBy { get; private set; }
    public int LocationId { get; private set; }

    public IReadOnlyCollection<PathwayScore> Scores => _scores.AsReadOnly();
    public IReadOnlyCollection<AssessmentAnswer> Answers => _answers.AsReadOnly();

    private ParticipantAssessment(Guid id, string participantId, string tenantId, int locationId)
    {
        Id = id;
        ParticipantId = participantId;
        TenantId = tenantId;
        LocationId = locationId;
        AddDomainEvent(new AssessmentCreatedDomainEvent(this));
    }

    public ParticipantAssessment SetAnswer(string questionCode, string answer)
    {
        _answers.RemoveAll(a => a.QuestionCode == questionCode);
        _answers.Add(new AssessmentAnswer(questionCode, answer));
        return this;
    }

    public ParticipantAssessment SetAnswers(string questionCode, IEnumerable<string> answers)
    {
        _answers.RemoveAll(a => a.QuestionCode == questionCode);
        foreach (var answer in answers)
        {
            _answers.Add(new AssessmentAnswer(questionCode, answer));
        }
        return this;
    }

    public string? GetAnswer(string questionCode)
        => _answers.FirstOrDefault(a => a.QuestionCode == questionCode)?.Answer;

    public IEnumerable<string> GetAnswers(string questionCode)
        => _answers.Where(a => a.QuestionCode == questionCode).Select(a => a.Answer);

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

    public static ParticipantAssessment Create(Guid id, string participantId, string tenantId, int locationId) 
        => new(id, participantId, tenantId, locationId);

    public string? TenantId {get; set;}

    public bool IsCompleted => Completed is not null;
        
}