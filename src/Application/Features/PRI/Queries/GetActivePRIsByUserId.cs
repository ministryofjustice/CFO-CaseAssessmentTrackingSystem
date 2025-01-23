using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.PRI.DTOs;
using Cfo.Cats.Application.Features.PRI.Specifications;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.PRI.Queries;

public static class GetActivePRIsByUserId
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]   

    public class Query : PRIAdvancedFilter, IRequest<PaginatedData<PRIPaginationDto>>
    {
        public PRIAdvancedSpecification Specification => new(this);
    }

    class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, PaginatedData<PRIPaginationDto>>
    {
        public async Task<PaginatedData<PRIPaginationDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            var data = await unitOfWork.DbContext.PRIs
                .Where(x => x.AssignedTo == request.CurrentUser!.UserId 
                        || x.CreatedBy == request.CurrentUser!.UserId 
                        && x.IsCompleted == false)
                .OrderBy($"{request.OrderBy} {request.SortDirection}")            
                .ProjectToPaginatedDataAsync<Domain.Entities.PRIs.PRI, PRIPaginationDto>(request.Specification, request.PageNumber, request.PageSize, mapper.ConfigurationProvider, cancellationToken);                                

            return data;
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