using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Events;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Domain.Entities.Bios;

public class ParticipantBio : BaseAuditableEntity<Guid>, IAuditTrial
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ParticipantBio()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private readonly List<BioAnswer> _answers = new();

    public string ParticipantId { get; private set; }
    public DateTime? Completed { get; private set; }
    public string? CompletedBy { get; private set; }
    public BioStatus Status { get; private set; } = BioStatus.NotStarted;

    public IReadOnlyCollection<BioAnswer> Answers => _answers.AsReadOnly();

    private ParticipantBio(Guid id, string participantId, BioStatus status)
    {
        Id = id;
        ParticipantId = participantId;
        Status = status;          
        AddDomainEvent(new BioCreatedDomainEvent(this));          
    }

    public ParticipantBio SetAnswer(string questionCode, string answer)
    {
        _answers.RemoveAll(a => a.QuestionCode == questionCode);
        _answers.Add(new BioAnswer(questionCode, answer));
        return this;
    }

    public ParticipantBio SetAnswers(string questionCode, IEnumerable<string> answers)
    {
        _answers.RemoveAll(a => a.QuestionCode == questionCode);
        foreach (var answer in answers)
        {
            _answers.Add(new BioAnswer(questionCode, answer));
        }
        return this;
    }

    public string? GetAnswer(string questionCode)
        => _answers.FirstOrDefault(a => a.QuestionCode == questionCode)?.Answer;

    public IEnumerable<string> GetAnswers(string questionCode)
        => _answers.Where(a => a.QuestionCode == questionCode).Select(a => a.Answer);

    public ParticipantBio Submit(string completedBy)
    {
        Completed = DateTime.UtcNow;
        CompletedBy = completedBy;
        AddDomainEvent(new BioSubmittedDomainEvent(this));
        return this;
    }

    public static ParticipantBio Create(Guid id, string participantId) 
        => new(id, participantId, BioStatus.InProgress);

    public ParticipantBio UpdateStatus(BioStatus status)
    {
        if(Status != BioStatus.Complete)
        {
            Status = status;
        }
        return this;
    }

    public bool IsCompleted => Completed is not null;
}