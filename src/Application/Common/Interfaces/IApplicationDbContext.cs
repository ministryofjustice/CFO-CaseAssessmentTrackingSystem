using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Candidates;
using Cfo.Cats.Domain.Entities.Documents;
using Cfo.Cats.Domain.Entities.Participants;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Cfo.Cats.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<AuditTrail> AuditTrails { get; }
    DbSet<Tenant> Tenants { get; }

    DbSet<Contract> Contracts { get;  }

    DbSet<Location> Locations { get; }

    public DbSet<Document> Documents { get; }
    
    public DbSet<Participant> Participants { get; }

    public DbSet<KeyValue> KeyValues { get; }
    
    public DbSet<Candidate> Candidates { get; }

    public DbSet<ParticipantEnrolmentHistory> ParticipantEnrolmentHistories { get; }
    
    ChangeTracker ChangeTracker { get; }

    DbSet<DataProtectionKey> DataProtectionKeys { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
