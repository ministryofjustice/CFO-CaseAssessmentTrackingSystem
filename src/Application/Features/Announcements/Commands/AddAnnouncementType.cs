using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Announcements;

namespace Cfo.Cats.Application.Features.Announcements.Commands;

public static class AddAnnouncementType
{
    [RequestAuthorize(Policy = SecurityPolicies.SeniorInternal)]
    public class Command : ICommand<Result>
    {
        public string Name { get; set; } = string.Empty;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator() => RuleFor(x => x.Name).NotEmpty()
            .MaximumLength(AnnouncementConstants.TypeNameMaximumLength);
    }

    public class Handler(IUnitOfWork unitOfWork) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var type = AnnouncementType.Create(request.Name);
            if (await unitOfWork.DbContext.AnnouncementTypes.AnyAsync(x => x.Name == type.Name, cancellationToken))
            {
                throw new ConflictException("An announcement type with this name already exists.");
            }
            await unitOfWork.DbContext.AnnouncementTypes.AddAsync(type, cancellationToken);
            return Result.Success();
        }
    }
}
