using ActualLab.Fusion.Blazor;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.Features.Activities.Queries;
using Cfo.Cats.Application.SecurityConstants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Cfo.Cats.Server.UI.Pages.QA.Activities.Components;

public partial class QaNotes
{
    private ActivityQaNoteDto[]? _notes;

    [CascadingParameter]
    private Task<AuthenticationState> AuthState { get; set; } = null!;

    [CascadingParameter]
    public UserProfile UserProfile { get; set; } = null!;

    [Parameter, EditorRequired]
    public Guid? ActivityId { get; set; }

    [Parameter]
    public string? FirstPassUser { get; set; }

    //Hide CFO Users from providers on timeline
    private bool _hideUser = true;
    private readonly HashSet<string> _cfoTenantNames = ["CFO", "CFO Evolution"];
    private readonly string[] _allowed = [RoleNames.QAOfficer, RoleNames.QASupportManager, RoleNames.QAManager, RoleNames.SMT, RoleNames.SystemSupport];

    protected override async Task OnParametersSetAsync()
    {
        if (ActivityId == null)
        {
            return;
        }

        if (_notes is not null)
        {
            return;
        }

        var state = await AuthState;

        bool includeInternalNotes =
            (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.Internal)).Succeeded;

        var result = await GetNewMediator().Send(new GetActivityQaNotes.Query()
        {
            ActivityId = ActivityId,
            CurrentUser = UserProfile,
            IncludeInternalNotes = includeInternalNotes
        });

        if (result.Succeeded)
        {
            _notes = result.Data!.OrderBy(n => n.Created).ToArray();
        }

        if (UserProfile.AssignedRoles.Any(r => _allowed.Contains(r)))
        {
            _hideUser = false;
        }
    }
}