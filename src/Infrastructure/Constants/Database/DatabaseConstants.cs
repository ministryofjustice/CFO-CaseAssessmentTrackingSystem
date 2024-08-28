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
        public const string Location = nameof(Location);
        public const string LocationMapping = nameof(LocationMapping);
        public const string Participant = nameof(Participant);
        public const string Assessment = nameof(Assessment);
        public const string Bio = nameof(Bio);
        public const string Note = nameof(Note);
        public const string Tenant = nameof(Tenant);
        public const string TenantDomain = nameof(TenantDomain);
        public const string KeyValue = nameof(KeyValue);
        public const string EnrolmentHistory = nameof(EnrolmentHistory);
        public const string RightToWork = nameof(RightToWork);
        public const string Risk = nameof(Risk);
        public const string Objective = nameof(Objective);
        public const string ObjectiveTask = nameof(ObjectiveTask);
        public const string PathwayPlan = nameof(PathwayPlan);
        public const string PathwayPlanReviewHistory = nameof(PathwayPlanReviewHistory);


        public const string AssessmentPathwayScore = nameof(AssessmentPathwayScore);

        public const string Timeline = nameof(Timeline);

        public const string EnrolmentPqaQueue = "PqaQueue";
        public const string EnrolmentQa1Queue = "Qa1Queue";
        public const string EnrolmentQa2Queue = "Qa2Queue";
        public const string EnrolmentEscalationQueue = "EscalationQueue";

        public const string PasswordHistory = nameof(PasswordHistory);



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

    }

}


