using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.Features.Dashboard.Queries;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.DeliveryManagement.Components;

public partial class ActivitiesInQaPots
{
    protected override IQuery<Result<ActivitiesInQaPotsDto>> CreateQuery()
        => new GetActivitiesInQaPots.Query
        {
            CurrentUser = CurrentUser,
            IncludeTeams = CurrentUser.AssignedRoles is { Length: > 0}
        };
}
