using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Application.Features.Locations.DTOs;

namespace Cfo.Cats.Server.UI.Components.Autocompletes
{
    public class LocationAutoComplete : MudAutocomplete<LocationDto>
    {
        [Inject]
        private ILocationService LocationService { get; set; } = default!;

        [Parameter]
        [EditorRequired]
        public string TenantId { get; set; } = default!;

        protected override void OnInitialized()
        {
            LocationService.OnChange += LocationService_OnChange;
        }

        private void LocationService_OnChange()
        {
            InvokeAsync(StateHasChanged);
        }

        protected override ValueTask DisposeAsyncCore()
        {
            LocationService.OnChange -= LocationService_OnChange;
            return base.DisposeAsyncCore();
        }

        public override Task SetParametersAsync(ParameterView parameters)
        {
            SearchFunc = SearchLocations;
            Clearable = true;
            Dense = true;
            ResetValueOnEmptyText = true;
            ShowProgressIndicator = true;
            return base.SetParametersAsync(parameters);
        }

        private async Task<IEnumerable<LocationDto>> SearchLocations(string? arg1, CancellationToken token)
        {
            if (string.IsNullOrEmpty(arg1)) 
            {
                return await Task.FromResult(LocationService.GetVisibleLocations(TenantId));
            }

            return await Task.FromResult(LocationService.GetVisibleLocations(TenantId)
                    .Where(t => t.Name.Contains(arg1, StringComparison.OrdinalIgnoreCase))
                );

        }
    }
}
