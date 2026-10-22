using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Announcements;

namespace Cfo.Cats.Application.Features.Announcements.Commands;

public static class DeleteAnnouncementType
{
    [RequestAuthorize(Policy = SecurityPolicies.SeniorInternal)]
    public class Command : ICommand<Result>
    {
        public Guid Id { get; set; }
        public string OriginalName { get; set; } = string.Empty;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.OriginalName).NotEmpty().MaximumLength(AnnouncementConstants.TypeNameMaximumLength);
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
            unitOfWork.DbContext.AnnouncementTypes.Remove(type);
            return Result.Success();
        }
    }
}
