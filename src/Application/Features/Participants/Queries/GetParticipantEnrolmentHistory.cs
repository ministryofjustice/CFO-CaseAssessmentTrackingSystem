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
        public required UserProfile CurrentUser { get; set; }
    }

    class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, IEnumerable<ParticipantEnrolmentHistoryDto>>
    {
        //Hide CFO Users details from providers
        private static readonly string[] HiddenTenantIds = ["1.1.", "1."];
        private static readonly string[] AllowedRoles =
        [
            RoleNames.QAOfficer,
            RoleNames.QASupportManager,
            RoleNames.QAManager,
            RoleNames.SMT,
            RoleNames.SystemSupport
        ];

        public async Task<IEnumerable<ParticipantEnrolmentHistoryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var db = unitOfWork.DbContext;

            var results = await (
                                 from peh in db.ParticipantEnrolmentHistories
                                 join u in db.Users on peh.CreatedBy equals u.Id

                                 // Do a group join (left join) to all PLHs for this participant
                                 join plhGroup in db.ParticipantLocationHistories
                                     on peh.ParticipantId equals plhGroup.ParticipantId into plhGroupJoin

                                 // For each PEH, select the most recent PLH where plh.From <= peh.Created
                                 from plh in plhGroupJoin
                                     .Where(plh => plh.From <= peh.Created)
                                     .OrderByDescending(plh => plh.From)
                                     .Take(1) // Take the latest valid PLH
                                     .DefaultIfEmpty()

                                 join l in db.Locations on plh.LocationId equals l.Id
                                 where peh.ParticipantId == request.ParticipantId
                                       && plh.From <= peh.Created

                                 select new ParticipantEnrolmentHistoryDto
                                 {
                                     Id = peh.Id,
                                     ActionDate = peh.Created,
                                     Status = peh.EnrolmentStatus == 4 ? "Archived" : "Active",
                                     Event = peh.EnrolmentStatus.Name,
                                     Reason = peh.Reason,
                                     AdditionalInformation = peh.AdditionalInformation!,
                                     LocationName = l.Name,
                                     SupportWorker = u!.DisplayName!,
                                     SupportWorkerTenantId = u!.TenantId
                                 })
                                  .OrderBy(x => x.ActionDate)
                                  .ToListAsync();

            if (results.Count == 0)
                return results;

            bool hideUser = !request.CurrentUser.AssignedRoles.Any(role => AllowedRoles.Contains(role));

            for (int i = 0; i < results.Count; i++)
            {
                // Mask support worker if needed
                if (hideUser && HiddenTenantIds.Contains(results[i].SupportWorkerTenantId))
                {
                    results[i].SupportWorker = "CFO User";
                }

                var current = results[i];
                var next = i == results.Count - 1
                    ? DateTime.UtcNow
                    : results[i + 1].ActionDate;

                current.DaysSincePrevious = (current.ActionDate.HasValue && next.HasValue)
                    ? (next.Value - current.ActionDate.Value).Days
                    : 0;
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