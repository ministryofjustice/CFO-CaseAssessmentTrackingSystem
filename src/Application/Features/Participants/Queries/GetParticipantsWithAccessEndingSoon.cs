using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetParticipantsWithAccessEndingSoon
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Query : IRequest<IEnumerable<ParticipantWithAccessEndingSoonDto>>
    {
        /// <summary>
        ///     Participant Owner. Current user will be used if not provided.
        /// </summary>
        public string? OwnerId { get; set; }
    }

    // Todo: move this dto
    public record ParticipantWithAccessEndingSoonDto
    {
        public required string ParticipantId { get; set; }
        public required string FullName { get; set; }
        public required DateTime LostAccessOn { get; set; }
        public DateTime HasAccessTo => LostAccessOn.AddDays(90);
    }

    private class Handler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser
    ) : IRequestHandler<Query, IEnumerable<ParticipantWithAccessEndingSoonDto>>
    {
        public async Task<IEnumerable<ParticipantWithAccessEndingSoonDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var lastNinetyDays = DateTime.UtcNow.AddDays(-90);
            var ownerId = request.OwnerId ?? currentUser.UserId!;

            FormattableString sql = $"""
                                     SELECT 
                                         [Id] as ParticipantId, 
                                         [To] as [LostAccessOn], 
                                         CONCAT(Participant.FirstName, ' ', Participant.LastName) as [FullName]  
                                     FROM 
                                         [Participant].[Participant]	
                                     INNER JOIN
                                     (
                                         -- They were assigned to me, but unassigned within the last x days
                                         SELECT 
                                             [oh].[ParticipantId], 
                                             MAX([oh].[To]) as [To] 
                                         FROM 
                                             [Participant].[OwnershipHistory] as [oh]
                                         WHERE 
                                             [oh].[OwnerId] = {ownerId} and [oh].[To] > {lastNinetyDays}
                                         GROUP BY [oh].[ParticipantId] 
                                             ) AS [LostAccessTo] on Participant.Id = LostAccessTo.ParticipantId
                                     WHERE
                                         -- who is not currently owned by me
                                         COALESCE( OwnerId, '' ) <> {ownerId}
                                         -- AND IS NOT IN AN AREA I CAN SEE
                                         AND CurrentLocationId NOT IN (
                                             SELECT 
                                                 tl.LocationId 
                                             FROM 
                                                 [Identity].[User] as [u]
                                             INNER JOIN
                                                 [Configuration].TenantLocation as [tl]
                                                     on [u].TenantId = [tl].TenantId
                                             WHERE 
                                                 [u].Id = {ownerId}
                                         );
                                     """;


            var results = await unitOfWork.DbContext.Database
                .SqlQuery<ParticipantWithAccessEndingSoonDto>(sql)
                .ToArrayAsync(cancellationToken);

            return results;
        }
    }

    public class Validator : AbstractValidator<Query>
    {
    }
}
