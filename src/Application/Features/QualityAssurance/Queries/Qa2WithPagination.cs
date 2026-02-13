using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.QualityAssurance.DTOs;
using Cfo.Cats.Application.Features.QualityAssurance.Specifications;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.QualityAssurance.Queries;

public static class Qa2WithPagination
{
    [RequestAuthorize(Roles = $"{RoleNames.QAManager}, {RoleNames.QASupportManager}, {RoleNames.SMT}, {RoleNames.SystemSupport}")]
    public class Query : QueueEntryFilter, IRequest<PaginatedData<EnrolmentQueueEntryDto>>
    {
        public Query()
        {
            SortDirection = "Desc";
            OrderBy = "Created";
        }
        
        public EnrolmentQa2QueueEntrySpecification Specification => new(this);
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, PaginatedData<EnrolmentQueueEntryDto>>
    {
        public async Task<PaginatedData<EnrolmentQueueEntryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = unitOfWork.DbContext
                .EnrolmentQa2Queue
                .AsNoTracking();
                
            var sortExpression = GetSortExpression(request);

            var ordered = request.SortDirection.Equals("Descending", StringComparison.CurrentCultureIgnoreCase) 
                ? query.OrderByDescending(sortExpression) 
                : query.OrderBy(sortExpression);

            var data = await ordered
                .ProjectToPaginatedDataAsync<EnrolmentQa2QueueEntry, EnrolmentQueueEntryDto>(request.Specification, request.PageNumber, request.PageSize, mapper.ConfigurationProvider, cancellationToken);

            var participantIds = data.Items
                .Select(x => x.ParticipantId )
                .Distinct()
                .ToArray();
            
            var qa1Owners = await unitOfWork.DbContext.EnrolmentQa1Queue
                .Where(q1 =>
                    participantIds.Contains(q1.ParticipantId) &&
                    q1.IsCompleted)
                .GroupBy(q1 => q1.ParticipantId) 
                .Select(g => new
                {
                    ParticipantId = g.Key,
                    Qa1CompletedBy = g
                        .OrderByDescending(x => x.LastModified) 
                        .Select(x => x.Owner!.DisplayName)
                        .FirstOrDefault()
                })
                .ToDictionaryAsync(
                    x => x.ParticipantId,
                    x => x.Qa1CompletedBy,
                    cancellationToken);
            
            foreach (var item in data.Items)
            {
                if (qa1Owners.TryGetValue(item.ParticipantId, out var completedBy))
                {
                    item.Qa1CompletedBy = completedBy;
                }
            }
            
            return data;
        }
        
        private Expression<Func<EnrolmentQa2QueueEntry, object?>> GetSortExpression(Query request)
        {
            Expression<Func<EnrolmentQa2QueueEntry, object?>> sortExpression = request.OrderBy switch
            {
                "ParticipantId" => (x => x.Participant!.FirstName + ' ' + x.Participant.LastName),
                "TenantId" => (x => x.TenantId),
                "Created" => (x => x.Created!),
                "SupportWorker" => (x => x.SupportWorker!.DisplayName!),
                "AssignedTo" => (x => x.OwnerId == null ? null : x.Owner!.DisplayName),
                _ => (x => x.Created!)
            };

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