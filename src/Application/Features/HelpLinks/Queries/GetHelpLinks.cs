using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.HelpLinks.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Dapper;

namespace Cfo.Cats.Application.Features.HelpLinks.Queries;

public static class GetHelpLinks
{
    [RequestAuthorize(Policy = SecurityPolicies.SeniorInternal)]
    public class Query : IQuery<Result<HelpLinkDto[]>>;

    public class Handler(ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<Query, Result<HelpLinkDto[]>>
    {
        public async Task<Result<HelpLinkDto[]>> Handle(Query request, CancellationToken cancellationToken)
        {
            using var connection = sqlConnectionFactory.CreateOpenConnection();

            const string sql = $"""
                                SELECT
                                    [Id] as [{nameof(HelpLinkDto.Id)}],
                                    [Title] as [{nameof(HelpLinkDto.Title)}],
                                    [Description] as [{nameof(HelpLinkDto.Description)}],
                                    [PageKey] as [{nameof(HelpLinkDto.PageKey)}],
                                    [TabName] as [{nameof(HelpLinkDto.TabName)}]
                                FROM [Configuration].[HelpLink]
                                ORDER BY [PageKey], [TabName]
                                """;

            var helpLinks = (await connection.QueryAsync<HelpLinkDto>(sql)).ToArray();

            if (helpLinks.Length == 0)
            {
                return helpLinks;
            }

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
                new { HelpLinkIds = helpLinks.Select(h => h.Id) });

            var urlsByHelpLinkId = urlRows
                .GroupBy(u => u.HelpLinkId)
                .ToDictionary(g => g.Key, g => g.Select(u => new HelpLinkUrlDto { Url = u.Url, DisplayName = u.DisplayName }).ToList());

            foreach (var helpLink in helpLinks)
            {
                helpLink.Urls = urlsByHelpLinkId.GetValueOrDefault(helpLink.Id, new List<HelpLinkUrlDto>());
            }

            return helpLinks;
        }
    }

    private class HelpLinkUrlRow
    {
        public Guid HelpLinkId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
    }
}
