using System.Net;
using System.Net.Http.Json;
using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Common.Models;
using Cfo.Cats.Infrastructure.Services.Candidates;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Candidates;

public class CandidateServiceTests
{
    [Test]
    public async Task SearchByIdentifierAsync_WhenDmsReturnsResults_ReturnsResults()
    {
        using var client = CreateClient(HttpStatusCode.OK, new[] { new SearchResult("UPCI-1", 1) });
        var service = CreateService(client);

        var result = await service.SearchByIdentifierAsync("CRN/123");

        result.Succeeded.ShouldBeTrue();
        result.Data.ShouldBe([new SearchResult("UPCI-1", 1)]);
        client.LastRequest!.RequestUri!.PathAndQuery.ShouldBe("/search?identifier=CRN%2F123");
    }

    [Test]
    public async Task SearchByIdentifierAsync_WhenDmsReturnsNotFound_ReturnsNotFound()
    {
        using var client = CreateClient(HttpStatusCode.NotFound);
        var service = CreateService(client);

        var result = await service.SearchByIdentifierAsync("CRN123");

        result.Succeeded.ShouldBeFalse();
        result.ShouldBeOfType<NotFoundResult<SearchResult[]>>();
    }

    [Test]
    public async Task SearchByIdentifierAsync_WhenDmsReturnsNull_ReturnsFailure()
    {
        using var client = CreateClient(HttpStatusCode.OK);
        var service = CreateService(client);

        var result = await service.SearchByIdentifierAsync("CRN123");

        result.Succeeded.ShouldBeFalse();
        result.ErrorMessage.ShouldBe("Error getting search results");
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
