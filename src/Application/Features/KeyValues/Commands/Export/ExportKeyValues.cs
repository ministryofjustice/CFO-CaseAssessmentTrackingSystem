using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Documents;

namespace Cfo.Cats.Application.Features.KeyValues.Commands.Export;

public static class ExportKeyValues
{
    [RequestAuthorize(Roles = RoleNames.SystemSupport)]
    public class Command : IRequest<Result>
    {
        public string? SearchCriteria { get; set; }
    }

    public class Handler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var document = GeneratedDocument
                .Create(DocumentTemplate.KeyValues, "KeyValues.xlsx", "KeyValues Export", currentUser.UserId!, currentUser.TenantId!, request.SearchCriteria)
                .WithStatus(DocumentStatus.Processing)
                .WithExpiry(DateTime.UtcNow.AddDays(7));

            await unitOfWork.DbContext.Documents.AddAsync(document, cancellationToken);

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {

            RuleFor(r => r.SearchCriteria)
                .Matches(ValidationConstants.Keyword)
                .WithMessage(string.Format(ValidationConstants.KeywordMessage, "Search Keyword"));
        }
    }
}
