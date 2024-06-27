using System.Net.Http.Json;
using Cfo.Cats.Application.Features.Candidates.DTOs;
using Cfo.Cats.Application.Features.Candidates.Queries.Search;

namespace Cfo.Cats.Infrastructure.Services.Candidates;

public class CandidateService(HttpClient client) : ICandidateService
{
    public async Task<IEnumerable<SearchResult>?> SearchAsync(CandidateSearchQuery searchQuery)
    {
        var queryParams = new[]
        {
            $"identifier={searchQuery.ExternalIdentifier}",
            $"lastName={searchQuery.LastName}",
            $"dateOfBirth={searchQuery.DateOfBirth!.Value:yyyy-MM-dd}"
        };

        return await client.GetFromJsonAsync<IEnumerable<SearchResult>>($"/search?{string.Join('&', queryParams)}");
    }

    public async Task<CandidateDto?> GetByUpciAsync(string upci)
    {
        return await client.GetFromJsonAsync<CandidateDto>($"clustering/{upci}/Aggregate");
    }
}