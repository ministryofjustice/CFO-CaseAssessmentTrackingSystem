using Cfo.Cats.Domain.Common.Entities;

namespace Cfo.Cats.Domain.Entities.Documents;

public class Document : BaseAuditableSoftDeleteEntity<Guid>
{
    public string FileName { get; set; }
    public string ContentType { get; set; }
}