using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Candidates.DTOs;
using Cfo.Cats.Application.Features.Candidates.Queries.Search;
using Cfo.Cats.Application.Features.Participants.Commands;

namespace Cfo.Cats.Server.UI.Pages.Candidates.Components;

public partial class MatchFound
{
    [Inject]
    private IPicklistService PicklistService { get; set; } = default!;

    private bool loading;

    private MudForm? _form;

    private CreateParticipant.Command? Model;

    private List<ComparisonRow>? _comparisons;

    [Parameter, EditorRequired]
    public required CandidateDto Candidate { get; set; }

    [CascadingParameter]
    public UserProfile? UserProfile { get; set; }

    [Parameter]
    public CandidateSearchQuery Query { get; set; } = default!;

    [Parameter]
    public EventCallback OnCanceled { get; set; }

    [Parameter]
    public EventCallback OnParticipantEnrolled { get; set; }


    protected async override Task OnInitializedAsync()
    {
        _comparisons = [
            new("First Name", Query.FirstName.ToUpper(), Candidate.FirstName.ToUpper()),
            new("Last Name", Query.LastName.ToUpper(), Candidate.LastName.ToUpper()),
            new("Date Of Birth", Query.DateOfBirth.GetValueOrDefault().ToShortDateString(), Candidate.DateOfBirth.ToShortDateString())
        ];

        Model = new CreateParticipant.Command
        {
            Candidate = Candidate,
            CurrentUser = UserProfile!
        };

        await base.OnInitializedAsync();
    }

    private Task BackToSearch()
    {
        return OnCanceled.InvokeAsync();
    }

    private async Task EnrolCandidate()
    {
        try
        {
            loading = true;

            await _form!.Validate().ConfigureAwait(false);

            if (_form!.IsValid)
            {
                var result = await GetNewMediator().Send(Model!);

                if (result.Succeeded)
                {
                    await OnParticipantEnrolled.InvokeAsync();
                }
                else
                {
                    Snackbar.Add(result.ErrorMessage, Severity.Error);
                }
            }

        }
        finally
        {
            loading = false;
        }
    }

}
