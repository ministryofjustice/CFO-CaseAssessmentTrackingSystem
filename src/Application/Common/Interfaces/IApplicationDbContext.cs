using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Assessments;
using Cfo.Cats.Domain.Entities.Documents;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Entities.Bios;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Domain.ValueObjects;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Cfo.Cats.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DatabaseFacade Database { get; }

    DbSet<AuditTrail> AuditTrails { get; }
    DbSet<Tenant> Tenants { get; }

    DbSet<Contract> Contracts { get; }

    DbSet<Location> Locations { get; }

    DbSet<Document> Documents { get; }

    DbSet<Participant> Participants { get; }

    DbSet<Risk> Risks { get; }

    DbSet<KeyValue> KeyValues { get; }

    DbSet<ParticipantAssessment> ParticipantAssessments { get; }
    DbSet<ParticipantBio> ParticipantBios { get; }
    DbSet<ParticipantEnrolmentHistory> ParticipantEnrolmentHistories { get; }

    DbSet<Timeline> Timelines { get; }

    DbSet<ApplicationUser> Users { get; }

    DbSet<EnrolmentPqaQueueEntry> EnrolmentPqaQueue { get; }
    DbSet<EnrolmentQa1QueueEntry> EnrolmentQa1Queue { get; }
    DbSet<EnrolmentQa2QueueEntry> EnrolmentQa2Queue { get; }
    DbSet<EnrolmentEscalationQueueEntry> EnrolmentEscalationQueue { get; }

    DbSet<LocationMapping> LocationMappings { get; }

    ChangeTracker ChangeTracker { get; }

    DbSet<DataProtectionKey> DataProtectionKeys { get; }

    DbSet<PasswordHistory> PasswordHistories { get; }

}

