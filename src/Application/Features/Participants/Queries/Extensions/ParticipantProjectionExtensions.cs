using Cfo.Cats.Application.Features.Labels.DTOs;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Specifications;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.Queries.Extensions;

public static class ParticipantProjectionExtensions
{
    public static IQueryable<ParticipantPaginationDto> ProjectToPaginationDto(
        this IQueryable<Participant> query,
        IApplicationDbContext context,
        RecentParticipantFilter recentAction,
        string currentUserId)
    {
        var recentlyAssignedCutoff = recentAction switch
        {
            RecentParticipantFilter.AssignedLast10Days => DateTime.UtcNow.Date.AddDays(-10),
            RecentParticipantFilter.AssignedLast30Days => DateTime.UtcNow.Date.AddDays(-30),
            _ => (DateTime?)null
        };

        var recentlyVisitedCutoff = recentAction switch
        {
            RecentParticipantFilter.VisitedLast7Days => DateTime.UtcNow.Date.AddDays(-7),
            _ => (DateTime?)null
        };

        var recentlyArchivedCutoff = recentAction switch
        {
            RecentParticipantFilter.ArchivedLast30Days => DateTime.UtcNow.Date.AddDays(-30),
            _ => (DateTime?)null
        };

        // Only join to the extra history tables when the selected filter needs those dates
        var transformedSource = recentlyAssignedCutoff.HasValue
            ? from p in query
              join oh in (
                  from h in context.ParticipantOwnershipHistories
                  where h.OwnerId == currentUserId
                        && h.To == null
                  group h by h.ParticipantId into g
                  select new
                  {
                      ParticipantId = g.Key,
                      MostRecentFrom = g.Max(x => x.From)
                  }
              ) on p.Id equals oh.ParticipantId into ownershipGroup
              from ownership in ownershipGroup.DefaultIfEmpty()
              select new
              {
                  AssignedOn = ownership != null ? ownership.MostRecentFrom : (DateTime?)null,
                  AccessedOn = (DateTime?)null,
                  ArchivedOn = (DateTime?)null,
                  Participant = p
              }
            : recentlyVisitedCutoff.HasValue
            ? from p in query
              join oh in (
                  from h in context.AccessAuditTrails
                  where h.UserId == currentUserId
                  group h by h.ParticipantId into g
                  select new
                  {
                      ParticipantId = g.Key,
                      MostRecentAccess = g.Max(x => x.AccessDate)
                  }
              ) on p.Id equals oh.ParticipantId into visitedGroup
              from visited in visitedGroup.DefaultIfEmpty()
              select new
              {
                  AssignedOn = (DateTime?)null,
                  AccessedOn = visited != null ? visited.MostRecentAccess : (DateTime?)null,
                  ArchivedOn = (DateTime?)null,
                  Participant = p
              }
            : recentlyArchivedCutoff.HasValue
            ? from p in query
              join oh in (
                  from h in context.ParticipantEnrolmentHistories
                  where h.EnrolmentStatus == EnrolmentStatus.ArchivedStatus.Value
                        && h.From >= recentlyArchivedCutoff.Value
                  group h by h.ParticipantId into g
                  select new
                  {
                      ParticipantId = g.Key,
                      MostRecentArchive = g.Max(x => x.From)
                  }
              ) on p.Id equals oh.ParticipantId into archivedGroup
              from archived in archivedGroup.DefaultIfEmpty()
              select new
              {
                  AssignedOn = (DateTime?)null,
                  AccessedOn = (DateTime?)null,
                  ArchivedOn = archived != null ? archived.MostRecentArchive : (DateTime?)null,
                  Participant = p
              }
            : from p in query
              select new
              {
                  AssignedOn = (DateTime?)null,
                  AccessedOn = (DateTime?)null,
                  ArchivedOn = (DateTime?)null,
                  Participant = p
              };

        return from item in transformedSource
               select new ParticipantPaginationDto()
               {
                   AssignedOn = item.AssignedOn,
                   AccessedOn = item.AccessedOn,
                   ArchivedOn = item.ArchivedOn,
                   DeactivatedInFeed = item.Participant.DeactivatedInFeed,
                   EnrolmentStatus = item.Participant.EnrolmentStatus!,
                   Owner = item.Participant.Owner!.DisplayName!,
                   ConsentStatus = item.Participant.ConsentStatus!,
                   CurrentLocation = new LocationDto
                   {
                       Id = item.Participant.CurrentLocation.Id,
                       Name = item.Participant.CurrentLocation.Name,
                       GenderProvision = item.Participant.CurrentLocation.GenderProvision,
                       LocationType = item.Participant.CurrentLocation.LocationType,
                       ContractName = item.Participant.CurrentLocation.Contract!.Description
                   },
                   Id = item.Participant.Id,
                   EnrolmentLocation = item.Participant.EnrolmentLocation == null
                       ? null
                       : new LocationDto
                       {
                           Name = item.Participant.EnrolmentLocation.Name,
                           GenderProvision = item.Participant.EnrolmentLocation.GenderProvision,
                           LocationType = item.Participant.EnrolmentLocation.LocationType,
                           Id = item.Participant.EnrolmentLocation.Id,
                           ContractName = item.Participant.EnrolmentLocation.Contract!.Description
                       },
                   FirstName = item.Participant.FirstName,
                   LastName = item.Participant.LastName,
                   RiskDue = item.Participant.RiskDue,
                   RiskDueReason = item.Participant.RiskDueReason!,
                   Tenant = item.Participant.Owner!.TenantName!,
                   Labels = (
                           from pl in context.ParticipantLabels
                           where EF.Property<string>(pl, "_participantId") == item.Participant.Id
                               && pl.Lifetime.EndDate > DateTime.UtcNow
                           orderby pl.Lifetime.StartDate descending
                           select new LabelDto
                           {
                               Name = pl.Label.Name,
                               Description = pl.Label.Description,
                               Scope = pl.Label.Scope,
                               Contract = pl.Label.ContractId!,
                               Id = pl.Label.Id.Value,
                               AppIcon = pl.Label.AppIcon,
                               Colour = pl.Label.Colour,
                               Variant = pl.Label.Variant
                           }).ToArray(),
                   ConsentGranted = item.Participant.DateOfFirstConsent
               };
    }
}
