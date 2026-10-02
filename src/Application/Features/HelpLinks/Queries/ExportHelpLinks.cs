using System.Text;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.HelpLinks.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Dapper;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.HelpLinks.Queries;

public static class ExportHelpLinks
{
    [RequestAuthorize(Policy = SecurityPolicies.SeniorInternal)]
    public class Query : IQuery<Result<byte[]>>;

    public class Handler(ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<Query, Result<byte[]>>
    {
        public async Task<Result<byte[]>> Handle(Query request, CancellationToken cancellationToken)
        {
            using var connection = sqlConnectionFactory.CreateOpenConnection();

            const string sql = $"""
                                SELECT
                                    [Id],
                                    [Title] as [{nameof(HelpLinkExportDto.Title)}],
                                    [Description] as [{nameof(HelpLinkExportDto.Description)}],
                                    [PageKey] as [{nameof(HelpLinkExportDto.PageKey)}],
                                    [TabName] as [{nameof(HelpLinkExportDto.TabName)}]
                                FROM [Configuration].[HelpLink]
                                ORDER BY [PageKey], [TabName]
                                """;

            var rows = (await connection.QueryAsync<HelpLinkRow>(sql)).ToArray();

            var exportItems = rows.Select(r => new HelpLinkExportDto
            {
                Title = r.Title,
                Description = r.Description,
                PageKey = r.PageKey,
                TabName = r.TabName
            }).ToArray();

            if (rows.Length > 0)
            {
                const string urlsSql = $"""
                                        SELECT
                                            [HelpLinkId],
                                            [Url] as [{nameof(HelpLinkUrlDto.Url)}],
                                            [DisplayName] as [{nameof(HelpLinkUrlDto.DisplayName)}]
                                        FROM [Configuration].[HelpLinkUrl]
                                        WHERE [HelpLinkId] IN @HelpLinkIds
                                        ORDER BY [HelpLinkId], [Id]
                                        """;

                var urlRows = await connection.QueryAsync<HelpLinkUrlRow>(
                    urlsSql,
                    new { HelpLinkIds = rows.Select(r => r.Id) });

                var urlsByHelpLinkId = urlRows
                    .GroupBy(u => u.HelpLinkId)
                    .ToDictionary(g => g.Key, g => g.Select(u => new HelpLinkUrlDto { Url = u.Url, DisplayName = u.DisplayName }).ToList());

                for (var i = 0; i < rows.Length; i++)
                {
                    exportItems[i].Urls = urlsByHelpLinkId.GetValueOrDefault(rows[i].Id, new List<HelpLinkUrlDto>());
                }
            }

            var json = JsonConvert.SerializeObject(exportItems, Formatting.Indented);
            return Encoding.UTF8.GetBytes(json);
        }
    }

    private class HelpLinkRow
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PageKey { get; set; } = string.Empty;
        public string? TabName { get; set; }
    }

    private class HelpLinkUrlRow
    {
        public Guid HelpLinkId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
    }
}
