using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;
using DocumentFormat.OpenXml.InkML;
namespace Cfo.Cats.Application.Features.Participants.Commands;

public static class UpsertPriCode
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result>
    {
        public required string ParticipantId { get; set; }
        public required int Code { get; set; }

    }
    
    class Handler(IUnitOfWork unitOfWork, ICurrentUserService userService) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var code = await unitOfWork.DbContext.PriCodes.FindAsync(
                [request.ParticipantId, userService.UserId], 
                cancellationToken);

            if(code is not null)
            {
                unitOfWork.DbContext.PriCodes.Remove(code);
            }

            code = PriCode.Create(request.ParticipantId, userService.UserId!);

            await unitOfWork.DbContext.PriCodes.AddAsync(code, cancellationToken);

            return Result.Success();
        }

    }
    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ParticipantId)
                .NotNull()
                .MinimumLength(9)
                .MaximumLength(9)
                .Matches(ValidationConstants.AlphaNumeric);
        }
    }
}
