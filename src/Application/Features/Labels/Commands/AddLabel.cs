using System.ComponentModel.DataAnnotations;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Labels.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Labels;

namespace Cfo.Cats.Application.Features.Labels.Commands;

public static class AddLabel
{
    [RequestAuthorize(Policy = SecurityPolicies.ManageLabels)]
    public class Command : IRequest<Result>
    {
        [Display(Name = "Name", Description = "The display name of the label. Must be unique within a contract")]
        public required string Name { get; set; }

        [Display(Name="Description", Description = "A textual description of the label. Will appear as a tool tip")]
        public required string Description { get; set; }

        [Display(Name = "Scope", Description = "The scope for adding a label (system labels are not available for user selection)")]
        public required LabelScope Scope { get; set; }

        [Display(Name="Colour", Description = "The colour for the label. More pronounced on Filled labels.")]
        public required AppColour Colour { get; set; }
        
        [Display(Name = "Variant", Description = "Label display mode (Text/Outlined/Filled)")]
        public required AppVariant Variant { get; set; }

        [Display(Name ="Icon", Description = "The icon to display (or None)")]
        public AppIcon AppIcon { get; set; }

        [Display(Name="Contract", Description = "Option contract to limit user applicability and visibility")]
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
                request.Scope, 
                request.Colour, 
                request.Variant, 
                request.AppIcon,
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
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Description"));
        }
    }
}