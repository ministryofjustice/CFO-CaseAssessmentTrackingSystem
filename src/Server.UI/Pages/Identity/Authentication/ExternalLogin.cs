using System.Security.Claims;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Identity.Notifications.IdentityEvents;
using Cfo.Cats.Application.Features.Identity.Notifications.SendTwoFactorCode;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Infrastructure.Services;
using Cfo.Cats.Server.UI.Pages.Identity.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Cfo.Cats.Infrastructure.Services.Identity.CustomSigninManager;
using IResult = Microsoft.AspNetCore.Http.IResult;

public static class ExternalLogin
{
    public static IEndpointRouteBuilder MapExternalLogins(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/external-login", Login);
        builder.MapGet("/external-login-callback", ExternalLoginCallback);
        return builder;
    }

    private static async Task<IResult> Login(
        string provider,
        string? returnUrl,
        [FromServices] SignInManager<ApplicationUser> signInManager)
    {
        var redirectUrl = $"/external-login-callback?returnUrl={Uri.EscapeDataString(returnUrl ?? "/")}";
        var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

        // Kick off the OIDC challenge
        return Results.Challenge(properties, new[] { provider });
    }

    private static async Task<IResult> ExternalLoginCallback(
        string? returnUrl,
        HttpContext httpContext,
        [FromServices] UserManager<ApplicationUser> userManager,
        [FromServices] SignInManager<ApplicationUser> signInManager,
        [FromServices] ISessionService sessionService,
        [FromServices] IMediator mediator,
        [FromServices] INetworkIpProvider networkIpProvider)
    {
        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info is null)
        {
            return await Redirect($"/pages/authentication/login?error={Uri.EscapeDataString(ConstantString.Errors.ExternalLoginFailed)}", httpContext);
        }

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new Exception("Cannot find email claim");
        }

        Microsoft.AspNetCore.Identity.SignInResult? result;
        
        // Sign in: if the user has previously linked their external login to a local account this will succeed
        result = await signInManager.ExternalLoginSignInAsync(
            loginProvider: info.LoginProvider,
            providerKey: info.ProviderKey,
            isPersistent: false,
            bypassTwoFactor: false);

        if (result.Succeeded)
        {
            await mediator.Publish(IdentityAuditNotification.ExternalLogin(email, networkIpProvider.IpAddress));
            return await Redirect(returnUrl ?? "/", httpContext);
        }

        // We continue below to link the external login to an existing user account
        
        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return await Redirect($"/pages/authentication/login?error={Uri.EscapeDataString(ConstantString.Errors.InvalidLoginAttempt)}", httpContext);
        }

        // Link the external login to this local user
        await userManager.AddLoginAsync(user, info);

        // Sign in
        result = await signInManager.ExternalLoginSignInAsync(
            loginProvider: info.LoginProvider,
            providerKey: info.ProviderKey,
            isPersistent: false,
            bypassTwoFactor: false);

        if(result is { RequiresTwoFactor: true })
        {
            var token = await userManager.GenerateTwoFactorTokenAsync(user!, "Email");
            await mediator.Publish(new SendTwoFactorEmailCodeNotification(user!.Email!, user.UserName!, token));
            return await Redirect(LoginWith2fa.PageUrl, httpContext);
        }

        if(result.Succeeded)
        {
            await mediator.Publish(IdentityAuditNotification.ExternalLogin(email, networkIpProvider.IpAddress));
            return await Redirect(returnUrl ?? "/", httpContext);
        }

        return await Redirect($"/pages/authentication/login?error={GetEscapedErrorCode(result)}", httpContext);
    }

    private static async Task<IResult> Redirect(string url, HttpContext httpContext)
    {
        await httpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        return Results.Redirect(url);
    }

    private static string GetEscapedErrorCode(Microsoft.AspNetCore.Identity.SignInResult result)
    {
        var error = result switch
        {
            CustomSignInResult { IsInactive: true } => ConstantString.Errors.AccountInactive,
            { IsLockedOut: true } => ConstantString.Errors.AccountLockedOut,
            { IsNotAllowed: true } => ConstantString.Errors.NotAllowed,
            _ => ConstantString.Errors.InvalidLoginAttempt
        };

        return Uri.EscapeDataString(error);
    }

}