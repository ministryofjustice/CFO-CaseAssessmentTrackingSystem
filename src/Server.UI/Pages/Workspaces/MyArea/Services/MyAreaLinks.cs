using Cfo.Cats.Server.UI.Models.Breadcrumb;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.MyArea.Services;

public static class MyAreaLinks
{
    public static BreadcrumbLinkModel Home => new ("My Area", "", "/pages/workspace/my-area");

    public static BreadcrumbLinkModel EditProfile = new("Profile", "View and edit profile details", $"{Home.Href}/profile");

    public static BreadcrumbLinkModel PasswordReset = new("Reset Password", "Reset your account password", $"{Home.Href}/passwordreset");

    public static BreadcrumbLinkModel MyDocuments = new ("My Documents", "List recently generated documents", $"{Home.Href}/my-documents");

}