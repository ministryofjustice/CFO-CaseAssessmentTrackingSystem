using Cfo.Cats.Application.SecurityConstants;
using Microsoft.AspNetCore.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Dashboard;

[Authorize(Policy = SecurityPolicies.AuthorizedUser)]
public partial class Dashboard
{
    private readonly string _title  = "Dashboard";
}
