using Cfo.Cats.Application.Features.Participants.DTOs;
using FluentValidation.Internal;

namespace Cfo.Cats.Server.UI.Pages.Risk.RiskComponents;

public partial class AdditionalInformation
{
    [Parameter, EditorRequired] public required RiskDto Model { get; set; }
    [Parameter, EditorRequired] public RiskComponentViewMode ViewMode { get; set; }

    public Action<ValidationStrategy<RiskDto>> Strategy => (options) =>
    {
        options.IncludeProperties(x => x.AdditionalInformation);
        options.IncludeProperties(x => x.ReferrerName);
        options.IncludeProperties(x => x.ReferrerEmail);
        options.IncludeProperties(x => x.ReferredOn);
    };
}