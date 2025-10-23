using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Specifications;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class ParticipantsArchivedResultsWithPagination
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Query : ParticipantsArchivedResultsAdvancedFilter, IRequest<Result<PaginatedData<ParticipantsArchivedResultsSummaryDto>>>
    {
        public ParticipantsArchivedResultsAdvancedSpecification Specification => new(this);
    }

    public class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<PaginatedData<ParticipantsArchivedResultsSummaryDto>>>
    {
        public async Task<Result<PaginatedData<ParticipantsArchivedResultsSummaryDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var db = unitOfWork.DbContext;

#pragma warning disable CS8602
            var query = from peh in db.ParticipantEnrolmentHistories.ApplySpecification(request.Specification)
                        join p in db.Participants on peh.ParticipantId equals p.Id
                        join createdBy in unitOfWork.DbContext.Users on peh.CreatedBy equals createdBy.Id
                        where createdBy.TenantId!.StartsWith(request.CurrentUser.TenantId!)                         
                        orderby peh.From  
                        select new ParticipantsArchivedResultsSummaryDto
                        {
                            Id = p.Id,
                            ParticipantName = $"{p.FirstName} {p.LastName}",
                            EnrolmentStatus = p.EnrolmentStatus!,
                            ArchivedDate = peh.From,
                            ToDate = peh.To,                            
                            ArchiveJustification = string.IsNullOrWhiteSpace(peh.AdditionalInformation)
                                ? "No justification"
                                : peh.AdditionalInformation,
                            ArchiveReason = peh.Reason!,
                            ArchivedBy= createdBy.DisplayName!
                        };
#pragma warning restore CS8602

            var count = await query.CountAsync(cancellationToken);

            var results = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedData<ParticipantsArchivedResultsSummaryDto>(results, count, request.PageNumber, request.PageSize);
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
            }
        }
    }
}