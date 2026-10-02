using Cfo.Cats.Domain.HelpLinks;
using Dapper;

namespace Cfo.Cats.Application.Features.HelpLinks;

public class HelpLinkCounter(ISqlConnectionFactory sqlConnectionFactory) : IHelpLinkCounter
{
    public int CountForPageAndTab(string pageKey, string? tabName, Guid? excludeId = null)
    {
        using var connection = sqlConnectionFactory.CreateOpenConnection();

        const string sql = """
                           SELECT Count(*)
                           FROM [Configuration].[HelpLink] as [HelpLink]
                           WHERE
                              [HelpLink].[PageKey] = @PageKey
                              AND (([HelpLink].[TabName] IS NULL AND @TabName IS NULL) OR [HelpLink].[TabName] = @TabName)
                              AND (@ExcludeId IS NULL OR [HelpLink].[Id] <> @ExcludeId)
                           """;

        return connection.QuerySingle<int>(sql, new
        {
            PageKey = pageKey,
            TabName = tabName,
            ExcludeId = excludeId
        });
    }
}
