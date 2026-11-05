using Cfo.Cats.Application.Features.PRIs.Queries;
using Cfo.Cats.Application.Features.PRIs.Queries.Extensions;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.PRIs;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.PRIs;

public class PriStatusFilterTests
{
    private static PRI Created() => PRI.Create("1FAK0001A", new DateOnly(2026, 1, 1), 1, "creator", 1);

    private static PRI Accepted() => Created().AssignTo("assignee");

    private static PRI Completed() => Accepted().Complete("assignee");

    private static PRI Abandoned() => Accepted().Abandon(PriAbandonReason.Other, "justification", "assignee");

    private static IQueryable<PRI> Pris(out PRI created, out PRI accepted, out PRI completed, out PRI abandoned)
    {
        created = Created();
        accepted = Accepted();
        completed = Completed();
        abandoned = Abandoned();
        return new[] { created, accepted, completed, abandoned }.AsQueryable();
    }

    [Test]
    public void Active_ShouldReturnCreatedAndAccepted()
    {
        var query = Pris(out var created, out var accepted, out _, out _);

        query.ApplyStatusFilter(PriStatusFilter.Active).ToList()
            .ShouldBe([created, accepted], ignoreOrder: true);
    }

    [Test]
    public void Completed_ShouldReturnOnlyCompleted()
    {
        var query = Pris(out _, out _, out var completed, out _);

        query.ApplyStatusFilter(PriStatusFilter.Completed).ToList().ShouldBe([completed]);
    }

    [Test]
    public void Abandoned_ShouldReturnOnlyAbandoned()
    {
        var query = Pris(out _, out _, out _, out var abandoned);

        query.ApplyStatusFilter(PriStatusFilter.Abandoned).ToList().ShouldBe([abandoned]);
    }

    [Test]
    public void All_ShouldReturnEveryPri()
    {
        var query = Pris(out _, out _, out _, out _);

        query.ApplyStatusFilter(PriStatusFilter.All).Count().ShouldBe(4);
    }

    [Test]
    public void DefaultQueryFilter_ShouldBeActive() =>
        new ActivePRIsWithPagination.Query().StatusFilter.ShouldBe(PriStatusFilter.Active);
}
