using System.Net.Http.Json;
using Cfo.Cats.Application.Features.Candidates.DTOs;
using Cfo.Cats.Application.Features.Candidates.Queries.Search;

namespace Cfo.Cats.Infrastructure.Services.Candidates;

public class CandidateService(
    HttpClient client, 
    IUnitOfWork unitOfWork) : ICandidateService
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
        var candidate = await client.GetFromJsonAsync<CandidateDto>($"clustering/{upci}/Aggregate");

        if(candidate is not null)
        {
            var locationMapping = candidate.Primary switch
            {
                "NOMIS" => (candidate.EstCode, "Prison"),
                "DELIUS" => (candidate.OrgCode, "Probation"),
                _ => (null, null)
            };

            var query = from dl in unitOfWork.DbContext.LocationMappings.AsNoTracking()
                        where dl.Code == locationMapping.Item1 && dl.CodeType == locationMapping.Item2
                        select new
                        {
                            dl.Code,
                            dl.CodeType,
                            dl.Description,
                            dl.DeliveryRegion,
                            dl.Location
                        };

            var location = await query.FirstOrDefaultAsync();

            candidate.LocationDescription = location switch
            {
                { Location: not null } => location.Location.Name,
                { Code: not null } => $"Unmapped Location ({location.Code} - {location.DeliveryRegion} - {location.Description})",
                _ => "Unmapped Location",
            };

            candidate.MappedLocationId = location switch
            {
                { Location: not null } => location.Location.Id,
                _ => 0
            };

        }

        return candidate;

    }
}