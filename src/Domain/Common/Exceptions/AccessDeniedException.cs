namespace Cfo.Cats.Domain.Common.Exceptions;

public class AccessDeniedException() 
    : DomainException(ExceptionMessage)
{
    public const string ExceptionMessage = "Access to the requested participant has not been granted.";
}
