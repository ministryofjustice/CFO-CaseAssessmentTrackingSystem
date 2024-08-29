using Microsoft.AspNetCore.Http;

namespace Cfo.Cats.Infrastructure.Services;

public class NetworkIpProvider(IHttpContextAccessor httpContextAccessor) : INetworkIpProvider
{
    public string IpAddress
    {
        get
        {
            var context = httpContextAccessor.HttpContext!;
            var ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim()
                            ?? context.Connection.RemoteIpAddress?.ToString()!;
            return ipAddress;
        }
    }
}
