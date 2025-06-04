using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Documents;
using Humanizer.Bytes;
using Microsoft.AspNetCore.Components.Forms;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public static class AddConsent
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result<string>>
    {
        [Description("Participant Id")]
        public required string ParticipantId { get; set; }
        
        [Description("Date of Consent")]
        public DateTime? ConsentDate { get; set; }

        [Description("Document Version")]
        public string? DocumentVersion { get; set; }

        [Description("Consent Document")]
        public IBrowserFile? Document { get; set; }

        [Description("I certify that any documents uploaded are the original or true copies of the original documents, and will not be used to claim payments / results against any other Government contract")]
        public bool Certify { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, IUploadService uploadService) : IRequestHandler<Command, Result<string>>
    {
        public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
        {
            // get the participant
            var participant = await unitOfWork.DbContext.Participants.FindAsync(request.ParticipantId!, cancellationToken);

            if (participant == null)
            {
                throw new NotFoundException("Cannot find participant", request.ParticipantId);
            }

            var document = Document.Create(request.Document!.Name,
            $"Consent form for {request.ParticipantId}",
            DocumentType.PDF);

            long maxSizeBytes = Convert.ToInt64(ByteSize.FromMegabytes(Infrastructure.Constants.Documents.Consent.MaximumSizeInMegabytes).Bytes);
            await using var stream = request.Document.OpenReadStream(maxSizeBytes);
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream, cancellationToken);

            var uploadRequest = new UploadRequest(request.Document.Name, UploadType.Document, memoryStream.ToArray());

            var result = await uploadService.UploadAsync($"{request.ParticipantId}/consent", uploadRequest);

            if (result.Succeeded)
            {
                document.SetURL(result);
                document.SetVersion(request.DocumentVersion!);

                participant.AddConsent(request.ConsentDate!.Value, document.Id);

                await unitOfWork.DbContext.Documents.AddAsync(document);
            }

            return result;
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.ParticipantId)
                .NotNull()
                .Length(9)
                .WithMessage("Invalid Participant Id")
                .MustAsync(Exist)
                .WithMessage("Participant does not exist")
                .Matches(ValidationConstants.AlphaNumeric).WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"))
                .MustAsync(MustNotBeArchived)
                .WithMessage("Participant is archived");

            RuleFor(v => v.ConsentDate)
                .NotNull()
                .WithMessage("You must provide the Date of Consent")
                .LessThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage(ValidationConstants.DateMustBeInPast)
                .GreaterThanOrEqualTo(DateTime.Today.AddMonths(-3))
                .WithMessage("Cannot backdate beyond 3 months");

            RuleFor(v => v.Document)
                .NotNull()
                .WithMessage("You must upload a Consent document")
                .Must(file => NotExceedMaximumFileSize(file, Infrastructure.Constants.Documents.Consent.MaximumSizeInMegabytes))
                .WithMessage($"File size exceeds the maxmimum allowed size of {Infrastructure.Constants.Documents.Consent.MaximumSizeInMegabytes} megabytes")
                .MustAsync(BePdfFile)
                .WithMessage("File is not a PDF");

            RuleFor(v => v.DocumentVersion)
                .NotEmpty()
                .WithMessage("You must select a document version")
                .Must(version => Infrastructure.Constants.Documents.Consent.Versions.Contains(version))
                .WithMessage("Unrecognised document version");

            RuleFor(v => v.Certify)
                .Equal(true)
                .WithMessage("You must upload a document and certify");
        }
        
        private async Task<bool> Exist(string identifier, CancellationToken cancellationToken) 
            => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == identifier, cancellationToken);

        private async Task<bool> MustNotBeArchived(string participantId, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == participantId && e.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value, cancellationToken);

        private static bool NotExceedMaximumFileSize(IBrowserFile? file, double maxSizeMB)
            => file?.Size < ByteSize.FromMegabytes(maxSizeMB).Bytes;

        private async Task<bool> BePdfFile(IBrowserFile? file, CancellationToken cancellationToken)
        {
            if (file is null)
                return false;

            // Check file extension
            if (!Path.GetExtension(file.Name).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                return false;

            // Check MIME type
            if (file.ContentType != "application/pdf")
                return false;

            long maxSizeBytes = Convert.ToInt64(ByteSize.FromMegabytes(Infrastructure.Constants.Documents.Consent.MaximumSizeInMegabytes).Bytes);

            // Check file signature (magic numbers)
            using (var stream = file.OpenReadStream(maxSizeBytes, cancellationToken))
            {
                byte[] buffer = new byte[4];
                await stream.ReadExactlyAsync(buffer.AsMemory(0, 4), cancellationToken);
                string header = System.Text.Encoding.ASCII.GetString(buffer);
                return header == "%PDF";
            }
        }
    }
}