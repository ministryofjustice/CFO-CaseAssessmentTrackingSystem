using Cfo.Cats.Application.Features.Dashboard.Queries;

namespace Cfo.Cats.Server.UI.Components.Dashboard;

public partial class CasesPerLocationPerSupportWorkerCardComponent
{
    [EditorRequired, Parameter]
    public string UserId { get; set; } = null!;

    protected override IRequest<Result<CasesPerLocationSupportWorkerDto>> CreateQuery()
     => new GetCasesPerLocationBySupportWorker.Query() { CurrentUser = this.CurrentUser, UserId = this.UserId };
}