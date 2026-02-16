namespace Cfo.Cats.Domain.Common.Exceptions;

public class PriMissingActualReleaseDateException : DomainException
{
    /// <summary>
    /// Exception that is raised if an attempt is made to close a PRI objective
    /// when the actual release date has not been set.
    /// </summary>
    public PriMissingActualReleaseDateException()
        : base("Task cannot be completed until the actual release date is set.")
    {
        
    }
}
