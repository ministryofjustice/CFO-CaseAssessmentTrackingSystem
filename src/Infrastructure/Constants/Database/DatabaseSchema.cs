using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Constants.Database;

internal static class DatabaseSchema
{
    public static class Tables 
    {
        public const string ApplicationRole = nameof(ApplicationRole);
        public const string ApplicationRoleClaim = nameof(ApplicationRoleClaim);
        public const string ApplicationUser = nameof(ApplicationUser);
        public const string ApplicationUserClaim = nameof(ApplicationUserClaim);
        public const string ApplicationUserLogin = nameof(ApplicationUserLogin);
        public const string ApplicationUserNote = nameof(ApplicationUserNote);
        public const string ApplicationUserToken = nameof(ApplicationUserToken);
        public const string ApplicationUserRole = nameof(ApplicationUserRole);
        public const string AuditTrail = nameof(AuditTrail);
        public const string Consent = nameof(Consent);
        public const string Contract = nameof(Contract);
        public const string Document = nameof(Document);
        public const string Location = nameof(Location);
        public const string LocationMapping = nameof(LocationMapping);
        public const string Participant = nameof(Participant);
        public const string ParticipantAssessment = nameof(ParticipantAssessment);
        public const string Tenant = nameof(Tenant);
        public const string TenantDomain = nameof(TenantDomain);
        public const string KeyValue = nameof(KeyValue);
        public const string ParticipantEnrolmentHistory = nameof(ParticipantEnrolmentHistory);
        public const string RightToWork = nameof(RightToWork);

        public const string ParticipantAssessmentPathwayScore = nameof(ParticipantAssessmentPathwayScore);
        
    }
    public static class Schema
    {
        public const string dms = nameof(dms);
    }

}

