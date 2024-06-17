using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public static class AddConsent
{
    [RequestAuthorize(Policy = PolicyNames.AllowEnrol)]
    public class Command : IRequest<Result<string>>
    {
        [Description("Participant Id")]
        public required string ParticipantId { get; set; }
        
        [Description("Consent Date")]
        public DateTime? ConsentDate { get; set; }
        
        public UploadRequest? UploadRequest { get; set; }
    }

    public class Handler(IUploadService uploadService) : IRequestHandler<Command, Result<string>>
    {
        public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
        {
            //todo: attach the upload to the participants account
            var result = await uploadService.UploadAsync(request.UploadRequest!);
            return result;
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.ParticipantId)
                .NotNull()
                .MinimumLength(9)
                .MaximumLength(9)
                .WithMessage("Invalid Participant Id");
            RuleFor(v => v.UploadRequest)
                .NotNull();
        }
    }
}
