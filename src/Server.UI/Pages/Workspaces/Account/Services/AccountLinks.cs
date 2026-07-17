using Cfo.Cats.Server.UI.Models.Breadcrumb;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Account.Services;

public static class AccountLinks
{
    public static BreadcrumbLinkModel Home => new ("Account", "", "/pages/workspace/account");

    public static BreadcrumbLinkModel EditProfile = new("My Profile", "View and edit profile details", $"{Home.Href}/profile");

    public static BreadcrumbLinkModel PasswordReset = new("Reset Password", "Reset your account password", $"{Home.Href}/passwordreset");

}