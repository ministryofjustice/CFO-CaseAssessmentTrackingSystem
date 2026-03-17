using Cfo.Cats.Domain.Participants;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cfo.Cats.Infrastructure.Persistence.Converters;

public class ParticipantIdValueConverter : ValueConverter<ParticipantId, string>
{
    public ParticipantIdValueConverter()
        : base (id => id.Value, value => new ParticipantId(value))
    {
        
    }
}