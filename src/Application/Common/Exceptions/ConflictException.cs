using System.Net;

namespace Cfo.Cats.Application.Common.Exceptions;

public class ConflictException : ServerException
{
    public ConflictException(string message)
        : base(message, HttpStatusCode.Conflict) { }
}
