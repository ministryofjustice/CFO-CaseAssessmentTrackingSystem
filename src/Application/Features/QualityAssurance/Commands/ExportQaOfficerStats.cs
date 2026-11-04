using Cfo.Cats.Application.Common.Exports;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.QualityAssurance.Queries;
using Cfo.Cats.Application.Features.QualityAssurance.Specifications;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Documents;
using Humanizer;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.QualityAssurance.Commands;

public static class ExportQaOfficerStats
{
    [RequestAuthorize(Policy = SecurityPolicies.ServiceDeskManagement)]
    public class Command : ICommand<Result>
    {
        public required QaOfficerStatsFilter Filter { get; init; }

        public string? QaUserName { get; init; }
    }

    public class Handler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var filter = request.Filter;

            var json = JsonConvert.SerializeObject(filter);

            var filename = ExportDocumentNaming.BuildFileName($"QaOfficer{filter.Process}");

            var description = ExportDocumentNaming.BuildDescription(
                $"QA Officer {filter.Process} Export",
                ("Start Date", filter.StartDate.ToString("dd MMM yyyy")),
                ("End Date", filter.EndDate.ToString("dd MMM yyyy")),
                ("Queues", filter.Queues is { Count: > 0 } ? string.Join(", ", filter.Queues.Select(q => q.Humanize())) : null),
                ("QA User", request.QaUserName),
                ("Is Accepted", filter.IsAccepted is { } accepted ? (accepted ? "Yes" : "No") : null),
                ("Notes Only", filter.NotesOnly ? "Yes" : null),
                ("CRN", filter.Crn));

            var document = GeneratedDocument.Create(
                DocumentTemplate.QaOfficerStats,
                filename,
                description,
                currentUser.UserId!,
                currentUser.TenantId!,
                json);

            await unitOfWork.DbContext.Documents.AddAsync(document, cancellationToken);

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IApplicationSettings _settings;
        private readonly IUnitOfWork _unitOfWork;

        public Validator(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IApplicationSettings settings)
        {
            _currentUserService = currentUserService;
            _settings = settings;
            _unitOfWork = unitOfWork;

            RuleFor(c => c.Filter.StartDate)
                .LessThanOrEqualTo(c => c.Filter.EndDate)
                .WithMessage("Start date must be on or before the end date.");

            RuleFor(c => c.Filter.IsWithinMaxRange())
                .Equal(true)
                .WithMessage(QaOfficerStatsFilter.MaxRangeMessage);

            RuleFor(c => c.Filter.Crn)
                .Matches(ValidationConstants.Keyword)
                .WithMessage(string.Format(ValidationConstants.KeywordMessage, "CRN"));

            RuleSet(ValidationConstants.RuleSet.Mediator, () =>
            {
                RuleFor(c => c)
                    .Must(WaitBeforeRequestingDocumentAgain)
                    .WithMessage($"You must wait {ExportDocumentNaming.GetDocumentExportCooldown(_settings).Humanize()} between requesting documents.");
            });
        }

        private bool WaitBeforeRequestingDocumentAgain(Command c)
        {
            var cooldownPeriod = DateTime.UtcNow - ExportDocumentNaming.GetDocumentExportCooldown(_settings);

            var hasRecentlyRequestedDocument = _unitOfWork.DbContext.GeneratedDocuments
                .Any(d => d.CreatedBy == _currentUserService.UserId && d.Created > cooldownPeriod);

            return hasRecentlyRequestedDocument is false;
        }
    }
}
