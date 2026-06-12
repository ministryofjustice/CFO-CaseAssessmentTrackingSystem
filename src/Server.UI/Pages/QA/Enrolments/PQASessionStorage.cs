using Cfo.Cats.Application.Features.QualityAssurance.Specifications;
using Cfo.Cats.Server.UI.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Cfo.Cats.Server.UI.Pages.QA.Enrolments;

public class PQASessionStorage(ProtectedSessionStorage protectedSessionStorage, ICurrentUserService currentUserService)
    : CatsSessionStorage<PQASessionData>(protectedSessionStorage, currentUserService)
{
    
}

public record PQASessionData
{
    public PQASessionData()
    {
    }

    public required string? SupportWorkerId { get; init; }

    public required string? TenantId { get; init; }
    public required string? Keyword { get; init; }
    public required string OrderBy { get; init; }
    public required string SortDirection { get; init; }
    public required int PageNumber { get; init; }

    internal static PQASessionData FromQuery(QueueEntryFilter query)
        => new()
        {
            SupportWorkerId = query.SupportWorkerId,
            TenantId = query.TenantId,
            Keyword = query.Keyword,
            OrderBy = query.OrderBy,
            SortDirection = query.SortDirection,
            PageNumber = query.PageNumber
        };

}