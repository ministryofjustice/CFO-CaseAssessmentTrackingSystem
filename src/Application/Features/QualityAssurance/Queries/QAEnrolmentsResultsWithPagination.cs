using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.QualityAssurance.DTOs;
using Cfo.Cats.Application.Features.QualityAssurance.Specifications;
using Cfo.Cats.Application.SecurityConstants;

using static Cfo.Cats.Application.Features.QualityAssurance.DTOs.QAEnrolmentsResultsSummaryDto;

namespace Cfo.Cats.Application.Features.QualityAssurance.Queries;

public static class QAEnrolmentsResultsWithPagination
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Query : QAEnrolmentsResultsAdvancedFilter, IRequest<PaginatedData<QAEnrolmentsResultsSummaryDto>>
    {
        public QAEnrolmentsResultsAdvancedSpecification Specification => new(this);
    }

    public class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, PaginatedData<QAEnrolmentsResultsSummaryDto>>
    {
        public async Task<PaginatedData<QAEnrolmentsResultsSummaryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var db = unitOfWork.DbContext;

            bool hideUser = ShouldHideUser(request.UserProfile);
            var CFOTenantNames = new HashSet<string> { "CFO", "CFO Evolution" };

            var cutoffDate = DateTime.UtcNow.AddDays(-30);
            
#pragma warning disable CS8602
            var query =

                from p in db.Participants.ApplySpecification(request.Specification)

                let firstEnrolment = db.ParticipantEnrolmentHistories
                     .Where(e => e.ParticipantId == p.Id && e.EnrolmentStatus == EnrolmentStatus.SubmittedToProviderStatus.Value)
                     .OrderBy(e => e.Created)
                     .FirstOrDefault()

                let lastEnrolment = db.ParticipantEnrolmentHistories
                    .Where(e => e.ParticipantId == p.Id)
                    .OrderByDescending(e => e.Created)
                    .FirstOrDefault()

                let previousEnrolment = db.ParticipantEnrolmentHistories
                     .Where(e => e.ParticipantId == p.Id
                             && e.Created < lastEnrolment.Created)
                     .OrderByDescending(e => e.Created)
                     .FirstOrDefault()

                let resubmissionEnrolment = db.ParticipantEnrolmentHistories
                     .Where(e => e.ParticipantId == p.Id
                             && e.Created < lastEnrolment.Created
                             && e.EnrolmentStatus == EnrolmentStatus.SubmittedToProviderStatus.Value)
                     .OrderByDescending(e => e.Created)
                     .FirstOrDefault()

                where (
                         lastEnrolment.EnrolmentStatus == EnrolmentStatus.EnrollingStatus.Value
                         && previousEnrolment.EnrolmentStatus == EnrolmentStatus.SubmittedToProviderStatus.Value
                     )
                     ||
                     (
                         lastEnrolment.EnrolmentStatus == EnrolmentStatus.ApprovedStatus.Value
                         && lastEnrolment.Created >= cutoffDate
                     )

                select new QAEnrolmentsResultsSummaryDto
                {
                    ParticipantId = p.Id,
                    Participant = $"{p.FirstName} {p.LastName}",

                    SubmittedBy = resubmissionEnrolment.CreatedBy!,
                    SubmittedOn = resubmissionEnrolment.Created,

                    OriginallySubmitted = firstEnrolment.Created,
                    TookPlaceAtLocationName = p.EnrolmentLocation.Name,

                    CompletedOn = lastEnrolment.Created,

                    Status = lastEnrolment.EnrolmentStatus,

                    PQA = (from pqa in db.EnrolmentPqaQueue
                           from n in pqa.Notes
                           where pqa.ParticipantId == p.Id                                                
                           select new EnrolmentSummaryNote(
                               n.Message,
                               CFOTenantNames.Contains(n.CreatedByUser!.TenantName!) && hideUser
                                    ? "Hidden"
                                    : n.CreatedByUser!.DisplayName!,
                               n.CreatedByUser.TenantId!,
                               n.CreatedByUser.TenantName!,
                               n.Created!.Value
                           )).ToArray(),

                    QA1 = (from qa1 in db.EnrolmentQa1Queue
                           from n in qa1.Notes
                           where qa1.ParticipantId == p.Id
                           && (n.IsExternal || request.IncludeInternalNotes)
                           select new EnrolmentSummaryNote(
                                n.Message,
                                CFOTenantNames.Contains(n.CreatedByUser!.TenantName!) && hideUser
                                     ? "Hidden"
                                     : n.CreatedByUser!.DisplayName!,
                                n.CreatedByUser.TenantId!,
                                n.CreatedByUser.TenantName!,
                                n.Created!.Value
                            )).ToArray(),

                    QA2 = (from qa2 in db.EnrolmentQa2Queue
                           from n in qa2.Notes
                           where qa2.ParticipantId == p.Id
                           && (n.IsExternal || request.IncludeInternalNotes)
                           select new EnrolmentSummaryNote(
                                 n.Message,
                                 CFOTenantNames.Contains(n.CreatedByUser!.TenantName!) && hideUser
                                      ? "Hidden"
                                      : n.CreatedByUser!.DisplayName!,
                                n.CreatedByUser.TenantId!,
                                n.CreatedByUser.TenantName!,
                                 n.Created!.Value
                            )).ToArray(),

                    Escalations = (from esc in db.EnrolmentEscalationQueue
                                   from n in esc.Notes
                                   where esc.ParticipantId == p.Id
                                   && (n.IsExternal || request.IncludeInternalNotes)
                                   select new EnrolmentSummaryNote(
                                       n.Message,
                                       CFOTenantNames.Contains(n.CreatedByUser!.TenantName!) && hideUser
                                            ? "Hidden"
                                            : n.CreatedByUser!.DisplayName!,
                                       n.CreatedByUser.TenantId!,
                                       n.CreatedByUser.TenantName!,
                                       n.Created!.Value
                                  )).ToArray()
                };

#pragma warning restore CS8602

            var count = await query.CountAsync(cancellationToken);

            var results = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedData<QAEnrolmentsResultsSummaryDto>(results, count, request.PageNumber, request.PageSize);
        }

        private bool ShouldHideUser(UserProfile user)
        {
            string[] allowed =
            [
                RoleNames.QAOfficer,
                RoleNames.QASupportManager,
                RoleNames.QAManager,
                RoleNames.SMT,
                RoleNames.SystemSupport
            ];

            return !user.AssignedRoles.Any(r => allowed.Contains(r));
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(x => x.UserProfile.UserId)
                    .NotNull();
            }
        }
    }
}