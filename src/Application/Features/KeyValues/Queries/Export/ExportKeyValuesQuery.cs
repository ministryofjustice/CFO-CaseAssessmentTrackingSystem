using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Application.Features.KeyValues.Queries.Export;

[RequestAuthorize(Roles = "Admin, Basic")]
public class ExportKeyValuesQuery : IRequest<byte[]>
{
    public string? Keyword { get; set; }
    public string OrderBy { get; set; } = "Id";
    public string SortDirection { get; set; } = "desc";
}