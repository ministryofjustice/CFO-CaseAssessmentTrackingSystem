using Cfo.Cats.Application.Common.Interfaces.Contracts;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Contracts.DTOs;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.DeliveryManagement.Pages;

public partial class ActivitiesInQaPots
{
    [CascadingParameter]
    public UserProfile CurrentUser { get; set; } = null!;

    [Inject]
    private IContractService ContractService { get; set; } = null!;

    private ContractDto? SelectedContract { get; set; }

    private IEnumerable<ContractDto> VisibleContracts => CurrentUser.TenantId is { Length: > 0 } tenantId
        ? ContractService.GetVisibleContracts(tenantId)
        : [];
}
