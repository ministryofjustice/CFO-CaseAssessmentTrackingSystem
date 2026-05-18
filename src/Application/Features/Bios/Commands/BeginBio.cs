using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Bios.DTOs;
using Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.Diversity;
using Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.ChildhoodExperiences;
using Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.RecentExperiences;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Bios;
using Cfo.Cats.Application.Common.Validators;

namespace Cfo.Cats.Application.Features.Bios.Commands;

public static class BeginBio
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Command : IRequest<Result<Guid>>
    {
        public required string ParticipantId { get; set; }
    }

    public class Handler : IRequestHandler<Command, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public Handler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var oldBio = await GetPreviousBio(request, cancellationToken);

            Guid bioId = Guid.CreateVersion7();
            
            ParticipantBio bioEntity = ParticipantBio.Create(bioId, request.ParticipantId);

            // Copy answers from previous bio if it exists
            if (oldBio != null)
            {
                foreach (var pathway in oldBio.Pathways)
                {
                    foreach (var question in pathway.Questions())
                    {
                        if (question is SingleChoiceQuestion single && single.Answer is not null)
                        {
                            bioEntity.SetAnswer(single.Code, single.Answer);
                        }
                    }
                }
            }

            await _unitOfWork.DbContext.ParticipantBios.AddAsync(bioEntity);

            return Result<Guid>.Success(bioId);
        }

        private async Task<Bio?> GetPreviousBio(Command request, CancellationToken cancellationToken)
        {
            var oldBioEntity = await _unitOfWork.DbContext.ParticipantBios
                .Where(b => b.ParticipantId == request.ParticipantId)
                .OrderByDescending(b => b.Created)
                .FirstOrDefaultAsync(cancellationToken);

            if (oldBioEntity == null)
            {
                return null;
            }

            var bio = new Bio
            {
                Id = oldBioEntity.Id,
                ParticipantId = oldBioEntity.ParticipantId,
                Pathways =
                [
                    new DiversityPathway(),
                    new ChildhoodExperiencesPathway(),
                    new RecentExperiencesPathway(),
                ]
            }.WithAnswers(oldBioEntity.Answers.ToLookup(a => a.QuestionCode, a => a.Answer));

            return bio;
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.ParticipantId)
                .MinimumLength(9)
                .MaximumLength(9)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c.ParticipantId)
                    .MustAsync(Exist)
                    .WithMessage("Participant not found")
                    .MustAsync(MustNotBeArchived)
                    .WithMessage("Participant is archived");
            });
        }

        private async Task<bool> MustNotBeArchived(string participantId, CancellationToken cancellationToken)
                => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == participantId && e.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value, cancellationToken);

        private async Task<bool> Exist(string participantId, CancellationToken cancellationToken)
                => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == participantId, cancellationToken);
    }
}