using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Assessments;
using Cfo.Cats.Domain.Entities.Documents;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Domain.ValueObjects;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Cfo.Cats.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    public DatabaseFacade Database { get; }
    
    DbSet<AuditTrail> AuditTrails { get; }
    DbSet<Tenant> Tenants { get; }

    DbSet<Contract> Contracts { get;  }

    DbSet<Location> Locations { get; }

    public DbSet<Document> Documents { get; }
    
    public DbSet<Participant> Participants { get; }

    public DbSet<Risk> Risks { get; }

    public DbSet<KeyValue> KeyValues { get; }
    
    public DbSet<ParticipantAssessment> ParticipantAssessments { get; }
    
    public DbSet<ParticipantEnrolmentHistory> ParticipantEnrolmentHistories { get; }

    public DbSet<Timeline> Timelines { get; }
    
    public DbSet<ApplicationUser> Users { get; }
    
    public DbSet<EnrolmentPqaQueueEntry> EnrolmentPqaQueue { get; }
    public DbSet<EnrolmentQa1QueueEntry> EnrolmentQa1Queue { get; }
    public DbSet<EnrolmentQa2QueueEntry> EnrolmentQa2Queue { get; }
    public DbSet<EnrolmentEscalationQueueEntry> EnrolmentEscalationQueue { get; }
    
    ChangeTracker ChangeTracker { get; }

    DbSet<DataProtectionKey> DataProtectionKeys { get; }

}

