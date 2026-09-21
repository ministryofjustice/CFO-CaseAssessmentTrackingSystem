namespace Cfo.Cats.Domain.Labels;

/// <summary>
/// Represents the association between a <see cref="Label"/> and a contract.
/// A label is only visible for the contracts it is associated with.
/// </summary>
public class LabelContract
{
    // ef core only
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private LabelContract()
    {
    }
#pragma warning restore CS8618

    internal LabelContract(string contractId) => ContractId = contractId;

    /// <summary>
    /// The id of the contract this label is associated with.
    /// </summary>
    public string ContractId { get; private set; }
}
