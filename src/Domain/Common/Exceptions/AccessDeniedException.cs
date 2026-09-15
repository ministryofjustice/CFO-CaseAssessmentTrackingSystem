namespace Cfo.Cats.Domain.Common.Exceptions;

public class AccessDeniedException() 
    : DomainException("Access to the requested participant has not been granted.");
