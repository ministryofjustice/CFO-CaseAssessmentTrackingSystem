using Cfo.Cats.Domain.Identity;

namespace Cfo.Cats.Infrastructure.Services.Identity;

public class ApplicationUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<ApplicationUserManager> logger)
    : UserManager<ApplicationUser>(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
{
    private readonly IServiceProvider _serviceProvider = services;

    public override async Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
    {
        if (await IsPasswordInHistory(user, newPassword))
        {
            IdentityError error = new IdentityError()
            {
                Code = "CATS_01",
                Description = "You cannot reuse your previous passwords"
            };
            return IdentityResult.Failed(error);
        }

        var result = await base.ChangePasswordAsync(user, currentPassword, newPassword);
        if (result.Succeeded)
        {
            await AddPasswordToHistory(user, user.PasswordHash!);
        }
        return result;
    }
    
    public override async Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword)
    {
        ThrowIfDisposed();
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        // Verify the reset token
        if (!await VerifyUserTokenAsync(user, Options.Tokens.PasswordResetTokenProvider, ResetPasswordTokenPurpose, token))
        {
            return IdentityResult.Failed(ErrorDescriber.InvalidToken());
        }

        // Check password history
        if (await IsPasswordInHistory(user, newPassword))
        {
            IdentityError error = new IdentityError()
            {
                Code = "CATS_01",
                Description = "You cannot reuse your previous passwords"
            };
            return IdentityResult.Failed([error]);
        }

        // Update the password hash
        var result = await UpdatePasswordHash(user, newPassword, validatePassword: true);
        if (result.Succeeded == false)
        {
            return result;
        }

        // Save the new password to the history
        await AddPasswordToHistory(user, user.PasswordHash!);

        // Update the user
        return await UpdateUserAsync(user);
    }
    
    private async ValueTask<bool> IsPasswordInHistory(ApplicationUser user, string newPassword)
    {
        var passwordHasher = new PasswordHasher<ApplicationUser>();

        using var scope = _serviceProvider.CreateScope();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        
        var passwordHistories = await uow.DbContext.PasswordHistories
            .AsNoTracking()
            .Where(ph => ph.UserId == user.Id)
            .ToListAsync();
        
        foreach (var passwordHistory in passwordHistories)
        {
            if (passwordHasher.VerifyHashedPassword(user, passwordHistory.PasswordHash, newPassword) != PasswordVerificationResult.Failed)
            {
                return true;
            }
        }
        return false;
    }
    
    private async Task AddPasswordToHistory(ApplicationUser user, string userPasswordHash)
    {
        using var scope = _serviceProvider.CreateScope();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var passwordHistory = new PasswordHistory()
        {
            UserId = user.Id,
            PasswordHash = userPasswordHash,
            CreatedAt = DateTime.UtcNow
        };
        uow.DbContext.PasswordHistories.Add(passwordHistory);
        await uow.SaveChangesAsync();
    }
    
}

