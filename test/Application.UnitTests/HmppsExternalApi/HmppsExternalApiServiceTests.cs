using System.Net;
using System.Text;
using Cfo.Cats.Application.Common.Models;
using Cfo.Cats.Application.Features.HmppsExternalApi.DTOs;
using Cfo.Cats.Infrastructure.Services.HmppsExternalApi;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.HmppsExternalApi;

public class HmppsExternalApiServiceTests
{
    private const string Crn = "X123456";

    [Test]
    public async Task GetOffencesAsync_FollowsPaginationUntilLastPage()
    {
        var handler = new StubHandler(request => request.RequestUri!.Query switch
        {
            "?page=1&perPage=100" => Json("""
                { "data": [ { "systemSource": "PROBATION_SYSTEMS", "description": "Burglary", "startDate": "2020-03-01", "courtDates": [ "2020-06-01" ] } ],
                  "pagination": { "isLastPage": false } }
                """),
            "?page=2&perPage=100" => Json("""
                { "data": [ { "systemSource": "PRISON_SYSTEMS", "description": "Theft" } ],
                  "pagination": { "isLastPage": true } }
                """),
            _ => new HttpResponseMessage(HttpStatusCode.BadRequest)
        });

        var result = await CreateSut(handler).GetOffencesAsync(Crn, CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        result.Data!.Select(o => o.Description).ShouldBe(["Burglary", "Theft"]);
        result.Data![0].StartDate.ShouldBe(new DateOnly(2020, 3, 1));
        result.Data![0].CourtDates.ShouldBe([new DateOnly(2020, 6, 1)]);
        handler.Requests.Select(r => r.PathAndQuery).ShouldBe([
            $"/v1/persons/{Crn}/offences?page=1&perPage=100",
            $"/v1/persons/{Crn}/offences?page=2&perPage=100"
        ]);
    }

    [Test]
    public async Task GetSentencesAsync_ReturnsNotFound_WhenPersonIsNotFound()
    {
        var handler = new StubHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound));

        var result = await CreateSut(handler).GetSentencesAsync(Crn, CancellationToken.None);

        result.ShouldBeOfType<NotFoundResult<SentenceDto[]>>();
    }

    [Test]
    public async Task GetLicenceConditionsAsync_UnwrapsDataAndFlagsRedactedConditions()
    {
        var handler = new StubHandler(_ => Json("""
            { "data": { "hmppsId": "X123456", "licences": [ { "status": "ACTIVE", "typeCode": "AP", "conditions": [
                { "type": "Standard", "category": "Residence", "condition": "Reside at an approved address" },
                { "type": "Additional", "category": "Contact", "condition": "**REDACTED**" } ] } ] } }
            """));

        var result = await CreateSut(handler).GetLicenceConditionsAsync(Crn, CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        handler.Requests.Single().PathAndQuery.ShouldBe($"/v1/persons/{Crn}/licences/conditions");
        result.Data!.Licences!.Single().Conditions!.Select(c => c.IsRedacted).ShouldBe([false, true]);
    }

    [Test]
    public void FromSystem_KeepsOnlyItemsFromTheRequestedSystem()
    {
        var result = Result<OffenceDto[]>.Success([
            new OffenceDto { SystemSource = HmppsSystemSource.PrisonSystems, Description = "Prison" },
            new OffenceDto { SystemSource = HmppsSystemSource.ProbationSystems, Description = "Probation" }
        ]);

        result.FromSystem(HmppsSystemSource.ProbationSystems).Data!.Select(o => o.Description).ShouldBe(["Probation"]);
    }

    [Test]
    public void FromSystem_PreservesNotFound() =>
        Result<OffenceDto[]>.NotFound()
            .FromSystem(HmppsSystemSource.PrisonSystems)
            .ShouldBeOfType<NotFoundResult<OffenceDto[]>>();

    private static HmppsExternalApiService CreateSut(StubHandler handler) =>
        new(new HttpClient(handler) { BaseAddress = new Uri("https://hmpps.example/") },
            NullLogger<HmppsExternalApiService>.Instance);

    private static HttpResponseMessage Json(string body) =>
        new(HttpStatusCode.OK) { Content = new StringContent(body, Encoding.UTF8, "application/json") };

    private sealed class StubHandler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler
    {
        public List<Uri> Requests { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(request.RequestUri!);
            return Task.FromResult(respond(request));
        }
    }
}
