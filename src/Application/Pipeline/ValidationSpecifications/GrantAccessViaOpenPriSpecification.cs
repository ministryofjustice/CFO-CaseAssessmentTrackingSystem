namespace Cfo.Cats.Application.Pipeline.ValidationSpecifications;

public class GrantAccessViaOpenPriSpecification(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : AccessValidationSpecification
{
    public override int Order => 1;

    protected override async Task<AccessGrant> CheckAccessAsync(string identifier, CancellationToken cancellationToken)
    {
        var pri = await unitOfWork.DbContext.PRIs
                    .Where(p => p.ParticipantId == identifier)
                    .Where(p => p.CompletedOn == null)
                    .Select(p => p.AssignedTo)
                    .FirstOrDefaultAsync(cancellationToken);

        return pri switch
        {
            null => AccessGrant.NotGranted,
            var p when p == currentUserService.UserId => AccessGrant.Granted,
            _ => AccessGrant.NotGranted
        };
    }

    protected override async Task<IReadOnlyDictionary<string, AccessGrant>> CheckAccessManyAsync(
        IReadOnlyCollection<string> identifiers,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            return identifiers.ToDictionary(id => id, _ => AccessGrant.NotGranted, StringComparer.OrdinalIgnoreCase);
        }

        var ids = identifiers.ToArray();
        var grantedIds = (await unitOfWork.DbContext.PRIs
                .Where(p => ids.Contains(p.ParticipantId) && p.CompletedOn == null && p.AssignedTo == userId)
                .Select(p => p.ParticipantId)
                .Distinct()
                .ToListAsync(cancellationToken))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return identifiers.ToDictionary(
            id => id,
            id => grantedIds.Contains(id) ? AccessGrant.Granted : AccessGrant.NotGranted,
            StringComparer.OrdinalIgnoreCase);
    }
}