namespace Cfo.Cats.Application.Pipeline.ValidationSpecifications;

/// <summary>
/// Runs the ordered access specifications for a participant. Shared by the access
/// validation pipeline and anything else (e.g. search) that needs the same decision.
/// </summary>
public static class ParticipantAccessEvaluator
{
    public static async Task<AccessGrant> EvaluateAsync(
        IEnumerable<AccessValidationSpecification> specifications,
        string participantId,
        CancellationToken cancellationToken,
        Action<AccessValidationSpecification, AccessGrant>? onEvaluated = null)
    {
        var grant = AccessGrant.NotGranted;

        foreach (var specification in specifications.OrderBy(s => s.Order))
        {
            grant = await specification.IsSatisfiedBy(participantId, grant, cancellationToken);
            onEvaluated?.Invoke(specification, grant);
        }

        return grant;
    }

    public static async Task<IReadOnlyDictionary<string, AccessGrant>> EvaluateManyAsync(
        IEnumerable<AccessValidationSpecification> specifications,
        IReadOnlyCollection<string> participantIds,
        CancellationToken cancellationToken)
    {
        var grants = participantIds
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToDictionary(id => id, _ => AccessGrant.NotGranted, StringComparer.OrdinalIgnoreCase);

        if (grants.Count == 0)
        {
            return grants;
        }

        foreach (var specification in specifications.OrderBy(s => s.Order))
        {
            await specification.EvaluateManyAsync(grants, cancellationToken);
        }

        return grants;
    }
}
