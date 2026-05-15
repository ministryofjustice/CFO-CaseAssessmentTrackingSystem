using Cfo.Cats.Application.Common.Validators;

namespace Cfo.Cats.Application.Features.Initiatives.Commands.AmendInitiativeLifetime;

public class AmendInitiativeLifetimeCommandValidator : AbstractValidator<AmendInitiativeLifetimeCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public AmendInitiativeLifetimeCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.StartDate)
            .NotNull()
            .WithMessage("Start date is required.");

        RuleFor(x => x.EndDate)
            .NotNull()
            .WithMessage("End date is required.")
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("End date must be on or after start date.");

        RuleSet(ValidationConstants.RuleSet.MediatR, () =>
        {
            RuleFor(x => x.Id)
                .MustAsync(FundMustExist)
                .WithMessage("Initiative not found.");
        });
    }

    private async Task<bool> FundMustExist(Guid id, CancellationToken cancellationToken)
        => await _unitOfWork.DbContext.Initiatives.AnyAsync(x => x.Id == id, cancellationToken);
}
