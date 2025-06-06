using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Documents.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Documents.Queries;


public static class GetDocumentById
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]

    public class Query : IRequest<Result<DownloadDocumentDto>>
    {
        public Guid Id { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, IUploadService uploadService) : IRequestHandler<Query, Result<DownloadDocumentDto>>
    {
        public async Task<Result<DownloadDocumentDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var document = await unitOfWork.DbContext.Documents.FindAsync([request.Id], cancellationToken);

            if(document is null)
            {
                return Result<DownloadDocumentDto>.Failure("Document not found.");
            }

            var streamResult = await uploadService.DownloadAsync(document);

            if(streamResult.Succeeded)
            {
                DownloadDocumentDto dto = new()
                {
                    FileStream = streamResult,
                    FileExtension = document.Title!.Split(".").Last(),
                    FileName = document.Title!
                };
                return dto;
            }
            else
            {
                return Result<DownloadDocumentDto>.Failure(streamResult.Errors);
            }
        }
    }
    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Id.ToString())
                .NotEmpty()
                .WithMessage(string.Format(ValidationConstants.GuidMessage, "Id"));

        }
    }
}
