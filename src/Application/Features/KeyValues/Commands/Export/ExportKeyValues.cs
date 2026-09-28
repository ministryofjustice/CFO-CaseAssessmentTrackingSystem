using Cfo.Cats.Application.Common.Exports;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.KeyValues.Queries.PaginationQuery;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Documents;
using Humanizer;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.KeyValues.Commands.Export;

public static class ExportKeyValues
{
    [RequestAuthorize(Roles = RoleNames.SystemSupport)]
    public class Command : ICommand<Result>
    {
        public required KeyValuesWithPaginationQuery? Query { get; set; }
    }

    public class Handler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var json = JsonConvert.SerializeObject(request.Query);

            var filename = ExportDocumentNaming.BuildFileName("KeyValues");

            var description = ExportDocumentNaming.BuildDescription(
                "KeyValues Export",
                ("Search", request.Query?.Keyword),
                ("Picklist", request.Query?.Picklist?.ToString()));

            var document = GeneratedDocument
                .Create(DocumentTemplate.KeyValues, filename, description, currentUser.UserId!, currentUser.TenantId!, json);

            await unitOfWork.DbContext.Documents.AddAsync(document, cancellationToken);

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly ICurrentUserService currentUserService;
        private readonly IApplicationSettings settings;
        private readonly IUnitOfWork unitOfWork;

        public Validator(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IApplicationSettings settings)
        {
            this.currentUserService = currentUserService;
            this.settings = settings;
            this.unitOfWork = unitOfWork;

            RuleFor(c => c)
                .Must(WaitBeforeRequestingDocumentAgain)
                .WithMessage($"You must wait {ExportDocumentNaming.GetDocumentExportCooldown(settings).Humanize()} between requesting documents.");
        }

        private bool WaitBeforeRequestingDocumentAgain(Command c)
        {
            var cooldownPeriod = DateTime.UtcNow - ExportDocumentNaming.GetDocumentExportCooldown(settings);

            var hasRecentlyRequestedDocument = unitOfWork.DbContext.GeneratedDocuments
                .Any(d => d.CreatedBy == currentUserService.UserId && d.Created > cooldownPeriod);

            return hasRecentlyRequestedDocument is false;
        }
    }
}
