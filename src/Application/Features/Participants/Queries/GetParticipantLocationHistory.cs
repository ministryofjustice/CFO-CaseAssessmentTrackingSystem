using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetParticipantLocationHistory
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<IEnumerable<ParticipantLocationHistoryDto>>
    {
        public required string ParticipantId { get; set; }
        public required UserProfile CurrentUser { get; set; }
    }

    class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, IEnumerable<ParticipantLocationHistoryDto>>
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

        public async Task<IEnumerable<ParticipantLocationHistoryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var db = unitOfWork.DbContext;

            var participantId = request.ParticipantId;

            // Get enrolment location first
            var enrolmentLocationQuery =
                from p in db.Participants
                join l in db.Locations on p.EnrolmentLocation!.Id equals l.Id
                join u in db.Users on p.CreatedBy equals u.Id
                where p.Id == participantId
                select new ParticipantLocationHistoryDto
                {
                    Id = 0, 
                    MoveDate = p.Created, 
                    LocationName = l.Name + " (Enrolment Location)",
                    SupportWorker = u!.DisplayName!,
                    SupportWorkerTenantId = u!.TenantId,
                    IsEnrolment = true
                };

            // Then get Location history
            var locationHistoryQuery =
                from plh in db.ParticipantLocationHistories
                join u in db.Users on plh.CreatedBy equals u.Id
                join l in db.Locations on plh.LocationId equals l.Id
                where plh.ParticipantId == participantId
                select new ParticipantLocationHistoryDto
                {
                    Id = plh.Id,
                    MoveDate = plh.From,
                    LocationName = l.Name,
                    SupportWorker = u.DisplayName!,
                    SupportWorkerTenantId = u.TenantId,
                    IsEnrolment = false
                };
                    
            var results = await enrolmentLocationQuery
                .Union(locationHistoryQuery)
                .OrderBy(x => x.IsEnrolment ? 0 : 1)
                .ThenBy(x => x.MoveDate ?? DateTime.MinValue)
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
                    : results[i + 1].MoveDate;

                current.DaysSincePrevious = (current.MoveDate.HasValue && next.HasValue)
                    ? (next.Value - current.MoveDate.Value).Days
                    : 0;
            }

            return results;
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.ParticipantId)
                .NotNull();

            RuleFor(x => x.ParticipantId)
                .Length(9)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c.ParticipantId)
                    .MustAsync(Exist)
                    .WithMessage("Participant does not exist");
            });
        }

        private async Task<bool> Exist(string identifier, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == identifier, cancellationToken);
    }
}