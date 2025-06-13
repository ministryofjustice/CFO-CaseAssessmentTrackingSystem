using Cfo.Cats.Application.Features.PathwayPlans.Commands;
using Cfo.Cats.Application.Features.PathwayPlans.DTOs;
using Cfo.Cats.Application.Features.PathwayPlans.Queries;
using Cfo.Cats.Server.UI.Pages.Objectives;
using FluentValidation;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components
{
    public partial class CasePathwayPlan
    {
        private bool loading;
        private bool hideCompletedObjectives = false;
        private bool hideCompletedTasks = false;

        private string selector = "Created";
        private Dictionary<string, Func<ObjectiveDto, dynamic>> selectors = new()
        {
            { "Created", (objective) => objective.Created },
            { "Title", (objective) => objective.Description },
            { "Outstanding", (objective) => objective.Tasks.Where(task => task.IsCompleted is false).Count() },
        };

        private SortDirection sortDirection = SortDirection.Ascending;

        [Parameter, EditorRequired]
        public required string ParticipantId { get; set; }

        [Parameter, EditorRequired]
        public bool ParticipantIsActive { get; set; } = default!;

        public PathwayPlanDto? Model { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await OnRefresh(firstRender: true);
            await base.OnInitializedAsync();
        }

        private async Task OnRefresh(bool firstRender = false)
        {
            loading = true;

            try
            {
                Model = await GetNewMediator().Send(new GetPathwayPlanByParticipantId.Query()
                {
                    ParticipantId = ParticipantId
                });

                if (firstRender is false)
                {
                    await OnUpdate.InvokeAsync(); // Bubble update, refreshing participant information                
                }
            }
            finally
            {
                loading = false;
            }
        }

        public async Task AddObjective()
        {
            var command = new AddObjective.Command()
            {
                PathwayPlanId = Model!.Id
            };

            var parameters = new DialogParameters<AddObjectiveDialog>()
            {
                { x => x.Model, command }
            };

            var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, CloseButton = true, BackdropClick = false };
            var dialog = await DialogService.ShowAsync<AddObjectiveDialog>("Add thematic objective", parameters, options);

            var state = await dialog.Result;

            if (state!.Canceled is false)
            {
                var result = await GetNewMediator().Send(command);

                if (result.Succeeded)
                {
                    await OnRefresh();
                }
            }
        }

        public async Task ReviewPathwayPlan()
        {
            var command = new ReviewPathwayPlan.Command()
            {
                PathwayPlanId = Model!.Id
            };

            var state = await DialogService.ShowMessageBox(
                title: "Review Pathway",
                message: "I confirm I have reviewed all objectives/tasks and they are still relevant to the participants' pathway plan.",
                options: new DialogOptions
                {
                    MaxWidth = MaxWidth.Small,
                    FullWidth = true,
                    CloseButton = true
                },
                yesText: "Review",
                cancelText: "Cancel");

            if (state is true)
            {
                var result = await GetNewMediator().Send(command);
                
                if (result.Succeeded)
                {
                    await OnRefresh();
                }
                else
                {
                    Snackbar.Add($"{result.ErrorMessage}", Severity.Error);
                }
            }
        }
    }
}