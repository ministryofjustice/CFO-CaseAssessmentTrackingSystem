using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.PathwayPlans.EventHandlers;
using Cfo.Cats.Application.SecurityConstants;
using Microsoft.AspNetCore.Authorization;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetRecentParticipants
{
    
    /// <summary>
    /// Returns the last 5 participants the user has accessed
    /// </summary>
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IQuery<Result<ParticipantSearchResultDto[]>>
    {
        public UserProfile CurrentUser { get; set; } = default!;
    }

    public class Handler(IUnitOfWork unitOfWork) : IQueryHandler<Query, Result<ParticipantSearchResultDto[]>>
    {
        public async Task<Result<ParticipantSearchResultDto[]>> Handle(Query request, CancellationToken cancellationToken)
        {
            // Find the most recent access date per participant for this user,
            // take the five most recently accessed, and project the participant
            // details - all in a single query so ordering is preserved.
            var recent = from a in unitOfWork.DbContext.AccessAuditTrails
                         where a.UserId == request.CurrentUser.UserId
                         group a by a.ParticipantId into g
                         select new { ParticipantId = g.Key, LastAccessed = g.Max(x => x.AccessDate) } into r
                         orderby r.LastAccessed descending
                         select r;

            var participantQuery = from r in recent.Take(5)
                                   join p in unitOfWork.DbContext.Participants on r.ParticipantId equals p.Id
                                   orderby r.LastAccessed descending
                                   select new ParticipantSearchResultDto()
                                   {
                                       FirstName = p.FirstName,
                                       LastName = p.LastName,
                                       Id = p.Id,
                                       CurrentLocation = p.CurrentLocation.Name,
                                   };

            return await participantQuery.ToArrayAsync(cancellationToken);
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(q => q.CurrentUser)
                .NotNull();

            RuleFor(q => q.CurrentUser.UserId)
                .NotEmpty()
                .NotNull();

            RuleFor(q => q.CurrentUser.TenantId)
                .NotNull();
        }
    }

}