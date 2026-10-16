using System.Net;
using System.Net.Http.Json;
using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Common.Models;
using Cfo.Cats.Application.Features.Candidates.Queries.Search;
using Cfo.Cats.Infrastructure.Services.Candidates;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Candidates;

public class CandidateServiceTests
{
    [TestCase("Smith&Jones", "Smith%26Jones")]
    [TestCase("Smith#Jones", "Smith%23Jones")]
    [TestCase("Smith+Jones", "Smith%2BJones")]
    [TestCase("De la Cruz", "De%20la%20Cruz")]
    [TestCase("O'Connor", "O%27Connor")]
    [SetCulture("th-TH")]
    public async Task SearchAsync_EncodesQueryValuesAndUsesInvariantDate(string lastName, string encodedLastName)
    {
        using var client = CreateClient(HttpStatusCode.OK, [new SearchResult("UPCI-1", 1)]);
        var service = CreateService(client);

        var result = await service.SearchAsync(new CandidateSearchQuery
        {
            ExternalIdentifier = "CRN/123",
            FirstName = string.Empty,
            LastName = lastName,
            DateOfBirth = new DateTime(1990, 1, 1),
            CurrentUser = null!
        });

        result.Succeeded.ShouldBeTrue();
        result.Data.ShouldBe([new SearchResult("UPCI-1", 1)]);
        client.LastRequest.RequestUri!.AbsoluteUri.ShouldBe(
            $"https://dms.test/search?identifier=CRN%2F123&lastName={encodedLastName}&dateOfBirth=1990-01-01");
        client.LastRequest.RequestUri.Fragment.ShouldBeEmpty();
    }

    [Test]
    public async Task SearchAsync_WhenDmsReturnsNotFound_ReturnsNotFound()
    {
        using var client = CreateClient(HttpStatusCode.NotFound);
        var service = CreateService(client);

        var result = await service.SearchAsync(new CandidateSearchQuery
        {
            ExternalIdentifier = "CRN123",
            FirstName = string.Empty,
            LastName = "Smith",
            DateOfBirth = new DateTime(1990, 1, 1),
            CurrentUser = null!
        });

        result.Succeeded.ShouldBeFalse();
        result.ShouldBeOfType<NotFoundResult<SearchResult[]>>();
    }

    private static CandidateService CreateService(HttpClient client)
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        return new CandidateService(client, unitOfWork.Object, NullLogger<CandidateService>.Instance);
    }

    private static RecordingHttpClient CreateClient(HttpStatusCode statusCode, SearchResult[] content = null)
    {
        var handler = new RecordingHandler(statusCode, content);
        return new RecordingHttpClient(handler)
        {
            BaseAddress = new Uri("https://dms.test")
        };
    }

    private sealed class RecordingHttpClient(HttpMessageHandler handler) : HttpClient(handler)
    {
        public HttpRequestMessage LastRequest => ((RecordingHandler)_handler).LastRequest!;

        private readonly HttpMessageHandler _handler = handler;
    }

    private sealed class RecordingHandler(HttpStatusCode statusCode, SearchResult[] content) : HttpMessageHandler
    {
        public HttpRequestMessage LastRequest { get; private set; } = null!;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            var response = new HttpResponseMessage(statusCode);
            if (content is not null)
            {
                response.Content = JsonContent.Create(content);
            }

            return Task.FromResult(response);
        }
    }
}
