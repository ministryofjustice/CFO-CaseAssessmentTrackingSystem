using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Assessments.Caching;
using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Assessments;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.Assessments.Commands;

public static class SaveAssessment
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : ICacheInvalidatorRequest<Result>
    {
        //TODO: cache individually
        public string[] CacheKeys =>  [ AssessmentsCacheKey.GetAllCacheKey ];
        public CancellationTokenSource? SharedExpiryTokenSource => AssessmentsCacheKey.SharedExpiryTokenSource();

        public bool Submit { get; set; } = false;
        
        public required Assessment Assessment { get; set; } 
        
    }

    public class Handler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            ParticipantAssessment pa = unitOfWork.DbContext.ParticipantAssessments.FirstOrDefault(r => r.Id == request.Assessment.Id && r.ParticipantId == request.Assessment.ParticipantId)
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
                var details = await unitOfWork.DbContext.Participants
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

                pa.Submit(currentUserService.UserId!);
            }

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.Assessment.Id)
                .MustAsync(Exist)
                .WithMessage("Assessment not found");

            RuleFor(c => c.Assessment.Id)
                .MustAsync(NotBeCompleted)
                .WithMessage("Assessment already complete");

            RuleFor(c => c.Assessment.ParticipantId)
                .MustAsync(Exist)
                .WithMessage("Participant not found")
                .MustAsync(HaveEnrolmentLocation)
                .WithMessage("Participant must have an enrolment location");
        }

        private async Task<bool> Exist(Guid assessmentId, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.ParticipantAssessments.AnyAsync(e => e.Id == assessmentId, cancellationToken);

        private async Task<bool> Exist(string participantId, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == participantId, cancellationToken);

        private async Task<bool> HaveEnrolmentLocation(string participantId, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == participantId && e.EnrolmentLocation != null, cancellationToken);

        private async Task<bool> NotBeCompleted(Guid assessmentId, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.ParticipantAssessments.AnyAsync(pa => pa.Id == assessmentId && pa.Completed == null, cancellationToken);
    }
}
