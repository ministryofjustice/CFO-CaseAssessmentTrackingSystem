using Cfo.Cats.Application.Features.Participants.DTOs;
using FluentValidation.Internal;

namespace Cfo.Cats.Server.UI.Pages.Risk.RiskComponents;

public partial class SpecificRisk
{
    [Parameter, EditorRequired] public required RiskDto Model { get; set; }

    public Action<ValidationStrategy<RiskDto>> Strategy => (options) =>
    {
        options.IncludeProperties(x => x.SpecificRisk);
        options.IncludeProperties(x => x.IsSubjectToSHPO);
        options.IncludeProperties(x => x.NSDCase);
    };
}