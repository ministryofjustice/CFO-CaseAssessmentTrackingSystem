using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Features.Identity.DTOs;

namespace Cfo.Cats.Server.UI.Components.Autocompletes;

public class PickUserAutocomplete : MudAutocomplete<ApplicationUserDto>
{
    [Parameter, EditorRequired]
    public string TenantId { get; set; } = default!;

    [Inject]
    private IUserService UserService { get; set; } = default!;

    public PickUserAutocomplete()
    {
        SearchFunc = SearchKeyValues;
        ToStringFunc = user => user?.DisplayName;
        Clearable = true;
        ResetValueOnEmptyText = true;
        ShowProgressIndicator = true;
        Strict = true;
        MaxItems = 50;
    }

    private Task<IEnumerable<ApplicationUserDto>> SearchKeyValues(string? value, CancellationToken cancellation)
    {
        var source = string.IsNullOrEmpty(TenantId)
            ? UserService.DataSource
            : UserService.DataSource.Where(x => x.TenantId!.StartsWith(TenantId));

        var result = string.IsNullOrEmpty(value)
            ? source
            : source.Where(x =>
                x.UserName.Contains(value, StringComparison.OrdinalIgnoreCase)
                || x.Email.Contains(value, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(result.Take(MaxItems ?? 50));
    }
}
