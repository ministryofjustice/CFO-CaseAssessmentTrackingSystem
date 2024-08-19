﻿using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.QualityAssurance.Commands;

public static class SubmitPqaResponse
{
    [RequestAuthorize(Policy = SecurityPolicies.Pqa)]
    public class Command : IRequest<Result>
    {
        public required Guid QueueEntryId { get; set; }
        
        public bool? Accept { get; set; }

        public string Message { get; set; } = default!;
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
            
            entry.Complete(request.Accept.GetValueOrDefault(), request.Message);
            entry.Participant!.TransitionTo(EnrolmentStatus.SubmittedToAuthorityStatus);
            
            return Result.Success();
        }
    }

    public class A_IsValidRequest : AbstractValidator<Command>
    {
        public A_IsValidRequest()
        {
            RuleFor(x => x.Accept)
                .NotNull()
                .WithMessage("You must accept or return the request");

            When(x => x.Accept is false, () =>
            {
                RuleFor(x => x.Message)
                    .NotEmpty()
                    .WithMessage("A message is required when returning")
                    .Matches(ValidationConstants.Notes)
                    .WithMessage(string.Format(ValidationConstants.NotesMessage, "Message"));
            });
        }
    }


    public class B_EntryMustExist : AbstractValidator<Command> 
    {
        private IUnitOfWork _unitOfWork;
        public B_EntryMustExist(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.QueueEntryId)
                .MustAsync(MustExist)
                .WithMessage("Queue item does not exist");
        }
        private async Task<bool> MustExist(Guid identifier, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.EnrolmentPqaQueue.AnyAsync(e => e.Id == identifier, cancellationToken);
    }

    public class C_ShouldNotBeComplete : AbstractValidator<Command>
    {
        private IUnitOfWork _unitOfWork;

        public C_ShouldNotBeComplete(IUnitOfWork unitOfWork)
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
    
    public class D_ShouldNotBeAtPqaStatus : AbstractValidator<Command>
    {
        private IUnitOfWork _unitOfWork;

        public D_ShouldNotBeAtPqaStatus(IUnitOfWork unitOfWork)
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
