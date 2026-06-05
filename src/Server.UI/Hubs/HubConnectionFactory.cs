using System.Net;
using System.Net.Security;
using Microsoft.AspNetCore.SignalR.Client;

namespace Cfo.Cats.Server.UI.Hubs;

/// <summary>
/// Creates authenticated <see cref="HubConnection"/> instances that loop back to this app's
/// own hubs. The current user's identity cookie is captured from the active
/// <see cref="HttpContext"/> and forwarded so the hub's <c>[Authorize]</c> attribute resolves
/// the same <see cref="System.Security.Claims.ClaimsPrincipal"/> (and therefore tenant) as the
/// originating request.
/// </summary>
public sealed class HubConnectionFactory : IHubConnectionFactory
{
    private const string IdentityCookieName = ".AspNetCore.Identity.Application";

    private readonly NavigationManager _navigationManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HubConnectionFactory(
        NavigationManager navigationManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _navigationManager = navigationManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public HubConnection CreateForCurrentUser(string relativeHubUrl)
    {
        var hubUrl = _navigationManager.BaseUri.TrimEnd('/') + relativeHubUrl;
        var host = new Uri(hubUrl).Host;
        var cookies = new CookieContainer();

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is not null &&
            httpContext.Request.Cookies.TryGetValue(IdentityCookieName, out var authCookie))
        {
            cookies.Add(new Cookie(IdentityCookieName, authCookie)
            {
                Domain = host,
                Path = "/"
            });
        }

        return new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                options.Cookies = cookies;

                // The loopback connection targets this same server; accept its certificate so
                // local/self-signed development certificates do not break the handshake.
                options.HttpMessageHandlerFactory = handler =>
                {
                    if (handler is SocketsHttpHandler sockets)
                    {
                        sockets.SslOptions ??= new SslClientAuthenticationOptions();
                        sockets.SslOptions.RemoteCertificateValidationCallback = (_, _, _, _) => true;
                    }
                    else if (handler is HttpClientHandler http)
                    {
                        http.ServerCertificateCustomValidationCallback =
                            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                    }

                    return handler;
                };
            })
            .WithAutomaticReconnect()
            .Build();
    }
}
