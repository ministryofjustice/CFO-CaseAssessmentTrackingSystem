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

    public static IReadOnlyCollection<EnrolmentStatus> ActiveList =>
        List.Where(e => e.ParticipantIsActive())
        .ToList();

    private EnrolmentStatus(string name, int value, int logicalOrder)
        : base(name, value) => LogicalOrder = logicalOrder;

    /// <summary>
    /// The logical order (i.e. where in the expected workflow this status stands)
    /// </summary>
    public int LogicalOrder { get;}

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
    /// Indicates this status allows Unarchiving.
    /// </summary>
    public bool AllowUnarchive() => 
        this==ArchivedStatus;

    /// <summary>
    /// Indicates the case can be made dormant
    /// </summary>
    public bool AllowSuspend() =>
        CanTransitionTo(DormantStatus);

    public bool AllowSubmitToPqa() =>
        this == EnrollingStatus;

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
    ///     Indicates that a participant at this enrolment stage is allowed to have assessment access.
    /// </summary>
    /// <returns>True if the current status allows user to add/continue/reassessment</returns>
    public virtual bool ParticipantIsActive() => true;

    /// <summary>
    ///     Indicates we can add right to work when the status is at this stage
    /// </summary>
    /// <returns>True if we allow addition of Right to Work documentation</returns>
    public virtual bool AllowRightToWorkAddition() => false;

    private sealed class Identified() : EnrolmentStatus("Identified", 0, 0)
    {
        protected override EnrolmentStatus[] GetAllowedTransitions() => [ArchivedStatus, EnrollingStatus];
    }

    private sealed class SubmittedToProvider() : EnrolmentStatus("Submitted to Provider", 1, 2)
    {
        protected override EnrolmentStatus[] GetAllowedTransitions() => [EnrollingStatus, SubmittedToAuthorityStatus];
        public override bool SupportsReassessment() => false;

        public override bool AllowEnrolmentLocationChange() => true;
    }

    private sealed class SubmittedToAuthority() : EnrolmentStatus("Submitted to Authority", 2, 3)
    {
        public override bool SupportsReassessment() => false;

        protected override EnrolmentStatus[] GetAllowedTransitions() => [SubmittedToProviderStatus, ApprovedStatus];
    }

    private sealed class Approved() : EnrolmentStatus("Approved", 3, 4)
    {
        protected override EnrolmentStatus[] GetAllowedTransitions() => [ArchivedStatus, DormantStatus];

        public override bool AllowRightToWorkAddition() => true;
    }

    private sealed class Archived() : EnrolmentStatus("Archived", 4, 5)
    {
        public override bool ParticipantIsActive() => false;        

        protected override EnrolmentStatus[] GetAllowedTransitions() => [IdentifiedStatus, EnrollingStatus, ApprovedStatus];
    }

    private sealed class Dormant() : EnrolmentStatus("Dormant", 5, 6)
    {
        protected override EnrolmentStatus[] GetAllowedTransitions() => [ArchivedStatus, ApprovedStatus];
    }

    private sealed class Enrolling() : EnrolmentStatus("Enrolling", 6, 1)
    {
        protected override EnrolmentStatus[] GetAllowedTransitions() => [ArchivedStatus, SubmittedToProviderStatus];

        public override bool AllowRightToWorkAddition() => true;
        public override bool AllowEnrolmentLocationChange() => true;
    } 
}