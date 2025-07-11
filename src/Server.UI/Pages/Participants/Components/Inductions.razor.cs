using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Inductions.Commands;
using Cfo.Cats.Application.Features.Inductions.DTOs;
using Cfo.Cats.Application.Features.Inductions.Queries;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Domain.Common.Enums;
using FluentValidation;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components
{
    public partial class Inductions
    {
        [Inject]
        private ILocationService LocationService { get; set; } = default!;

        [Parameter, EditorRequired]
        public string ParticipantId { get; set; } = default!;

        [Parameter, EditorRequired]
        public bool ParticipantIsActive { get; set; } = default!;

        [CascadingParameter]
        public UserProfile? CurrentUser { get; set; }

        private HubInductionDto[]? HubInductions { get; set; }
        private LocationDto[] HubLocations { get; set; } = [];

        public WingInductionDto[] WingInductions { get; set; } = [];
        private LocationDto[] WingLocations { get; set; } = [];

        protected override async Task OnInitializedAsync() => await OnRefresh();

        public async Task AddHubInduction()
        {
            var parameters = new DialogParameters<AddHubInductionDialog>
        {
            { x => x.Model, new AddHubInduction.Command()
            {
                ParticipantId = ParticipantId,
                CurrentUser = CurrentUser
            } },
            { x => x.Locations, this.HubLocations }
        };

            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };

            var dialog = await DialogService.ShowAsync<AddHubInductionDialog>
                ("Add a Hub Induction", parameters, options);

            var state = await dialog.Result;

            if (!state!.Canceled)
            {
                await OnRefresh();
            }
        }

        public async Task AddWingInduction()
        {
            var parameters = new DialogParameters<AddWingInductionDialog>
        {
            { x => x.Model, new AddWingInduction.Command()
            {
                ParticipantId = ParticipantId,
                CurrentUser = CurrentUser
            } },
            { x => x.Locations, this.WingLocations }
        };

            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };

            var dialog = await DialogService.ShowAsync<AddWingInductionDialog>
                ("Add a Wing Induction", parameters, options);

            var state = await dialog.Result;

            if (!state!.Canceled)
            {
                await OnRefresh();
            }
        }

        private async Task AddPhase(WingInductionDto induction)
        {
            var parameters = new DialogParameters<AddInductionPhaseDialog>()
        {
            {
                x => x.Model, new AddInductionPhase.Command()
                {
                    WingInductionId = induction.Id,
                    CurrentUser = CurrentUser,
                }
            },
            {
                x => x.EarliestStartDate,
                induction.InductionDate
            }
        };

            var options = new DialogOptions()
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true
            };
            var dialog = await DialogService.ShowAsync<AddInductionPhaseDialog>
                ("Add a Wing Phase", parameters, options);

            var state = await dialog.Result;

            if (!state!.Canceled)
            {
                await OnRefresh();
            }
        }

        private async Task CompletePhase(WingInductionDto induction)
        {
            DateTime earliestCompletionDate = induction.Phases.Max(x => x.StartDate);

            var parameters = new DialogParameters<CompletePhaseDialog>()
        {
            {
                x => x.Model, new CompleteInductionPhase.Command()
                {
                    WingInductionId = induction.Id,
                    CurrentUser = CurrentUser,
                }
            },
            {
                x => x.EarliestCompletionDate,
                earliestCompletionDate
            }
        };

            var options = new DialogOptions()
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true
            };
            var dialog = await DialogService.ShowAsync<CompletePhaseDialog>
                ("Complete Wing Phase", parameters, options);

            var state = await dialog.Result;

            if (!state!.Canceled)
            {
                await OnRefresh();
            }
        }

        private async Task AbandonPhase(WingInductionDto induction)
        {
            DateTime earliestCompletionDate = induction.Phases.Max(x => x.StartDate);

            var parameters = new DialogParameters<AbandonPhaseDialog>()
        {
            {
                x => x.Model, new AbandonInductionPhase.Command()
                {
                    WingInductionId = induction.Id,
                    CurrentUser = CurrentUser,
                    CompletionDate=null,
                    AbandonJustification="",
                    AbandonReason=WingInductionPhaseAbandonReason.Other,
                }
            },
            {
                x => x.EarliestCompletionDate,
                earliestCompletionDate
            }
        };

            var options = new DialogOptions()
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true
            };

            var dialog = await DialogService.ShowAsync<AbandonPhaseDialog>
                ("Abandon Wing Phase", parameters, options);

            var state = await dialog.Result;

            if (!state!.Canceled)
            {
                await OnRefresh();
            }
        }

        private async Task OnRefresh()
        {
            var mediator = GetNewMediator();
            var hubQuery = new GetInductionsByParticipantId.Query()
            {
                ParticipantId = ParticipantId
            };

            var results = await mediator.Send(hubQuery);

            if (results.Succeeded)
            {
                HubInductions = results.Data!.HubInductions;
                WingInductions = results.Data!.WingInductions;
            }
            var locations = LocationService.GetVisibleLocations(CurrentUser?.TenantId ?? string.Empty)
                .ToArray();
            HubLocations = locations.Where(x => x.LocationType.IsHub).ToArray();
            WingLocations = locations.Where(x => x.LocationType == LocationType.Wing).ToArray();
        }
    }
}