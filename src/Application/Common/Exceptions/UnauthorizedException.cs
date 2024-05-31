using System.Net;

namespace Cfo.Cats.Application.Common.Exceptions;

public class UnauthorizedException : ServerException
{
    public UnauthorizedException(string message)
        : base(message, HttpStatusCode.Unauthorized) { }
}
