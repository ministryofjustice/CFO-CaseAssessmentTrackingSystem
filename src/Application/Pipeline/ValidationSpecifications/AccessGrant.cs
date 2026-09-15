namespace Cfo.Cats.Application.Pipeline.ValidationSpecifications;

public enum AccessGrant
{
    /// <summary>
    /// This indicates the specification does not grant access.
    ///
    /// The user may be granted access via a difference specificaiton
    /// </summary>
    NotGranted = 0,

    /// <summary>
    /// Indicates the strategy grants access to a record.
    /// </summary>
    Granted = 1,

    /// <summary>
    /// Indicates the user is denied access to this record. This overrides any allowed grant
    /// </summary>
    Denied = 2
}
