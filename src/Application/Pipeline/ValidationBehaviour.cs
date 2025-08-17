using Cfo.Cats.Application.Common.Validators;
using FluentValidation.Internal;

namespace Cfo.Cats.Application.Pipeline;

public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{

    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request, new PropertyChain(), 
        new RulesetValidatorSelector([ValidationConstants.RuleSet.Default, 
            ValidationConstants.RuleSet.MediatR]));
            
        var failures = new List<FluentValidation.Results.ValidationFailure>();

        foreach (var validator in _validators.OrderBy(c => c.GetType().Name))
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
                var invalidResult = Activator.CreateInstance(invalidResultType);
                var invalidResultMethod = invalidResultType.GetMethod("Failure", [typeof(string[])]);
                return (TResponse)invalidResultMethod!.Invoke(invalidResult, [errors])!;
            }

            return (TResponse)(object)Result.Failure(errors);
        }

        return await next(cancellationToken);
    }
}