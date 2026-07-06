using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Application.Features.Tenants.DTOs;
using Cfo.Cats.Domain.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Components.Users;

public partial class UserForm
{
    //private IEnumerable<string> allowedDomains = [];
    public IEnumerable<TenantDto> Tenants { get; set; } = [];

    public class CheckItem
    {
        public string Key { get; set; } = string.Empty;
        public bool Value { get; set; }
        public bool Enabled { get; set; }
    }

    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = null!;
    [EditorRequired] [Parameter] public ApplicationUserDto Model { get; set; } = null!;
    [EditorRequired] [Parameter] public EventCallback<ApplicationUserDto> OnFormSubmit { get; set; }
    public string? CallReference { get; set; }
    public string Message { get; set; } = string.Empty;
    public ApplicationUser CurrentUser { get; private set; } = null!;
    private MudForm? _form;
    private List<CheckItem> Roles { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;

        CurrentUser = (await UserManager.GetUserAsync(state.User))!;

        Tenants = TenantsService
            .GetVisibleTenants(CurrentUser.TenantId!)
            .OrderBy(x => x.Id);

        var roleNames = await UserManager.GetRolesAsync(CurrentUser);
        List<ApplicationRole> userRoles = new();
        foreach (var roleName in roleNames)
        {
            var role = await RoleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                userRoles.Add(role);
            }
        }

        var min = userRoles.Min(x => x.RoleRank);

        var allRoles = await RoleManager.Roles.ToListAsync();

        foreach (var role in allRoles.OrderByDescending(r => r.RoleRank))
        {
            var isEnabled = role.RoleRank >= min;
            var isChecked = Model.AssignedRoles.Contains(role.Name);

            Roles.Add(new CheckItem { Key = role.Name!, Value = isChecked, Enabled = isEnabled });
        }
    }

    // private void OnProviderValueChanged(string value)
    // {
    //     Model.ProviderId = value;
    //     Model.TenantId = null;
    //     Model.TenantName = null;
    // }

    private void OnTenantValueChange(string value)
    {
        Model.TenantId = value ?? throw new ArgumentNullException(nameof(value));
        Model.TenantName = Tenants.First(t => t.Id == Model.TenantId).Name;
    }

    private IEnumerable<string> ValidateCallReferenceNo(string callRefNo)
    {
        if (string.IsNullOrWhiteSpace(callRefNo))
        {
            yield return "Call Reference No is required!";
            yield break;
        }

        if (callRefNo.Length > 20)
        {
            yield return "Call Reference No must be less than or equal to 20 characters";
        }
    }

    private IEnumerable<string> ValidateNotes(string notes)
    {
        if (string.IsNullOrWhiteSpace(notes))
        {
            yield return "Notes is required!";
            yield break;
        }

        if (notes.Length > 255)
        {
            yield return "Call Reference No must be less than or equal to 255 characters";
        }
    }

    private IEnumerable<string> AllowedDomains(string tenantId) =>
        Tenants.Where(x => x.Id == tenantId)
            .SelectMany(x => x.Domains);

    private bool IsValidTenant(string tenantId) => AllowedDomains(tenantId).Any();

    public async Task Submit()
    {
        if (_form is not null)
        {
            await _form.ValidateAsync();
            if (_form.IsValid)
            {
                Model.ProviderId = Model.TenantId;
                Model.AssignedRoles = Roles.Where(x => x.Value).Select(x => x.Key).ToArray();
                Model.Notes.Add(new ApplicationUserNoteDto
                {
                    Message = Message,
                    CallReference = CallReference,
                    ApplicationUserId = Model.Id
                });
                await _form.ValidateAsync();

                if (_form.IsValid)
                {
                    await OnFormSubmit.InvokeAsync(Model);
                }
            }
        }
    }

    // private IEnumerable<TenantDto> GetDescendants(string tenantId)
    // {
    //     int len = tenantId.Length / 2;
    //
    //     var maxDepth = len > 1 ? 1 : 2;
    //
    //     var depth = tenantId.Length + (maxDepth * 2);
    //
    //     return Tenants.Where(x => x.Id.StartsWith(tenantId) && x.Id.Length <= depth);
    // }
}
