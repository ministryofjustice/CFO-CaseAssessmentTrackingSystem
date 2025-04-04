using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public static class DeleteContactDetail
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Command : IRequest<Result<int>>
    {
        public required Guid ContactDetailId { get; set; }
    }

    class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result<int>>
    {
        public async Task<Result<int>> Handle(Command request, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.DbContext.ParticipantContactDetails
                .Where(pcd => pcd.Id == request.ContactDetailId)
                .ExecuteDeleteAsync(cancellationToken);

            return Result<int>.Success(result);
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private IUnitOfWork _unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.ContactDetailId)
                .Must(Exist);
        }

        bool Exist(Guid identifier) => _unitOfWork.DbContext.ParticipantContactDetails.Any(pcd => pcd.Id == identifier);
    }

}
