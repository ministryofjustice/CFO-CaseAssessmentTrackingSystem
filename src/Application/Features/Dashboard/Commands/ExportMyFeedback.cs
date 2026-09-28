using Cfo.Cats.Application.Common.Exports;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Documents;
using Humanizer;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.Dashboard.Commands;

public static class ExportMyFeedback
{
    [RequestAuthorize(Policy = SecurityPolicies.Internal)]
    public class Command : ICommand<Result>
    {
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
    }

    public class Handler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            // UserId is forced to the caller's own id - never trust a client-supplied value here -
            // so a service desk officer can only ever export the feedback they have personally received.
            var exportRequest = new ExportProviderFeedback.ProviderFeedbackExportRequest
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                UserId = currentUser.UserId
            };

            var json = JsonConvert.SerializeObject(exportRequest);

            var filename = ExportDocumentNaming.BuildFileName("MyFeedback");

            var description = ExportDocumentNaming.BuildDescription(
                "My Feedback Export",
                ("Start Date", request.StartDate.ToString("dd MMM yyyy")),
                ("End Date", request.EndDate.ToString("dd MMM yyyy")));

            var document = GeneratedDocument.Create(
                DocumentTemplate.ProviderFeedback,
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
