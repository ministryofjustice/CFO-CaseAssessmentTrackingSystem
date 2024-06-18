using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Entities.Documents;

namespace Cfo.Cats.Domain.Entities.Participants;

public class Consent : BaseAuditableEntity<int>
{
    private string _participantId;
    private Guid _documentId;
    
    
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Consent()
    {
        
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private Consent(string participantId, DateTime consentDate, Guid documentId)
    {
        ConsentDate = consentDate;
        _participantId = participantId;
        _documentId = documentId;
    }

    public DateTime ConsentDate { get; private set; }
    
    public Document? Document { get; private set; }

    public static Consent Create(string participantId, DateTime consentDate, Guid documentId) 
        => new(participantId, consentDate, documentId);

}
