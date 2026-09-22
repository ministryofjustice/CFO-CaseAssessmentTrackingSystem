using Cfo.Cats.Application.Common.Exports;
using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Application.Common.Interfaces.MultiTenant;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Application.Features.Participants.Specifications;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Documents;
using Humanizer;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public static class ExportParticipants
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Command : ICommand<Result>
    {
        public required ParticipantsWithPagination.Query Query { get; set; }
    }

    public class Handler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        ITenantService tenantService,
        ILocationService locationService) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var json = JsonConvert.SerializeObject(request.Query);

            var filename = ExportDocumentNaming.BuildFileName("Participants");

            var tenantName = string.IsNullOrWhiteSpace(request.Query.TenantId)
                ? null
                : tenantService.DataSource.FirstOrDefault(t => t.Id == request.Query.TenantId)?.Name;

            var ownerName = string.IsNullOrWhiteSpace(request.Query.OwnerId)
                ? null
                : await unitOfWork.DbContext.Users
                    .Where(u => u.Id == request.Query.OwnerId)
                    .Select(u => u.DisplayName)
                    .FirstOrDefaultAsync(cancellationToken);

            var locationNames = request.Query.Locations.Length > 0
                ? string.Join(", ", request.Query.Locations
                    .Select(id => locationService.DataSource.FirstOrDefault(l => l.Id == id)?.Name ?? id.ToString()))
                : null;

            var labelName = request.Query.Label is null
                ? null
                : await unitOfWork.DbContext.Labels
                    .Where(l => l.Id == request.Query.Label)
                    .Select(l => l.Name)
                    .FirstOrDefaultAsync(cancellationToken);

            var description = ExportDocumentNaming.BuildDescription(
                "Participants Export",
                ("Search", request.Query.Keyword),
                ("List View", request.Query.ListView == ParticipantListView.Default ? null : request.Query.ListView.ToString()),
                ("Just My Cases", request.Query.JustMyCases ? "Yes" : null),
                ("Locations", locationNames),
                ("Label", labelName),
                ("Owner", ownerName),
                ("Tenant", tenantName),
                ("Risk Due", request.Query.RiskDue?.ToString("d")),
                ("Recent Action", request.Query.RecentAction == RecentParticipantFilter.All ? null : request.Query.RecentAction.ToString()));

            var document = GeneratedDocument
                .Create(DocumentTemplate.Participants, filename, description, currentUser.UserId!, currentUser.TenantId!, json);

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