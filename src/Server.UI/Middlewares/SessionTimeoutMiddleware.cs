using Cfo.Cats.Application.Common.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Cfo.Cats.Server.UI.Middlewares;

public class SessionTimeoutMiddleware(RequestDelegate next, ISessionService sessionService) 
{

    public async Task InvokeAsync(HttpContext context)
    {
        bool isAuthPipeline = context.Request.Path.StartsWithSegments("/pages/authentication") ||
                            context.Request.Path.StartsWithSegments("/external-login");

        if (context.User.Identity is { IsAuthenticated: true } &&  isAuthPipeline == false)
        {
            var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (sessionService.IsSessionValid(userId) == false)
            {
                await context.SignOutAsync(IdentityConstants.ApplicationScheme);
                await context.SignOutAsync(IdentityConstants.ExternalScheme);

                context.Response.Redirect($"/pages/authentication/login?timeout=true");
                return;
            }
            
            // refresh the activity on a page refresh
            sessionService.UpdateActivity(userId);
        }
        
        await next(context);
    }
    
}
