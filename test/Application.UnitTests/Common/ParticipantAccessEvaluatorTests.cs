using Cfo.Cats.Application.Pipeline.ValidationSpecifications;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Common;

public class ParticipantAccessEvaluatorTests
{
    private sealed class StubSpecification(int order, AccessGrant result, bool canDeny = false, List<int> calls = null)
        : AccessValidationSpecification
    {
        protected override bool CanDenyAccess => canDeny;

        public override int Order => order;

        protected override Task<AccessGrant> CheckAccessAsync(string identifier, CancellationToken cancellationToken)
        {
            calls?.Add(order);
            return Task.FromResult(result);
        }
    }

    [Test]
    public async Task No_specifications_should_not_grant()
    {
        var grant = await ParticipantAccessEvaluator.EvaluateAsync([], "P1", CancellationToken.None);

        grant.ShouldBe(AccessGrant.NotGranted);
    }

    [Test]
    public async Task Any_granting_specification_should_grant()
    {
        AccessValidationSpecification[] specs =
        [
            new StubSpecification(1, AccessGrant.NotGranted),
            new StubSpecification(2, AccessGrant.Granted)
        ];

        var grant = await ParticipantAccessEvaluator.EvaluateAsync(specs, "P1", CancellationToken.None);

        grant.ShouldBe(AccessGrant.Granted);
    }

    [Test]
    public async Task Specifications_should_run_in_order()
    {
        var calls = new List<int>();
        AccessValidationSpecification[] specs =
        [
            new StubSpecification(30, AccessGrant.NotGranted, calls: calls),
            new StubSpecification(10, AccessGrant.NotGranted, calls: calls),
            new StubSpecification(20, AccessGrant.NotGranted, calls: calls)
        ];

        await ParticipantAccessEvaluator.EvaluateAsync(specs, "P1", CancellationToken.None);

        calls.ShouldBe([10, 20, 30]);
    }

    [Test]
    public async Task Granted_should_skip_specifications_that_cannot_deny()
    {
        var calls = new List<int>();
        AccessValidationSpecification[] specs =
        [
            new StubSpecification(1, AccessGrant.Granted, calls: calls),
            new StubSpecification(2, AccessGrant.NotGranted, calls: calls)
        ];

        var grant = await ParticipantAccessEvaluator.EvaluateAsync(specs, "P1", CancellationToken.None);

        grant.ShouldBe(AccessGrant.Granted);
        calls.ShouldBe([1]);
    }

    [Test]
    public async Task Denied_should_override_an_earlier_grant_and_stop_later_grants()
    {
        AccessValidationSpecification[] specs =
        [
            new StubSpecification(1, AccessGrant.Granted),
            new StubSpecification(2, AccessGrant.Denied, canDeny: true),
            new StubSpecification(3, AccessGrant.Granted, canDeny: true)
        ];

        var grant = await ParticipantAccessEvaluator.EvaluateAsync(specs, "P1", CancellationToken.None);

        grant.ShouldBe(AccessGrant.Denied);
    }

    [Test]
    public async Task Callback_should_be_invoked_for_each_evaluated_specification()
    {
        var seen = new List<AccessGrant>();
        AccessValidationSpecification[] specs =
        [
            new StubSpecification(1, AccessGrant.NotGranted),
            new StubSpecification(2, AccessGrant.Granted)
        ];

        await ParticipantAccessEvaluator.EvaluateAsync(specs, "P1", CancellationToken.None, (_, g) => seen.Add(g));

        seen.ShouldBe([AccessGrant.NotGranted, AccessGrant.Granted]);
    }
}
