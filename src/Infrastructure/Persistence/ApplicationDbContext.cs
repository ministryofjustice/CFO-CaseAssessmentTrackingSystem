using System.Reflection;
using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Entities.Activities;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Assessments;
using Cfo.Cats.Domain.Entities.Documents;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Entities.Bios;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Persistence.Configurations.Enrolments;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Cfo.Cats.Domain.Entities.Inductions;
using Cfo.Cats.Domain.Entities.Notifications;
using Cfo.Cats.Infrastructure.Persistence.Configurations.ManagementInformation;
using Cfo.Cats.Domain.Entities.ManagementInformation;

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
    public DbSet<PathwayPlan> PathwayPlans => Set<PathwayPlan>();
    public DbSet<Risk> Risks => Set<Risk>();
    public DbSet<ParticipantAssessment> ParticipantAssessments => Set<ParticipantAssessment>();

    public DbSet<ParticipantBio> ParticipantBios => Set<ParticipantBio>();
    public DbSet<KeyValue> KeyValues => Set<KeyValue>();
    
    
    public DbSet<ParticipantEnrolmentHistory> ParticipantEnrolmentHistories => Set<ParticipantEnrolmentHistory>();
    public DbSet<ParticipantLocationHistory> ParticipantLocationHistories => Set<ParticipantLocationHistory>();
    public DbSet<ParticipantOwnershipHistory> ParticipantOwnershipHistories => Set<ParticipantOwnershipHistory>();

    public DbSet<Location> Locations => Set<Location>();

    public DbSet<Timeline> Timelines => Set<Timeline>();
    public DbSet<EnrolmentPqaQueueEntry> EnrolmentPqaQueue => Set<EnrolmentPqaQueueEntry>();
    public DbSet<EnrolmentQa1QueueEntry> EnrolmentQa1Queue => Set<EnrolmentQa1QueueEntry>();
    public DbSet<EnrolmentQa2QueueEntry> EnrolmentQa2Queue => Set<EnrolmentQa2QueueEntry>();
    public DbSet<EnrolmentEscalationQueueEntry> EnrolmentEscalationQueue => Set<EnrolmentEscalationQueueEntry>();

    public DbSet<PasswordHistory> PasswordHistories => Set<PasswordHistory>();

    public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();

    public DbSet<LocationMapping> LocationMappings => Set<LocationMapping>();

    public DbSet<IdentityAuditTrail> IdentityAuditTrails => Set<IdentityAuditTrail>();

    public DbSet<HubInduction> HubInductions => Set<HubInduction>();

    public DbSet<WingInduction> WingInductions => Set<WingInduction>();

    public DbSet<ParticipantAccessAuditTrail> AccessAuditTrails => Set<ParticipantAccessAuditTrail>();
    
    public DbSet<Notification> Notifications => Set<Notification>();

    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<EducationTrainingActivity> EducationTrainingActivities => Set<EducationTrainingActivity>();
    public DbSet<EmploymentActivity> EmploymentActivities => Set<EmploymentActivity>();
    public DbSet<ISWActivity> ISWActivities => Set<ISWActivity>();
    public DbSet<NonISWActivity> NonISWActivities => Set<NonISWActivity>();

    public DbSet<ParticipantIncomingTransferQueueEntry> ParticipantIncomingTransferQueue => Set<ParticipantIncomingTransferQueueEntry>();
    public DbSet<ParticipantOutgoingTransferQueueEntry> ParticipantOutgoingTransferQueue => Set<ParticipantOutgoingTransferQueueEntry>();

    public DbSet<ActivityPqaQueueEntry> ActivityPqaQueue => Set<ActivityPqaQueueEntry>();
    public DbSet<ActivityQa1QueueEntry> ActivityQa1Queue => Set<ActivityQa1QueueEntry>();
    public DbSet<ActivityQa2QueueEntry> ActivityQa2Queue => Set<ActivityQa2QueueEntry>();
    public DbSet<ActivityEscalationQueueEntry> ActivityEscalationQueue => Set<ActivityEscalationQueueEntry>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public DbSet<EnrolmentPayment> EnrolmentPayments => Set<EnrolmentPayment>();
    public DbSet<InductionPayment> InductionPayments => Set<InductionPayment>();
    public DbSet<ActivityPayment> ActivityPayments => Set<ActivityPayment>();
    public DbSet<EducationPayment> EducationPayments => Set<EducationPayment>();

    public DbSet<EmploymentPayment> EmploymentPayments => Set<EmploymentPayment>();

    public DbSet<DateDimension> DateDimensions => Set<DateDimension>();

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

