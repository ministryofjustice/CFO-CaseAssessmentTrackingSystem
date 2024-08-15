using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Bios.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Bios;
using DocumentFormat.OpenXml.Office.PowerPoint.Y2021.M06.Main;

namespace Cfo.Cats.Application.Features.Bios.Commands;

public static class SkipBioForNow
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result>
    {
        public string? ParticipantId { get; set;}
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            ParticipantBio? bio = await unitOfWork.DbContext.ParticipantBios.FirstOrDefaultAsync(r => r.ParticipantId == request.ParticipantId);

            if (bio == null)
            {
                bio = ParticipantBio.Create(Guid.NewGuid(), request.ParticipantId!, string.Empty, BioStatus.NotStarted);
                unitOfWork.DbContext.ParticipantBios.Add(bio);
            }

            bio.UpdateStatus(BioStatus.SkippedForNow);
            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private IUnitOfWork _unitOfWork;

        public  Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.ParticipantId)
                .MinimumLength(9)
                .MaximumLength(9)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, nameof(Command.ParticipantId)));
        }
    }

}
