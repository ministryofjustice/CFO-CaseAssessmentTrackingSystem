using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.QualityAssurance.DTOs;
using Cfo.Cats.Application.Features.QualityAssurance.Specifications;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.QualityAssurance.Queries;

public static class GetQaOfficerStats
{
    [RequestAuthorize(Policy = SecurityPolicies.ServiceDeskManagement)]
    public class Query : PaginationFilter, IQuery<Result<QaOfficerStatsDto>>
    {
        public Query()
        {
            OrderBy = nameof(QaOfficerStatDto.LastModified);
            SortDirection = "Descending";
        }

        public required QaOfficerStatsFilter Filter { get; set; }

        public bool VisualMode { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IQueryHandler<Query, Result<QaOfficerStatsDto>>
    {
        public async Task<Result<QaOfficerStatsDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            if (request.VisualMode is false)
            {
                var rows = await new QaOfficerStatsWithPagination.Handler(unitOfWork).Handle(
                    new QaOfficerStatsWithPagination.Query
                    {
                        Filter = request.Filter,
                        PageNumber = request.PageNumber,
                        PageSize = request.PageSize,
                        OrderBy = request.OrderBy,
                        SortDirection = request.SortDirection
                    },
                    cancellationToken);

                return new QaOfficerStatsDto { Rows = rows.Data };
            }

            var summary = await QaOfficerStatsWithPagination.BuildQuery(unitOfWork.DbContext, request.Filter)
                .Where(x => x.LastModified != null)
                .GroupBy(x => new { Day = x.LastModified!.Value.Date, x.Queue, x.QaUserId, x.QaUser })
                .Select(g => new QaOfficerStatSummaryDto
                {
                    Day = g.Key.Day,
                    Queue = g.Key.Queue,
                    QaUser = g.Key.QaUser,
                    Count = g.Select(x => x.EntryId).Distinct().Count()
                })
                .ToListAsync(cancellationToken);

            return new QaOfficerStatsDto { Summary = summary };
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(r => r.Filter.StartDate)
                .LessThanOrEqualTo(r => r.Filter.EndDate)
                .WithMessage("Start date must be on or before the end date.");

            RuleFor(r => r.Filter.IsWithinMaxRange())
                .Equal(true)
                .WithMessage(QaOfficerStatsFilter.MaxRangeMessage);

            RuleFor(r => r.Filter.Crn)
                .Matches(ValidationConstants.Keyword)
                .WithMessage(string.Format(ValidationConstants.KeywordMessage, "CRN"));

            RuleFor(r => r.PageNumber)
                .GreaterThan(0)
                .WithMessage(string.Format(ValidationConstants.PositiveNumberMessage, "Page Number"));

            RuleFor(r => r.PageSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(ValidationConstants.MaximumPageSize)
                .WithMessage(ValidationConstants.MaximumPageSizeMessage);

            RuleFor(r => r.SortDirection)
                .Matches(ValidationConstants.SortDirection)
                .WithMessage(ValidationConstants.SortDirectionMessage);

            RuleFor(r => r.OrderBy)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "OrderBy"));
        }
    }
}
