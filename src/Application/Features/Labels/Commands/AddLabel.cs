using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Labels.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Labels;

namespace Cfo.Cats.Application.Features.Labels.Commands;

public static class AddLabel
{
    [RequestAuthorize(Policy = SecurityPolicies.UserHasAdditionalRoles)]
    public class Command : IRequest<Result>
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required AppColour Colour { get; set; }
        
        public required AppVariant Variant { get; set; }
        public string? ContractId { get; set; }

        
    }

    public class Handler(
        ILabelRepository repository, 
        ILabelCounter labelCounter) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(
            Command request, 
            CancellationToken cancellationToken)
        {
            var l = Label.Create(
                request.Name, 
                request.Description, 
                request.Colour, 
                request.Variant, 
                request.ContractId, 
                labelCounter);
            await repository.AddAsync(l);
            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MinimumLength(LabelConstants.NameMinimumLength)
                .MaximumLength(LabelConstants.NameMaximumLength);

            RuleFor(x => x.Description)
                .MinimumLength(LabelConstants.DescriptionMinimumLength)
                .MaximumLength(LabelConstants.DescriptionMaximumLength);
            
            RuleFor(v => v.Name)
                .Matches(ValidationConstants.LettersSpacesUnderscores)
                .WithMessage(string.Format(ValidationConstants.LettersSpacesUnderscoresMessage, "Name"));
            
            RuleFor(v => v.Description)
                .Matches(ValidationConstants.LettersSpacesUnderscores)
                .WithMessage(string.Format(ValidationConstants.LettersSpacesUnderscoresMessage, "Description"));
        }
    }
}