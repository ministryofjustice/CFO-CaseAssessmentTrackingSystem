using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Outbox.DTOs;
using Cfo.Cats.Application.Features.Outbox.Specifications;
using Cfo.Cats.Application.Outbox;

namespace Cfo.Cats.Application.Features.Outbox.Queries.PaginationQuery;


public static class OutboxMessagesWithPagination
{
    [RequestAuthorize(Roles = RoleNames.SystemSupport)]
    public class Query : OutboxMessageAdvancedFilter, IRequest<PaginatedData<OutboxMessageDto>>
    {
        public OutboxMessageAdvancedSpecification Specification => new(this);
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, PaginatedData<OutboxMessageDto>>
    {
        public Task<PaginatedData<OutboxMessageDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var data = unitOfWork.DbContext
                .OutboxMessages
                .AsNoTracking()
                .OrderBy($"{request.OrderBy} {request.SortDirection}")
                .ProjectToPaginatedDataAsync<OutboxMessage, OutboxMessageDto>(
                    request.Specification,
                    request.PageNumber,
                    request.PageSize,
                    mapper.ConfigurationProvider,
                    cancellationToken
                );

            return data;
        }
    }

    public class Validator : AbstractValidator<Query>
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