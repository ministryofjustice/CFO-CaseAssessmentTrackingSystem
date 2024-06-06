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
        public const string ApplicationUserToken = nameof(ApplicationUserToken);
        public const string ApplicationUserRole = nameof(ApplicationUserRole);
        public const string AuditTrail = nameof(AuditTrail);
        public const string Candidate = nameof(Candidate);
        public const string CandidateIdentifier = nameof(CandidateIdentifier);
        public const string Contract = nameof(Contract);
        public const string Document = nameof(Document);
        public const string Location = nameof(Location);
        public const string Participant = nameof(Participant);
        public const string Tenant = nameof(Tenant);
        public const string KeyValue = nameof(KeyValue);
        public const string ParticipantEnrolmentHistory = nameof(ParticipantEnrolmentHistory);
    }

}
