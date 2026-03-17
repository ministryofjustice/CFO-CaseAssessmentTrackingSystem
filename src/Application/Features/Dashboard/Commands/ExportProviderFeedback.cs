using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Documents;
using Humanizer;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.Dashboard.Commands;

public static class ExportProviderFeedback
{
    [RequestAuthorize(Policy = SecurityPolicies.Internal)]
    public class Command : IRequest<Result>
    {
        public required ProviderFeedbackExportRequest Request { get; set; }
    }

    public class Handler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var json = JsonConvert.SerializeObject(request.Request);

            var document = GeneratedDocument.Create(
                DocumentTemplate.ProviderFeedback,
                "ProviderFeedback.xlsx",
                "Provider Feedback Export",
                currentUser.UserId!,
                currentUser.TenantId!,
                json);

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

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c.Request)
                    .Must(r => r.IncludeEnrolmentReturns || r.IncludeActivitiesReturns || r.IncludeEnrolmentAdvisories || r.IncludeActivitiesAdvisories)
                    .WithMessage("At least one sheet must be selected for export.");

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

    public class ProviderFeedbackExportRequest
    {
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public string? TenantId { get; set; }
        public bool IncludeEnrolmentReturns { get; set; }
        public bool IncludeActivitiesReturns { get; set; }
        public bool IncludeEnrolmentAdvisories { get; set; }
        public bool IncludeActivitiesAdvisories { get; set; }
    }
}
