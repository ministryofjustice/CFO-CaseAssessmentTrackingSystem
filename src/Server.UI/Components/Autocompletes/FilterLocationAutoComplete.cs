using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Application.Features.Locations.DTOs;

namespace Cfo.Cats.Server.UI.Components.Autocompletes
{
    public class FilterLocationAutoComplete : MudAutocomplete<LocationDto>
    {
        public FilterLocationAutoComplete()
        {
            ToStringFunc = location => location is not null ? location.Name : string.Empty;
        }

        [Parameter, EditorRequired] 
        public LocationDto[] Locations { get; set; } = [];

        public override Task SetParametersAsync(ParameterView parameters)
        {
            SearchFunc = SearchLocations;
            Clearable = true;
            Dense = true;
            ResetValueOnEmptyText = true;
            ShowProgressIndicator = true;
            return base.SetParametersAsync(parameters);
        }

        private Task<IEnumerable<LocationDto>> SearchLocations(string? arg1, CancellationToken token)
        {
            var results = string.IsNullOrEmpty(arg1) 
                ? Locations.AsEnumerable() 
                : Locations.Where(t => t.Name.Contains(arg1, StringComparison.InvariantCultureIgnoreCase));
            
            return Task.FromResult(results);
        }
    }
}
