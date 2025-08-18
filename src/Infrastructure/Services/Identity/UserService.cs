using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cfo.Cats.Application.Features.Identity.DTOs;

namespace Cfo.Cats.Infrastructure.Services.Identity;

public class UserService(IMapper mapper, IServiceScopeFactory scopeFactory, ILogger<UserService> logger) 
    : IUserService
{
    public event Action? OnChange;

    public IReadOnlyList<ApplicationUserDto> DataSource 
    {
        get 
        {
            logger.LogDebug("DataSource called, getting from the database");

            using var scope = scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var query = from a in unitOfWork.DbContext.Users
                            .AsNoTracking()
                            .Include(x => x.UserRoles)
                            .ThenInclude(e => e.Role)
                        orderby a.UserName ascending
                        select a;

            var list = query
                        .ProjectTo<ApplicationUserDto>(mapper.ConfigurationProvider)
                        .ToList();
            return list.AsReadOnly();
        }
    }

    public string? GetDisplayName(string userId)
    {
        logger.LogDebug("GetDisplayName called, getting from the database");

        using var scope = scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var query = from a in unitOfWork.DbContext.Users
                        .Where(x => x.Id == userId )
                    select a.DisplayName;

        return query.FirstOrDefault();
    }

   
    public void Refresh()
    {
        logger.LogInformation("Refresh of UserService called, ignored as this is the none caching service");
        OnChange?.Invoke();
    }

}
