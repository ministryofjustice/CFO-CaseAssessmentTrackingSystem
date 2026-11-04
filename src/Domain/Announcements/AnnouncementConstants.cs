namespace Cfo.Cats.Domain.Announcements;

public enum AnnouncementAudience
{
    AllStaff = 0,
    InternalStaff = 1
}

public static class AnnouncementConstants
{
    public const int TypeNameMaximumLength = 100;
    public const int TitleMaximumLength = 150;
    public const int PostedByMaximumLength = 150;
    public const int DescriptionMaximumLength = 10000;
    public const int UrlMaximumLength = 2000;
    public const int DisplayNameMaximumLength = 150;
}
