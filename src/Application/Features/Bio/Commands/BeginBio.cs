using System.Text.Json.Serialization;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Bio.Caching;
using Cfo.Cats.Application.Features.Bio.DTOs;
using Cfo.Cats.Application.Features.Bio.DTOs.V1.Pathways.Diversity;
using Cfo.Cats.Application.Features.Bio.DTOs.V1.Pathways.ChildhoodExperiences;
using Cfo.Cats.Application.Features.Bio.DTOs.V1.Pathways.RecentExperiences;
using Cfo.Cats.Application.Features.Bio.DTOs.V1.Pathways.Exemption;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Bios;
using Newtonsoft.Json;
using Cfo.Cats.Application.Features.Assessments.Caching;

namespace Cfo.Cats.Application.Features.Assessments.Commands;

public static class BeginBio
{
    [RequestAuthorize(Policy = PolicyNames.AllowEnrol)]
    public class Command : ICacheInvalidatorRequest<Result<Guid>>
    {
        public required string ParticipantId { get; set; }
        
        //TODO: this could be done at a per participant level
        public string[] CacheKeys => [ BioCacheKey.GetAllCacheKey ];
        public CancellationTokenSource? SharedExpiryTokenSource 
            => BioCacheKey.SharedExpiryTokenSource();
    }

    public class Handler : IRequestHandler<Command, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public Handler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            BioSurvey bio = new BioSurvey()
            {
                Id = Guid.NewGuid(),
                ParticipantId = request.ParticipantId,
                Pathways =
                [
                    new DiversityPathway(),
                ]
            };

            string json = JsonConvert.SerializeObject(bio, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            
            BioSurvey bs = bio.Create(bio.Id, request.ParticipantId, bioJson: json, _currentUserService.TenantId!);


            _unitOfWork.DbContext.bios.Add(bs);
            return await Result<Guid>.SuccessAsync(bio.Id);
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.ParticipantId)
                .MinimumLength(9)
                .MaximumLength(9);
        }
    }

}
