using Cfo.Cats.Application.Common.Validators;

namespace Cfo.Cats.Application.Features.KeyValues.Commands.AddEdit;

public class AddEditKeyValueCommandValidator : AbstractValidator<AddEditKeyValueCommand>
{
    public AddEditKeyValueCommandValidator()
    {
        RuleFor(c => c.Name)
                        .NotNull()
                        .NotEmpty()
                        .WithMessage("Name is required");

        RuleFor(c => c.Name)
            .IsInEnum()
            .WithMessage("Invalid option, must select an option from the picklist");

        RuleFor(c => c.Value!)
            .NotNull()
            .NotEmpty()
            .WithMessage("Value is required");

        RuleFor(c => c.Value!)
            .MinimumLength(2)
            .MaximumLength(100)
            .WithMessage("Value must be between 2 and 100 characters long")
            .Matches(ValidationConstants.Keyword)
            .WithMessage(string.Format(ValidationConstants.KeywordMessage, "Value"));

        RuleFor(c => c.Text!)
            .NotNull()
            .NotEmpty()
            .WithMessage("Text is required");

        RuleFor(c => c.Text!)
            .MinimumLength(2)
            .MaximumLength(100)
            .WithMessage("Text must be between 2 and 100 characters long")
            .Matches(ValidationConstants.Keyword)
            .WithMessage(string.Format(ValidationConstants.KeywordMessage, "Text"));

        RuleFor(c => c.Description!)
            .MinimumLength(0)
            .MaximumLength(100)
            .WithMessage("Description must be less or equal to 100 characters long")
            .Matches(ValidationConstants.Keyword)
            .WithMessage(string.Format(ValidationConstants.KeywordMessage, "Description"));
    }
}
