using System.Net;

namespace Cfo.Cats.Application.Common.Exceptions;

public class ForbiddenException : ServerException
{
    public ForbiddenException(string message)
        : base(message, HttpStatusCode.Forbidden) { }
}
