using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Dashboard.Commands;
using Cfo.Cats.Domain.Entities.Documents;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Dashboard.Commands;

public class ExportDeliveryFeedbackTests
{
    [Test]
    public async Task Handler_ShouldQueueProviderOnlyExportForCallerTenant()
    {
        GeneratedDocument addedDocument = null;
        var documents = new Mock<DbSet<Document>>();
        documents.Setup(set => set.AddAsync(It.IsAny<Document>(), It.IsAny<CancellationToken>()))
            .Callback<Document, CancellationToken>((document, cancellationToken) =>
                addedDocument = (GeneratedDocument)document);

        var context = new Mock<IApplicationDbContext>();
        context.SetupGet(db => db.Documents).Returns(documents.Object);
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(work => work.DbContext).Returns(context.Object);
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.SetupGet(user => user.UserId).Returns("provider-user");
        currentUser.SetupGet(user => user.TenantId).Returns("1.2.");

        var command = new ExportDeliveryFeedback.Command
        {
            StartDate = new DateTime(2026, 10, 1),
            EndDate = new DateTime(2026, 10, 12)
        };

        var result = await new ExportDeliveryFeedback.Handler(unitOfWork.Object, currentUser.Object)
            .Handle(command, CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        addedDocument.ShouldNotBeNull();
        addedDocument.Template.ShouldBe(DocumentTemplate.ProviderFeedback);
        addedDocument.CreatedBy.ShouldBe("provider-user");
        addedDocument.TenantId.ShouldBe("1.2.");
        addedDocument.Status.ShouldBe(DocumentStatus.Processing);

        var request = JsonConvert.DeserializeObject<ExportProviderFeedback.ProviderFeedbackExportRequest>(
            addedDocument.SearchCriteriaUsed);
        request.ShouldNotBeNull();
        request.TenantId.ShouldBe("1.2.");
        request.IncludeInternalData.ShouldBeFalse();
        request.UserId.ShouldBeNull();
        request.StartDate.ShouldBe(command.StartDate);
        request.EndDate.ShouldBe(command.EndDate);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public async Task Handler_WithoutTenant_ShouldNotQueueExport(string tenantId)
    {
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.SetupGet(user => user.TenantId).Returns(tenantId);

        var result = await new ExportDeliveryFeedback.Handler(unitOfWork.Object, currentUser.Object)
            .Handle(new ExportDeliveryFeedback.Command
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today
            }, CancellationToken.None);

        result.Succeeded.ShouldBeFalse();
        unitOfWork.VerifyNoOtherCalls();
    }

    [Test]
    public void LegacyRequest_ShouldContinueIncludingInternalData()
    {
        var request = JsonConvert.DeserializeObject<ExportProviderFeedback.ProviderFeedbackExportRequest>(
            "{\"StartDate\":\"2026-10-01\",\"EndDate\":\"2026-10-12\",\"TenantId\":\"1.\"}");

        request.ShouldNotBeNull();
        request.IncludeInternalData.ShouldBeTrue();
    }
}