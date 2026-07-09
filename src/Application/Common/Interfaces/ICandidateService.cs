using Cfo.Cats.Application.Features.Candidates.DTOs;
using Cfo.Cats.Application.Features.Candidates.Queries.Search;

namespace Cfo.Cats.Application.Common.Interfaces;

public interface ICandidateService
{
    Task<Result<SearchResult[]>> SearchAsync(CandidateSearchQuery searchQuery);
    Task<Result<CandidateDto>> GetByUpciAsync(string upci);

    Task<Result<bool>> SetStickyLocation(string upci, string location);
    
    Task<Result<bool>> SetHardLink(string participantId, string? primaryRecordKeyAtCreation, DateTime occurredOn);
}

public record SearchResult(string Upci, int Precedence);