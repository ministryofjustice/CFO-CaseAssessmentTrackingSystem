using Cfo.Cats.Application.Common.Extensions;
using Cfo.Cats.Application.Common.Interfaces;
using FluentValidation;
using FluentValidation.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Cfo.Cats.Infrastructure.Services;

public class ValidationService : IValidationService
{
    private readonly IServiceProvider serviceProvider;

    public ValidationService(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue<TRequest>()
    {
        return async (model, propertyName) =>
            await ValidatePropertyAsync((TRequest)model, propertyName);
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue<TRequest>(TRequest _)
    {
        return ValidateValue<TRequest>();
    }

    public async Task<IDictionary<string, string[]>> ValidateAsync<TRequest>(
        TRequest model,
        CancellationToken cancellationToken = default
    )
    {
        var validators = serviceProvider.GetServices<IValidator<TRequest>>();

        var context = new ValidationContext<TRequest>(model);

        return (await validators.ValidateAsync(context, cancellationToken)).ToDictionary();
    }

    public async Task<IDictionary<string, string[]>> ValidateAsync<TRequest>(
        TRequest model,
        Action<ValidationStrategy<TRequest>> options,
        CancellationToken cancellationToken = default
    )
    {
        var validators = serviceProvider.GetServices<IValidator<TRequest>>();

        var context = ValidationContext<TRequest>.CreateWithOptions(model, options);

        return (await validators.ValidateAsync(context, cancellationToken)).ToDictionary();
    }

    public async Task<IEnumerable<string>> ValidatePropertyAsync<TRequest>(
        TRequest model,
        string propertyName,
        CancellationToken cancellationToken = default
    )
    {
        var validationResult = await ValidateAsync(
            model,
            options =>
            {
                options.IncludeProperties(propertyName);
            },
            cancellationToken
        );

        return validationResult.Where(x => x.Key == propertyName).SelectMany(x => x.Value);
    }
}
