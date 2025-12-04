using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Constants.Database;

internal static class DatabaseConstants
{
    public static class Tables 
    {
        public const string Role = nameof(Role);
        public const string RoleClaim = nameof(RoleClaim);
        public const string User = nameof(User);
        public const string UserClaim = nameof(UserClaim);
        public const string UserLogin = nameof(UserLogin);
        
        public const string UserToken = nameof(UserToken);
        public const string UserRole = nameof(UserRole);
        public const string AuditTrail = nameof(AuditTrail);

        public const string IdentityAuditTrail = nameof(IdentityAuditTrail);
        public const string Consent = nameof(Consent);
        public const string Contract = nameof(Contract);
        public const string Document = nameof(Document);
        public const string GeneratedDocument = nameof(GeneratedDocument);
        public const string Location = nameof(Location);
        public const string LocationMapping = nameof(LocationMapping);
        public const string Participant = nameof(Participant);
        public const string Assessment = nameof(Assessment);
        public const string Bio = nameof(Bio);
        public const string Note = nameof(Note);
        public const string ExternalIdentifier = nameof(ExternalIdentifier);
        public const string Tenant = nameof(Tenant);
        public const string TenantDomain = nameof(TenantDomain);
        public const string KeyValue = nameof(KeyValue);
        public const string EnrolmentHistory = nameof(EnrolmentHistory);
        public const string ActiveStatusHistory = nameof(ActiveStatusHistory);
        public const string RightToWork = nameof(RightToWork);
        public const string Risk = nameof(Risk);
        public const string Objective = nameof(Objective);
        public const string ObjectiveTask = nameof(ObjectiveTask);
        public const string PathwayPlan = nameof(PathwayPlan);
        public const string PathwayPlanReviewHistory = nameof(PathwayPlanReviewHistory);
        public const string PersonalDetails = nameof(PersonalDetails);
        public const string Supervisor = nameof(Supervisor);
        public const string LocationHistory = nameof(LocationHistory);
        public const string OwnershipHistory = nameof(OwnershipHistory);
        public const string IncomingTransferQueue = nameof(IncomingTransferQueue);
        public const string OutgoingTransferQueue = nameof(OutgoingTransferQueue);
        public const string Activity = nameof(Activity);
        public const string EducationTrainingActivity = nameof(EducationTrainingActivity);
        public const string EmploymentActivity = nameof(EmploymentActivity);
        public const string IswActivity = nameof(IswActivity);
        public const string NonIsqActivity = nameof(NonIsqActivity);
        public const string PriCode = nameof(PriCode);

        public const string AssessmentPathwayScore = nameof(AssessmentPathwayScore);

        public const string Timeline = nameof(Timeline);

        public const string EnrolmentPqaQueue = "PqaQueue";
        public const string EnrolmentQa1Queue = "Qa1Queue";
        public const string EnrolmentQa2Queue = "Qa2Queue";
        public const string EnrolmentEscalationQueue = "EscalationQueue";

        public const string PasswordHistory = nameof(PasswordHistory);

        public const string AccessAuditTrail = nameof(AccessAuditTrail);
        public const string DocumentAuditTrail = nameof(DocumentAuditTrail);

        public const string HubInduction = nameof(HubInduction);
        public const string WingInduction = nameof(WingInduction);
        public const string WingInductionPhase = nameof(WingInductionPhase);

        public const string Notification = nameof(Notification);

        public const string ActivityPqaQueue = nameof(ActivityPqaQueue);
        public const string ActivityQa1Queue = nameof(ActivityQa1Queue);
        public const string ActivityQa2Queue = nameof(ActivityQa2Queue);
        public const string ActivityEscalationQueue = nameof(ActivityEscalationQueue);

        public const string PRI = nameof(PRI);
        
        public const string Label = nameof(Label);
    }
    public static class Schemas
    {
        public const string Dms = nameof(Dms);
        public const string Participant = nameof(Participant);
        public const string Identity = nameof(Identity);
        public const string Audit = nameof(Audit);
        public const string Configuration = nameof(Configuration);
        public const string Document = nameof(Document);
        public const string Enrolment = nameof(Enrolment);
        public const string Induction = nameof(Induction);
        public const string Activities = nameof(Activities);
        public const string Mi = nameof(Mi);
        public const string PRI = nameof(PRI);
    }

    public static class FieldLengths
    {
        /// <summary>
        /// The length of a string representation of a GUID.
        /// </summary>
        public const int GuidId = 36;
        
        /// <summary>
        /// The maximum length of a tenant id (i.e. 1.1.1.)
        /// </summary>
        public const int TenantId = 50;
        
        /// <summary>
        /// The maximum (actually fixed) length of a participant id 
        /// </summary>
        public const int ParticipantId = 9;
        
        /// <summary>
        /// The maximum length of a call reference 
        /// </summary>
        public const int CallReference = 20;

        /// <summary>
        /// The maximum length for a key value pair.
        /// </summary>
        public const int KeyValue = 100;

        /// <summary>
        /// The maximum length for an external identifier (e.g. CRN, Nomis Number).
        /// </summary>
        public const int ExternalIdentifier = 16;

        /// <summary>
        /// The maximum length for the name of a location.
        /// </summary>
        public const int LocationName = 200;

        /// <summary>
        /// The maximum length for the name of a tenant.
        /// </summary>
        public const int TenantName = 50;

        /// <summary>
        /// The maximum length for the display name of a user.
        /// </summary>
        public const int UserDisplayName = 100;

        /// <summary>
        /// The maximum length for the description of a contract.
        /// </summary>
        public const int ContractDescription = 50;

        /// <summary>
        /// The maximum length for a contract id.
        /// </summary>
        public const int ContractId = 12;

    }

}

