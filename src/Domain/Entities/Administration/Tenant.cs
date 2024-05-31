﻿using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.Administration;

public class Tenant : BaseAuditableEntity<string>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Tenant()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private Tenant(string id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
        
        AddDomainEvent(new TenantCreatedEvent(this));
    }

    public static Tenant Create(string id, string name, string description)
    {
        return new Tenant(id, name, description);
    }

    public string? Name { get; private set; }
    public string? Description { get; private set; }
}
