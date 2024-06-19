using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.KeyValues.Caching;
using Cfo.Cats.Application.Features.KeyValues.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.KeyValues.Commands.AddEdit;

[RequestAuthorize(Policy = PolicyNames.SystemFunctionsWrite)]
public class AddEditKeyValueCommand : ICacheInvalidatorRequest<Result<int>>
{
    [Description("Id")] public int Id { get; set; }

    [Description("Name")] public Picklist Name { get; set; }

    [Description("Value")] public string? Value { get; set; }

    [Description("Text")] public string? Text { get; set; }

    [Description("Description")] public string? Description { get; set; }

    public TrackingState TrackingState { get; set; } = TrackingState.Unchanged;
    public string CacheKey => KeyValueCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => KeyValueCacheKey.SharedExpiryTokenSource();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<KeyValueDto, AddEditKeyValueCommand>(MemberList.None);
            CreateMap<AddEditKeyValueCommand, KeyValue>(MemberList.None);
        }
    }
}