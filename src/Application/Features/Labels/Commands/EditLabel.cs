using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Labels.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Labels;

namespace Cfo.Cats.Application.Features.Labels.Commands;

public static class EditLabel
{
    [RequestAuthorize(Policy = SecurityPolicies.ManageLabels)]
    public class Command : IRequest<Result>
    {
        public required LabelId LabelId { get; set; }

        [Display(Name = "Name", Description = "The display name of the label. Must be unique within a contract")]
        public required string NewName { get; set; }

        [Display(Name="Description", Description = "A textual description of the label. Will appear as a tool tip")]
        public required string NewDescription { get; set; } 
        
        [Display(Name = "Scope", Description = "The scope for adding a label (system labels are not available for user selection)")]
        public required LabelScope NewScope { get; set; }
        
        [Display(Name="Colour", Description = "The colour for the label. More pronounced on Filled labels.")]
        public AppColour NewColour { get; set; }
        
        [Display(Name = "Variant", Description = "Label display mode (Text/Outlined/Filled)")]        
        public AppVariant NewVariant { get; set; }
        
       [Display(Name ="Icon", Description = "The icon to display (or None)")]
        public AppIcon NewAppIcon { get; set; }
        
    }

    public class Handler(ILabelRepository repository, ILabelCounter labelCounter) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var label = await repository.GetByIdAsync(request.LabelId);

            if (label == null)
            {
                throw new NotFoundException("Label does not exist");
            }
            
            label.Edit(request.NewName, request.NewDescription,  request.NewScope, request.NewColour, request.NewVariant, request.NewAppIcon, labelCounter);
            
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
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Description"));
        }
    }
}