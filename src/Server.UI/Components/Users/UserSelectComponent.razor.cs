using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Domain.Common.Enums;

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
    [Parameter] public Variant Variant { get; set; } = Variant.Filled;
    [Parameter] public bool ActiveOnly { get; set; }
    private ApplicationUserDto[] _users = [];
    
    protected override void OnInitialized()
    {
        var users = UserService.DataSource
            .Where(d => d.TenantId!.StartsWith(TenantId));
        
        if (ActiveOnly)
        {
            users = users.Where(u => u.Status == UserStatus.Active);
        }
        
        _users = users.OrderBy(u => u.DisplayName).ToArray();
    }
    
    private string GetDisplayName(ApplicationUserDto? user) => user?.DisplayName ?? string.Empty;
    private async Task HandleValueChanged(ApplicationUserDto? value)
    {
        Value = value;
        await ValueChanged.InvokeAsync(value);
    }
}
