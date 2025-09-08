using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.Features.Activities.Specifications;
using Cfo.Cats.Application.SecurityConstants;
using static Cfo.Cats.Application.Features.Activities.DTOs.QAActivitiesResultsSummaryDto;

namespace Cfo.Cats.Application.Features.Activities.Queries;

public static class QAActivitiesResultsWithPagination
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Query : QAActivitiesResultsAdvancedFilter, IRequest<PaginatedData<QAActivitiesResultsSummaryDto>>
    {
        public QAActivitiesResultsAdvancedSpecification Specification => new(this);
    }

    public class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, PaginatedData<QAActivitiesResultsSummaryDto>>
    {
        public async Task<PaginatedData<QAActivitiesResultsSummaryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var db = unitOfWork.DbContext;

            bool hideUser = ShouldHideUser(request.UserProfile);
            var CFOTenantNames = new HashSet<string> { "CFO", "CFO Evolution" };

#pragma warning disable CS8602
            var query = from a in db.Activities.ApplySpecification(request.Specification)
                        select new QAActivitiesResultsSummaryDto
                        {
                            ParticipantId = a.Participant.Id,
                            Participant = $"{a.Participant.FirstName} {a.Participant.LastName}",
                            Status = a.Status,
                            Definition = a.Definition,
                            ApprovedOn = a.ApprovedOn,
                            LastModified = a.LastModified,
                            Created = a.Created,
                            CommencedOn = a.CommencedOn,
                            TookPlaceAtLocationName = a.TookPlaceAtLocation.Name,
                            AdditionalInformation = a.AdditionalInformation!,
                            PQA = (from pqa in db.ActivityPqaQueue
                                   from n in pqa.Notes
                                   where pqa.ActivityId == a.Id 
                                   select new ActSummaryNote(
                                       n.Message,
                                       CFOTenantNames.Contains(n.CreatedByUser.TenantName!) && hideUser 
                                            ? "Hidden" 
                                            : n.CreatedByUser!.DisplayName!,
                                       n.CreatedByUser.TenantId!,
                                       n.CreatedByUser.TenantName!,
                                       n.Created!.Value
                                   )).ToArray(),
                                                                        
                            QA1 = (from qa1 in db.ActivityQa1Queue 
                                   from n in qa1.Notes
                                   where qa1.ActivityId == a.Id
                                   select new ActSummaryNote(
                                        n.Message,
                                        CFOTenantNames.Contains(n.CreatedByUser.TenantName!) && hideUser
                                             ? "Hidden"
                                             : n.CreatedByUser!.DisplayName!,
                                        n.CreatedByUser.TenantId!,
                                        n.CreatedByUser.TenantName!,
                                        n.Created!.Value
                                    )).ToArray(),

                            QA2 = (from qa2 in db.ActivityQa2Queue 
                                   from n in qa2.Notes
                                   where qa2.ActivityId == a.Id
                                   select new ActSummaryNote(
                                         n.Message,
                                         CFOTenantNames.Contains(n.CreatedByUser.TenantName!) && hideUser
                                              ? "Hidden"
                                              : n.CreatedByUser!.DisplayName!,
                                        n.CreatedByUser.TenantId!,
                                        n.CreatedByUser.TenantName!,
                                         n.Created!.Value
                                    )).ToArray(),

                            Escalations = (from esc in db.ActivityEscalationQueue 
                                           from n in esc.Notes
                                           where esc.ActivityId == a.Id
                                           select new ActSummaryNote(
                                               n.Message,
                                               CFOTenantNames.Contains(n.CreatedByUser.TenantName!) && hideUser
                                                    ? "Hidden"
                                                    : n.CreatedByUser!.DisplayName!,
                                               n.CreatedByUser.TenantId!,
                                               n.CreatedByUser.TenantName!,
                                               n.Created!.Value
                                          )).ToArray(),
                            ActivityId = a.Id,
                            SubmittedBy = request.JustMyParticipants ? "You" : $"{a.Owner.DisplayName!} ({a.Owner.TenantName})"
                        };
#pragma warning restore CS8602

            var count = await query.CountAsync(cancellationToken);

            var results = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedData<QAActivitiesResultsSummaryDto>(results, count, request.PageNumber, request.PageSize);
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