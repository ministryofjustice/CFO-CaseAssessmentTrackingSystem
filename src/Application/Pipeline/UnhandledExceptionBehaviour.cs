using Cfo.Cats.Domain.Common.Exceptions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Cfo.Cats.Application.Pipeline;

public class UnhandledExceptionBehaviour<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<TRequest> _logger;

    public UnhandledExceptionBehaviour(
        ILogger<TRequest> logger,
        ICurrentUserService currentUserService
    )
    {
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        try
        {
            return await next(cancellationToken);
        }
        catch (Exception de) when(de is DomainException or ServerException)
        {
            var requestName = typeof(TRequest).Name;
            var userName = _currentUserService.UserName;
            _logger.LogError(
                de,
                "{Name}: {Exception} with {@Request} by {@UserName}",
                requestName,
                de.Message,
                request,
                userName
            );

            if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                var resultType = typeof(TResponse).GetGenericArguments()[0];
                var invalidResultType = typeof(Result<>).MakeGenericType(resultType);
                var invalidResult = Activator.CreateInstance(invalidResultType);
                var invalidResultMethod = invalidResultType.GetMethod("Failure", [typeof(string[])]);
                return (TResponse)invalidResultMethod!.Invoke(invalidResult, [de.Message])!;
            }

            if (typeof(TResponse).IsAssignableFrom(typeof(Result)))
            {
                return (TResponse)(object)Result.Failure(de.Message);
            }

            // if we get to here we are not a RESULT type, therefore we need to throw.
            throw;
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;
            var userName = _currentUserService.UserName;
            _logger.LogError(
                ex,
                "{Name}: {Exception} with {@Request} by {@UserName}",
                requestName,
                ex.Message,
                request,
                userName
            );
            
            if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                var resultType = typeof(TResponse).GetGenericArguments()[0];
                var invalidResultType = typeof(Result<>).MakeGenericType(resultType);
                var invalidResult = Activator.CreateInstance(invalidResultType);
                var invalidResultMethod = invalidResultType.GetMethod("Failure", [typeof(string[])]);
                return (TResponse)invalidResultMethod!.Invoke(invalidResult, ["Unexpected error"])!;
            }

            if (typeof(TResponse).IsAssignableFrom(typeof(Result)))
            {
                return (TResponse)(object)Result.Failure("Unexpected error");
            }

            throw;
        }
    }
}
