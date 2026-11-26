#nullable enable
using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Common.Models;
using Cfo.Cats.Application.Features.HmppsExternalApi.DTOs;
using Cfo.Cats.Application.Features.HmppsExternalApi.Queries;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.ValueObjects;
using Cfo.Cats.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.HmppsExternalApi;

public class HmppsRiskQueryTests
{
    private const string TenantId = "1.1.";

    private ApplicationDbContext _context = null!;
    private Mock<IHmppsExternalApiService> _service = null!;
    private Mock<IUnitOfWork> _unitOfWork = null!;
    private Mock<ICurrentUserService> _currentUser = null!;

    [SetUp]
    public async Task SetUp()
    {
        _context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

        _context.Participants.AddRange(
            CreateParticipant("AAAAAAAA1", "X111111"),
            CreateParticipant("BBBBBBBB2", "X222222"));
        await _context.SaveChangesAsync();

        _unitOfWork = new Mock<IUnitOfWork>();
        _unitOfWork.SetupGet(x => x.DbContext).Returns(_context);

        _currentUser = new Mock<ICurrentUserService>();
        _currentUser.SetupGet(x => x.TenantId).Returns(TenantId);

        _service = new Mock<IHmppsExternalApiService>();
        _service.Setup(x => x.GetSeriousHarmRiskAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<RisksDto>.Success(new RisksDto()));
        _service.Setup(x => x.GetTierDetailsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<TierDetailsDto>.Success(new TierDetailsDto()));
    }

    [TearDown]
    public void TearDown() => _context.Dispose();

    [Test]
    public async Task Arns_WhenCrnBelongsToParticipant_FetchesThatCrn()
    {
        var result = await ArnsHandler().Handle(new GetArnsRisk.Query { ParticipantId = "AAAAAAAA1", Crn = "X111111" }, CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        _service.Verify(x => x.GetSeriousHarmRiskAsync("X111111", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Tier_WhenCrnBelongsToParticipant_FetchesThatCrn()
    {
        var result = await TierHandler().Handle(new GetTierDetails.Query { ParticipantId = "AAAAAAAA1", Crn = "X111111" }, CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        _service.Verify(x => x.GetTierDetailsAsync("X111111", It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestCase("X222222")]
    [TestCase("X999999")]
    public async Task Arns_WhenCrnDoesNotBelongToParticipant_NeverCallsApi(string crn)
    {
        var result = await ArnsHandler().Handle(new GetArnsRisk.Query { ParticipantId = "AAAAAAAA1", Crn = crn }, CancellationToken.None);

        result.Succeeded.ShouldBeFalse();
        _service.VerifyNoOtherCalls();
    }

    [TestCase("X222222")]
    [TestCase("X999999")]
    public async Task Tier_WhenCrnDoesNotBelongToParticipant_NeverCallsApi(string crn)
    {
        var result = await TierHandler().Handle(new GetTierDetails.Query { ParticipantId = "AAAAAAAA1", Crn = crn }, CancellationToken.None);

        result.Succeeded.ShouldBeFalse();
        _service.VerifyNoOtherCalls();
    }

    [Test]
    public async Task WhenFeatureNotEnabledForTenant_NeverCallsApi()
    {
        _currentUser.SetupGet(x => x.TenantId).Returns("2.");

        var result = await ArnsHandler().Handle(new GetArnsRisk.Query { ParticipantId = "AAAAAAAA1", Crn = "X111111" }, CancellationToken.None);

        result.Succeeded.ShouldBeFalse();
        _service.VerifyNoOtherCalls();
    }

    [Test]
    public async Task WhenIntegrationNotRegistered_ReturnsFailure()
    {
        var handler = new GetArnsRisk.Handler(_unitOfWork.Object, _currentUser.Object, Configuration(), new ServiceCollection().BuildServiceProvider());

        var result = await handler.Handle(new GetArnsRisk.Query { ParticipantId = "AAAAAAAA1", Crn = "X111111" }, CancellationToken.None);

        result.Succeeded.ShouldBeFalse();
    }

    private GetArnsRisk.Handler ArnsHandler() => new(_unitOfWork.Object, _currentUser.Object, Configuration(), ServiceProvider());

    private GetTierDetails.Handler TierHandler() => new(_unitOfWork.Object, _currentUser.Object, Configuration(), ServiceProvider());

    private IServiceProvider ServiceProvider() => new ServiceCollection()
        .AddSingleton(_service.Object)
        .BuildServiceProvider();

    private static IConfiguration Configuration() => new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Features:HmppsRisk:Enabled"] = "true",
            ["Features:HmppsRisk:Tenants:0"] = TenantId,
        })
        .Build();

    private static Participant CreateParticipant(string id, string crn) =>
        Participant.CreateFrom(
                id: id,
                firstName: "Test",
                middleName: null,
                lastName: "Participant",
                gender: "Male",
                dateOfBirth: new DateTime(1990, 1, 1),
                registrationDetailsJson: null,
                referralSource: "Test",
                referralComments: null,
                locationId: 1,
                nationality: null,
                primaryRecordKeyAtCreation: null,
                ethnicity: null)
            .AddOrUpdateExternalIdentifier(ExternalIdentifier.Create(crn, ExternalIdentifierType.Crn));
}
