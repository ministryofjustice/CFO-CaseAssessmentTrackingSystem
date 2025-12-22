using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public abstract class ConsentStatus : SmartEnum<ConsentStatus>
{
    public static readonly ConsentStatus PendingStatus = new Pending();
    public static readonly ConsentStatus GrantedStatus = new Granted();

    private ConsentStatus(string name, int value)
        : base(name, value)
    {
    }

    public virtual bool AllowEnrolmentLocationChange() => false;
    
    private sealed class Pending() : ConsentStatus("Pending", 0)
    {
        public override bool AllowEnrolmentLocationChange() => true;
    } 
    
    private sealed class Granted() : ConsentStatus("Granted", 1)
    {
        public override bool AllowEnrolmentLocationChange() => false;
    } 
}