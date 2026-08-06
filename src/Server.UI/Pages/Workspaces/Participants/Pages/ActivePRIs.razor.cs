using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Application.Features.PRIs.Commands;
using Cfo.Cats.Application.Features.PRIs.DTOs;
using Cfo.Cats.Application.Features.PRIs.Queries;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Pages.PRIs.Components;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Pages;

public partial class ActivePRIs
{
    [CascadingParameter] private UserProfile? UserProfile { get; set; }

    [SupplyParameterFromQuery(Name = "ListView")]
    public string? ListView { get; set; }

    private string? Title { get; set; }
    private int _defaultPageSize = 15;
    private HashSet<PRIPaginationDto> _selectedItems = [];
    private bool _loading;
    private bool _downloading;
    private int _totalItems;
    private int _totalPages;
    private int _currentPage = 1;
    private PRIPaginationDto[] _data = [];
    private bool Tabular { get; set; } = true;

    private GetActivePRIsByUserId.Query? Query { get; set; }
    private PriTypeFilter _priTypeFilter = PriTypeFilter.All;

    protected override async Task OnInitializedAsync()
    {
        Title = @ConstantString.ActivePreReleaseInventoryPRI;

        Query = new GetActivePRIsByUserId.Query
        {
            CurrentUser = UserProfile,
            IncludeOutgoing = true,
            IncludeIncoming = true,
            PageNumber = 1,
            PageSize = _defaultPageSize,
            OrderBy = "Id",
            SortDirection = "Descending"
        };

        await OnRefresh();
        await base.OnInitializedAsync();
    }
    
    private async Task OnRefresh()
    {
        _loading = true;
        try
        {
            Query!.CurrentUser = UserProfile;
            var result = await GetNewMediator().Send(Query);

            if (result is { Succeeded: true, Data: not null })
            {
                _data = result.Data.Items.ToArray();
                _totalPages = result.Data.TotalPages;
                _totalItems = result.Data.TotalItems;
                _currentPage = Query.PageNumber;
            }
            else
            {
                _data = [];
                _totalPages = 0;
                _totalItems = 0;
                if (!string.IsNullOrEmpty(result.ErrorMessage))
                {
                    Snackbar.Add(result.ErrorMessage, Severity.Error);
                }
            }
        }
        finally
        {
            _loading = false;
        }
    }
    
    private async Task TabularChanged(bool? tabular)
    {
        Tabular = tabular ?? true;
        // Data is already loaded, just switching views
        await Task.CompletedTask;
    }
    
    private async Task PageChanged(int page)
    {
        Query!.PageNumber = page;
        await OnRefresh();
    }
    
    private async Task OnExport()
    {
        try
        {
            _downloading = true;

            var result = await GetNewMediator().Send(new ExportActivePRIs.Command
            {
                Request = new ExportActivePRIs.ActivePRIsExportRequest
                {
                    UserId = UserProfile?.UserId,
                    Keyword = Query!.Keyword,
                    IncludeOutgoing = Query.IncludeOutgoing,
                    IncludeIncoming = Query.IncludeIncoming,
                    OrderBy = Query.OrderBy,
                    SortDirection = Query.SortDirection
                }
            });

            if (result.Succeeded)
            {
                Snackbar.Add(ConstantString.ExportSuccess, Severity.Info);
            }
            else
            {
                Snackbar.Add(result.ErrorMessage, Severity.Error);
            }
        }
        catch (Exception)
        {
            Snackbar.Add("An error has occurred while generating the PRIs export.", Severity.Error);
        }
        finally
        {
            _downloading = false;
        }
    }

    private async Task OnSearch(string? text)
    {
        if (_loading)
        {
            return;
        }
        
        _selectedItems = [];
        Query!.Keyword = text ?? string.Empty;
        Query.PageNumber = 1; // Reset to first page on search
        await OnRefresh();
    }

    private string GetPriTypeLabel() =>
        _priTypeFilter switch
        {
            PriTypeFilter.Outgoing => "Outgoing Only",
            PriTypeFilter.Incoming => "Incoming Only",
            _ => "All"
        };

    private async Task SetPriType(PriTypeFilter filter)
    {
        _priTypeFilter = filter;
        
        Query!.IncludeOutgoing = filter is PriTypeFilter.All or PriTypeFilter.Outgoing;
        Query!.IncludeIncoming = filter is PriTypeFilter.All or PriTypeFilter.Incoming;
        Query.PageNumber = 1; // Reset to first page on filter change
        
        await OnRefresh();
    }

    private enum PriTypeFilter
    {
        All,
        Outgoing,
        Incoming
    }

    private void ViewParticipant(PRIPaginationDto pri) => Navigation.NavigateTo($"/pages/workspace/participants/{pri.ParticipantId}?from=activepri");

    private async Task CreatePriCode()
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

    private async Task AddActualReleaseDate(PRIPaginationDto pri)
    {
        var parameters = new DialogParameters<AddActualReleaseDateDialog>()
        {
            {
                x => x.Model, new  AddActualReleaseDate.Command()
                {
                    ParticipantId = pri.ParticipantId
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

    private async Task CompletePri(PRIPaginationDto pri)
    {
        var completePriCommand = new CompletePRI.Command()
        {
            ParticipantId = pri.ParticipantId,
            CompletedBy = CurrentUser.UserId
        };

        var result = await GetNewMediator().Send(completePriCommand);

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

    private async Task AbandonPri(PRIPaginationDto pri)
    {
        var parameters = new DialogParameters<AbandonPriDialog>()
        {
            { x => x.Model, new AbandonPRI.Command()
            {
                ParticipantId = pri.ParticipantId,
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
