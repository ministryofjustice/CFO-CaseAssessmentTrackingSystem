using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public abstract class EnrolmentStatus : SmartEnum<EnrolmentStatus>
{

    public static readonly EnrolmentStatus PendingStatus = new Pending();
    public static readonly EnrolmentStatus SubmittedToProviderStatus = new SubmittedToProvider();
    public static readonly EnrolmentStatus SubmittedToAuthorityStatus = new SubmittedToAuthority();
    public static readonly EnrolmentStatus ApprovedStatus = new Approved();
    public static readonly EnrolmentStatus AbandonedStatus = new Abandoned();


    private EnrolmentStatus(string name, int value)
        : base(name, value) { }

    /// <summary>
    /// Statuses that we can transition to.
    /// </summary>
    protected abstract EnrolmentStatus[] GetAllowedTransitions();

    public virtual bool CanTransitionTo(EnrolmentStatus next)
    {
        return GetAllowedTransitions()
            .Any(e => next == e);
    }

    private sealed class Pending : EnrolmentStatus
    {

        public Pending()
            : base(nameof(Pending), 0) { }

        protected override EnrolmentStatus[] GetAllowedTransitions() 
            => [ AbandonedStatus, SubmittedToProviderStatus];
    }

    private sealed class SubmittedToProvider : EnrolmentStatus
    {
        public SubmittedToProvider()
            : base(nameof(SubmittedToProvider), 1) { }

        protected override EnrolmentStatus[] GetAllowedTransitions()
            => [AbandonedStatus, PendingStatus, SubmittedToProviderStatus];
    }

    private sealed class SubmittedToAuthority : EnrolmentStatus
    { 
        public SubmittedToAuthority(): base(nameof(SubmittedToAuthority), 2) { }

        protected override EnrolmentStatus[] GetAllowedTransitions() => 
            [ SubmittedToProviderStatus, ApprovedStatus ];
    }

    private sealed class Approved : EnrolmentStatus
    { 
        public Approved() :base(nameof(Approved), 3)
        { }

        protected override EnrolmentStatus[] GetAllowedTransitions() =>
            [AbandonedStatus];

    }

    private sealed class Abandoned : EnrolmentStatus
    { 
        public Abandoned() : base(nameof(Abandoned), 4) { }

        protected override EnrolmentStatus[] GetAllowedTransitions() =>
            [PendingStatus];

    }

}
