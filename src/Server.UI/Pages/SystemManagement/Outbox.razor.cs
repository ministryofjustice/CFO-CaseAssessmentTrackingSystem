using System.Text.Json;
using Cfo.Cats.Application.Common.Extensions;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Outbox.Commands;
using Cfo.Cats.Application.Features.Outbox.DTOs;
using Cfo.Cats.Application.Features.Outbox.Queries.PaginationQuery;
using Cfo.Cats.Application.Features.Outbox.Specifications;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.SystemManagement;

public partial class Outbox
{
    private string Title { get; set; } = "Outbox Messages";

    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = null!;

    [CascadingParameter] private UserProfile? UserProfile { get; set; }

    private OutboxMessagesWithPagination.Query Query { get; } = new();
    private MudDataGrid<OutboxMessageDto> _table = null!;
    private bool _loading;
    private int _defaultPageSize = 15;
    private readonly OutboxMessageDto _currentDto = new();

    protected override async Task OnInitializedAsync()
    {
        Title = L[_currentDto.GetClassDescription()];
        await AuthState;
    }

    private async Task OnChangedListView(OutboxMessageListView listview)
    {
        Query.ListView = listview;
        await _table.ReloadServerData();
    }

    private async Task OnRefresh()
    {
        Query.Keyword = string.Empty;
        await _table.ReloadServerData();
    }

    private async Task OnSearch(string text)
    {
        Query.Keyword = text;
        await _table.ReloadServerData();
    }

    private List<JsonRow> ParseJsonToKeyValuePairs(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<JsonRow>();
        }

        try
        {
            var jsonObject = JsonDocument.Parse(json).RootElement;
            var rows = new List<JsonRow>();

            foreach (var property in jsonObject.EnumerateObject())
            {
                rows.Add(new JsonRow
                {
                    Key = property.Name,
                    Value = property.Value.ToString()
                });
            }

            return rows;
        }
        catch
        {
            return new List<JsonRow> { new JsonRow { Key = "Error", Value = "Invalid JSON" } };
        }
    }

    private async Task<GridData<OutboxMessageDto>> ServerReload(GridState<OutboxMessageDto> state,
        CancellationToken cancellationToken)
    {
        try
        {
            _loading = true;
            Query.CurrentUser = UserProfile;
            Query.OrderBy = state.SortDefinitions.FirstOrDefault()?.SortBy ?? "OccurredOnUtc";
            Query.SortDirection = state.SortDefinitions.FirstOrDefault()?.Descending ?? true
                ? nameof(SortDirection.Descending)
                : nameof(SortDirection.Ascending);
            Query.PageNumber = state.Page + 1;
            Query.PageSize = state.PageSize;

            var result = await Mediator.Send(Query, cancellationToken);

            if (result.Succeeded)
            {
                return new GridData<OutboxMessageDto>
                    { TotalItems = result.Data!.TotalItems, Items = result.Data.Items };
            }

            Snackbar.Add(result.ErrorMessage, Severity.Error);
            return new GridData<OutboxMessageDto> { TotalItems = 0, Items = [] };
        }
        finally
        {
            _loading = false;
        }
    }

    public class JsonRow
    {
        public string Key { get; set; } = null!;
        public string Value { get; set; } = null!;
    }

    private async Task Reschedule(OutboxMessageDto message)
    {
        var mediator = GetNewMediator();
        var command = new RescheduleOutboxMessage.Command
        {
            OutboxMessageId = message.Id
        };
        var result = await mediator.Send(command);

        if (result.Succeeded)
        {
            Snackbar.Add("Rescheduled message", Severity.Success);
        }
        else
        {
            Snackbar.Add($"Failed to reschedule message {result.ErrorMessage}", Severity.Success);
        }

        await OnRefresh();
    }
}
