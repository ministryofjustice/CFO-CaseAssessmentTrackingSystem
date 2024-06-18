using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Documents;
using Cfo.Cats.Domain.Entities.Participants;

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

    public class Handler(IApplicationDbContext context, IUploadService uploadService) : IRequestHandler<Command, Result<string>>
    {
        public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
        {
            // get the participant
            var participant = await context.Participants.FindAsync(request.ParticipantId!, cancellationToken);

            if (participant == null)
            {
                throw new NotFoundException("Cannot find participant", request.ParticipantId);
            }

            var document = new Document()
            {
                Description = $"Consent form for {request.ParticipantId}",
                DocumentType = DocumentType.Document,
                IsPublic = false,
                Title = request.UploadRequest!.FileName
            };
            
            //todo: attach the upload to the participants account
            var result = await uploadService.UploadAsync($"{request.ParticipantId}/consent", request.UploadRequest!);

            document.URL = result;

            participant.AddConsent(request.ConsentDate!.Value, document.Id);
            
            context.Documents.Add(document);
            
            await context.SaveChangesAsync(cancellationToken);
            
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
