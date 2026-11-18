namespace Cfo.Cats.Application.Pipeline.ValidationSpecifications;

public abstract class AccessValidationSpecification
{
    /// <summary>
    /// Checks whether the specification is passed.
    /// </summary>
    /// <param name="identifier">The participant Id</param>
    /// <param name="previousGrant">The current grant state. Used to short curcuit some checks</param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    ///     If true => Granted
    ///     If false => NotGranted
    ///     If Explicitly blocked => Denied
    /// </returns>
    public async Task<AccessGrant> IsSatisfiedBy(
        string identifier,
        AccessGrant previousGrant,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if(previousGrant == AccessGrant.Denied)
        {
            return AccessGrant.Denied;
        }

        if(CanDenyAccess is false && previousGrant == AccessGrant.Granted)
        {
            return AccessGrant.Granted;
        }

        return await CheckAccessAsync(identifier, cancellationToken);
    }

    public async Task EvaluateManyAsync(
        IDictionary<string, AccessGrant> grants,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var candidates = grants
            .Where(kvp => kvp.Value != AccessGrant.Denied && (CanDenyAccess || kvp.Value != AccessGrant.Granted))
            .Select(kvp => kvp.Key)
            .ToArray();

        if (candidates.Length == 0)
        {
            return;
        }

        var evaluated = await CheckAccessManyAsync(candidates, cancellationToken);
        foreach (var id in candidates)
        {
            if (evaluated.TryGetValue(id, out var grant))
            {
                grants[id] = grant;
            }
        }
    }

    /// <summary>
    /// Indicates this specification can explicitly deny access, not just grant access.
    /// 
    /// When true, the rule will be checked even if other strategies have granted access.
    /// </summary>
    protected virtual bool CanDenyAccess => false;

    protected abstract Task<AccessGrant> CheckAccessAsync(string identifier, CancellationToken cancellationToken);

    protected virtual async Task<IReadOnlyDictionary<string, AccessGrant>> CheckAccessManyAsync(
        IReadOnlyCollection<string> identifiers,
        CancellationToken cancellationToken)
    {
        var results = new Dictionary<string, AccessGrant>(identifiers.Count, StringComparer.OrdinalIgnoreCase);
        foreach (var id in identifiers)
        {
            results[id] = await CheckAccessAsync(id, cancellationToken);
        }

        return results;
    }

    /// <summary>
    /// Order weighting. The higher the number the later the call (keep cheap specifications lower to run them first)
    /// </summary>
    public abstract int Order { get; } 
    
}