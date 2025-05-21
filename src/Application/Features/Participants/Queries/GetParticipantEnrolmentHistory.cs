using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetParticipantEnrolmentHistory
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<IEnumerable<ParticipantEnrolmentHistoryDto>>    
    {
        public required string ParticipantId { get; set; }
    }

    class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, IEnumerable<ParticipantEnrolmentHistoryDto>>
    {
        public async Task<IEnumerable<ParticipantEnrolmentHistoryDto>> Handle(Query request, CancellationToken cancellationToken)
        {      
            var db = unitOfWork.DbContext;

            var results = await (from peh in db.ParticipantEnrolmentHistories
                                 join u in db.Users on peh.CreatedBy equals u.Id
                                 join plh in db.ParticipantLocationHistories on peh.ParticipantId equals plh.ParticipantId
                                 join l in db.Locations on plh.LocationId equals l.Id
                                 where peh.ParticipantId == request.ParticipantId
                                 select new ParticipantEnrolmentHistoryDto
                                 {
                                     Id = peh.Id,
                                     ActionDate = peh.Created.HasValue
                                             ? DateOnly.FromDateTime(peh.Created.Value)
                                                     : null,
                                     Status = peh.EnrolmentStatus == 4 ? "Archived" : "Active",
                                     Event = peh.EnrolmentStatus.Name,
                                     Reason=peh.Reason,
                                     AdditionalInformation = peh.AdditionalInformation!,
                                     LocationName = l.Name,
                                     SupportWorker = u!.DisplayName!
                                 }).ToListAsync();

            for (int i = 1; i < results.Count; i++)
            {
                if (results[i].ActionDate.HasValue && results[i - 1].ActionDate.HasValue)
                {
                    var current = results[i].ActionDate!.Value.ToDateTime(TimeOnly.MinValue);
                    var previous = results[i - 1].ActionDate!.Value.ToDateTime(TimeOnly.MinValue);
                    results[i - 1].DaysSincePrevious = (current - previous).Days;
                }
                else
                {
                    results[i - 1].DaysSincePrevious = 0;
                }
            }

            // Handle last row using today's date
            if (results.Count > 0 && results[^1].ActionDate.HasValue)
            {
                var lastActionDate = results[^1].ActionDate!.Value.ToDateTime(TimeOnly.MinValue);
                var today = DateTime.Today;
                results[^1].DaysSincePrevious = (today - lastActionDate).Days;
            }
            else if (results.Count > 0)
            {
                results[^1].DaysSincePrevious = 0;
            }

            return results;
        }
    }
  
    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.ParticipantId)
                .NotNull();

            RuleFor(x => x.ParticipantId)
                .MinimumLength(9)
                .MaximumLength(9)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));
        }
    }
}