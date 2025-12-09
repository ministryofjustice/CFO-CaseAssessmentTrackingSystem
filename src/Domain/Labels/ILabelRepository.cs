namespace Cfo.Cats.Domain.Labels;

public interface ILabelRepository
{
    Task AddAsync(Label label);
    Task<Label?> GetByIdAsync(LabelId labelId);
    Task<int> CountParticipants(LabelId labelId);
}