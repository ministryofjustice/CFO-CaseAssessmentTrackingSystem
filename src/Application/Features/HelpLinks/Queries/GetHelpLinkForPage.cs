using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.HelpLinks.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Dapper;

namespace Cfo.Cats.Application.Features.HelpLinks.Queries;

public static class GetHelpLinkForPage
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query(string pageKey, string? tabName) : IQuery<Result<HelpLinkDto?>>
    {
        public string PageKey { get; } = pageKey;
        public string? TabName { get; } = tabName;
    }

    public class Handler(ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<Query, Result<HelpLinkDto?>>
    {
        public async Task<Result<HelpLinkDto?>> Handle(Query request, CancellationToken cancellationToken)
        {
            using var connection = sqlConnectionFactory.CreateOpenConnection();

            const string sql = $"""
                                SELECT TOP (1)
                                    [Id] as [{nameof(HelpLinkDto.Id)}],
                                    [Title] as [{nameof(HelpLinkDto.Title)}],
                                    [Description] as [{nameof(HelpLinkDto.Description)}],
                                    [PageKey] as [{nameof(HelpLinkDto.PageKey)}],
                                    [TabName] as [{nameof(HelpLinkDto.TabName)}]
                                FROM [Configuration].[HelpLink]
                                WHERE [PageKey] = @PageKey
                                    AND ([TabName] = @TabName OR [TabName] IS NULL)
                                ORDER BY
                                    CASE WHEN [TabName] = @TabName THEN 0 ELSE 1 END,
                                    [Created] DESC
                                """;

            var helpLink = await connection.QueryFirstOrDefaultAsync<HelpLinkDto>(
                sql,
                new { request.PageKey, request.TabName });

            if (helpLink is not null)
            {
                const string urlsSql = $"""
                                        SELECT
                                            [Url] as [{nameof(HelpLinkUrlDto.Url)}],
                                            [DisplayName] as [{nameof(HelpLinkUrlDto.DisplayName)}]
                                        FROM [Configuration].[HelpLinkUrl]
                                        WHERE [HelpLinkId] = @Id
                                        ORDER BY [Id]
                                        """;

                var urls = await connection.QueryAsync<HelpLinkUrlDto>(urlsSql, new { helpLink.Id });
                helpLink.Urls = urls.ToList();
            }

            return helpLink;
        }
    }
}
