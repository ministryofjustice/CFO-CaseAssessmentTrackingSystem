using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.Entities.Participants.Rules;

public class ParticipantMustBeArchivableByUser(
    string? currentUserId,
    string? currentUserTenantId,
    string? participantOwnerId,
    string? participantOwnerTenantId,
    bool hasActiveIncomingTransfer) : IBusinessRule
{
    public string Message => hasActiveIncomingTransfer
        ? "Participant cannot be archived while there is an active incoming transfer"
        : "You are not authorized to archive this participant";

    public bool IsBroken()
    {
        if (hasActiveIncomingTransfer)
        {
            return true;
        }

        if (string.IsNullOrEmpty(currentUserId) || string.IsNullOrEmpty(currentUserTenantId))
        {
            return true;
        }

        if (string.IsNullOrEmpty(participantOwnerId))
        {
            return false;
        }

        if (currentUserId == participantOwnerId)
        {
            return false;
        }

        return string.IsNullOrEmpty(participantOwnerTenantId)
               || participantOwnerTenantId.StartsWith(currentUserTenantId, StringComparison.Ordinal) is false;
    }
}