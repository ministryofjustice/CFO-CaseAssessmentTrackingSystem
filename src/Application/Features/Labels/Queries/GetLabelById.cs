using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Labels.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Labels;
using Dapper;

namespace Cfo.Cats.Application.Features.Labels.Queries;

public static class GetLabelById
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query(Guid id) : IRequest<Result<LabelDto>>
    {
        public Guid LabelId { get; } = id;
    }

    public class Handler(ISqlConnectionFactory sqlConnectionFactory) : IRequestHandler<Query, Result<LabelDto>>
    {
        public async Task<Result<LabelDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var connection = sqlConnectionFactory.GetOpenConnection();

            const string sql = $"""
                                SELECT 
                                    [Label].[Id] as [{nameof(LabelDto.Id)}],
                                    [Label].[Name] as [{nameof(LabelDto.Name)}],
                                    [Label].[Description] as [{nameof(LabelDto.Description)}],
                                    [Label].[Colour] as [{nameof(LabelDto.Colour)}],
                                    [Contract].[Description] as [{nameof(LabelDto.Contract)}],
                                    [Label].[Variant] as [{nameof(LabelDto.Variant)}],
                                    [Label].[AppIcon] as [{nameof(LabelDto.AppIcon)}]
                                FROM [Configuration].[Label] as [Label]
                                LEFT JOIN [Configuration].[Contract] as [Contract]
                                    on [Label].[ContractId] = [Contract].[Id]
                                WHERE [Label].[Id] = @LabelId
                                """;
            
            var label = await connection.QuerySingleAsync<LabelDto>(sql, new {request.LabelId});
            return label;
        }
    }

}