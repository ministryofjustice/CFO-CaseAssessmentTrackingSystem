namespace Cfo.Cats.Domain.Participants;

public readonly record struct ParticipantId
{
    private readonly string? _value;

    public ParticipantId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException("Id value cannot be null, empty or just whitespace");
        }

        if(value.Length != 9)
        {
            throw new InvalidOperationException("Id must be 9 characters long.");
        }

        _value = value;
    }

    public string Value => _value ?? throw new InvalidOperationException("ParticipantId has not been initialized.");
}