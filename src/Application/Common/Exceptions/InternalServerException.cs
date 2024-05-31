namespace Cfo.Cats.Application.Common.Exceptions;

public class InternalServerException : ServerException
{
    public InternalServerException(string message)
        : base(message) { }
}
