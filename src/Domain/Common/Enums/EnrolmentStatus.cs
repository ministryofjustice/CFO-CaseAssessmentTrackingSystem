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
        : base(name, value) { }

    /// <summary>
    /// Statuses that we can transition to.
    /// </summary>
    protected abstract EnrolmentStatus[] GetAllowedTransitions();

    /// <summary>
    /// Indicates the status of the enrolment is at a QA stage.
    /// </summary>
    /// <returns></returns>
    public virtual bool IsQaStage()
    {
        return false;
    }

    public virtual bool CanTransitionTo(EnrolmentStatus next)
    {
        return GetAllowedTransitions()
            .Any(e => next == e);
    }

    private sealed class Identified : EnrolmentStatus
    {

        public Identified()
            : base("Identified", 0) { }

        protected override EnrolmentStatus[] GetAllowedTransitions() 
            => [ ArchivedStatus, EnrollingStatus];
    }

    private sealed class SubmittedToProvider : EnrolmentStatus
    {
        public SubmittedToProvider()
            : base("Submitted to Provider", 1) { }

        protected override EnrolmentStatus[] GetAllowedTransitions()
            => [ArchivedStatus, EnrollingStatus, SubmittedToAuthorityStatus];
        
        public override bool StatusSupportsReassessment() => false;

        public override bool IsQaStage() => true;

    }

    private sealed class SubmittedToAuthority : EnrolmentStatus
    { 
        public SubmittedToAuthority()
            : base("Submitted to Authority", 2) { }

        public override bool StatusSupportsReassessment() => false;

        public override bool IsQaStage() => true;

        protected override EnrolmentStatus[] GetAllowedTransitions() => 
            [ SubmittedToProviderStatus, ApprovedStatus ];
    }

    private sealed class Approved : EnrolmentStatus
    { 
        public Approved() 
            : base("Approved", 3) { }

        protected override EnrolmentStatus[] GetAllowedTransitions() =>
            [ArchivedStatus, DormantStatus];

    }

    private sealed class Archived : EnrolmentStatus
    { 
        public Archived() 
            : base("Archived", 4) { }

        protected override EnrolmentStatus[] GetAllowedTransitions() =>
            [IdentifiedStatus];

    }

    private sealed class Dormant : EnrolmentStatus 
    {
        public Dormant() 
            : base("Dormant", 5) { }

        protected override EnrolmentStatus[] GetAllowedTransitions() =>
            [ ArchivedStatus, ApprovedStatus ];
    }

    private sealed class Enrolling : EnrolmentStatus
    {
        public Enrolling()
            : base("Enrolling", 6) { }

        protected override EnrolmentStatus[] GetAllowedTransitions()
            => [ArchivedStatus, SubmittedToProviderStatus];
    }

    /// <summary>
    /// Indicates that a participant at this enrolment stage is allowed to have a new assessment created
    /// </summary>
    public virtual bool StatusSupportsReassessment() => true;
}

