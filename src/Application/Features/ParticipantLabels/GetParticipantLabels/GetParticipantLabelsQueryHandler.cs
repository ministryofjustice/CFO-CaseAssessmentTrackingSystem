using Cfo.Cats.Domain.ParticipantLabels;
using Cfo.Cats.Domain.ValueObjects;
using Dapper;

namespace Cfo.Cats.Application.Features.ParticipantLabels.GetParticipantLabels;

public class GetParticipantLabelsQueryHandler(ISqlConnectionFactory sqlConnectionFactory) : IRequestHandler<GetParticipantLabelsQuery, Result<GetParticipantLabelsDto>>
{
    public async Task<Result<GetParticipantLabelsDto>> Handle(GetParticipantLabelsQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnectionFactory.GetOpenConnection();

        var sql = """
            select
                pl.Id as [ParticipantLabelId],
                l.Name,
                l.Description,
                l.Scope,
                l.Colour,
                l.Variant,
                l.AppIcon,
                l.Id as [LabelId],
                pl.LifetimeStart,
                pl.LifetimeEnd,
                cb.DisplayName as [AddedBy],
                COALESCE(mb.DisplayName, '') as [ClosedBy]
            from
                Participant.Label as [pl]
            inner join Configuration.Label as [l]
                on [pl].LabelId = [l].Id
            inner join [Identity].[User] as [cb]
                on pl.CreatedBy = [cb].Id
            left join [Identity].[User] as [mb]
                on pl.LastModifiedBy = [mb].Id
            where
                pl.ParticipantId = @ParticipantId
            """;

        var labels = await connection.QueryAsync<ParticipantLabelDto>(
            sql,
            new { ParticipantId = request.ParticipantId.Value });
        var dto = new GetParticipantLabelsDto(request.ParticipantId)
        {
            Labels = labels.ToArray()
        };

        return Result<GetParticipantLabelsDto>.Success(dto);
        
    }
}