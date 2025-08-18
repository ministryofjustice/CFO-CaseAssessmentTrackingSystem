using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Documents;
using Humanizer.Bytes;
using Microsoft.AspNetCore.Components.Forms;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public static class AddRightToWork
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result<string>>
    {
        [Description("Participant Id")]
        public required string ParticipantId { get; set; }

        public bool RightToWorkRequired { get; set; } = true;

        [Description("Indefinite Right to Work")]
        public bool IndefiniteRightToWork { get; set; }
        
        [Description("Valid From")]
        public DateTime? ValidFrom { get; set; }
        
        [Description("Valid To")]
        public DateTime? ValidTo { get; set; }

        [Description("Right to Work document")]
        public IBrowserFile? Document { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, IUploadService uploadService)
        : IRequestHandler<Command, Result<string>>
    {
        public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
        {
            var participant = await unitOfWork.DbContext.Participants.FindAsync(request.ParticipantId);
            
            if(participant == null)
            {
                throw new NotFoundException("Cannot find participant", request.ParticipantId);
            }

            var document = Document.Create(request.Document!.Name,
                $"Right to work evidence for {request.ParticipantId}",
                DocumentType.PDF);

            long maxSizeBytes = Convert.ToInt64(ByteSize.FromMegabytes(Infrastructure.Constants.Documents.RightToWork.MaximumSizeInMegabytes).Bytes);
            await using var stream = request.Document.OpenReadStream(maxSizeBytes);
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream, cancellationToken);

            var uploadRequest = new UploadRequest(request.Document.Name, UploadType.Document, memoryStream.ToArray());

            var result = await uploadService.UploadAsync($"{request.ParticipantId}/rtw", uploadRequest);
            if (result.Succeeded)
            {
                document.SetURL(result);

                if (request.IndefiniteRightToWork)
                {
                    request.ValidTo = DateTime.MaxValue;
                }

                participant.AddRightToWork(request.ValidFrom!.Value, request.ValidTo!.Value, document.Id);

                unitOfWork.DbContext.Documents.Add(document);
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
                .Matches(ValidationConstants.AlphaNumeric).WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));
            
            When(v => v.RightToWorkRequired, () =>
            {
                RuleFor(v => v.Document)
                    .NotNull()
                    .WithMessage("You must upload a Right to Work document")
                    .Must(file => NotExceedMaximumFileSize(file, Infrastructure.Constants.Documents.RightToWork.MaximumSizeInMegabytes))
                    .WithMessage($"File size exceeds the maxmimum allowed size of {Infrastructure.Constants.Documents.RightToWork.MaximumSizeInMegabytes} megabytes")
                    .MustAsync(BePdfFile)
                    .WithMessage("File is not a PDF");

                RuleFor(v => v.ValidFrom)
                    .NotNull()
                    .WithMessage("You must provide the Valid From date")
                    .LessThanOrEqualTo(DateTime.UtcNow.Date)
                    .WithMessage(ValidationConstants.DateMustBeInPast);

                When(v => v.IndefiniteRightToWork is false, () =>
                {
                    RuleFor(v => v.ValidTo)
                        .NotNull()
                        .WithMessage("You must provide the Valid To date")
                        .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
                        .WithMessage(ValidationConstants.DateMustBeInFuture);
                });

                RuleSet(ValidationConstants.RuleSet.MediatR, () =>
                {
                    RuleFor(c => c.ParticipantId)
                        .MustAsync(Exist)
                        .WithMessage("Participant does not exist")
                        .MustAsync(MustNotBeArchived)
                        .WithMessage("Participant is archived");

                });
            });
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
            {
                return false;
            }

            // Check file extension
            if (!Path.GetExtension(file.Name).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // Check MIME type
            if (file.ContentType != "application/pdf")
            {
                return false;
            }

            long maxSizeBytes = Convert.ToInt64(ByteSize.FromMegabytes(Infrastructure.Constants.Documents.RightToWork.MaximumSizeInMegabytes).Bytes);

            // Check file signature (magic numbers)
            await using var stream = file.OpenReadStream(maxSizeBytes, cancellationToken);
            var buffer = new byte[4];
            await stream.ReadExactlyAsync(buffer.AsMemory(0, 4), cancellationToken);
            var header = System.Text.Encoding.ASCII.GetString(buffer);
            return header == "%PDF";
        }
    }    
}