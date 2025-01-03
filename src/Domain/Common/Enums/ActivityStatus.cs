﻿using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;



public abstract class ActivityStatus : SmartEnum<ActivityStatus>
{
    public static readonly ActivityStatus PendingStatus = new Pending();
    public static readonly ActivityStatus SubmittedToProviderStatus = new SubmittedToProvider();
    public static readonly ActivityStatus SubmittedToAuthorityStatus = new SubmittedToAuthority();
    public static readonly ActivityStatus ApprovedStatus = new Approved();
    public static readonly ActivityStatus AbandonedStatus = new Abandoned();

    private ActivityStatus(string name, int value, bool allowsChanges = false)
    : base(name, value)
    {
        AllowsChanges = allowsChanges;
    }

    public bool AllowsChanges { get; }

    /// <summary>
    ///     Statuses that we can transition to.
    /// </summary>
    protected abstract ActivityStatus[] GetAllowedTransitions();

    public bool AllowSubmitToPqa() =>
        this == PendingStatus;

    public bool CanTransitionTo(ActivityStatus next) =>
        GetAllowedTransitions()
            .Any(e => next == e);

    private sealed class Pending() : ActivityStatus("Pending Submission", 0, allowsChanges: true)
    {
        protected override ActivityStatus[] GetAllowedTransitions() => [AbandonedStatus, SubmittedToProviderStatus, ApprovedStatus];
    }

    private sealed class SubmittedToProvider() : ActivityStatus("Submitted to Provider", 1)
    {
        protected override ActivityStatus[] GetAllowedTransitions() => [AbandonedStatus, PendingStatus, SubmittedToAuthorityStatus];
    }

    private sealed class SubmittedToAuthority() : ActivityStatus("Submitted to Authority", 2)
    {
        protected override ActivityStatus[] GetAllowedTransitions() => [AbandonedStatus, SubmittedToProviderStatus, ApprovedStatus];
    }

    private sealed class Approved() : ActivityStatus("Approved", 3)
    {
        protected override ActivityStatus[] GetAllowedTransitions() => [AbandonedStatus];
    }

    private sealed class Abandoned() : ActivityStatus("Abandoned", 4)
    {
        protected override ActivityStatus[] GetAllowedTransitions() => [ApprovedStatus];
    }
}