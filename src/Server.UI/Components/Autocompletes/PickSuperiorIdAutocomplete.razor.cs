using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Features.Identity.DTOs;

namespace Cfo.Cats.Server.UI.Components.Autocompletes;

public class PickSuperiorIdAutocomplete : MudAutocomplete<string>
{
    private List<ApplicationUserDto>? userList;

    [Parameter]
    public string? TenantId { get; set; }

    [Parameter]
    public string OwnerName { get; set; } = string.Empty;

    [Inject]
    private IIdentityService IdentityService { get; set; } = default!;

    public override Task SetParametersAsync(ParameterView parameters)
    {
        SearchFuncWithCancel = SearchKeyValues;
        ToStringFunc = ToString;
        Clearable = true;
        Dense = true;
        ResetValueOnEmptyText = true;
        ShowProgressIndicator = true;
        MaxItems = 50;
        return base.SetParametersAsync(parameters);
    }

    private async Task<IEnumerable<string>> SearchKeyValues(
        string value,
        CancellationToken cancellation
    )
    {
        // if text is null or empty, show complete list
        userList = await IdentityService.GetUsers(TenantId, cancellation);
        List<int> result = new();

        if (string.IsNullOrEmpty(value) && userList is not null)
        {
            result = userList.Select(x => x.Id).Take(MaxItems ?? 50).ToList();
        }
        else if (userList is not null)
        {
            result = userList
                .Where(x =>
                    !x.UserName.Equals(OwnerName, StringComparison.OrdinalIgnoreCase)
                    && (
                        x.UserName.Contains(value, StringComparison.OrdinalIgnoreCase)
                        || x.Email.Contains(value, StringComparison.OrdinalIgnoreCase)
                    )
                )
                .Select(x => x.Id)
                .Take(MaxItems ?? 50)
                .ToList();
            ;
        }

        return result.Select(e => e.ToString());
    }

    private string ToString(string str)
    {
        
        if (
            userList is not null
            && !string.IsNullOrEmpty(str)
            && int.TryParse(str, out int id)
            && userList.Any(x => x.Id == id)
        )
        {
            var userDto = userList.First(x => x.Id == id);
            return userDto.UserName;
        }

        /*if (userList is null && !string.IsNullOrEmpty(str))
        {
            var userName = IdentityService.GetUserName(str);
            return userName;
        }*/

        return string.Empty;
    }
}
