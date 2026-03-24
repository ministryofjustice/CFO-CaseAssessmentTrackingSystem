using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Identity.DTOs;

namespace Cfo.Cats.Server.UI.Components.Users;

public partial class UserSelectComponent
{
    [Parameter]
    public string TenantId { get; set; } = null!;
    [Parameter] public ApplicationUserDto? Value { get; set; }
    [Parameter] public EventCallback<ApplicationUserDto?> ValueChanged { get; set; }
    [Parameter] public string Label { get; set; } = "Select User";
    [Parameter] public string? Placeholder { get; set; } = "Choose a user...";
    [Parameter] public bool Required { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public Variant Variant { get; set; } = MudBlazor.Variant.Filled;
    private ApplicationUserDto[] _users = [];
    protected override void OnInitialized() => _users = UserService.DataSource
        .Where(d => d.TenantId!.StartsWith(TenantId))
        .OrderBy(u => u.DisplayName)
        .ToArray();
    private string GetDisplayName(ApplicationUserDto? user) => user?.DisplayName ?? string.Empty;
    private async Task HandleValueChanged(ApplicationUserDto? value)
    {
        Value = value;
        await ValueChanged.InvokeAsync(value);
    }
}
