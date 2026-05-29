using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Locations.Caching;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.Features.Locations.Specifications;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Locations.Queries.GetAll;

[RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
public class GetAllLocationsQuery : LocationAdvancedFilter,  IQuery<Result<LocationDto[]>>
{
    public LocationAdvancedSpecification Specification => new(this);
}