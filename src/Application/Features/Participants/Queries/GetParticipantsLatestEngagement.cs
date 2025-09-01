﻿using Ardalis.Specification.EntityFrameworkCore;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetParticipantsLatestEngagement
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Query : PaginationFilter, IRequest<Result<PaginatedData<ParticipantEngagementDto>>>
    {
        public required UserProfile CurrentUser { get; set; }
        public bool JustMyCases { get; set; } = false;
        public bool HideRecentEngagements { get; set; } = false;
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<PaginatedData<ParticipantEngagementDto>>>
    {
        public async Task<Result<PaginatedData<ParticipantEngagementDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var db = unitOfWork.DbContext;

#pragma warning disable CS8602, CS8604
            var query =
                from participant in db.Participants
                join engagement in db.ParticipantEngagements
                    on participant.Id equals engagement.ParticipantId into leftJoin
                from engagement in leftJoin
                    .OrderByDescending(pe => pe.EngagedOn)
                    .ThenByDescending(pe => pe.CreatedOn)
                    .Take(1)
                    .DefaultIfEmpty()
                join owner in db.Users on participant.OwnerId equals owner.Id
                join currentLocation in db.Locations on participant.CurrentLocation.Id equals currentLocation.Id
                where participant.Owner.TenantId.StartsWith(request.CurrentUser.TenantId)
                where request.JustMyCases == false || participant.Owner.Id == request.CurrentUser.UserId
                where participant.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value
                where request.HideRecentEngagements == false || (engagement == null || engagement.EngagedOn < DateOnly.FromDateTime(DateTime.Today).AddMonths(-3))
                where string.IsNullOrWhiteSpace(request.Keyword)
                    || new[] 
                    { 
                        engagement.Description, 
                        engagement.Category, 
                        engagement.EngagedAtLocation, 
                        participant.FirstName, 
                        participant.LastName, 
                        participant.Id 
                    }.Any(f => f.Contains(request.Keyword))
                select new
                {
                    participant.Id,
                    FullName = participant.FirstName + " " + participant.LastName,
                    engagement.Category,
                    engagement.Description,
                    engagement.EngagedAtLocation,
                    engagement.EngagedAtContract,
                    engagement.EngagedWith,
                    engagement.EngagedWithTenant,
                    owner.DisplayName,
                    CurrentLocationName = currentLocation.Name,
                    engagement.EngagedOn 
                };
#pragma warning restore CS8602, CS8604

            var count = await query.CountAsync(cancellationToken);

            var engagements = await query
                .AsQueryable()
                .OrderBy($"{request.OrderBy} {request.SortDirection}")
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(e => new ParticipantEngagementDto(
                    e.Id,
                    e.FullName,
                    e.Category,
                    e.Description,
                    e.EngagedAtLocation,
                    e.EngagedAtContract,
                    e.EngagedWith,
                    e.EngagedWithTenant,
                    e.DisplayName,
                    e.CurrentLocationName,
                    e.EngagedOn
                )).ToListAsync(cancellationToken);

            return Result<PaginatedData<ParticipantEngagementDto>>.Success(
                new PaginatedData<ParticipantEngagementDto>(engagements, count, request.PageNumber, request.PageSize));
        }
    }
}
