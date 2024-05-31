using System.Text.Json;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Constants.ClaimTypes;
using Cfo.Cats.Infrastructure.Constants.Role;
using Cfo.Cats.Infrastructure.PermissionSet;
using Cfo.Cats.Infrastructure.Persistence.Initializers;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace Cfo.Cats.Infrastructure.Persistence;

public class ApplicationDbContextInitializer
{
    private readonly ApplicationDbContext context;
    private readonly ILogger<ApplicationDbContextInitializer> logger;
    private readonly RoleManager<ApplicationRole> roleManager;
    private readonly UserManager<ApplicationUser> userManager;

    public ApplicationDbContextInitializer(
        ILogger<ApplicationDbContextInitializer> logger,
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager
    )
    {
        this.logger = logger;
        this.context = context;
        this.userManager = userManager;
        this.roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (
                context.Database.IsSqlServer()
                || context.Database.IsSqlite()
            )
            {
                await context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initialising the database");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
            context.ChangeTracker.Clear();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        await SeedTenants();
        await SeedContracts();
        await SeedLocations();
        await SeedDictionaries();

        // Default roles
        var administratorRole = new ApplicationRole(RoleName.Admin) { Description = "Admin Group" };
        var basicRole = new ApplicationRole(RoleName.Basic) { Description = "Basic User Group" };
        
        var permissions = Permissions.GetRegisteredPermissions();

        if (roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await roleManager.CreateAsync(administratorRole);

            foreach (var permission in permissions)
            {
                await roleManager.AddClaimAsync(
                    administratorRole,
                    new Claim(ApplicationClaimTypes.Permission, permission)
                );
            }
        }

        if (roleManager.Roles.All(r => r.Name != basicRole.Name))
        {
            await roleManager.CreateAsync(basicRole);
            foreach (var permission in permissions.Where(p => p.EndsWith(".View")))
            {
                await roleManager.AddClaimAsync(basicRole, new Claim(ApplicationClaimTypes.Permission, permission));
            }
        }

        var defaultUser = new ApplicationUser()
        {
            UserName = "support.worker@justice.gov.uk",
            Provider = "Local",
            IsActive = true,
            TenantId = context.Tenants.First().Id,
            TenantName = context.Tenants.First().Name,
            DisplayName = "Support Worker",
            Email = "support.worker@justice.gov.uk",
            EmailConfirmed = true,
            ProfilePictureDataUrl =
                "https://avatars.githubusercontent.com/u/9332472?s=400&u=73c208bf07ba967d5407aae9068580539cfc80a2&v=4",
            TwoFactorEnabled = false
        };

        if (userManager.Users.All(u => u.UserName != "support.worker@justice.gov.uk"))
        {
            await userManager.CreateAsync(defaultUser, "Password123!");
            await userManager.AddToRolesAsync(defaultUser, new[] { administratorRole.Name! });
        }
    }
    private async Task SeedDictionaries()
    {
        if (await context.KeyValues.AnyAsync() == false)
        {
            var kvps = SeedingData.GetDictionaries();
            context.KeyValues.AddRange(kvps);
            await context.SaveChangesAsync();
        }
    }

    private async Task SeedLocations()
    {
        if (await context.Locations.AnyAsync() == false)
        {
            var locations = SeedingData.GetLocations();
            context.Locations.AddRange(locations);
            await context.SaveChangesAsync();

        }
    }

    private async Task SeedTenants()
    {
        if (await context.Tenants.OrderBy(r => r.Id).Select(e => e.Id).FirstOrDefaultAsync() == null)
        {
            var tenants = SeedingData.GetTenants();
            context.Tenants.AddRange(tenants);
            await context.SaveChangesAsync();
        }
    }

    private async Task SeedContracts()
    {
        if (await context.Contracts.OrderBy(r => r.Id).Select(e => e.Id).FirstOrDefaultAsync() is null)
        {
            var contracts = SeedingData.GetContracts();
            context.Contracts.AddRange(contracts);
            await context.SaveChangesAsync();
        }
    }


}
