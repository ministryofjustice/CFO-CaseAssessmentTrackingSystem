using Cfo.Cats.Application.Features.HmppsExternalApi.DTOs;

namespace Cfo.Cats.Server.UI.Pages.Risk.RiskComponents;

public partial class ArnsOffenderManagersDetail : ComponentBase
{
    [Parameter]
    [EditorRequired]
    public IHmppsExternalApiService Service { get; set; } = null!;

    [Parameter]
    [EditorRequired]
    public string HmppsId { get; set; } = null!;

    private bool _loading = true;
    private string? _error;
    private ResponsibleOfficerDto? _officers;

    protected override async Task OnInitializedAsync()
    {
        var result = await Service.GetResponsibleOfficerAsync(HmppsId, CancellationToken.None);

        if (result.Succeeded)
        {
            _officers = result.Data;
        }
        else
        {
            _error = result.ErrorMessage is { Length: > 0 } ? result.ErrorMessage : "No offender manager data found.";
        }

        _loading = false;
    }

    private static string? FullName(OffenderManagerDto manager)
    {
        var name = $"{manager.Name?.Forename} {manager.Name?.Surname}".Trim();
        return name.Length > 0 ? name : null;
    }
}
