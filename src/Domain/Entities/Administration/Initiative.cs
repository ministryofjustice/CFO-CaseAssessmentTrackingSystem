using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Events;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Domain.Entities.Administration;

public class Initiative : BaseAuditableEntity<Guid>, ILifetime
{
    private string _contractId = null!;

#pragma warning disable CS8618
    private Initiative()
    {
    }
#pragma warning restore CS8618

    private Initiative(string code, string description, string contractId, DateTime startDate, DateTime endDate)
    {
        Id = Guid.CreateVersion7();
        Code = code;
        Description = description;
        _contractId = contractId;
        Lifetime = new Lifetime(startDate, endDate);

        AddDomainEvent(new InitiativeCreatedDomainEvent(this));
    }

    public static Initiative Create(string code, string description, string contractId, DateTime startDate, DateTime endDate)
        => new(code, description, contractId, startDate, endDate);

    public string Code { get; private set; }

    public string Description { get; private set; }

    public Lifetime Lifetime { get; private set; }

    public virtual Contract? Contract { get; private set; }

    public void Edit(string code, string description, string contractId)
    {
        Code = code;
        Description = description;
        _contractId = contractId;
    }

    public void AmendLifetime(DateTime startDate, DateTime endDate)
        => Lifetime = new Lifetime(startDate, endDate);

    public void Deactivate()
        => Lifetime = new Lifetime(Lifetime.StartDate, DateTime.UtcNow);
}
