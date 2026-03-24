using Cfo.Cats.Application.Features.Participants.DTOs;
using FluentValidation.Internal;

namespace Cfo.Cats.Server.UI.Pages.Risk.RiskComponents;

public partial class LicenseConditions
{
    [Parameter, EditorRequired] public RiskDto Model { get; set; } = null!;

    [Parameter, EditorRequired] public RiskComponentViewMode ViewMode { get; set; }

    public Action<ValidationStrategy<RiskDto>> Strategy => (options) =>
    {
        options.IncludeProperties(x => x.LicenseConditions);
        options.IncludeProperties(x => x.LicenseEnd);
        options.IncludeProperties(x => x.NoLicenseEndDate);
    };
}