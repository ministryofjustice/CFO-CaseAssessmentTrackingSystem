using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;

namespace Cfo.Cats.Server.UI.Pages.Histories
{
    public partial class ParticipantActiveHistory
    {
        //private bool isLoadingPreviousActivities = true;
        private IEnumerable<ParticipantEnrolmentHistoryDto> _participantEnrolmentHistory = Enumerable.Empty<ParticipantEnrolmentHistoryDto>();

        //[CascadingParameter] public UserProfile UserProfile { get; set; } = default!;

        [Parameter]
        public string Upci { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            if (!string.IsNullOrEmpty(Upci))
            {
                var query = new GetParticipantEnrolmentHistory.Query()
                {
                    ParticipantId = Upci
                };

                _participantEnrolmentHistory = await GetNewMediator().Send(query);               
            }
        }
    }
}