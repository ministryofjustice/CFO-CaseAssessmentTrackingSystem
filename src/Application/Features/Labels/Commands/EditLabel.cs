using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Labels.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Labels;

namespace Cfo.Cats.Application.Features.Labels.Commands;

public static class EditLabel
{
    [RequestAuthorize(Policy = SecurityPolicies.UserHasAdditionalRoles)]
    public class Command : IRequest<Result>
    {
        public required LabelId LabelId { get; set; }
        public required string NewName { get; set; }
        public required string NewDescription { get; set; }
        public AppColour NewColour { get; set; }
        public AppVariant NewVariant { get; set; }
        
    }

    public class Handler(ILabelRepository repository) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var label = await repository.GetByIdAsync(request.LabelId);

            if (label == null)
            {
                throw new NotFoundException("Label does not exist");
            }
            
            label.Edit(request.NewName, request.NewDescription, request.NewColour, request.NewVariant);
            
            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.NewName)
                .NotEmpty()
                .MinimumLength(LabelConstants.NameMinimumLength)
                .MaximumLength(LabelConstants.NameMaximumLength);

            RuleFor(x => x.NewDescription)
                .NotEmpty()
                .MinimumLength(LabelConstants.DescriptionMinimumLength)
                .MaximumLength(LabelConstants.DescriptionMaximumLength);

            RuleFor(v => v.NewName)
                .Matches(ValidationConstants.LettersSpacesUnderscores)
                .WithMessage(string.Format(ValidationConstants.LettersSpacesUnderscoresMessage, "Name"));
            
            RuleFor(v => v.NewDescription)
                .Matches(ValidationConstants.LettersSpacesUnderscores)
                .WithMessage(string.Format(ValidationConstants.LettersSpacesUnderscoresMessage, "Description"));
        }
    }
}