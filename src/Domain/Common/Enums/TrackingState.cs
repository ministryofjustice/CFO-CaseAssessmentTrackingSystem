namespace Cfo.Cats.Domain.Common.Enums;

public enum TrackingState
{
    /// <summary>Existing entity that has not been modified.</summary>
    Unchanged,

    /// <summary>Newly created entity.</summary>
    Added,

    /// <summary>Existing entity that has been modified.</summary>
    Modified,

    /// <summary>Existing entity that has been marked as deleted.</summary>
    Deleted
}