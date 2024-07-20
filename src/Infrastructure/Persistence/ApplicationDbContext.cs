using System.Reflection;
using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Assessments;
using Cfo.Cats.Domain.Entities.Documents;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Persistence.Configurations.Enrolments;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;

namespace Cfo.Cats.Infrastructure.Persistence;

#nullable disable
public class ApplicationDbContext
    : IdentityDbContext<
        ApplicationUser,
        ApplicationRole,
        string,
        ApplicationUserClaim,
        ApplicationUserRole,
        UserLogin,
        ApplicationRoleClaim,
        ApplicationUserToken
    >,
        IApplicationDbContext,
        IDataProtectionKeyContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Tenant> Tenants => Set<Tenant>();

    public DbSet<AuditTrail> AuditTrails => Set<AuditTrail>();

    public DbSet<Contract> Contracts => Set<Contract>();

    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Participant> Participants => Set<Participant>();

    public DbSet<ParticipantAssessment> ParticipantAssessments => Set<ParticipantAssessment>();
    
    public DbSet<KeyValue> KeyValues => Set<KeyValue>();
    
    
    public DbSet<ParticipantEnrolmentHistory> ParticipantEnrolmentHistories => Set<ParticipantEnrolmentHistory>();

    public DbSet<Location> Locations => Set<Location>();

    public DbSet<Timeline> Timelines => Set<Timeline>();
    public DbSet<EnrolmentPqaQueueEntry> EnrolmentPqaQueue => Set<EnrolmentPqaQueueEntry>();
    public DbSet<EnrolmentQa1QueueEntry> EnrolmentQa1Queue => Set<EnrolmentQa1QueueEntry>();
    public DbSet<EnrolmentQa2QueueEntry> EnrolmentQa2Queue => Set<EnrolmentQa2QueueEntry>();
    public DbSet<EnrolmentEscalationQueueEntry> EnrolmentEscalationQueue => Set<EnrolmentEscalationQueueEntry>();

    public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());


        builder.ApplyGlobalFilters<ISoftDelete>(s => s.Deleted == null);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured) { }
    }
}

