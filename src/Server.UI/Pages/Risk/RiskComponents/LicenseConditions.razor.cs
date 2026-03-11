using Cfo.Cats.Application.Features.Participants.DTOs;
using FluentValidation.Internal;

namespace Cfo.Cats.Server.UI.Pages.Risk.RiskComponents;

public partial class LicenseConditions
{
    [Parameter, EditorRequired] public required RiskDto Model { get; set; }

    public Action<ValidationStrategy<RiskDto>> Strategy => (options) =>
    {
        options.IncludeProperties(x => x.LicenseConditions);
        options.IncludeProperties(x => x.LicenseEnd);
        options.IncludeProperties(x => x.NoLicenseEndDate);
    };
}