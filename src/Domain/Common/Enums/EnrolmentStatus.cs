using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public abstract class EnrolmentStatus : SmartEnum<EnrolmentStatus>
{
    public static readonly EnrolmentStatus IdentifiedStatus = new Identified();
    public static readonly EnrolmentStatus EnrollingStatus = new Enrolling();
    public static readonly EnrolmentStatus SubmittedToProviderStatus = new SubmittedToProvider();
    public static readonly EnrolmentStatus SubmittedToAuthorityStatus = new SubmittedToAuthority();
    public static readonly EnrolmentStatus ApprovedStatus = new Approved();
    public static readonly EnrolmentStatus ArchivedStatus = new Archived();
    public static readonly EnrolmentStatus DormantStatus = new Dormant();


    private EnrolmentStatus(string name, int value)
        : base(name, value)
    {
    }

    /// <summary>
    ///     Statuses that we can transition to.
    /// </summary>
    protected abstract EnrolmentStatus[] GetAllowedTransitions();

    /// <summary>
    /// Indicates this status allows archiving.
    /// </summary>
    public bool AllowArchive() =>
        CanTransitionTo(ArchivedStatus);

    /// <summary>
    /// Indicates the case can be made dormant
    /// </summary>
    public bool AllowSuspend() =>
        CanTransitionTo(DormantStatus);

    public bool AllowSubmitToPqa() =>
        CanTransitionTo(SubmittedToProviderStatus);

    public virtual bool AllowEnrolmentLocationChange()
        => false;
        
    public bool CanTransitionTo(EnrolmentStatus next) =>
        GetAllowedTransitions()
            .Any(e => next == e);

    /// <summary>
    ///     Indicates that a participant at this enrolment stage is allowed to have a new assessment created
    /// </summary>
    /// <returns>True if the current status allows reassessment</returns>
    public virtual bool SupportsReassessment() => true;

    /// <summary>
    ///     Indicates we can add right to work when the status is at this stage
    /// </summary>
    /// <returns>True if we allow addition of Right to Work documentation</returns>
    public virtual bool AllowRightToWorkAddition() => false;

    private sealed class Identified() : EnrolmentStatus("Identified", 0)
    {
        protected override EnrolmentStatus[] GetAllowedTransitions() => [ArchivedStatus, EnrollingStatus];
    }

    private sealed class SubmittedToProvider() : EnrolmentStatus("Submitted to Provider", 1)
    {
        protected override EnrolmentStatus[] GetAllowedTransitions() => [ArchivedStatus, EnrollingStatus, SubmittedToAuthorityStatus];

        public override bool SupportsReassessment() => false;

        public override bool AllowEnrolmentLocationChange() => true;
    }

    private sealed class SubmittedToAuthority() : EnrolmentStatus("Submitted to Authority", 2)
    {
        public override bool SupportsReassessment() => false;

        protected override EnrolmentStatus[] GetAllowedTransitions() => [SubmittedToProviderStatus, ApprovedStatus];
    }

    private sealed class Approved() : EnrolmentStatus("Approved", 3)
    {
        protected override EnrolmentStatus[] GetAllowedTransitions() => [ArchivedStatus, DormantStatus];

        public override bool AllowRightToWorkAddition() => true;
    }

    private sealed class Archived() : EnrolmentStatus("Archived", 4)
    {
        protected override EnrolmentStatus[] GetAllowedTransitions() => [IdentifiedStatus];
    }

    private sealed class Dormant() : EnrolmentStatus("Dormant", 5)
    {
        protected override EnrolmentStatus[] GetAllowedTransitions() => [ArchivedStatus, ApprovedStatus];
    }

    private sealed class Enrolling() : EnrolmentStatus("Enrolling", 6)
    {
        protected override EnrolmentStatus[] GetAllowedTransitions() => [ArchivedStatus, SubmittedToProviderStatus];

        public override bool AllowRightToWorkAddition() => true;
        public override bool AllowEnrolmentLocationChange() => true;
    }

  
}