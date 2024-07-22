using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Domain.Entities.Participants;

public class Risk : BaseAuditableEntity<int>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Risk()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private readonly List<Note> _notes = new();

    public IReadOnlyCollection<Note> Notes => _notes.AsReadOnly();

    public string ParticipantId { get; private set; }
    public RiskLevel? RiskToChildren { get; private set; }
    public RiskLevel? RiskToPublic { get; private set; }
    public RiskLevel? RiskToKnownAdult { get; private set; }
    public RiskLevel? RiskToStaff { get; private set; }
    public RiskLevel? RiskToOtherPrisoners { get; private set; }
    public RiskLevel? RiskToSelf { get; private set; }

    public Risk AddNote(Note note)
    {
        if (_notes.Contains(note) is false)
        {
            _notes.Add(note);
        }

        return this;
    }

}
