using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Announcements;

public class AnnouncementHostRegistrationTests
{
    [TestCase(true)]
    [TestCase(false)]
    public void BackgroundHostsKeepUserDataServicesWithoutIdentityOrSignIn(bool worker)
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string> { ["Quartz:SyncParticipantsJob:Enabled"] = "false" })
            .Build();
        var environment = new Mock<IHostEnvironment>().Object;
        if (worker)
        {
            services.AddWorkerInfrastructure(configuration, environment);
        }
        else
        {
            services.AddConsumerInfrastructure(configuration, environment);
        }

        services.ShouldContain(x => x.ServiceType == typeof(IUserService));
        services.ShouldNotContain(x => x.ServiceType == typeof(IIdentityService));
        services.ShouldNotContain(x => x.ServiceType == typeof(UserManager<ApplicationUser>));
        services.ShouldNotContain(x => x.ServiceType == typeof(IUserClaimsPrincipalFactory<ApplicationUser>));
        services.ShouldNotContain(x => x.ServiceType == typeof(IAuthorizationService));
        services.ShouldNotContain(x => x.ServiceType == typeof(IPolicyEvaluator));
        services.ShouldNotContain(x => x.ServiceType == typeof(SignInManager<ApplicationUser>));
    }

    [Test]
    public void AnnouncementHandlersDoNotRequireIdentityServices()
    {
        var handlers = typeof(Cfo.Cats.Application.Features.Announcements.Commands.SaveAnnouncement.Handler).Assembly
            .GetTypes().Where(type => type.Namespace?.StartsWith(
                "Cfo.Cats.Application.Features.Announcements", StringComparison.Ordinal) == true);
        foreach (var type in handlers)
        {
            type.GetConstructors().SelectMany(constructor => constructor.GetParameters())
                .ShouldNotContain(parameter => parameter.ParameterType == typeof(IIdentityService));
        }
    }
}
