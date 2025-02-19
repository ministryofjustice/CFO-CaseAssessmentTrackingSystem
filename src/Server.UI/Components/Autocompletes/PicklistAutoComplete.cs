using Cfo.Cats.Application.Features.KeyValues.DTOs;
using Microsoft.AspNetCore.Components.Web;

namespace Cfo.Cats.Server.UI.Components.Autocompletes;

public class PicklistAutoComplete : MudAutocomplete<string>
{
    [Parameter] 
    public Picklist Picklist { get; set; }

    [Inject] 
    private IPicklistService PicklistService { get; set; } = default!;
    
    protected override async Task OnInitializedAsync()
    {
        PicklistService.OnChange += PicklistService_OnChange;
        await base.OnInitializedAsync();
    }
    
    
    
    private void PicklistService_OnChange()
    {
        InvokeAsync(StateHasChanged);
    }

    protected override ValueTask DisposeAsyncCore()
    {
        PicklistService.OnChange -= PicklistService_OnChange;
        return base.DisposeAsyncCore();
    }
    
    public override Task SetParametersAsync(ParameterView parameters)
    {
        SearchFunc = SearchKeyValues;
        Clearable = true;
        Dense = true;
        ResetValueOnEmptyText = true;
        return base.SetParametersAsync(parameters);
    }
    
    private Task<IEnumerable<string>> SearchKeyValues(string? value, CancellationToken cancellation=default)
    {
        // if text is null or empty, show complete list
        return string.IsNullOrEmpty(value)
            ? Task.FromResult(PicklistService.DataSource
                .Where(x => x.Name == Picklist)
                .Select(x => x.Value ?? string.Empty))
            : Task.FromResult(PicklistService.DataSource
                .Where(x => x.Name == Picklist && Contains(x, value))
                .Select(x => x.Value ?? string.Empty));
    }
    
    private static bool Contains(KeyValueDto model, string value)
    {
        return (model.Value != null && model.Value.Contains(value, StringComparison.InvariantCultureIgnoreCase))
               || (model.Text != null && model.Text.Contains(value, StringComparison.InvariantCultureIgnoreCase));
    }
    
}
