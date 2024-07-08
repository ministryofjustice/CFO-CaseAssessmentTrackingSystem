using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Assessments.Caching;
using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Assessments;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.Assessments.Commands;

public static class SaveAssessment
{
    [RequestAuthorize(Policy = PolicyNames.AllowEnrol)]
    public class Command : ICacheInvalidatorRequest<Result>
    {
        //TODO: cache individually
        public string CacheKey => AssessmentsCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => AssessmentsCacheKey.SharedExpiryTokenSource();

        public bool Submit { get; set; } = false;
        
        public required Assessment Assessment { get; set; } 
        
    }

    public class Handler : IRequestHandler<Command, Result>
    {
        private readonly IApplicationDbContext _context;
        
        public Handler(IApplicationDbContext context)
        {
            _context = context;

        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            ParticipantAssessment pa = _context.ParticipantAssessments.FirstOrDefault(r => r.Id == request.Assessment.Id && r.ParticipantId == request.Assessment.ParticipantId)
                                       ?? throw new NotFoundException(nameof(Assessment), new
                                       {
                                           request.Assessment.Id,
                                           request.Assessment.ParticipantId
                                       });
         
            
            pa.UpdateJson(JsonConvert.SerializeObject(request.Assessment, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            }));

            if (request.Submit)
            {
                var details = await _context.Participants
                    .Where(p => p.Id == request.Assessment.ParticipantId)
                    .Select(p =>
                        new
                        {
                            p.DateOfBirth,
                            LotNumber = p.EnrolmentLocation!.Contract!.LotNumber,
                            Gender = "Male"
                        }
                    ).FirstAsync(cancellationToken);

                Sex sex = Sex.FromName(details.Gender);
                AssessmentLocation location = AssessmentLocation.FromValue(details.LotNumber);
                var age = details.DateOfBirth!.Value.CalculateAge();
                
                foreach(var pathway in request.Assessment.Pathways)
                {
                    pa.SetPathwayScore(pathway.Title, pathway.GetRagScore(age, location, sex));
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return await Result.SuccessAsync();
        }
    }

}
