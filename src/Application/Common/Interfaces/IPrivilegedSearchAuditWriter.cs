using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Application.Common.Interfaces;

public interface IPrivilegedSearchAuditWriter
{
    Task RecordAdvancedSearchAttemptAsync(
        UserProfile userProfile,
        string identifier,
        string justification,
        IEnumerable<string> catsParticipantIds,
        CancellationToken cancellationToken = default);

    Task RecordAdvancedSearchSentenceLookupAttemptAsync(
        UserProfile userProfile,
        string recordIdentifier,
        string nomisNumber,
        string justification,
        CancellationToken cancellationToken = default);
}
