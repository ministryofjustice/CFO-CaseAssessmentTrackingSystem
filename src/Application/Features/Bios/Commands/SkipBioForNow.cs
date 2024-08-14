using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Bios.Caching;
using Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.ChildhoodExperiences;
using Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.Diversity;
using Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.RecentExperiences;
using Cfo.Cats.Application.Features.Bios.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Bios;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.Bios.Commands;

public static class SkipBioForNow
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : ICacheInvalidatorRequest<Result>
    {
        //TODO: cache individually
        public string[] CacheKeys => [BiosCacheKey.GetAllCacheKey];
        public CancellationTokenSource? SharedExpiryTokenSource => BiosCacheKey.SharedExpiryTokenSource();

        public required Guid BioId { get; set; }

    }

    public class Handler : IRequestHandler<Command, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            Domain.Entities.Bios.ParticipantBio bio = _unitOfWork.DbContext.ParticipantBios.FirstOrDefault(r => r.Id == request.BioId)
                                       ?? throw new NotFoundException(nameof(Bio), new
                                       {
                                           request.BioId
                                       });

            bio.UpdateStatus(BioStatus.SkippedForNow);
            return Result.Success();
        }
    }

}
