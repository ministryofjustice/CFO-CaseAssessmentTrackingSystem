using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Features.Identity.DTOs;

namespace Cfo.Cats.Server.UI.Components.Autocompletes;

public class PickUserAutocomplete : MudAutocomplete<ApplicationUserDto>
{
    private List<ApplicationUserDto> _userList = [];

    [Parameter, EditorRequired]
    public string TenantId { get; set; } = default!;

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
        ResetValueOnEmptyText = true;
        ShowProgressIndicator = true;
        Strict = true;
        MaxItems = 50;
        _userList = string.IsNullOrEmpty(TenantId)
            ? UserService.DataSource
            : UserService.DataSource.Where(x => x.TenantId!.StartsWith(TenantId)).ToList();
        ToStringFunc = user => user?.DisplayName;

        return base.SetParametersAsync(parameters);
    }

    private Task<IEnumerable<ApplicationUserDto>> SearchKeyValues(string? value, CancellationToken cancellation)
    {
        var result = string.IsNullOrEmpty(value)
            ? _userList?.Select(x => x)
            : _userList
                ?.Where(x =>
                    x.UserName.Contains(value, StringComparison.OrdinalIgnoreCase)
                    || x.Email.Contains(value, StringComparison.OrdinalIgnoreCase)
                )
                .Select(x => x);

        return Task.FromResult(result?.AsEnumerable() ?? []);
    }

}
