using Cfo.Cats.Application.Common.Validators;
using FluentValidation.Internal;

namespace Cfo.Cats.Application.Pipeline;

public abstract class ValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    where TRequest : class
{
    protected async Task<TResponse> HandleCore(
        TRequest request,
        Func<Task<TResponse>> next,
        CancellationToken cancellationToken)
    {
        var span = SentrySdk.GetSpan()
            ?.StartChild("validation", "FluentValidation checks");

        try
        {
            var context = new ValidationContext<TRequest>(
                request,
                new PropertyChain(),
                new RulesetValidatorSelector([
                    ValidationConstants.RuleSet.Default,
                    ValidationConstants.RuleSet.Mediator
                ]));

            var failures = new List<FluentValidation.Results.ValidationFailure>();

            foreach (var validator in validators.OrderBy(c => c.GetType().Name))
            {
                var result = await validator.ValidateAsync(context, cancellationToken);
                if (result.Errors.Any())
                {
                    failures.AddRange(result.Errors);
                    break;
                }
            }

            if (failures.Any())
            {
                var errors = failures.Select(f => f.ErrorMessage).ToArray();

                if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
                {
                    var resultType = typeof(TResponse).GetGenericArguments()[0];
                    var invalidResultType = typeof(Result<>).MakeGenericType(resultType);
                    var invalidResultMethod = invalidResultType.GetMethod("Failure", [typeof(string[])]);
                    return (TResponse)invalidResultMethod!.Invoke(null, [errors])!;
                }

                return (TResponse)(object)Result.Failure(errors);
            }

            return await next();
        }
        finally
        {
            span?.Finish();
        }
    }
}

public sealed class CommandValidationBehaviour<TCommand, TResponse>(IEnumerable<IValidator<TCommand>> validators)
    : ValidationBehaviour<TCommand, TResponse>(validators),
        ICommandPipelineBehavior<TCommand, TResponse>
    where TCommand : class, ICommand<TResponse>
{
    public Task<TResponse> Handle(
        TCommand command,
        CommandHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
        => HandleCore(command, () => next(), cancellationToken);
}

public sealed class QueryValidationBehaviour<TQuery, TResponse>(IEnumerable<IValidator<TQuery>> validators)
    : ValidationBehaviour<TQuery, TResponse>(validators),
        IQueryPipelineBehavior<TQuery, TResponse>
    where TQuery : class, IQuery<TResponse>
{
    public Task<TResponse> Handle(
        TQuery query,
        QueryHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
        => HandleCore(query, () => next(), cancellationToken);
}
