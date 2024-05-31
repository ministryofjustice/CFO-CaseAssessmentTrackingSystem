using Cfo.Cats.Application.Features.Candidates.DTOs;
using Cfo.Cats.Application.Features.Participants.Commands.Enrol;
using Cfo.Cats.Server.UI.Pages.Enrolments.Search;

namespace Cfo.Cats.Server.UI.Pages.Enrolments.CandidateSearch;
public partial class CandidateSearch
{
    public List<string> Reasons { get; set; } = [];

    private CandidateDto? Candidate { get; set; }

    public void ClearErrors() => Reasons = [];

    public async Task CandidateFound(CandidateDto candidate)
    {
        Candidate = candidate;

        if (Candidate is null)
        {
            return;
        }

        var enrolment = await Mediator.Send(new EnrolParticipantCommand() { Identifier = ""});
        
        //await CaseService!.AddDummyCase(Candidate.Identifier);
        Snackbar.Add("Enrolment started!", Severity.Success);
    }

    public void OnUpdate(List<string> reasons)
    {
        Reasons = reasons.ToList();

        if (reasons is [])
        {
            selectedType = typeof(CandidateFinder);
        }
        else
        {
            selectedType = typeof(Error);
        }
        StateHasChanged();
    }

    public void NavigateToEnrolment(string participantId) 
        => NavMan.NavigateTo($"/Enrolments/{participantId}");

    private Type selectedType = typeof(CandidateFinder);

 
    private Dictionary<string, ComponentMetaData> Components =>
        new()
        {
            {
                nameof(CandidateFinder),
                new ComponentMetaData
                {
                    Name = nameof(CandidateFinder),
                    Parameters = new()
                    {
                        {
                            "OnCandidateFound",
                            EventCallback.Factory.Create<CandidateDto>(this, CandidateFound)
                        },
                        {
                            "OnUpdate",
                            EventCallback.Factory.Create<List<string>>(this, OnUpdate)
                        }
                    }
                }
            },
            {
                nameof(Success),
                new ComponentMetaData
                {
                    Name = nameof(Success),
                    Parameters = new() 
                    {
                        { "ParticipantId", Candidate?.Identifier ?? string.Empty  }, 
                        { "Successes", Reasons.OfType<string>().ToArray() } 
                    }
                }
            },
            {
                nameof(Error),
                new ComponentMetaData
                {
                    Name = nameof(Error),
                    Parameters = new() { { "Errors", Reasons.ToArray() } }
                }
            }
        };


    public class ComponentMetaData
    {
        public required string Name { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = [];
    }
}
