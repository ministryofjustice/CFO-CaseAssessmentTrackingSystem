using Cfo.Cats.Application.Common.Exports;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Documents;
using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Application.Common.Interfaces.MultiTenant;
using Humanizer;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public static class ExportEngagementsByLocation
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Command : ICommand<Result>
    {
        public required GetEngagementsByLocation.Query Query { get; set; }
    }

    public class Handler(
        IUnitOfWork unitOfWork,
        ILocationService locationService,
        ITenantService tenantService,
        ICurrentUserService currentUser) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var json = JsonConvert.SerializeObject(request.Query);

            var filename = ExportDocumentNaming.BuildFileName("EngagementsByLocation");

            var locationName = request.Query.LocationId.HasValue
                ? locationService.DataSource.FirstOrDefault(l => l.Id == request.Query.LocationId.Value)?.Name
                : null;

            var tenantName = string.IsNullOrWhiteSpace(request.Query.TenantId)
                ? null
                : tenantService.DataSource.FirstOrDefault(t => t.Id == request.Query.TenantId)?.Name;

            var description = ExportDocumentNaming.BuildDescription(
                "Engagements By Location Export",
                ("Location", locationName),
                ("Category", request.Query.EngagementType),
                ("Location Type", request.Query.LocationType),
                ("Contract", tenantName),
                ("Date", new DateTime(request.Query.Year, request.Query.Month, 1).ToString("MMM yyyy")));

            var document = GeneratedDocument
                .Create(DocumentTemplate.EngagementsByLocation, filename, description, currentUser.UserId!, currentUser.TenantId!, json);

            await unitOfWork.DbContext.Documents.AddAsync(document, cancellationToken);

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly ICurrentUserService currentUserService;
        private readonly IUnitOfWork unitOfWork;
        private readonly TimeSpan cooldown = TimeSpan.FromSeconds(30);

        public Validator(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            this.currentUserService = currentUserService;
            this.unitOfWork = unitOfWork;

            RuleSet(ValidationConstants.RuleSet.Mediator, () =>
            {
                RuleFor(c => c)
                    .Must(WaitBeforeRequestingDocumentAgain)
                    .WithMessage($"You must wait {cooldown.Humanize()} between requesting documents.");
            });
        }

        private bool WaitBeforeRequestingDocumentAgain(Command c)
        {
            var cooldownPeriod = DateTime.UtcNow - cooldown;

            var hasRecentlyRequestedDocument = unitOfWork.DbContext.GeneratedDocuments
                .Any(d => d.CreatedBy == currentUserService.UserId && d.Created > cooldownPeriod);

            return hasRecentlyRequestedDocument is false;
        }
    }
}
