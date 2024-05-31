namespace Cfo.Cats.Application.Pipeline.PreProcessors;

public sealed class ValidationPreProcessor<TRequest> : IRequestPreProcessor<TRequest>
    where TRequest : notnull
{
    private readonly IReadOnlyCollection<IValidator<TRequest>> validators;

    public ValidationPreProcessor(IEnumerable<IValidator<TRequest>> validators)
    {
        this.validators =
            validators.ToList() ?? throw new ArgumentNullException(nameof(validators));
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        if (validators.Any() == false)
        {
            return;
        }

        var validationContext = new ValidationContext<TRequest>(request);

        var failures = await validators.ValidateAsync(validationContext, cancellationToken);

        if (failures.Any())
        {
            throw new ValidationException(failures);
        }
    }
}
