using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Announcements;

namespace Cfo.Cats.Application.Features.Announcements.Commands;

public static class RenameAnnouncementType
{
    [RequestAuthorize(Policy = SecurityPolicies.SeniorInternal)]
    public class Command : ICommand<Result>
    {
        public Guid Id { get; set; }
        public string OriginalName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.OriginalName).NotEmpty().MaximumLength(AnnouncementConstants.TypeNameMaximumLength);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(AnnouncementConstants.TypeNameMaximumLength);
        }
    }

    public class Handler(IUnitOfWork unitOfWork) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var type = await unitOfWork.DbContext.AnnouncementTypes
                .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                ?? throw new NotFoundException("This announcement type is no longer available.");
            if (type.Name != request.OriginalName)
            {
                throw new ConflictException("This announcement type has changed. Refresh the list and try again.");
            }
            var name = request.Name.Trim();
            if (await unitOfWork.DbContext.AnnouncementTypes
                    .AnyAsync(x => x.Id != request.Id && x.Name == name, cancellationToken))
            {
                throw new ConflictException("An announcement type with this name already exists.");
            }
            type.Rename(name);
            return Result.Success();
        }
    }
}
