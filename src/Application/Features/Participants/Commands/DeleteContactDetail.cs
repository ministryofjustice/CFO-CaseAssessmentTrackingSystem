using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
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
                .Must(Exist)
                .WithMessage("Participant does not exist");
        }

        bool Exist(Guid identifier) => _unitOfWork.DbContext.ParticipantContactDetails.Any(pcd => pcd.Id == identifier);
    }

    public class A_ParticipantMustNotBeArchived : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public A_ParticipantMustNotBeArchived(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c.ContactDetailId)
                .Must(ParticipantMustNotBeArchived)
                .WithMessage("Participant is archived");
            });
        }

        private bool ParticipantMustNotBeArchived(Guid contactDetailId)
        {
            var participantId = (from pp in _unitOfWork.DbContext.ParticipantContactDetails
                                 join p in _unitOfWork.DbContext.Participants on pp.ParticipantId equals p.Id
                                 where (pp.Id == contactDetailId
                                 && p.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value)
                                 select p.Id
                                   )
                        .AsNoTracking()
                        .FirstOrDefault();

            return participantId != null;
        }
    }
}