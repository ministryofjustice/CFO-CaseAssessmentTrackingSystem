using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Server.UI.Extensions;

public static class ParticipantNavigationExtensions
{
    /// <summary>
    /// Returns the workspace URL for a participant. Identified participants continue their
    /// enrolment; everyone else opens in the participant workspace.
    /// </summary>
    public static string GetWorkspaceUri(this ParticipantPaginationDto participant)
        => participant.EnrolmentStatus == EnrolmentStatus.IdentifiedStatus
            ? $"/pages/enrolments/{participant.Id}"
            : $"/pages/workspace/participants/{participant.Id}";
}
