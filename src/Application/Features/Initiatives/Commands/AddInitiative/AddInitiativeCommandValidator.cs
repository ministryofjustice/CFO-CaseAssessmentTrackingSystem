using Cfo.Cats.Application.Common.Validators;

namespace Cfo.Cats.Application.Features.Initiatives.Commands.AddInitiative;

public class AddInitiativeCommandValidator : AbstractValidator<AddInitiativeCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddInitiativeCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(8)
            .Matches(ValidationConstants.AlphabetsDigitsSpaceSlashHyphenDot)
            .WithMessage(string.Format(ValidationConstants.AlphabetsDigitsSpaceSlashHyphenDotMessage, "Code"));

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(256)
            .Matches(ValidationConstants.Notes)
            .WithMessage(string.Format(ValidationConstants.NotesMessage, "Description"));

        RuleFor(x => x.Contract)
            .NotNull()
            .WithMessage("A contract must be selected.");

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
            RuleFor(x => x.Code)
                .MustAsync(BeUniqueCode)
                .WithMessage("An Initiative with this code already exists.");
        });
    }

    private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
        => !await _unitOfWork.DbContext.Initiatives.AnyAsync(x => x.Code == code, cancellationToken);
}
