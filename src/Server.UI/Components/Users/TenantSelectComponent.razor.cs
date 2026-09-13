using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Application.Features.Tenants.DTOs;

namespace Cfo.Cats.Server.UI.Components.Users;

public partial class TenantSelectComponent
{
    [Parameter]
    public string TenantId { get; set; } = null!;
    [Parameter] public TenantDto? Value { get; set; }
    [Parameter] public EventCallback<TenantDto?> ValueChanged { get; set; }
    [Parameter] public string Label { get; set; } = "Select Tenant";
    [Parameter] public string? Placeholder { get; set; } = "Choose a tenant...";
    [Parameter] public bool Required { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public Variant Variant { get; set; } = MudBlazor.Variant.Filled;
    [Parameter] public bool ShowAllOption { get; set; }
    [Parameter] public string AllOptionLabel { get; set; } = "All Tenants";

    private TenantDto[] _tenants = [];

    private TenantDto? _allTenantsOption;

    protected override void OnInitialized()
    {
        var filtered = TenantService.DataSource
            .Where(d => d.Id!.StartsWith(TenantId))
            .OrderBy(u => u.Id)
            .ToArray();

        if (ShowAllOption)
        {
            _allTenantsOption = new TenantDto { Id = string.Empty, Name = AllOptionLabel };
            _tenants = [_allTenantsOption, .. filtered];
        }
        else
        {
            _tenants = filtered;
        }
    }

    private string GetDisplayName(TenantDto? tenant) => tenant?.Name ?? string.Empty;
    private async Task HandleValueChanged(TenantDto? value)
    {
        Value = value;
        await ValueChanged.InvokeAsync(Value);
    }
    private bool Search(TenantDto? tenant, string searchText)
    {
        if (tenant == null)
        {
            return true;
        }

        if (tenant?.Name!.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true)
        {
            return true;
        }

        if(tenant is {  Name: not null})
        {
            if (tenant.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }

            if (tenant.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
        }
        
        return false;
    }
}

