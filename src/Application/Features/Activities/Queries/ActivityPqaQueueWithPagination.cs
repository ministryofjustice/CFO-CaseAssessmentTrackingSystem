using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.Features.Activities.Specifications;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Activities;
using System.Text.Json.Serialization;

namespace Cfo.Cats.Application.Features.Activities.Queries
{
    public static class ActivityPqaQueueWithPagination
    {
        [RequestAuthorize(Policy = SecurityPolicies.Pqa)]
        public class Query : ActivityQueueEntryFilter, IRequest<PaginatedData<ActivityQueueEntryDto>>
        {
            public Query()
            {
                SortDirection = "Desc";
                OrderBy = "Created";
            }

            [JsonIgnore]
            public ActivityPqaQueueEntrySpecification Specification => new(this);
        }

        public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, PaginatedData<ActivityQueueEntryDto>>
        {
            public async Task<PaginatedData<ActivityQueueEntryDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = unitOfWork.DbContext
                    .ActivityPqaQueue          
                    .Include(a => a.Activity)
                    .AsNoTracking();

                var sortExpression = GetSortExpression(request);

                var ordered = request.SortDirection.Equals("Descending", StringComparison.CurrentCultureIgnoreCase)
                    ? query.OrderByDescending(sortExpression)
                    : query.OrderBy(sortExpression);

                var data = await ordered
                    .ProjectToPaginatedDataAsync<ActivityPqaQueueEntry, ActivityQueueEntryDto>(request.Specification, request.PageNumber, request.PageSize, mapper.ConfigurationProvider, cancellationToken);

                var submissionCounts = await
                (
                    from q in query
                    where data.Items.Select(c => c.ActivityId).Contains(q.ActivityId)
                    group q by q.ActivityId into g
                    select new
                    {
                        ActivityId = g.Key,
                        PreviousSubmissions = g.Count() - 1 // Exclude current submission
                    }
                ).ToDictionaryAsync(x => x.ActivityId, cancellationToken);

                foreach (var item in data.Items)
                {
                    item.NoOfPreviousSubmissions = submissionCounts[item.ActivityId].PreviousSubmissions;
                }

                return data;
            }
            private Expression<Func<ActivityPqaQueueEntry, object?>> GetSortExpression(Query request)
            {
                Expression<Func<ActivityPqaQueueEntry, object?>> sortExpression;
                switch (request.OrderBy)
                {
                    case "ParticipantId":
                        sortExpression = (x => x.Participant!.FirstName + ' ' + x.Participant.LastName);
                        break;             
                    case "TenantId":
                        sortExpression = (x => x.TenantId);
                        break;
                    case "Created":
                        sortExpression = (x => x.Created!);
                        break;
                    case "SupportWorker":
                        sortExpression = (x => x.Participant!.Owner!.DisplayName!);
                        break;
                    case "AssignedTo":
                        sortExpression = (x => x.OwnerId == null ? null : x.Owner!.DisplayName);
                        break;
                    case "CommencedOn":
                    case "Expiry":
                        sortExpression = (x => x.Activity == null ? null : x.Activity.CommencedOn);
                        break;
                    default:
                        sortExpression = (x => x.Created!);
                        break;
                }

                return sortExpression;
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
                    .LessThanOrEqualTo(1000)
                    .WithMessage(string.Format(ValidationConstants.MaximumPageSizeMessage, "Page Size"));

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
}
