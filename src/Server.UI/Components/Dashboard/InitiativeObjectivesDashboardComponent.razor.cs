using Cfo.Cats.Application.Common.Interfaces.Initiatives;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Application.Features.Initiatives.DTOs;

namespace Cfo.Cats.Server.UI.Components.Dashboard;

public partial class InitiativeObjectivesDashboardComponent
{
    [Parameter]
    public string? UserId { get; set; }

    [Parameter]
    public string? TenantId { get; set; }

    [Inject]
    private IInitiativeService InitiativeService { get; set; } = null!;

    private string _initiativeFilter = string.Empty;
    private string _statusFilter = string.Empty;

    private IReadOnlyList<InitiativeDto> _initiatives = [];

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _initiatives = InitiativeService.GetActiveInitiatives(CurrentUser.TenantId!).ToList();
    }

    protected override IRequest<Result<GetInitiativeObjectivesDashboard.InitiativeObjectiveRowDto[]>> CreateQuery()
        => new GetInitiativeObjectivesDashboard.Query
        {
            UserId = UserId,
            TenantId = TenantId,
            CurrentUser = CurrentUser
        };

    private IEnumerable<GetInitiativeObjectivesDashboard.InitiativeObjectiveRowDto> FilteredRows
    {
        get
        {
            if (Data is null)
            {
                return [];
            }

            var rows = Data.AsEnumerable();

            if (!string.IsNullOrEmpty(_initiativeFilter))
            {
                rows = rows.Where(r => r.InitiativeCode == _initiativeFilter);
            }

            if (_statusFilter == "active")
            {
                rows = rows.Where(r => !r.IsObjectiveCompleted);
            }
            else if (_statusFilter == "completed")
            {
                rows = rows.Where(r => r.IsObjectiveCompleted);
            }

            return rows;
        }
    }
}
