using System.Net.Http.Json;
using Cfo.Cats.Application.Features.Candidates.DTOs;
using Cfo.Cats.Application.Features.Candidates.Queries.Search;
using Cfo.Cats.Domain.Entities.Administration;
using Newtonsoft.Json;

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
            // 1. Primary record is a prison record (excluding OUT)
            // 2. Primary record is a community record
            //  2a. Use Sticky Location if present
            //  2b. Use Org code if not
            // 3. Primary record is a prison record but shown as "OUT" (outside)...
            //  3a. Use Sticky Location or Org Code probation if an organisation code is present
            //  3b. Use prison if an organisation code is not present
            // 4. Unknown
            var locationMapping = candidate switch
            {
                /* 1  */ { Primary: "NOMIS", EstCode: not "OUT" } => (Code: candidate.EstCode, Type: "Prison"),
                /* 2  */ { Primary: "DELIUS" } => (Code: candidate.StickyLocation ?? candidate.OrgCode, Type: "Probation"),
                /* 3a */ { Primary: "NOMIS", EstCode: "OUT", OrgCode: not null } => (Code: candidate.StickyLocation ?? candidate.OrgCode, Type: "Probation"),
                /* 3b */ { Primary: "NOMIS", EstCode: "OUT", OrgCode: null } => (Code: candidate.EstCode, Type: "Prison"),
                /* 4  */ _ => (null, null)
            };

            var query = from dl in unitOfWork.DbContext.LocationMappings.AsNoTracking()
                        where dl.Code == locationMapping.Code && dl.CodeType == locationMapping.Type
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

            if (location is { Location: not null })
            {
                candidate.MappedLocationId = location.Location.Id;
            }
            else
            {
                candidate.MappedLocationId = locationMapping.Type switch
                {
                    "Prison" => Location.Constants.UnmappedCustody,
                    "Probation" => Location.Constants.UnmappedCommunity,
                    _ => Location.Constants.Unknown
                };
            }

            candidate.RegistrationDetailsJson = JsonConvert.SerializeObject(candidate.RegistrationDetails);
        }

        return candidate;
    }

    public async Task<bool> SetStickyLocation(string upci, string location)
    {
        var result = await client.PostAsync($"/reference/{upci}/{location}", null);
        return result.IsSuccessStatusCode;
    }
}