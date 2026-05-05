using Cfo.Cats.Application.Common.Validators;

namespace Cfo.Cats.Application.Features.Initiatives.Commands.EditInitiative;

public class EditInitiativeCommandValidator : AbstractValidator<EditInitiativeCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public EditInitiativeCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.Id)
            .NotEmpty();

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

        RuleSet(ValidationConstants.RuleSet.MediatR, () =>
        {
            RuleFor(x => x)
                .MustAsync(BeUniqueCode)
                .WithMessage("An Initiative with this code already exists.");
        });
    }

    private async Task<bool> BeUniqueCode(EditInitiativeCommand command, CancellationToken cancellationToken)
        => !await _unitOfWork.DbContext.Initiatives.AnyAsync(
            x => x.Code == command.Code && x.Id != command.Id, cancellationToken);
}
