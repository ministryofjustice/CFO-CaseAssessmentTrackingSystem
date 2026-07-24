using System.Linq.Expressions;
using System.Security.Claims;
using Cfo.Cats.Application.Common.Exceptions;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Application.Features.Identity.Notifications.IdentityEvents;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Infrastructure.Services;
using Cfo.Cats.Server.UI.Extensions;
using Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Components.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Pages.Users;

public partial class Users
{
    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = null!;

    private UserManager<ApplicationUser> _userManager = null!;
    private RoleManager<ApplicationRole> _roleManager = null!;

    [CascadingParameter] public UserProfile UserProfile { get; set; } = null!;

    private ApplicationUser? _currentUser;
    private IEnumerable<string> _currentRoles = [];
    private int _defaultPageSize = 15;
    private readonly ApplicationUserDto _currentDto = new();
    private string _searchString = string.Empty;
    private string? _selectedTenantId;
    private string Title { get; set; } = "Users";
    private readonly List<PermissionModel> _permissions = new();
    private readonly IList<Claim> _assignedClaims = null!;
    private MudDataGrid<ApplicationUserDto> _table = null!;
    private bool _initialised;
    private bool _processing;
    private bool _showPermissionsDrawer;
    private bool _canCreate;
    private bool _canSearch;
    private bool _canEdit;
    private bool _canArchive;
    private bool _canToggleActiveStatus;
    private bool _canUnlock;
    private bool _canManageRoles;
    private bool _canResetPassword;
    private bool _canManagePermissions;
    private bool _loading;
    private bool _downloading;
    private List<ApplicationRoleDto> _roles = new();
    private string? _searchRole;
    private Dictionary<string, bool>? _policies;
    private HashSet<string> _onlineUsers = new(StringComparer.OrdinalIgnoreCase);

    protected override async Task OnInitializedAsync()
    {
        _initialised = false;

        Title = L[_currentDto.GetClassDescription()];
        _roleManager = ScopedServices.GetRequiredService<RoleManager<ApplicationRole>>();
        _userManager = ScopedServices.GetRequiredService<UserManager<ApplicationUser>>();

        var state = await AuthState;
        _currentUser = await _userManager.GetUserAsync(state.User);
        _currentRoles = await _userManager.GetRolesAsync(_currentUser!);

        _policies ??= new()
        {
            {
                SecurityPolicies.SystemFunctionsWrite,
                (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.SystemFunctionsWrite)).Succeeded
            }
        };

        _canCreate = _policies.GetValueOrDefault(SecurityPolicies.SystemFunctionsWrite);
        _canSearch = _policies.GetValueOrDefault(SecurityPolicies.SystemFunctionsWrite);
        _canEdit = _policies.GetValueOrDefault(SecurityPolicies.SystemFunctionsWrite);
        _canArchive = _policies.GetValueOrDefault(SecurityPolicies.SystemFunctionsWrite);
        _canToggleActiveStatus = _policies.GetValueOrDefault(SecurityPolicies.SystemFunctionsWrite);
        _canUnlock = _policies.GetValueOrDefault(SecurityPolicies.SystemFunctionsWrite);
        _canManageRoles = _policies.GetValueOrDefault(SecurityPolicies.SystemFunctionsWrite);
        _canResetPassword = _policies.GetValueOrDefault(SecurityPolicies.SystemFunctionsWrite);
        _canManagePermissions = _policies.GetValueOrDefault(SecurityPolicies.SystemFunctionsWrite);

        _roles = await _roleManager.Roles
            .ProjectTo<ApplicationRoleDto>(Mapper.ConfigurationProvider)
            .ToListAsync();

        UsersStateContainer.OnChange += OnPresenceChanged;
        await RefreshOnlineUsersAsync();

        _initialised = true;
    }

    private bool CanResetPassword(string[] affectedUserRoles)
    {
        if (affectedUserRoles is { Length: 0 })
        {
            return _canResetPassword;
        }

        var userRole = _roles.Where(role => affectedUserRoles.Contains(role.Name)).MinBy(role => role.RoleRank);
        var currentUserRole = _roles.Where(role => _currentRoles.Contains(role.Name)).MinBy(role => role.RoleRank);
        return _canResetPassword && currentUserRole?.RoleRank <= userRole?.RoleRank;
    }

    private bool IsOnline(string username) => _onlineUsers.Contains(username);

    private Expression<Func<ApplicationUser, bool>> CreateSearchPredicate() =>
        x =>
            (x.UserName!.Contains(_searchString) || x.Email!.Contains(_searchString) ||
             x.DisplayName!.Contains(_searchString) || x.PhoneNumber!.Contains(_searchString))
            && (_searchRole == null ||
                (_searchRole != null && x.UserRoles.Any(userRole => userRole.Role.Name == _searchRole)))
            && (_selectedTenantId == null || (_selectedTenantId != null && x.TenantId == _selectedTenantId));

    private async Task<GridData<ApplicationUserDto>> ServerReload(GridState<ApplicationUserDto> state,
        CancellationToken cancellationToken)
    {
        try
        {
            _loading = true;

            if (state.SortDefinitions.Count == 0)
            {
                state.SortDefinitions.Add(new SortDefinition<ApplicationUserDto>("Email", false, 1, x => x.Email));
            }

            var query = _userManager.Users.Where(CreateSearchPredicate());
            var items = await query
                .Where(x => x.TenantId!.StartsWith(_currentUser!.TenantId!))
                .Include(x => x.UserRoles)
                .Include(x => x.Superior)
                .EfOrderBySortDefinitions(state)
                .Skip(state.Page * state.PageSize).Take(state.PageSize)
                .ProjectTo<ApplicationUserDto>(Mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            var total = await _userManager.Users.CountAsync(CreateSearchPredicate(), cancellationToken);
            return new GridData<ApplicationUserDto> { TotalItems = total, Items = items };
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task OnChangedListView(string? tenantId)
    {
        _selectedTenantId = tenantId;
        await _table.ReloadServerData();
    }

    private async Task OnSearch(string? text)
    {
        if (_loading)
        {
            return;
        }

        _searchString = text!.ToLower();
        await _table.ReloadServerData();
    }

    private async Task OnSearchRole(string? role)
    {
        if (_loading)
        {
            return;
        }

        _searchRole = role;
        await _table.ReloadServerData();
    }

    private async Task OnRefresh()
    {
        TenantsService.Refresh();
        await _table.ReloadServerData();
    }

    private async Task OnExport()
    {
        try
        {
            _downloading = true;
            var result = await GetNewMediator().Send(new Application.Features.Identity.Commands.ExportUsers.Command()
            {
                TenantId = _selectedTenantId,
                SearchString = _searchString,
                Role = _searchRole
            });

            if (result.Succeeded)
            {
                Snackbar.Add($"{ConstantString.ExportSuccess}", Severity.Info);
                return;
            }

            Snackbar.Add(result.ErrorMessage, Severity.Error);
        }
        catch
        {
            Snackbar.Add($"An error occurred while generating your document.", Severity.Error);
        }
        finally
        {
            _downloading = false;
        }
    }

    private async Task ShowCreateUserDialog(ApplicationUserDto model, string title)
    {
        var parameters = new DialogParameters<UserFormDialog>
        {
            { x => x.Model, model }
        };
        var options = new DialogOptions
            { CloseButton = true, CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
        var dialog = await DialogService.ShowAsync<UserFormDialog>(title, parameters, options);
        var result = await dialog.Result;

        if (!result!.Canceled)
        {
            await ProcessUserCreation(model);
        }
        else
        {
            await OnRefresh();
        }
    }

    private async Task ShowIdentityAuditDialog(ApplicationUserDto model, string title)
    {
        var parameters = new DialogParameters<UserAuditDialog>
        {
            { x => x.UserName, model.UserName }
        };
        var options = new DialogOptions
            { CloseButton = true, CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };
        await DialogService.ShowAsync<UserAuditDialog>(title, parameters, options);
    }

    private async Task ShowEditUserDialog(ApplicationUserDto model, string title)
    {
        var parameters = new DialogParameters<UserFormDialog>
        {
            { x => x.Model, model }
        };
        var options = new DialogOptions
            { CloseButton = true, CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
        var dialog = await DialogService.ShowAsync<UserFormDialog>(title, parameters, options);
        var result = await dialog.Result;

        if (!result!.Canceled)
        {
            await ProcessUserUpdate(model);
        }
        else
        {
            await OnRefresh();
        }
    }

    private async Task ProcessUserCreation(ApplicationUserDto model)
    {
        var applicationUser = Mapper.Map<ApplicationUser>(model);
        applicationUser.EmailConfirmed = true;
        applicationUser.Status = UserStatus.PendingActivation;
        applicationUser.TwoFactorEnabled = true;
        applicationUser.PhoneNumberConfirmed = string.IsNullOrWhiteSpace(applicationUser.PhoneNumber) == false;

        var identityResult = await _userManager.CreateAsync(applicationUser);
        if (!identityResult.Succeeded)
        {
            Snackbar.Add($"{string.Join(",", identityResult.Errors.Select(x => x.Description).ToArray())}",
                Severity.Error);
            return;
        }

        Snackbar.Add($"{L["New user created successfully."]}", Severity.Info);
        await AssignRolesToUser(applicationUser, model.AssignedRoles);

        Logger.LogInformation("Create a user succeeded. Username: {@UserName:l}, UserId: {@UserId}",
            applicationUser.UserName, applicationUser.Id);
        UserService.Refresh();
        await OnRefresh();
    }

    private async Task ProcessUserUpdate(ApplicationUserDto model)
    {
        var user = await _userManager.FindByIdAsync(model.Id) ??
                   throw new NotFoundException($"The application user [{model.Id}] was not found.");

        Mapper.Map(model, user);

        user.PhoneNumberConfirmed = string.IsNullOrWhiteSpace(user.PhoneNumber) == false;

        var identityResult = await _userManager.UpdateAsync(user);
        if (identityResult.Succeeded)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Count > 0)
            {
                await _userManager.RemoveFromRolesAsync(user, roles);
            }

            if (model.AssignedRoles.Length > 0)
            {
                await _userManager.AddToRolesAsync(user, model.AssignedRoles);
            }

            Snackbar.Add($"{L["The user updated successfully."]}", Severity.Info);
            await OnRefresh();
            UserService.Refresh();

            await Cache.RemoveAsync(ApplicationUserClaimsPrincipalFactory.GetCacheKey(user.Id));
        }
        else
        {
            Snackbar.Add($"{string.Join(",", identityResult.Errors.Select(x => x.Description).ToArray())}",
                Severity.Error);
        }
    }

    private async Task AssignRolesToUser(ApplicationUser user, string[]? roles)
    {
        if (roles is not null && roles.Length > 0)
        {
            await _userManager.AddToRolesAsync(user, roles);
        }
    }

    private async Task OnCreate()
    {
        var model = new ApplicationUserDto { AssignedRoles = [] };
        await ShowCreateUserDialog(model, L["Create a new user"]);
    }

    private async Task OnEdit(ApplicationUserDto item) => await ShowEditUserDialog(item, L["Edit the user"]);

    private async Task OnUnlock(ApplicationUserDto item)
    {
        var user = await _userManager.FindByIdAsync(item.Id) ??
                   throw new NotFoundException($"Application user not found {item.Id}.");

        user.LockoutEnd = null;
        var identityResult = await _userManager.UpdateAsync(user);

        if (identityResult.Succeeded)
        {
            item.LockoutEnd = null;
            Snackbar.Add($"{L["The user has been unlocked."]}", Severity.Info);
        }
        else
        {
            Snackbar.Add($"{string.Join(",", identityResult.Errors.Select(x => x.Description).ToArray())}",
                Severity.Error);
        }
    }

    private Task OnReactivate(ApplicationUserDto item) =>
        ChangeUserStatus(item, UserStatus.Active, lockoutEnd: null,
            IdentityAuditNotification.ActivateAccount(item.UserName, NetworkIpProvider.IpAddress, _currentUser!.UserName!),
            "The user has been reactivated.");

    private Task OnSuspend(ApplicationUserDto item) =>
        ChangeUserStatus(item, UserStatus.Suspended, lockoutEnd: DateTimeOffset.MaxValue,
            IdentityAuditNotification.SuspendAccount(item.UserName, NetworkIpProvider.IpAddress, _currentUser!.UserName!),
            "The user has been suspended.");

    private Task OnMarkAsLeft(ApplicationUserDto item) =>
        ChangeUserStatus(item, UserStatus.Left, lockoutEnd: DateTimeOffset.MaxValue,
            IdentityAuditNotification.MarkAccountAsLeft(item.UserName, NetworkIpProvider.IpAddress, _currentUser!.UserName!),
            "The user has been marked as left.");

    private async Task ChangeUserStatus(ApplicationUserDto item, UserStatus newStatus, DateTimeOffset? lockoutEnd,
        IdentityAuditNotification audit, string successMessage)
    {
        var user = await _userManager.FindByIdAsync(item.Id) ??
                   throw new NotFoundException($"Application user not found {item.Id}.");

        user.Status = newStatus;
        user.LockoutEnd = lockoutEnd;

        if (newStatus == UserStatus.Active)
        {
            user.EmailConfirmed = true;
        }

        var identityResult = await _userManager.UpdateAsync(user);

        if (identityResult.Succeeded)
        {
            item.Status = newStatus;
            item.LockoutEnd = lockoutEnd;

            var mediator = GetNewMediator();
            await mediator.Publish(audit);

            // Drop any cached claims so the status change takes effect immediately.
            await Cache.RemoveAsync(ApplicationUserClaimsPrincipalFactory.GetCacheKey(user.Id));

            Snackbar.Add($"{L[successMessage]}", Severity.Info);
        }
        else
        {
            Snackbar.Add($"{string.Join(",", identityResult.Errors.Select(x => x.Description).ToArray())}",
                Severity.Error);
        }
    }

    private static Color GetStatusColor(UserStatus status) => status.Name switch
    {
        nameof(UserStatus.Active) => Color.Success,
        nameof(UserStatus.Suspended) => Color.Warning,
        nameof(UserStatus.Left) => Color.Error,
        nameof(UserStatus.PendingActivation) => Color.Info,
        _ => Color.Surface
    };

    private static string GetStatusIcon(UserStatus status) => status.Name switch
    {
        nameof(UserStatus.Active) => Icons.Material.Filled.CheckCircleOutline,
        nameof(UserStatus.Suspended) => Icons.Material.Filled.PauseCircleOutline,
        nameof(UserStatus.Left) => Icons.Material.Filled.ExitToApp,
        nameof(UserStatus.PendingActivation) => Icons.Material.Filled.HourglassEmpty,
        _ => Icons.Material.Filled.HighlightOff
    };

    private async Task OnResetPassword(ApplicationUserDto item)
    {
        var model = new ResetPasswordFormModel { UserId = item.Id };

        var parameters = new DialogParameters<ResetPasswordDialog>
        {
            { x => x.Model, model }
        };

        var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true, MaxWidth = MaxWidth.Small };
        var dialog =
            await DialogService.ShowAsync<ResetPasswordDialog>(L[$"Reset Password for {item.DisplayName}"], parameters,
                options);

        var result = await dialog.Result;

        if (!result!.Canceled)
        {
            await GetNewMediator()
                .Publish(IdentityAuditNotification.PasswordReset(item.UserName, NetworkIpProvider.IpAddress,
                    UserProfile.Email));
            Snackbar.Add($"{L["Reset password successfully"]}", Severity.Info);
        }
    }

    private Task OnOpenChangedHandler(bool state)
    {
        _showPermissionsDrawer = state;
        return Task.CompletedTask;
    }

    private async Task OnAssignAllChangedHandler(List<PermissionModel> models)
    {
        try
        {
            _processing = true;
            var userId = models.First().UserId;
            var user = await _userManager.FindByIdAsync(userId!) ??
                       throw new NotFoundException($"not found application user: {userId}");

            foreach (var model in models)
            {
                await ProcessPermissionChange(user, model);
            }

            Snackbar.Add($"{L["Authorization has been changed"]}", Severity.Info);
            await ClearClaimsCache(user.Id);
        }
        finally
        {
            _processing = false;
        }
    }

    private async Task ProcessPermissionChange(ApplicationUser user, PermissionModel model)
    {
        if (model.Assigned)
        {
            if (model.ClaimType is not null)
            {
                await _userManager.AddClaimAsync(user, new Claim(model.ClaimType, model.ClaimValue));
            }
        }
        else
        {
            var removed = _assignedClaims.FirstOrDefault(x => x.Value == model.ClaimValue);
            if (removed is not null)
            {
                await _userManager.RemoveClaimAsync(user, removed);
            }
        }
    }

    private async Task ClearClaimsCache(string userId)
    {
        var key = $"get-claims-by-{userId}";
        await FusionCache.RemoveAsync(key);
        await Task.Delay(300);
    }

    private async Task OnAssignChangedHandler(PermissionModel model)
    {
        try
        {
            _processing = true;
            var userId = model.UserId!;
            var user = await _userManager.FindByIdAsync(userId) ??
                       throw new NotFoundException($"Application user Not Found {userId}.");

            model.Assigned = !model.Assigned;
            if (model.Assigned)
            {
                if (model.ClaimType is not null)
                {
                    await _userManager.AddClaimAsync(user, new Claim(model.ClaimType, model.ClaimValue));
                    Snackbar.Add($"{L["Permission assigned successfully"]}", Severity.Info);
                }
            }
            else
            {
                var removed = _assignedClaims.FirstOrDefault(x => x.Value == model.ClaimValue);
                if (removed is not null)
                {
                    await _userManager.RemoveClaimAsync(user, removed);
                }

                Snackbar.Add($"{L["Permission removed successfully"]}", Severity.Info);
            }

            await ClearClaimsCache(user.Id);
        }
        finally
        {
            _processing = false;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            UsersStateContainer.OnChange -= OnPresenceChanged;
        }

        base.Dispose(disposing);
    }

    private void OnPresenceChanged() => InvokeAsync(RefreshOnlineUsersAsync);

    private async Task RefreshOnlineUsersAsync()
    {
        var online = await UsersStateContainer.GetOnlineUsersAsync();
        _onlineUsers = new HashSet<string>(online, StringComparer.OrdinalIgnoreCase);
        StateHasChanged();
    }

}