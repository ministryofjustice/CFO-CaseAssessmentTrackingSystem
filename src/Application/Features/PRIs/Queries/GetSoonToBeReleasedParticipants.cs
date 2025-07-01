using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.PRIs.DTOs;
using Cfo.Cats.Application.Features.PRIs.Specifications;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.PRIs.Queries;

public static class GetSoonToBeReleasedParticipants
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]

    public class Query : PRIAdvancedFilter, IRequest<PaginatedData<SoonToBeReleasedPaginationDto>>
    {
        public PRIAdvancedSpecification Specification => new(this);
    }

    class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, PaginatedData<SoonToBeReleasedPaginationDto>>
    {
        public async Task<PaginatedData<SoonToBeReleasedPaginationDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            //This query will need rewriting for when expected release date is sent from DMS.
            //This will need to be modified for the information needed as CFODEV-1253

            var data =
                 from p in unitOfWork.DbContext.Participants
                 join pri in unitOfWork.DbContext.PRIs
                 on p.Id equals pri.ParticipantId
                 where (p.OwnerId == request.CurrentUser!.UserId)
                 orderby pri.ExpectedReleaseDate
                 select p; ;
            //     select new SoonToBeReleasedPaginationDto(p.Id, p.FirstName, p.LastName);

            //return data;
            return await Task.FromResult(new PaginatedData<SoonToBeReleasedPaginationDto>(
                    Enumerable.Empty<SoonToBeReleasedPaginationDto>(), // no items
                    0,     // total items
                    1,     // current page index (default to 1)
                    10     // page size (adjust as needed)
                ));
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(r => r.Keyword)
                .Matches(ValidationConstants.Keyword)
                .WithMessage(string.Format(ValidationConstants.KeywordMessage, "Search Keyword"));

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

            //May be at some point in future validate against columns of query result dataset
            RuleFor(r => r.OrderBy)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "OrderBy"));
        }
    }
}