namespace Cfo.Cats.Domain.Entities;

public class ParticipantAccessAuditTrail
{
    private ParticipantAccessAuditTrail(string requestType, string participantId, string userId)
    {
        RequestType = requestType;
        ParticipantId = participantId;
        UserId = userId;
        AccessDate = DateTime.UtcNow;
    }

    public static ParticipantAccessAuditTrail Create(string requestType, string participantId, string userId) 
        => new ParticipantAccessAuditTrail(requestType, participantId, userId);

    public string RequestType {get ; private set;} 
    public string ParticipantId {get; private set; } 
    public string UserId {get; private set;} 
    public DateTime AccessDate {get; private set;} 



}