using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Domain.Entities.Activities;
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
using Cfo.Cats.Domain.Entities.Inductions;
using Cfo.Cats.Domain.Entities.Notifications;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using Cfo.Cats.Domain.Entities.PRIs;

namespace Cfo.Cats.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DatabaseFacade Database { get; }

    DbSet<AuditTrail> AuditTrails { get; }
    DbSet<Tenant> Tenants { get; }

    DbSet<Contract> Contracts { get; }

    DbSet<Location> Locations { get; }

    DbSet<Document> Documents { get; }
    DbSet<GeneratedDocument> GeneratedDocuments { get; }

    DbSet<Participant> Participants { get; }
    
    DbSet<PathwayPlan> PathwayPlans { get; }

    DbSet<Risk> Risks { get; }

    DbSet<KeyValue> KeyValues { get; }

    DbSet<ParticipantAssessment> ParticipantAssessments { get; }
    DbSet<ParticipantBio> ParticipantBios { get; }
    DbSet<ParticipantEnrolmentHistory> ParticipantEnrolmentHistories { get; }
    DbSet<ParticipantLocationHistory> ParticipantLocationHistories { get; }
    DbSet<ParticipantOwnershipHistory> ParticipantOwnershipHistories { get; }

    DbSet<ParticipantContactDetail> ParticipantContactDetails { get; }

    DbSet<Timeline> Timelines { get; }

    DbSet<ApplicationUser> Users { get; }

    DbSet<EnrolmentPqaQueueEntry> EnrolmentPqaQueue { get; }
    DbSet<EnrolmentQa1QueueEntry> EnrolmentQa1Queue { get; }
    DbSet<EnrolmentQa2QueueEntry> EnrolmentQa2Queue { get; }
    DbSet<EnrolmentEscalationQueueEntry> EnrolmentEscalationQueue { get; }
    DbSet<ParticipantIncomingTransferQueueEntry> ParticipantIncomingTransferQueue { get; }
    DbSet<ParticipantOutgoingTransferQueueEntry> ParticipantOutgoingTransferQueue { get; }

    DbSet<LocationMapping> LocationMappings { get; }

    ChangeTracker ChangeTracker { get; }

    DbSet<DataProtectionKey> DataProtectionKeys { get; }

    DbSet<PasswordHistory> PasswordHistories { get; }

    DbSet<IdentityAuditTrail> IdentityAuditTrails { get; }

    DbSet<ParticipantAccessAuditTrail> AccessAuditTrails { get; }

    DbSet<DocumentAuditTrail> DocumentAuditTrails { get; }

    DbSet<HubInduction> HubInductions { get; }

    DbSet<WingInduction> WingInductions { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<Activity> Activities { get; }
    DbSet<EducationTrainingActivity> EducationTrainingActivities { get; }
    DbSet<EmploymentActivity> EmploymentActivities { get; }
    DbSet<ISWActivity> ISWActivities { get; }
    DbSet<NonISWActivity> NonISWActivities { get; }

    DbSet<ActivityPqaQueueEntry> ActivityPqaQueue { get; }
    DbSet<ActivityQa1QueueEntry> ActivityQa1Queue { get; }
    DbSet<ActivityQa2QueueEntry> ActivityQa2Queue { get; }
    DbSet<ActivityEscalationQueueEntry> ActivityEscalationQueue { get; }

    DbSet<OutboxMessage> OutboxMessages { get; }

    DbSet<EnrolmentPayment> EnrolmentPayments { get; }
    DbSet<InductionPayment> InductionPayments { get; }
    DbSet<ActivityPayment> ActivityPayments { get; }

    DbSet<EducationPayment> EducationPayments { get; }

    DbSet<EmploymentPayment> EmploymentPayments { get; }

    DbSet<PRI> PRIs { get; }

    DbSet<DateDimension> DateDimensions { get; }

    DbSet<PriCode> PriCodes { get; }

    DbSet<SupportAndReferralPayment> SupportAndReferralPayments { get; }
    DbSet<ReassessmentPayment> ReassessmentPayments { get; }

    DbSet<OutcomeQualityDipSample> OutcomeQualityDipSamples { get; }
    DbSet<OutcomeQualityDipSampleParticipant> OutcomeQualityDipSampleParticipants { get; }
    
    DbSet<Objective> Objectives { get; }
    
    DbSet<ObjectiveTask> ObjectiveTasks { get; }
    
}