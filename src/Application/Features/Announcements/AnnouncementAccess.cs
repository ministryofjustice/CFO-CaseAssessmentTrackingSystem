using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Domain.Announcements;

namespace Cfo.Cats.Application.Features.Announcements;

public static class AnnouncementAccess
{
    public static ApplicationUserDto GetUser(IUserService userService, string userId)
        => userService.DataSource.SingleOrDefault(user => user.Id == userId)
            ?? throw new NotFoundException("User not found.");

    public static void EnsureAudience(
        bool isInternalUser,
        AnnouncementAudience audience)
    {
        if (audience == AnnouncementAudience.InternalStaff
            && !isInternalUser)
        {
            throw new ForbiddenException("Only internal staff can access internal announcements.");
        }
    }

    public static void EnsureManagementAudience(
        bool isInternalUser,
        Announcement announcement)
    {
        if (announcement.PendingRevision?.Audience == AnnouncementAudience.InternalStaff
            || announcement.PublishedRevision?.Audience == AnnouncementAudience.InternalStaff)
        {
            EnsureAudience(isInternalUser, AnnouncementAudience.InternalStaff);
        }
    }
}
