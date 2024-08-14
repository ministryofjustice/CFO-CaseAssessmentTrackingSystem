using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Bios.Caching;
using Cfo.Cats.Application.Features.Bios.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Bios;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.Bios.Commands;

public static class SaveBio
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : ICacheInvalidatorRequest<Result>
    {
        //TODO: cache individually
        public string[] CacheKeys =>  [ BiosCacheKey.GetAllCacheKey ];
        public CancellationTokenSource? SharedExpiryTokenSource => BiosCacheKey.SharedExpiryTokenSource();

        public bool Submit { get; set; } = false;
        
        public required Bio Bio { get; set; } 
        
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
            Domain.Entities.Bios.ParticipantBio bio = _unitOfWork.DbContext.ParticipantBios.FirstOrDefault(r => r.Id == request.Bio.Id && r.ParticipantId == request.Bio.ParticipantId)
                                       ?? throw new NotFoundException(nameof(Bio), new
                                       {
                                           request.Bio.Id,
                                           request.Bio.ParticipantId
                                       });
         
            
            bio.UpdateJson(JsonConvert.SerializeObject(request.Bio, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            }));
            bio.UpdateStatus(BioStatus.InProgress);
            if (request.Submit)
            {
                bio.UpdateStatus(BioStatus.Complete);
                bio.Submit();
            }

            return Result.Success();
        }
    }

}
