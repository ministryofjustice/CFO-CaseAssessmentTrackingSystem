using Cfo.Cats.Domain.Common.Entities;

namespace Cfo.Cats.Domain.Entities.Participants;

public class Supervisor : BaseAuditableEntity<Guid>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Supervisor()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public string? Name { get; set; }
    public string? TelephoneNumber { get; set; }
    public string? MobileNumber { get; set; }
    public string? EmailAddress { get; set; }
    public string? Address { get; set; }

    public static Supervisor CreateFrom(string? name, string? telephoneNumber, string? mobileNumber, string? emailAddress, string? address)
    {
        Supervisor s = new()
        {
            Name = name,
            TelephoneNumber = telephoneNumber,
            MobileNumber = mobileNumber,
            EmailAddress = emailAddress,
            Address = address
        };

        return s;
    }

}
