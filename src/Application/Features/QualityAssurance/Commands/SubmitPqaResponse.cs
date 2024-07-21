using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.QualityAssurance.Commands;

public static class SubmitPqaResponse
{
    [RequestAuthorize(Policy = PolicyNames.CanSubmitToQA)]
    public class Command : IRequest<Result>
    {
        public required Guid QueueEntryId { get; set; }
        
        public bool Accept { get; set; }
        
        public string? Message { get; set; }
    }
    
    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var entry = await unitOfWork.DbContext.EnrolmentPqaQueue
                .Include(pqa => pqa.Participant)
                .FirstOrDefaultAsync(x => x.Id == request.QueueEntryId, cancellationToken: cancellationToken);

            if (entry == null)
            {
                return Result.Failure("Cannot find queue item");
            }
            
            entry.Complete(request.Accept, request.Message);
            entry.Participant!.TransitionTo(EnrolmentStatus.SubmittedToAuthorityStatus);
            
            return Result.Success();
        }
    }
    
    public class A_EntryMustExist : AbstractValidator<Command> 
    {
        private IUnitOfWork _unitOfWork;
        public A_EntryMustExist(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.QueueEntryId)
                .MustAsync(MustExist)
                .WithMessage("Queue item does not exist");
        }
        private async Task<bool> MustExist(Guid identifier, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.EnrolmentPqaQueue.AnyAsync(e => e.Id == identifier, cancellationToken);
    }

    public class B_ShouldNotBeComplete : AbstractValidator<Command>
    {
        private IUnitOfWork _unitOfWork;

        public B_ShouldNotBeComplete(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.QueueEntryId)
                .MustAsync(MustBeOpen)
                .WithMessage("Queue item is already completed.");
        }

        private async Task<bool> MustBeOpen(Guid id, CancellationToken cancellationToken)
        {
            var entry = await _unitOfWork.DbContext.EnrolmentPqaQueue.Include(c => c.Participant)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken: cancellationToken);

            return entry is { IsCompleted: false };

        }
    }
    
    public class C_ShouldNotBeAtPqaStatus : AbstractValidator<Command>
    {
        private IUnitOfWork _unitOfWork;

        public C_ShouldNotBeAtPqaStatus(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.QueueEntryId)
                .MustAsync(MustBeAtPqA)
                .WithMessage("Queue item is not a PQA stage");
        }

        private async Task<bool> MustBeAtPqA(Guid id, CancellationToken cancellationToken)
        {
            var entry = await _unitOfWork.DbContext.EnrolmentPqaQueue.Include(c => c.Participant)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken: cancellationToken);

            return entry != null && entry.Participant!.EnrolmentStatus == EnrolmentStatus.SubmittedToProviderStatus;

        }
    }
    
}
