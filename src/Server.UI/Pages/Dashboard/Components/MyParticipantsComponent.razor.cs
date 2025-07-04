using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Application.Features.Participants.Specifications;

namespace Cfo.Cats.Server.UI.Pages.Dashboard.Components;

public partial class MyParticipantsComponent
{
    [CascadingParameter] public UserProfile? UserProfile { get; set; }

    private double[] _data = [];

    private readonly string[] _labels =
    [
        "Identified",
        "Enrolling",
        "Submitted To PQA",
        "Submitted To Authority",
        "Approved"
    ];

    private string GetParticipantsUrl(ParticipantListView listView) =>
        $"/pages/participants?listView={listView.ToString()}";

    private ParticipantCountSummaryDto? _participantSteps;

    protected override async Task OnInitializedAsync()
    {
        var query = new GetMyParticipantsDashboard.Query()
        {
            CurrentUser = UserProfile!
        };

        var result = await GetNewMediator().Send(query);

        if (result.Succeeded)
        {
            _participantSteps = result.Data;
            _data =
            [
                _participantSteps!.IdentifiedCases,
                _participantSteps.EnrollingCases,
                _participantSteps.CasesAtPqa,
                _participantSteps.CasesAtCfo,
                _participantSteps.ApprovedCases
            ];
        }
    }

    private void GotoParticipants() => Navigation.NavigateTo("/pages/participants");
}