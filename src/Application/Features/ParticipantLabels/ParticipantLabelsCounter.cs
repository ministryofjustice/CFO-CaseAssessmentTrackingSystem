using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Domain.ParticipantLabels;
using Cfo.Cats.Domain.Participants;
using Dapper;

namespace Cfo.Cats.Application.Features.ParticipantLabels;

public class ParticipantLabelsCounter(ISqlConnectionFactory sqlConnectionFactory) : IParticipantLabelsCounter
{
    public int CountOpenLabels(ParticipantId participantId, LabelId labelId)
    {
        using var connection = sqlConnectionFactory.CreateOpenConnection();

        const string sql = """
                            SELECT COUNT(*)
                            FROM [Participant].[Label] as [ParticipantLabel]
                            WHERE 
                                 [ParticipantId] = @ParticipantId 
                                 AND [LabelId] = @LabelId
                                 AND [LifetimeEnd] >= GETUTCDATE()
                            """;

        return connection.QuerySingle<int>(
            sql,
            new
            {
                ParticipantId = participantId.Value,
                LabelId = labelId.Value
            });
    }
}