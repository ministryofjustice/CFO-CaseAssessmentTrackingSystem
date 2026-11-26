using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Candidates.Queries.AdvancedSearch;
using Cfo.Cats.Domain.Entities.ManagementInformation;

namespace Cfo.Cats.Infrastructure.Services;

public class PrivilegedSearchAuditWriter(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IPrivilegedSearchAuditWriter
{
    private const int UsageEventContextMaxLength = 256;

    public async Task RecordAdvancedSearchAttemptAsync(
        UserProfile userProfile,
        string identifier,
        string justification,
        IEnumerable<string> catsParticipantIds,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        dbContext.UsageEvents.Add(UsageEvent.Create(
            area: "Search",
            activity: "ParticipantAdvancedSearch",
            userId: userProfile.UserId,
            userName: userProfile.UserName,
            tenantId: userProfile.TenantId,
            context: BuildBoundedContext($"identifier={identifier.Trim().ToUpperInvariant()} | justification={justification.Trim()}"),
            occurredOn: DateTime.UtcNow));

        var userId = string.IsNullOrWhiteSpace(userProfile.UserId) ? Guid.Empty.ToString() : userProfile.UserId;
        foreach (var participantId in catsParticipantIds
                     .Where(x => !string.IsNullOrWhiteSpace(x))
                     .Select(x => x.Trim().ToUpperInvariant())
                     .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            dbContext.AccessAuditTrails.Add(ParticipantAccessAuditTrail.Create(
                nameof(ParticipantAdvancedSearchQuery),
                participantId,
                userId));
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RecordAdvancedSearchSentenceLookupAttemptAsync(
        UserProfile userProfile,
        string recordIdentifier,
        string nomisNumber,
        string justification,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        dbContext.UsageEvents.Add(UsageEvent.Create(
            area: "Search",
            activity: "ParticipantAdvancedSearchSentenceLookup",
            userId: userProfile.UserId,
            userName: userProfile.UserName,
            tenantId: userProfile.TenantId,
            context: BuildBoundedContext(
                $"record={recordIdentifier.Trim().ToUpperInvariant()} | nomis={nomisNumber.Trim().ToUpperInvariant()} | justification={justification.Trim()}"),
            occurredOn: DateTime.UtcNow));

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string BuildBoundedContext(string context)
        => context.Length <= UsageEventContextMaxLength ? context : context[..UsageEventContextMaxLength];
}
