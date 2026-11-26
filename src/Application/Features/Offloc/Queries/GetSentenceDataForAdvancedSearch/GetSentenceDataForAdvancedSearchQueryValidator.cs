using Cfo.Cats.Application.Common.Validators;

namespace Cfo.Cats.Application.Features.Offloc.Queries.GetSentenceDataForAdvancedSearch;

public class GetSentenceDataForAdvancedSearchQueryValidator : AbstractValidator<GetSentenceDataForAdvancedSearchQuery>
{
    public GetSentenceDataForAdvancedSearchQueryValidator()
    {
        RuleFor(x => x.NomisNumber)
            .NotEmpty()
            .WithMessage("Nomis Number is required")
            .MaximumLength(20);

        RuleFor(x => x.RecordIdentifier)
            .MaximumLength(ValidationConstants.ParticipantIdLength);

        RuleFor(x => x.Justification)
            .MaximumLength(ValidationConstants.PicklistTextLength)
            .WithMessage($"Search justification must be less than or equal to {ValidationConstants.PicklistTextLength} characters")
            .Matches(ValidationConstants.Notes)
            .WithMessage(string.Format(ValidationConstants.NotesMessage, "Search justification"))
            .When(x => !string.IsNullOrWhiteSpace(x.Justification));
    }
}
