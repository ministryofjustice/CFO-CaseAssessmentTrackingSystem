using Cfo.Cats.Application.Common.Exceptions;
using Cfo.Cats.Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cfo.Cats.Server.Endpoints;

public class AuthController : Controller
{
    private readonly IDataProtectionProvider dataProtectionProvider;
    private readonly ILogger<AuthController> logger;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly UserManager<ApplicationUser> userManager;

    public AuthController(
        ILogger<AuthController> logger,
        IDataProtectionProvider dataProtectionProvider,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager
    )
    {
        this.logger = logger;
        this.dataProtectionProvider = dataProtectionProvider;
        this.userManager = userManager;
        this.signInManager = signInManager;
    }

    [HttpGet("/auth/login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(string token, string returnUrl)
    {
        var dataProtector = dataProtectionProvider.CreateProtector("Login");
        var data = dataProtector.Unprotect(token);
        var parts = data.Split('|');
        var identityUser = await userManager.FindByIdAsync(parts[0]);
        if (identityUser == null)
        {
            return Unauthorized();
        }

        var isTokenValid = await userManager.VerifyUserTokenAsync(
            identityUser,
            TokenOptions.DefaultProvider,
            "Login",
            parts[1]
        );
        if (isTokenValid)
        {
            var isPersistent = true;
            await userManager.ResetAccessFailedCountAsync(identityUser);
            await signInManager.SignInAsync(identityUser, isPersistent);
            identityUser.IsLive = true;
            await userManager.UpdateAsync(identityUser);
            logger.LogInformation("{@UserName} has successfully logged in", identityUser.UserName);
            return Redirect($"/{returnUrl}");
        }

        return Unauthorized();
    }

    [HttpGet("/auth/logout")]
    public async Task<IActionResult> Logout()
    {
        var userId = signInManager.Context.User.GetUserId();
        var identityUser =
            await userManager.Users.FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new NotFoundException("Application user not found.");
        identityUser.IsLive = false;
        await userManager.UpdateAsync(identityUser);
        logger.LogInformation("{@UserName} logout successful", identityUser.UserName);
        await signInManager.SignOutAsync();
        return Redirect("/");
    }
}
