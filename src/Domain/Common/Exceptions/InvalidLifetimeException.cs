namespace Cfo.Cats.Domain.Common.Exceptions;

public class InvalidLifetimeException() : Exception("The given start and end dates do not make a valid lifetime");