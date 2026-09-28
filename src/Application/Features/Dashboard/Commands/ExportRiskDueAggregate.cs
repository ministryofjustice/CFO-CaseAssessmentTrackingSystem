using Cfo.Cats.Application.Common.Exports;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Documents;
using Humanizer;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.Dashboard.Export;

public static class ExportRiskDueAggregate
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Command : ICommand<Result>
    {
        public required GetRiskDueAggregate.Query Query { get; set; }
    }

    public class Handler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var json = JsonConvert.SerializeObject(request.Query);

            var filename = ExportDocumentNaming.BuildFileName("RiskDueAggregate");

            var description = ExportDocumentNaming.BuildDescription(
                "RiskDue Aggregate Export",
                ("Grouping", request.Query.GroupingType.ToString()));

            var document = GeneratedDocument
                .Create(DocumentTemplate.RiskDueAggregate, filename, description, currentUser.UserId!, currentUser.TenantId!, json);

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

            RuleSet(ValidationConstants.RuleSet.Mediator, () =>
            {
                RuleFor(c => c)
                    .Must(WaitBeforeRequestingDocumentAgain)
                    .WithMessage($"You must wait {ExportDocumentNaming.GetDocumentExportCooldown(settings).Humanize()} between requesting documents.");
            });
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