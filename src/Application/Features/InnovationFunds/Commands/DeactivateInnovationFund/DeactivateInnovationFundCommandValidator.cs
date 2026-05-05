using Cfo.Cats.Application.Common.Validators;

namespace Cfo.Cats.Application.Features.InnovationFunds.Commands.DeactivateInnovationFund;

public class DeactivateInnovationFundCommandValidator : AbstractValidator<DeactivateInnovationFundCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateInnovationFundCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.Id)
            .NotEmpty();

        RuleSet(ValidationConstants.RuleSet.MediatR, () =>
        {
            RuleFor(x => x.Id)
                .MustAsync(NotBeAlreadyEnded)
                .WithMessage("This Innovation Fund has already ended and cannot be deactivated.");
        });
    }

    private async Task<bool> NotBeAlreadyEnded(Guid id, CancellationToken cancellationToken)
        => await _unitOfWork.DbContext.InnovationFunds.AnyAsync(
            x => x.Id == id && x.Lifetime.EndDate > DateTime.UtcNow, cancellationToken);
}
