using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Outbox.Commands;

public static class RescheduleOutboxMessage
{
    [RequestAuthorize(Roles = RoleNames.SystemSupport)]
    public class Command : IRequest<Result>
    {
        public Guid OutboxMessageId { get;set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var message = await unitOfWork.DbContext
                            .OutboxMessages
                            .AsNoTracking()
                            .FirstAsync(om => om.Id == request.OutboxMessageId);

            var newMessage = message.Replicate();
            
            await unitOfWork.DbContext
                .OutboxMessages
                .AddAsync(newMessage);

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private IUnitOfWork _unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.OutboxMessageId)
                .NotEmpty();

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(x => x.OutboxMessageId)
                    .MustAsync(Exist)
                    .WithMessage("Outbox Message not found");
            });
        }

        private async Task<bool> Exist(Guid identifier, CancellationToken cancellationToken) 
            => await _unitOfWork.DbContext
                        .OutboxMessages
                        .AnyAsync(e => e.Id == identifier, cancellationToken);
    }
}