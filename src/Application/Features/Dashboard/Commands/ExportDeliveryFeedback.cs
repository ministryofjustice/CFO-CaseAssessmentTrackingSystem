using Cfo.Cats.Application.Common.Exports;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Documents;
using Humanizer;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.Dashboard.Commands;

public static class ExportDeliveryFeedback
{
    [RequestAuthorize(Policy = SecurityPolicies.DeliveryFeedbackView)]
    public class Command : ICommand<Result>
    {
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var tenantId = currentUser.TenantId;
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                return Result.Failure("Current user TenantId is required.");
            }

            var exportRequest = new ExportProviderFeedback.ProviderFeedbackExportRequest
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TenantId = tenantId,
                IncludeInternalData = false
            };

            var document = GeneratedDocument.Create(
                DocumentTemplate.ProviderFeedback,
                ExportDocumentNaming.BuildFileName("DeliveryFeedback"),
                ExportDocumentNaming.BuildDescription(
                    "Delivery Feedback Export",
                    ("Start Date", request.StartDate.ToString("dd MMM yyyy")),
                    ("End Date", request.EndDate.ToString("dd MMM yyyy"))),
                currentUser.UserId!,
                tenantId,
                JsonConvert.SerializeObject(exportRequest));

            await unitOfWork.DbContext.Documents.AddAsync(document, cancellationToken);
            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator(IUnitOfWork unitOfWork, ICurrentUserService currentUser, IApplicationSettings settings)
        {
            RuleSet(ValidationConstants.RuleSet.Mediator, () =>
            {
                RuleFor(command => command.StartDate).LessThanOrEqualTo(command => command.EndDate);
                RuleFor(command => command)
                    .Must(command =>
                    {
                        var cooldownPeriod = DateTime.UtcNow - ExportDocumentNaming.GetDocumentExportCooldown(settings);
                        return !unitOfWork.DbContext.GeneratedDocuments.Any(document =>
                            document.CreatedBy == currentUser.UserId && document.Created > cooldownPeriod);
                    })
                    .WithMessage($"You must wait {ExportDocumentNaming.GetDocumentExportCooldown(settings).Humanize()} between requesting documents.");
            });
        }
    }
}