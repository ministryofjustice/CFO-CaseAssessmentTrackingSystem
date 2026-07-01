using Cfo.Cats.Application.SecurityConstants;
using Microsoft.AspNetCore.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Pages;

[Authorize(Policy = SecurityPolicies.SystemSupportFunctions)]
public partial class CacheManagement
{
}