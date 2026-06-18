namespace Cfo.Cats.Application.Features.Participants;

/// <summary>
/// Determines whether a user has access to archive or unarchive a participant.
/// Access is granted when the user is the current owner of the participant, or
/// when the participant's owner sits within the user's tenant branch (i.e. the
/// owner's TenantId starts with the user's TenantId, meaning the user is at the
/// same or a higher level in the tenant hierarchy).
/// </summary>
public static class ParticipantArchiveAccess
{
    public static bool CanAccess(
        string? currentUserId,
        string? currentUserTenantId,
        string? participantOwnerId,
        string? participantOwnerTenantId)
    {
        if (string.IsNullOrEmpty(currentUserId) || string.IsNullOrEmpty(currentUserTenantId))
        {
            return false;
        }

        if (currentUserId == participantOwnerId)
        {
            return true;
        }

        if (!string.IsNullOrEmpty(participantOwnerTenantId)
            && participantOwnerTenantId.StartsWith(currentUserTenantId, StringComparison.Ordinal))
        {
            return true;
        }

        return false;
    }
}
