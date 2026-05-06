using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Common.Interfaces.Initiatives;
using Cfo.Cats.Application.Features.Initiatives.DTOs;

namespace Cfo.Cats.Server.UI.Components.Autocompletes;

public class InitiativeAutoComplete : MudAutocomplete<InitiativeDto>
{
    [Inject] private IInitiativeService InitiativeService { get; set; } = default!;
    [Inject] private ICurrentUserService CurrentUserService { get; set; } = default!;

    protected override void OnInitialized()
    {
        InitiativeService.OnChange += InitiativeService_OnChange;
        ToStringFunc = i => i is null ? string.Empty : $"{i.Code} \u2013 {i.Description}";
    }

    private void InitiativeService_OnChange() => InvokeAsync(StateHasChanged);

    protected override ValueTask DisposeAsyncCore()
    {
        InitiativeService.OnChange -= InitiativeService_OnChange;
        return base.DisposeAsyncCore();
    }

    public override Task SetParametersAsync(ParameterView parameters)
    {
        SearchFunc = SearchInitiatives;
        Clearable = true;
        ResetValueOnEmptyText = true;
        ShowProgressIndicator = true;
        return base.SetParametersAsync(parameters);
    }

    private Task<IEnumerable<InitiativeDto>> SearchInitiatives(string? text, CancellationToken token)
    {
        var tenantId = CurrentUserService.TenantId ?? string.Empty;
        var source = InitiativeService.GetActiveInitiatives(tenantId);

        var results = string.IsNullOrWhiteSpace(text)
            ? source
            : source.Where(i =>
                i.Code.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                i.Description.Contains(text, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(results);
    }
}
