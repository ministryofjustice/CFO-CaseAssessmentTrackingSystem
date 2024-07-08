using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Assessments;
using Cfo.Cats.Domain.Entities.Documents;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Identity;
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
    
    public DbSet<ParticipantAssessment> ParticipantAssessments { get; }
    
    public DbSet<ParticipantEnrolmentHistory> ParticipantEnrolmentHistories { get; }
    
    public DbSet<ApplicationUser> Users { get; }
    
    ChangeTracker ChangeTracker { get; }

    DbSet<DataProtectionKey> DataProtectionKeys { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
