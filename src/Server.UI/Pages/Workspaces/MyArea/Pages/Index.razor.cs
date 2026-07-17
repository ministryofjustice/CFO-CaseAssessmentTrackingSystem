using Cfo.Cats.Server.UI.Models.Breadcrumb;
using Cfo.Cats.Server.UI.Pages.Workspaces.MyArea.Services;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.MyArea.Pages;

public partial class Index
{
    private BreadcrumbLinkModel[] Links { get; set; } = [
        MyAreaLinks.EditProfile,
        MyAreaLinks.PasswordReset,
        MyAreaLinks.MyDocuments,
    ];
}