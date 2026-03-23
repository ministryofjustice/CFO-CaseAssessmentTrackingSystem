using Cfo.Cats.Application.Features.Participants.DTOs;
using FluentValidation.Internal;

namespace Cfo.Cats.Server.UI.Pages.Risk.RiskComponents;

public partial class Restrictions
{
    [Parameter, EditorRequired] public required RiskDto Model { get; set; }

    [Parameter, EditorRequired] 
    public RiskComponentViewMode ViewMode { get; set; }

    public Action<ValidationStrategy<RiskDto>> Strategy => (options) =>
    {
        options.IncludeProperties(x => x.ActivityRestrictions);
        options.IncludeProperties(x => x.ActivityRestrictionsReceived);
        options.IncludeProperties(x => x.PSFRestrictions);
        options.IncludeProperties(x => x.PSFRestrictionsReceived);
    };
}