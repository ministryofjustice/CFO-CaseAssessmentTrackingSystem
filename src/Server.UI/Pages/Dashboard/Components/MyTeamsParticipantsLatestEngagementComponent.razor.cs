using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Pages.Dashboard.Components;

public partial class MyTeamsParticipantsLatestEngagementComponent
{
    private bool _loading;
    private bool _downloading;

    private Result<IEnumerable<ParticipantEngagementDto>>? Model { get; set; }

    [CascadingParameter] public UserProfile CurrentUser { get; set; } = default!;

    private GetParticipantsLatestEngagement.Query Query { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            _loading = true;

            Query = new GetParticipantsLatestEngagement.Query()
            {
                JustMyCases = false,
                CurrentUser = CurrentUser
            };

            Model = await GetNewMediator().Send(Query);
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task OnExport()
    {
        try
        {
            _downloading = true;
            var result = await GetNewMediator().Send(new ExportParticipantsLatestEngagement.Command()
            {
                Query = Query
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
}
