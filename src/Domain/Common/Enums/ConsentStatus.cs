using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class ConsentStatus : SmartEnum<ConsentStatus>
{
    public static readonly ConsentStatus PendingStatus = new("Pending", 0);
    public static readonly ConsentStatus GrantedStatus = new("Granted", 1);
    
    private ConsentStatus(string name, int value) : base(name, value)
    {
    }
}