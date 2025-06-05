using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Dashboard.IntegrationEvents;
using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Documents;

namespace Cfo.Cats.Application.Features.Dashboard.Export;

public static class ExportCaseWorkload 
{
    [RequestAuthorize(Policy = SecurityPolicies.UserHasAdditionalRoles)]
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
                .Create("CaseWorkload.xlsx", "CaseWorkload Export", currentUser.UserId!)
                .WithStatus(DocumentStatus.Processing)
                .WithExpiry(DateTime.UtcNow.AddDays(7));

            await unitOfWork.DbContext.Documents.AddAsync(document, cancellationToken);

            // Queue document generation
            await unitOfWork.DbContext.InsertOutboxMessage(new ExportCaseWorkloadIntegrationEvent(document.Id, currentUser.UserId!, currentUser.TenantId!, request.SearchCriteria));

            return Result.Success();
        }
    }
}