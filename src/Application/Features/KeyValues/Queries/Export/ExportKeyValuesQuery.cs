using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.KeyValues.Queries.Export;

[RequestAuthorize(Roles = RoleNames.SystemSupport)]
public class ExportKeyValuesQuery : IRequest<byte[]>
{
    public string? Keyword { get; set; }
    public string OrderBy { get; set; } = "Id";
    public string SortDirection { get; set; } = "desc";
}