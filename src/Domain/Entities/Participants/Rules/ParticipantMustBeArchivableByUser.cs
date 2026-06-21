using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.Entities.Participants.Rules;

public class ParticipantMustBeArchivableByUser(
    string? currentUserId,
    string? currentUserTenantId,
    string? participantOwnerId,
    string? participantOwnerTenantId) : IBusinessRule
{
    public string Message => "You are not authorized to archive this participant";

    public bool IsBroken()
    {
        if (string.IsNullOrEmpty(currentUserId) || string.IsNullOrEmpty(currentUserTenantId))
        {
            return true;
        }

        if (currentUserId == participantOwnerId)
        {
            return false;
        }

        return string.IsNullOrEmpty(participantOwnerTenantId)
               || participantOwnerTenantId.StartsWith(currentUserTenantId, StringComparison.Ordinal) is false;
    }
}