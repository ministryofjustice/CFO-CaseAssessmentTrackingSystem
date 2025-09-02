using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Activities;

namespace Cfo.Cats.Application.Features.Activities.Queries;

public static class GetPreviousSameTypeActivitiesById
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Query : IRequest<List<ActivityQaDetailsDto>>
    {
        public required Guid? Id { get; set; }
    }

    private class Handler(IUnitOfWork unitOfWork, IMapper mapper)
        : IRequestHandler<Query, List<ActivityQaDetailsDto>>
    {
        public async Task<List<ActivityQaDetailsDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var activity = GetActivityById(request.Id);

            var previousActivities = await unitOfWork.DbContext.Activities
                .Include(a => a.TookPlaceAtLocation)
                .Include(a => a.Participant)
                .Where(a => a.ParticipantId == activity!.ParticipantId
                            && a.Type == activity!.Type
                            && a.Id != request.Id
                            && a.ApprovedOn != null)
                .ToListAsync(cancellationToken);

            List<ActivityQaDetailsDto> listOfPreviousActivities = new List<ActivityQaDetailsDto>();

            foreach (var act in previousActivities)
            {
                if (act is ActivityWithTemplate x)
                {
                    await unitOfWork.DbContext.Activities.Entry(x).Reference(a => (a as ActivityWithTemplate)!.Document).LoadAsync();
                }

                var _activityQaDetailsDto = mapper.Map<ActivityQaDetailsDto>(act);
                _activityQaDetailsDto.ActivityId = act.Id;
                listOfPreviousActivities.Add(_activityQaDetailsDto);
            }

            return listOfPreviousActivities;
        }

        private ActivityWithTemplate? GetActivityById(Guid? id)
        {
            var activity = unitOfWork.DbContext.Activities
                .Include(a => a.TookPlaceAtLocation)
                .Include(a => a.Participant)
                .SingleOrDefault(a => a.Id == id);

            return (ActivityWithTemplate?)activity;
        }
    };
}