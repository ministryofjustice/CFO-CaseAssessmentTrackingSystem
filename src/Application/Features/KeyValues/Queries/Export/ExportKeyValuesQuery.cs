namespace Cfo.Cats.Application.Features.KeyValues.Queries.Export;

public class ExportKeyValuesQuery : IRequest<byte[]>
{
    public string? Keyword { get; set; }
    public string OrderBy { get; set; } = "Id";
    public string SortDirection { get; set; } = "desc";
}