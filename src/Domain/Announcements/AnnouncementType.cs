using Cfo.Cats.Domain.Announcements.Rules;
using Cfo.Cats.Domain.Common.Entities;

namespace Cfo.Cats.Domain.Announcements;

public class AnnouncementType : BaseAuditableEntity<Guid>
{
    private AnnouncementType() { }

    public string Name { get; private set; } = string.Empty;

    public static AnnouncementType Create(string name)
    {
        var type = new AnnouncementType { Id = Guid.CreateVersion7() };
        type.Rename(name);
        return type;
    }

    public void Rename(string name)
    {
        CheckRule(new AnnouncementWorkflowRule(
            string.IsNullOrWhiteSpace(name) || name.Trim().Length > AnnouncementConstants.TypeNameMaximumLength,
            $"Type name is required and must not exceed {AnnouncementConstants.TypeNameMaximumLength} characters."));
        Name = name.Trim();
    }
}
