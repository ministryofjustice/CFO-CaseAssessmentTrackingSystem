using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Features.Identity.DTOs;

namespace Cfo.Cats.Server.UI.Components.Autocompletes;

public class PickUserAutocomplete : MudAutocomplete<string>
{
    private List<ApplicationUserDto>? userList;

    [Parameter]
    public string? TenantId { get; set; }

    [Inject]
    private IUserService UserService { get; set; } = default!;

    protected override void OnInitialized()
    {
        UserService.OnChange += TenantsService_OnChange;
    }

    private void TenantsService_OnChange()
    {
        InvokeAsync(StateHasChanged);
    }

    protected override ValueTask DisposeAsyncCore()
    {
        UserService.OnChange -= TenantsService_OnChange;
        return base.DisposeAsyncCore();
    }

    public override Task SetParametersAsync(ParameterView parameters)
    {
        SearchFunc = SearchKeyValues;
        Clearable = true;
        Dense = true;
        ResetValueOnEmptyText = true;
        ShowProgressIndicator = true;
        MaxItems = 50;
        userList = string.IsNullOrEmpty(TenantId)
            ? UserService.DataSource
            : UserService.DataSource.Where(x => x.TenantId == TenantId).ToList();
        return base.SetParametersAsync(parameters);
    }

    private Task<IEnumerable<string>> SearchKeyValues(string? value, CancellationToken cancellation)
    {
        var result = string.IsNullOrEmpty(value)
            ? userList?.Select(x => x.UserName)
            : userList
                ?.Where(x =>
                    x.UserName.Contains(value, StringComparison.OrdinalIgnoreCase)
                    || x.Email.Contains(value, StringComparison.OrdinalIgnoreCase)
                )
                .Select(x => x.UserName);

        return Task.FromResult(result?.AsEnumerable() ?? new string[] { });
    }

    private string ToString(string str)
    {
        var user = userList?.FirstOrDefault(x =>
            (
                x.DisplayName != null
                && x.DisplayName.Contains(str, StringComparison.OrdinalIgnoreCase)
            ) || x.UserName.Contains(str, StringComparison.OrdinalIgnoreCase)
        );

        return user?.DisplayName ?? str;
    }
}
