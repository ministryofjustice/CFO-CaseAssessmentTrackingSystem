using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.Features.Dashboard.Specifications;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class FirstPassQAEnrolmentsResultsWithPagination
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Query : FirstPassQAEnrolmentsResultsAdvancedFilter, IRequest<Result<PaginatedData<FirstPassQADetailsDto>>>
    {
        public FirstPassQAEnrolmentsResultsAdvancedSpecification Specification => new(this);
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        // public required string UserId { get; set; }
        // public required UserProfile CurrentUser { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<PaginatedData<FirstPassQADetailsDto>>>
    {
        public async Task<Result<PaginatedData<FirstPassQADetailsDto>>> Handle(Query request,
            CancellationToken cancellationToken)
        {
            var db = unitOfWork.DbContext;

            var startDate = request.StartDate;
            var endDate = request.EndDate;
            
#pragma warning disable CS8602
            
            var qa1DtoQuery =
                from p in db.Participants.ApplySpecification(request.Specification)
                join q in db.EnrolmentQa1Queue on p.Id equals q.ParticipantId
                join u in db.Users on q.OwnerId equals u.Id
                where q.IsCompleted && q.LastModified >= startDate && q.LastModified <= endDate
                select new FirstPassQADetailsDto
                {
                    ParticipantId = q.ParticipantId!,
                    ParticipantName = p.FirstName + ' ' + p.LastName,
                    Queue = "QA1",
                    LastModified = q.LastModified,
                    QAUser = u.DisplayName!,
                    IsAccepted = q.IsAccepted ? "Yes" : "No",
                    Escalated = "",
                    Note = q.Notes.Select(n => n.Message).FirstOrDefault() ?? ""
                };
            
            var qa2DtoQuery =
                from p in db.Participants.ApplySpecification(request.Specification)
                join q in db.EnrolmentQa2Queue on p.Id equals q.ParticipantId
                join u in db.Users on q.OwnerId equals u.Id
                where q.IsCompleted && q.LastModified >= startDate && q.LastModified <= endDate
                select new FirstPassQADetailsDto
                {
                    ParticipantId = q.ParticipantId!,
                    ParticipantName = p.FirstName + ' ' + p.LastName,
                    Queue = "QA2",
                    LastModified = q.LastModified,
                    QAUser = u.DisplayName!,
                    IsAccepted = q.IsAccepted ? "Yes" : "No",
                    Escalated = q.IsEscalated ? "Yes" : "No",
                    Note = q.Notes.Select(n => n.Message).FirstOrDefault() ?? ""
                };

            var escDtoQuery =
                from p in db.Participants.ApplySpecification(request.Specification)
                join q in db.EnrolmentEscalationQueue on p.Id equals q.ParticipantId
                join u in db.Users on q.OwnerId equals u.Id
                where q.IsCompleted && q.LastModified >= startDate && q.LastModified <= endDate
                select new FirstPassQADetailsDto
                {
                    ParticipantId = q.ParticipantId!,
                    ParticipantName = p.FirstName + ' ' + p.LastName,
                    Queue = "Esc",
                    LastModified = q.LastModified,
                    QAUser = u.DisplayName!,
                    IsAccepted = q.IsAccepted ? "Yes" : "No",
                    Escalated = "",
                    Note = q.Notes.Select(n => n.Message).FirstOrDefault() ?? ""
                };

// Materialize each query async
            var qa1List = await qa1DtoQuery.ToListAsync(cancellationToken);
            var qa2List = await qa2DtoQuery.ToListAsync(cancellationToken);
            var escList = await escDtoQuery.ToListAsync(cancellationToken);

// Merge in memory
            var combinedList = qa1List.Concat(qa2List).Concat(escList)
                .OrderBy(x => x.LastModified)
                .ToList();
           
#pragma warning restore CS8602

            var count = combinedList.Count; // in-memory
            var results = combinedList
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList(); // in-memory

            return new PaginatedData<FirstPassQADetailsDto>(results, count, request.PageNumber, request.PageSize);
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