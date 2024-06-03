using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Candidates.DTOs;
using Cfo.Cats.Application.Features.Participants.Caching;
using Cfo.Cats.Application.Features.Participants.DTOs;

namespace Cfo.Cats.Application.Features.Participants.Commands.Enrol;

[RequestAuthorize(Roles = "Admin, Basic")]
public class EnrolParticipantCommand : ICacheInvalidatorRequest<Result<string>>
{
    /// <summary>
    /// The CATS identifier
    /// </summary>
    public string? Identifier { get; set; }
    
    public string? ReferralSource { get; set; }
    
    public string? ReferralComments { get; set; }
    
    public UserProfile? CurrentUser { get; set; }
    
    public string CacheKey => ParticipantCacheKey.GetCacheKey($"{this}");

    public CancellationTokenSource? SharedExpiryTokenSource 
        => ParticipantCacheKey.SharedExpiryTokenSource();

    public override string ToString()
    {
        return $"Id:{this.Identifier}";
    }
}