using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.AuditTrails.Caching;
using Cfo.Cats.Application.Features.AuditTrails.DTOs;
using Cfo.Cats.Application.Features.AuditTrails.Specifications.DocumentAuditTrail;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Documents;

namespace Cfo.Cats.Application.Features.AuditTrails.Queries;

public static class GetDocumentAuditTrailsWithPagination
{
    [RequestAuthorize(Policy = SecurityPolicies.ViewAudit)]
    public class Query : DocumentAuditTrailAdvancedFilter, ICacheableRequest<PaginatedData<DocumentAuditTrailDto>>
    {
        public DocumentAuditTrailAdvancedSpecification Specification => new(this);

        public string CacheKey => DocumentAuditTrailsCacheKey.GetPaginationCacheKey($"{this}");
        public MemoryCacheEntryOptions? Options => DocumentAuditTrailsCacheKey.MemoryCacheEntryOptions;

        public override string ToString()
        {
            return $"Search:{Keyword},Sort:{SortDirection},OrderBy:{OrderBy},{PageNumber},{PageSize}";
        }
    }

    private class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, PaginatedData<DocumentAuditTrailDto>>
    {
        public async Task<PaginatedData<DocumentAuditTrailDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var data = await unitOfWork.DbContext
                .DocumentAuditTrails.OrderBy($"{request.OrderBy} {request.SortDirection}")
                .ProjectToPaginatedDataAsync<DocumentAuditTrail, DocumentAuditTrailDto>(
                    request.Specification,
                    request.PageNumber,
                    request.PageSize,
                    mapper.ConfigurationProvider,
                    cancellationToken
                );

            return data;
        }
    }

    private class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(r => r.PageNumber)
                .GreaterThan(0)
                .WithMessage(string.Format(ValidationConstants.PositiveNumberMessage, "Page Number"));

            RuleFor(r => r.PageSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(1000)
                .WithMessage(string.Format(ValidationConstants.MaximumPageSizeMessage, "Page Size"));

            RuleFor(r => r.SortDirection)
                .Matches(ValidationConstants.SortDirection)
                .WithMessage(ValidationConstants.SortDirectionMessage);
        }
    }

}
