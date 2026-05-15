using Cfo.Cats.Application.Common.Validators;

namespace Cfo.Cats.Application.Features.Initiatives.Commands.DeactivateInitiative;

public class DeactivateInitiativeCommandValidator : AbstractValidator<DeactivateInitiativeCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateInitiativeCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.Id)
            .NotEmpty();

        RuleSet(ValidationConstants.RuleSet.MediatR, () =>
        {
            RuleFor(x => x.Id)
                .MustAsync(NotBeAlreadyEnded)
                .WithMessage("This Initiative has already ended and cannot be deactivated.");
        });
    }

    private async Task<bool> NotBeAlreadyEnded(Guid id, CancellationToken cancellationToken)
        => await _unitOfWork.DbContext.Initiatives.AnyAsync(
            x => x.Id == id && x.Lifetime.EndDate > DateTime.UtcNow, cancellationToken);
}
