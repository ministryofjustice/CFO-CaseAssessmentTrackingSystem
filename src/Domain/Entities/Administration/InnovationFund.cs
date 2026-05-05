using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Events;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Domain.Entities.Administration;

public class InnovationFund : BaseAuditableEntity<Guid>, ILifetime
{
    private string _contractId = null!;

#pragma warning disable CS8618
    private InnovationFund()
    {
    }
#pragma warning restore CS8618

    private InnovationFund(string code, string description, string contractId, DateTime startDate, DateTime endDate)
    {
        Id = Guid.NewGuid();
        Code = code;
        Description = description;
        _contractId = contractId;
        Lifetime = new Lifetime(startDate, endDate);

        AddDomainEvent(new InnovationFundCreatedDomainEvent(this));
    }

    public static InnovationFund Create(string code, string description, string contractId, DateTime startDate, DateTime endDate)
        => new(code, description, contractId, startDate, endDate);

    public string Code { get; private set; }

    public string Description { get; private set; }

    public Lifetime Lifetime { get; private set; }

    public virtual Contract? Contract { get; private set; }

    public void Edit(string code, string description, string contractId, DateTime startDate, DateTime endDate)
    {
        Code = code;
        Description = description;
        _contractId = contractId;
        Lifetime = new Lifetime(startDate, endDate);
    }
}
