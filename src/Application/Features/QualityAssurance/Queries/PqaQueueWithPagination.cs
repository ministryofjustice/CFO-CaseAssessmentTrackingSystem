using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.QualityAssurance.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.QualityAssurance.Queries;

public static class PqaQueueWithPagination
{
    [RequestAuthorize(Roles = $"{RoleNames.QAFinance}, {RoleNames.SystemSupport}")]
    public class Query : QueueEntryFilter, IRequest<PaginatedData<EnrolmentQueueEntryDto>>
    {
        public Query()
        {
            SortDirection = "Desc";
            OrderBy = "Created";
        }
        public EnrolmentPqaQueueEntrySpecification Specification => new(this);
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, PaginatedData<EnrolmentQueueEntryDto>>
    {
        public async Task<PaginatedData<EnrolmentQueueEntryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var data = await unitOfWork.DbContext.EnrolmentPqaQueue.OrderBy($"{request.OrderBy} {request.SortDirection}")
                .ProjectToPaginatedDataAsync<EnrolmentPqaQueueEntry, EnrolmentQueueEntryDto>(request.Specification, request.PageNumber, request.PageSize, mapper.ConfigurationProvider, cancellationToken);

            return data;
        }
    }
    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(r => r.Keyword)
                .Matches(RegularExpressionValidation.Keyword)
                .WithMessage(string.Format(RegularExpressionValidation.KeywordMessage, "Search Keyword"));

            RuleFor(r => r.PageNumber)
                .GreaterThan(0)
                .WithMessage(string.Format(RegularExpressionValidation.PositiveNumberMessage, "Page Number"));

            RuleFor(r => r.PageSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(1000)
                .WithMessage(string.Format(RegularExpressionValidation.PageSizeMessage, "Page Size"));

            RuleFor(r => r.SortDirection)
                .Matches(RegularExpressionValidation.SortDirection)
                .WithMessage(RegularExpressionValidation.SortDirectionMessage);

            //May be at some point in future validate against columns of query result dataset
            RuleFor(r => r.OrderBy)
                .Matches(RegularExpressionValidation.AlphaNumeric)
                .WithMessage(string.Format(RegularExpressionValidation.AlphaNumericMessage, "OrderBy"));

        }
    }
}
