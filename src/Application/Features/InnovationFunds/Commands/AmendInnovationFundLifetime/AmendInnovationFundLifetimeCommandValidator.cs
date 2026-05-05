using Cfo.Cats.Application.Common.Validators;

namespace Cfo.Cats.Application.Features.InnovationFunds.Commands.AmendInnovationFundLifetime;

public class AmendInnovationFundLifetimeCommandValidator : AbstractValidator<AmendInnovationFundLifetimeCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public AmendInnovationFundLifetimeCommandValidator(IUnitOfWork unitOfWork)
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
                .WithMessage("Innovation Fund not found.");
        });
    }

    private async Task<bool> FundMustExist(Guid id, CancellationToken cancellationToken)
        => await _unitOfWork.DbContext.InnovationFunds.AnyAsync(x => x.Id == id, cancellationToken);
}
