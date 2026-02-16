namespace Cfo.Cats.Domain.Common.Exceptions;

public class PriNeverBeenInLocationException : DomainException
{
    /// <summary>
    /// Exception that is raised if an attempt is made to close a PRI objective
    /// when the participant has not been recorded as being in the specified release
    /// region.
    /// </summary>
    public PriNeverBeenInLocationException()
        : base("Task cannot be completed as notice of the participants transfer to the expected release region has not yet been received.")
    {
        
    }
}