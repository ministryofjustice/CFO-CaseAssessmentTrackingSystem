using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Payables.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Payables.Queries
{
    public static class GetActivityQaNotes
    {
        [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
        public class Query : IRequest<Result<ActivityQaNoteDto[]>>
        {
            public Guid? ActivityId { get; set; }

            public bool IncludeInternalNotes { get; set; }

            public UserProfile? CurentUser { get; set; }
        }

        public class Handler(
            IUnitOfWork unitOfWork,
            IMapper mapper) : IRequestHandler<Query, Result<ActivityQaNoteDto[]>>
        {
            public async Task<Result<ActivityQaNoteDto[]>> Handle(Query request, CancellationToken cancellationToken)
            {
                var pqa = await GetPqaNotes(request.ActivityId!);

                //todo under CFODEV-863
                //var qa1 = await GetQa1Notes(request.ActivityId!, request.IncludeInternalNotes);
                //var qa2 = await GetQa2Notes(request.ActivityId!, request.IncludeInternalNotes);
                //var es = await GetEscalationNotes(request.ActivityId!, request.IncludeInternalNotes);
                //return Result<ActivityQaNoteDto[]>.Success(pqa.Union(qa1).Union(qa2).Union(es).ToArray()
                //);
                return Result<ActivityQaNoteDto[]>.Success(pqa.ToArray());
            }

            private async Task<ActivityQaNoteDto[]> GetPqaNotes(Guid? activityId)
            {
                var query1 = unitOfWork.DbContext.ActivityPqaQueue
                                        .AsNoTracking()
                                        .Where(c => c.ActivityId == activityId)
                                        .SelectMany(c => c.Notes)
                                        .ProjectTo<ActivityQaNoteDto>(mapper.ConfigurationProvider);

                var results = await query1.ToArrayAsync()!;
                return results;
            }

            //todo under CFODEV-863
            //private async Task<ActivityQaNoteDto[]> GetQa1Notes(string participantId, bool includeInternalNotes)
            //{
            //    var query1 = unitOfWork.DbContext.EnrolmentQa1Queue
            //                            .AsNoTracking()
            //                            .Where(c => c.ParticipantId == participantId)
            //                            .SelectMany(c => c.Notes.Where(n => n.IsExternal || includeInternalNotes))
            //                            .ProjectTo<ActivityQaNoteDto>(mapper.ConfigurationProvider);


            //    var results = await query1.ToArrayAsync()!;
            //    return results;
            //}

            //private async Task<ActivityQaNoteDto[]> GetQa2Notes(string participantId, bool includeInternalNotes)
            //{
            //    var query1 = unitOfWork.DbContext.EnrolmentQa2Queue
            //                            .AsNoTracking()
            //                            .Where(c => c.ParticipantId == participantId)
            //                            .SelectMany(c => c.Notes.Where(n => n.IsExternal || includeInternalNotes))
            //                            .ProjectTo<ActivityQaNoteDto>(mapper.ConfigurationProvider);


            //    var results = await query1.ToArrayAsync()!;
            //    return results;
            //}
            //private async Task<ActivityQaNoteDto[]> GetEscalationNotes(string participantId, bool includeInternalNotes)
            //{
            //    var query1 = unitOfWork.DbContext.EnrolmentEscalationQueue
            //                            .AsNoTracking()
            //                            .Where(c => c.ParticipantId == participantId)
            //                            .SelectMany(c => c.Notes.Where(n => n.IsExternal || includeInternalNotes))
            //                            .ProjectTo<ActivityQaNoteDto>(mapper.ConfigurationProvider);


            //    var results = await query1.ToArrayAsync()!;
            //    return results;
            //}
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(x => x.CurentUser)
                    .NotNull();
            }
        }
    }
}