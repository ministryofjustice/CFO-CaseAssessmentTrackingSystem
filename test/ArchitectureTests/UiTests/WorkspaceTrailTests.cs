using Cfo.Cats.Server.UI.Services.Navigation;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Domain.ArchitectureTests.UiTests;

public class WorkspaceTrailTests
{
    [Test]
    public void Home_path_should_return_only_home()
    {
        foreach (var workspace in Enum.GetValues<Workspace>())
        {
            var home = WorkspaceRegistry.HomeOf(workspace);

            var trail = WorkspaceRegistry.TrailFor(workspace, home.Href);

            trail.Items.ShouldBe([home]);
            trail.RemainingSegments.ShouldBeEmpty();
        }
    }

    [Test]
    public void Every_registered_page_should_end_its_trail_with_itself()
    {
        foreach (var entry in WorkspaceRegistry.Entries)
        {
            var trail = WorkspaceRegistry.TrailFor(entry.Workspace, entry.Link.Href);

            trail.Items.First().ShouldBe(WorkspaceRegistry.HomeOf(entry.Workspace));
            trail.Items.Last().ShouldBe(entry.Link);
            trail.RemainingSegments.ShouldBeEmpty();
        }
    }

    [Test]
    public void Page_with_a_parent_should_include_it_before_the_page()
    {
        var entry = WorkspaceRegistry.Entries.First(e => e.Parent is not null);

        var trail = WorkspaceRegistry.TrailFor(entry.Workspace, entry.Link.Href);

        trail.Items.ShouldBe([WorkspaceRegistry.HomeOf(entry.Workspace), entry.Parent!, entry.Link]);
    }

    [Test]
    public void Path_below_a_registered_page_should_return_the_extra_segments()
    {
        var entry = WorkspaceRegistry.Entries.First();

        var trail = WorkspaceRegistry.TrailFor(entry.Workspace, entry.Link.Href + "/abc/def");

        trail.Items.Last().ShouldBe(entry.Link);
        trail.RemainingSegments.ShouldBe(["abc", "def"]);
    }

    [Test]
    public void Trailing_slash_and_casing_should_be_ignored()
    {
        var entry = WorkspaceRegistry.Entries.First();

        var trail = WorkspaceRegistry.TrailFor(entry.Workspace, entry.Link.Href.ToUpperInvariant() + "/");

        trail.Items.Last().ShouldBe(entry.Link);
    }

    [Test]
    public void Unknown_path_should_fall_back_to_home_without_throwing()
    {
        foreach (var workspace in Enum.GetValues<Workspace>())
        {
            var home = WorkspaceRegistry.HomeOf(workspace);

            var trail = WorkspaceRegistry.TrailFor(workspace, home.Href + "/not-a-real-page");

            trail.Items.ShouldBe([home]);
            trail.RemainingSegments.ShouldBe(["not-a-real-page"]);
        }
    }
}
