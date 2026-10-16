using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Announcements.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Announcements;

namespace Cfo.Cats.Application.Features.Announcements.Commands;

public static class SaveAnnouncement
{
    [RequestAuthorize(Policy = SecurityPolicies.SeniorInternal)]
    public class Command : ICommand<Result<Guid>>
    {
        public Guid? Id { get; set; }
        public Guid Version { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public AnnouncementAudience Audience { get; set; }
        public string Title { get; set; } = string.Empty;
        public string PostedBy { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<AnnouncementLinkDto> Links { get; set; } = [];
        public bool PublishImmediately { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEqual(Guid.Empty).When(x => x.Id.HasValue);
            RuleFor(x => x.Version).NotEmpty().When(x => x.Id.HasValue);
            RuleFor(x => x.TypeName).NotEmpty().MaximumLength(AnnouncementConstants.TypeNameMaximumLength);
            RuleFor(x => x.Audience).IsInEnum();
            RuleFor(x => x.Title).NotEmpty().MaximumLength(AnnouncementConstants.TitleMaximumLength);
            RuleFor(x => x.PostedBy).NotEmpty().MaximumLength(AnnouncementConstants.PostedByMaximumLength);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(AnnouncementConstants.DescriptionMaximumLength);
            RuleFor(x => x.Links).NotNull();
            RuleForEach(x => x.Links).ChildRules(link =>
            {
                link.RuleFor(x => x.Url).NotEmpty()
                    .MaximumLength(AnnouncementConstants.UrlMaximumLength)
                    .Must(UrlValidator.IsHttpOrHttpsUrl)
                    .WithMessage("Links must use a valid, absolute HTTP or HTTPS URL.");
                link.RuleFor(x => x.DisplayName).MaximumLength(AnnouncementConstants.DisplayNameMaximumLength);
            });
        }
    }

    public class Handler(
        IAnnouncementRepository repository,
        ICurrentUserService currentUserService,
        IUserService userService) : ICommandHandler<Command, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = currentUserService.UserId ?? throw new UnauthorizedException("Sign in to manage announcements.");
            var user = AnnouncementAccess.GetUser(userService, userId);
            var isInternalUser = currentUserService.IsInternalUser.GetValueOrDefault();
            AnnouncementAccess.EnsureAudience(isInternalUser, request.Audience);
            var typeName = await repository.GetTypeNameAsync(request.TypeName, cancellationToken);
            var canAutoApprove = request.PublishImmediately
                && user.IsInRole(RoleNames.SystemSupport);
            if (request.PublishImmediately && !canAutoApprove)
            {
                throw new ForbiddenException("Only System Support can approve and publish immediately.");
            }

            var links = request.Links.Select(x => new AnnouncementLink(x.Url.Trim(), x.DisplayName?.Trim())).ToArray();
            var submittedByName = string.IsNullOrWhiteSpace(user.DisplayName) ? user.UserName : user.DisplayName;
            Announcement announcement;
            if (request.Id is { } id)
            {
                announcement = await repository.GetByIdAsync(id, cancellationToken);
                AnnouncementAccess.EnsureManagementAudience(isInternalUser, announcement);
                announcement.Edit(request.Version, typeName, request.Audience, request.Description, links, userId,
                    request.Title, request.PostedBy, submittedByName);
            }
            else
            {
                announcement = Announcement.Create(typeName, request.Audience, request.Description, links, userId,
                    request.Title, request.PostedBy, submittedByName);
                await repository.AddAsync(announcement, cancellationToken);
            }

            if (request.PublishImmediately)
            {
                announcement.AutoApprove(announcement.Version, userId, canAutoApprove);
            }
            return Result<Guid>.Success(announcement.Id);
        }
    }
}
