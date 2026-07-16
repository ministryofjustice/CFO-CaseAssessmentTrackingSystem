namespace Cfo.Cats.Server.UI.Models.Breadcrumb;

public record BreadcrumbLinkModel(string Title, string Description, string Href, string? Group = null, int? Order = null)
{
    public BreadcrumbItem AsMudBreadcrumb() => new (Title, Href, false, null);
}