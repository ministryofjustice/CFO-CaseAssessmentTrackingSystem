using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;

namespace Cfo.Cats.Server.UI.Middlewares;

public class SessionTimeoutMiddleware(RequestDelegate next, ISessionService sessionService) 
{

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity is { IsAuthenticated: true } && 
            context.Request.Path.StartsWithSegments("/pages/authentication") == false)
        {
            var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (sessionService.IsSessionValid(userId) == false)
            {
                context.Response.Cookies.Append(".AspNetCore.Identity.Application", "", new CookieOptions
                {
                    Expires = DateTimeOffset.UnixEpoch,
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Path = "/"
                });
                
                context.Response.StatusCode = 200;
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(@"
                    <html><head>
                    <meta http-equiv='Cache-Control' content='no-store' />
                    <script>
                        window.location.replace('/pages/authentication/login?timeout=true');
                    </script>
                    </head><body></body></html>");
            }
            
            // refresh the activity on a page refresh
            sessionService.UpdateActivity(userId);
        }
        
        await next(context);
    }
    
}
