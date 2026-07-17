using Cfo.Cats.Server.UI.Models.Breadcrumb;
using Cfo.Cats.Server.UI.Pages.Workspaces.Account.Services;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Account.Pages;

public partial class Index
{
    private BreadcrumbLinkModel[] Links { get; set; } = [
        AccountLinks.EditProfile,
        AccountLinks.PasswordReset,
    ];
}