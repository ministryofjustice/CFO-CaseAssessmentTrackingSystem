namespace Cfo.Cats.Application.Pipeline.ValidationSpecifications;

public abstract class AccessValidationSpecification
{
    /// <summary>
    /// Checks whether the specification is passed.
    /// </summary>
    /// <param name="identifier">The participant Id</param>
    /// <param name="previousGrant">The current grant state. Used to short curcuit some checks</param>
    /// <returns>
    ///     If true => Granted
    ///     If false => NotGranted
    ///     If Explicitly blocked => Denied
    /// </returns>
    public async Task<AccessGrant> IsSatisfiedBy(
        string identifier,
        AccessGrant previousGrant)
    {
        if(previousGrant == AccessGrant.Denied)
        {
            return AccessGrant.Denied;
        }

        if(CanDenyAccess is false && previousGrant == AccessGrant.Granted)
        {
            return AccessGrant.Granted;
        }

        return await CheckAccessAsync(identifier);
    }

    /// <summary>
    /// Indicates this specification can explicitly deny access, not just grant access.
    /// 
    /// When true, the rule will be checked even if other strategies have granted access.
    /// </summary>
    protected virtual bool CanDenyAccess => false;

    protected abstract Task<AccessGrant> CheckAccessAsync(string identifier);

    /// <summary>
    /// Order weighting. The higher the number the later the call (keep cheap specifications lower to run them first)
    /// </summary>
    public abstract int Order { get; } 
    
}