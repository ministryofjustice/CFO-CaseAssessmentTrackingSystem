using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.QualityAssurance.DTOs;
using Cfo.Cats.Application.Features.QualityAssurance.Queries;
using Cfo.Cats.Application.SecurityConstants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.QA.Enrolments.Components;

public partial class QaNotes
{
    private EnrolmentQaNoteDto[]? _notes;

    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = null!;

    [CascadingParameter] public UserProfile UserProfile { get; set; } = null!;

    [Parameter, EditorRequired] public string ParticipantId { get; set; } = null!;

    //Hide CFO Users from providers on timeline
    private bool _hideUser = true;
    private readonly HashSet<string> _cfoTenantNames = ["CFO", "CFO Evolution"];
    private readonly string[] _allowed =
        [RoleNames.QAOfficer, RoleNames.QASupportManager, RoleNames.QAManager, RoleNames.SMT, RoleNames.SystemSupport];

    protected override async Task OnInitializedAsync()
    {
        if (_notes is null)
        {
            var state = await AuthState;

            bool includeInternalNotes =
                (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.Internal)).Succeeded;

            var result = await GetNewMediator().Send(new GetEnrolmentQaNotes.Query()
            {
                ParticipantId = ParticipantId,
                CurentUser = UserProfile,
                IncludeInternalNotes = includeInternalNotes
            });

            if (result.Succeeded)
            {
                _notes = result.Data!.OrderBy(n => n.Created).ToArray();
            }
        }

        if (UserProfile.AssignedRoles.Any(r => _allowed.Contains(r)))
        {
            _hideUser = false;
        }
    }
}