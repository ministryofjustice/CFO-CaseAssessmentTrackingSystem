using Cfo.Cats.Application.Common.Interfaces.Contracts;
using Cfo.Cats.Application.Features.Contracts.DTOs;
using Cfo.Cats.Infrastructure.Services.Locations;

namespace Cfo.Cats.Server.UI.Components.Autocompletes
{
    public class ContractAutoComplete : MudAutocomplete<ContractDto>
    {
        [Inject] 
        private IContractService ContractService { get; set; } = default!;

        [Parameter, EditorRequired] 
        public string TenantId { get; set; } = default!;

        protected override void OnInitialized()
        {
            ContractService.OnChange += ContractService_OnChange;
        }

        private void ContractService_OnChange()
        {
            InvokeAsync(StateHasChanged);
        }

        protected override void Dispose(bool disposing)
        {
            ContractService.OnChange -= ContractService_OnChange;
            base.Dispose(disposing);
        }

        public override Task SetParametersAsync(ParameterView parameters)
        {
            SearchFunc = SearchLocations;
            Label = "Contract";
            Clearable = true;
            ResetValueOnEmptyText = true;
            ShowProgressIndicator = true;
            ToStringFunc = (e => e?.Name);
            return base.SetParametersAsync(parameters);
        }

        private async Task<IEnumerable<ContractDto>> SearchLocations(string arg1, CancellationToken token)
        {
            if (string.IsNullOrEmpty(arg1))
            {
                return await Task.FromResult(ContractService.GetVisibleContracts(TenantId));
            }

            return await Task.FromResult(ContractService.GetVisibleContracts(TenantId)
                .Where(t => t.Name.Contains(arg1, StringComparison.OrdinalIgnoreCase))
            );

        }
    }
}
