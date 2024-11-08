﻿using System.Net.Http.Json;
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
            var locationMapping = candidate.Primary switch
            {
                "NOMIS" => (Code: candidate.EstCode, Type: "Prison"),
                "DELIUS" => (Code: candidate.OrgCode, Type: "Probation"),
                _ => (null, null)
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

            if(location is { Location: not null })
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
}