using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Application.Features.PRIs.Commands;
using Cfo.Cats.Application.Features.PRIs.DTOs;
using Cfo.Cats.Application.Features.PRIs.Queries;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Pages.PRIs.Components;

namespace Cfo.Cats.Server.UI.Pages.PRIs
{
    public partial class ActivePRIs
    {
        [CascadingParameter] private UserProfile? UserProfile { get; set; }

        [SupplyParameterFromQuery(Name = "ListView")]
        public string? ListView { get; set; }

        public string? Title { get; private set; }
        private int _defaultPageSize = 15;
        private HashSet<PRIPaginationDto> _selectedItems = new();
        private MudDataGrid<PRIPaginationDto> _table = default!;
        private bool _loading;

        private GetActivePRIsByUserId.Query? Query { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Title = @ConstantString.ActivePreReleaseInventoryPRI;

            Query = new GetActivePRIsByUserId.Query()
            {
                CurrentUser = UserProfile
            };

            await base.OnInitializedAsync();
        }

        private async Task<GridData<PRIPaginationDto>> ServerReload(GridState<PRIPaginationDto> state)
        {
            try
            {
                _loading = true;
                Query!.CurrentUser = UserProfile;
                Query.OrderBy = state.SortDefinitions.FirstOrDefault()?.SortBy ?? "Id";
                Query.SortDirection = state.SortDefinitions.FirstOrDefault()?.Descending ?? true ? SortDirection.Descending.ToString() : SortDirection.Ascending.ToString();
                Query.PageNumber = state.Page + 1;
                Query.PageSize = state.PageSize;
                var result = await GetNewMediator().Send(Query);
                return new GridData<PRIPaginationDto> { TotalItems = result.TotalItems, Items = result.Items };
            }
            finally
            {
                _loading = false;
            }
        }

        private async Task OnRefresh()
        {
            _selectedItems = [];
            Query!.Keyword = string.Empty;
            await _table.ReloadServerData();
        }

        private async Task OnSearch(string text)
        {
            if (_loading)
            {
                return;
            }
            _selectedItems = new();
            Query!.Keyword = text;
            await _table.ReloadServerData();
        }

        private async Task OnOutgoingChange()
        {
            if (Query!.IncludeOutgoing) // Only toggle the other off if this one is turned on
            {
                Query!.IncludeIncoming = false;
            }

            await _table.ReloadServerData();
        }

        private async Task OnIncomingChange()
        {
            if (Query!.IncludeIncoming) // Only toggle the other off if this one is turned on
            {
                Query!.IncludeOutgoing = false;
            }

            await _table.ReloadServerData();
        }

        private void ViewParticipant(PRIPaginationDto PRI)
        {
            Navigation.NavigateTo($"/pages/Participants/{PRI.ParticipantId}");
        }

        public async Task CreatePriCode()
        {
            var parameters = new DialogParameters<PriGenerateCodeDialog>()
            {
                { x => x.Model, new UpsertPriCode.Command()
                {
                    ParticipantId = ""
                } }
            };

            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, BackdropClick = false };

            var dialog = await DialogService.ShowAsync<PriGenerateCodeDialog>(ConstantString.GeneratePRICode, parameters, options);

            var state = await dialog.Result;

            if (!state!.Canceled)
            {
                await OnRefresh();
            }
        }

        private async Task AddActualReleaseDate(PRIPaginationDto PRI)
        {
            var parameters = new DialogParameters<AddActualReleaseDateDialog>()
            {
                {
                    x => x.Model, new  AddActualReleaseDate.Command()
                    {
                        ParticipantId = PRI.ParticipantId
                    }
                }
            };

            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, BackdropClick = false };

            var dialog = await DialogService.ShowAsync<AddActualReleaseDateDialog>(ConstantString.AddActualReleaseDate, parameters, options);

            var state = await dialog.Result;

            if (!state!.Canceled)
            {
                await OnRefresh();
            }
        }

        private async Task CompletePRI(PRIPaginationDto PRI)
        {
            var completePRICommand = new CompletePRI.Command()
            {
                ParticipantId = PRI.ParticipantId,
                CompletedBy = CurrentUser.UserId
            };

            var result = await GetNewMediator().Send(completePRICommand);

            if (result.Succeeded)
            {
                Snackbar.Add($"{ConstantString.PRISuccessfullyCompleted}", Severity.Info);
                await OnRefresh();
            }
            else
            {
                Snackbar.Add($"{result.ErrorMessage}", Severity.Error);
            }
        }

        private async Task AbandonPRI(PRIPaginationDto PRI)
        {
            var parameters = new DialogParameters<AbandonPriDialog>()
            {
                { x => x.Model, new AbandonPRI.Command()
                {
                    ParticipantId = PRI.ParticipantId,
                    AbandonJustification="",
                    AbandonReason=PriAbandonReason.Other,
                    AbandonedBy=CurrentUser.UserId!
                } }
            };

            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, BackdropClick = false };

            var dialog = await DialogService.ShowAsync<AbandonPriDialog>(ConstantString.AbandonPRI, parameters, options);

            var state = await dialog.Result;

            if (!state!.Canceled)
            {
                await OnRefresh();
            }
        }
    }
}