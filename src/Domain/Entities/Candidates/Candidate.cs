using System.ComponentModel.DataAnnotations.Schema;
using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Domain.Entities.Candidates;

public class Candidate : BaseAuditableEntity<string>
{
    private Candidate()
    {
    }

    private List<CandidateIdentifier> _identifiers = new();
    
    public string FirstName { get; private set; }
    
    public string? MiddleName { get; private set; }
    public string LastName { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public int CurrentLocationId { get; private set; }

    public Location CurrentLocation { get; private set; } 

    public Participant? Participant { get; private set; }

    
    public IReadOnlyCollection<CandidateIdentifier> Identifiers => _identifiers.AsReadOnly();

    public void AddIdentifier(CandidateIdentifier identifier)
    {
        if (_identifiers.Any(id => id.Equals(identifier)))
        {
            throw new InvalidOperationException("The identifier already exists for this candidate.");
        }

        _identifiers.Add(identifier);
    }
    
    public void RemoveIdentifier(CandidateIdentifier identifier)
    {
        if (!_identifiers.Remove(identifier))
        {
            throw new InvalidOperationException("The identifier does not exist for this candidate.");
        }
    }
    
}

