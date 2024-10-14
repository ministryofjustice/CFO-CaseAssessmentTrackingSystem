using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Notifications.Command
{
    public static class MarkAsUnread
    {
        [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
        public class Command : IRequest<Result<bool>>
        {
            public Guid[] NotificationsToMarkAsUnread { get; set; } = [];
            public UserProfile? CurrentUser { get; set; }
        }

        public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result<bool>>
        {
            public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
            {
                foreach (var notificationId in request.NotificationsToMarkAsUnread)
                {
                    var notification = await unitOfWork.DbContext.Notifications
                        .FirstOrDefaultAsync(x => x.Id == notificationId);

                    if (notification is null)
                    {
                        return Result<bool>.Failure("Notification not found");
                    }
                    else if (notification.ReadDate.HasValue)
                    {
                        notification.MarkAsUnread();
                    }

                }

                return Result<bool>.Success(true);
            }
        }

        public class A_ : AbstractValidator<Command>
        {
            public A_()
            {
                RuleFor(x => x.NotificationsToMarkAsUnread)
                .NotEmpty();

                RuleFor(x => x.CurrentUser)
                    .NotNull();
            }
        }

        public class A_NotificationsExists : AbstractValidator<Command>
        {
            private readonly IUnitOfWork _unitOfWork;

            public A_NotificationsExists(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;

                // Validate each NotificationId in the list
                RuleForEach(p => p.NotificationsToMarkAsUnread)
                        .NotEmpty()
                        .WithMessage(string.Format(ValidationConstants.GuidMessage, "Notification"))
                        .MustAsync(Exist)
                        .WithMessage("Notification does not exist");

            }

            // Check if the participant exists in the database
            private async Task<bool> Exist(Guid notificationId, CancellationToken cancellationToken)
                => await _unitOfWork.DbContext.Notifications.AnyAsync(p => p.Id == notificationId, cancellationToken);
        }

    }
}