using System.Threading;
using System.Threading.Tasks;
using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Features.Candidates.DTOs;
using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Application.Pipeline.PreProcessors;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Cfo.Cats.Application.UnitTests.Common.Behaviours;

[TestFixture]
public class RequestLoggerTests
{
    private readonly Mock<ICurrentUserService> currentUserService = new();
    private readonly Mock<IIdentityService> identityService = new();
    private readonly Mock<ILogger<CreateParticipant.Command>> logger = new();
    
    [Test]
    public async Task ShouldCallGetUserNameAsyncOnceIfAuthenticated()
    {
        currentUserService.Setup(x => x.UserId).Returns("Administrator");
        var requestLogger = new LoggingPreProcessor<CreateParticipant.Command>(logger.Object, currentUserService.Object);
        await requestLogger.Process(
            new CreateParticipant.Command { Candidate = DummyCandidate()  },
            new CancellationToken());
        currentUserService.Verify(i => i.UserName, Times.Once);
    }

    [Test]
    public async Task ShouldNotCallGetUserNameAsyncOnceIfUnauthenticated()
    {
        var requestLogger = new LoggingPreProcessor<CreateParticipant.Command>(logger.Object, currentUserService.Object);
        await requestLogger.Process(
            new CreateParticipant.Command { Candidate = DummyCandidate() },
            new CancellationToken());
        identityService.Verify(i => i.GetUserNameAsync(It.IsAny<string>(), CancellationToken.None), Times.Never);
    }

    private CandidateDto DummyCandidate()
    {
        return new CandidateDto
        {
            Identifier = "aABBB",
            FirstName = "",
            LastName = "",
            Gender = "",
            Nationality = "",
            Ethnicity = "",
            Primary = "",
            LocationDescription = "",

        };
    }
}