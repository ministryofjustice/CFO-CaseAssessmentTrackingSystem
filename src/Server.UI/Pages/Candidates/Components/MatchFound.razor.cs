
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Candidates.DTOs;
using Cfo.Cats.Application.Features.Candidates.Queries.Search;
using Cfo.Cats.Application.Features.Participants.Commands;

namespace Cfo.Cats.Server.UI.Pages.Candidates.Components;

public partial class MatchFound
{
    [Inject]
    private IPicklistService PicklistService { get; set; } = default!;

    private MudForm? _form;

    private CreateParticipant.Command? Model;

    private CandidateDto? candidate;

    private bool _confirmation = false;
    private List<ComparisonRow>? _comparisons;

    [Parameter]
    [EditorRequired]
    public string CandidateId { get; set; } = default!;

    [CascadingParameter]
    public UserProfile? UserProfile { get; set; }

    [Parameter]
    public CandidateSearchQuery Query { get; set; } = default!;

    [Parameter]
    public EventCallback OnCanceled { get; set; }

    [Parameter]
    public EventCallback OnParticipantEnrolled { get; set; }

    [Inject]
    public ICandidateService CandidateService { get; set; } = default!;


    protected override async Task OnParametersSetAsync()
    {
        candidate = await CandidateService.GetByUpciAsync(CandidateId)
            ?? throw new ApplicationException("We found a candidate, but then could not get it");

        _comparisons = [
            new("First Name", Query.FirstName.ToUpper(), candidate.FirstName.ToUpper()),
            new("Last Name", Query.LastName.ToUpper(), candidate.LastName.ToUpper()),
            new("Date Of Birth", Query.DateOfBirth.GetValueOrDefault().ToShortDateString(), candidate.DateOfBirth.ToShortDateString())
        ];

        Model = new CreateParticipant.Command
        {
            Candidate = candidate,
            CurrentUser = UserProfile!
        };
    }

    private Task BackToSearch()
    {
        return OnCanceled.InvokeAsync();
    }

    private async Task EnrolCandidate()
    {
        await _form!.Validate().ConfigureAwait(false);
        if (_form!.IsValid)
        {
            await GetNewMediator().Send(Model!);
            await OnParticipantEnrolled.InvokeAsync();
        }
    }

}
