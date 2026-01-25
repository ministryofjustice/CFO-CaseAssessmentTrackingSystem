#nullable enable
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Labels.Commands;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Common.Exceptions;
using Cfo.Cats.Domain.Labels;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Labels.Commands;

public class DeleteLabelCommandTests
{
    private TestLabelRepository _repository = null!;
    private TestLabelCounter _labelCounter = null!;
    private DeleteLabel.Handler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _repository = new TestLabelRepository();
        _labelCounter = new TestLabelCounter();
        _handler = new DeleteLabel.Handler(_repository, _labelCounter);
    }

    [Test]
    public async Task Handle_WithValidCommand_ShouldDeleteLabel()
    {
        var label = Label.Create(
            "ToDelete",
            "Description",
            AppColour.Primary,
            AppVariant.Filled,
            AppIcon.Label,
            "CONTRACT-001",
            new TestLabelCounter());

        _repository.SetExistingLabel(label);
        _labelCounter.SetParticipantCount(0);

        var command = new DeleteLabel.Command
        {
            UserProfile = CreateInternalUserProfile(),
            LabelId = label.Id
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
    }

    [Test]
    public async Task Handle_WithNonExistentLabel_ShouldReturnFailure()
    {
        var command = new DeleteLabel.Command
        {
            UserProfile = CreateInternalUserProfile(),
            LabelId = new LabelId(Guid.NewGuid())
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.ShouldBeFalse();
        result.Errors.ShouldContain("Label not found.");
    }

    [Test]
    public void Handle_AsExternalUserOnGlobalLabel_ShouldThrowBusinessRuleException()
    {
        var globalLabel = Label.Create(
            "Global",
            "Description",
            AppColour.Primary,
            AppVariant.Filled,
            AppIcon.Label,
            null,
            new TestLabelCounter());

        _repository.SetExistingLabel(globalLabel);
        _labelCounter.SetParticipantCount(0);

        var command = new DeleteLabel.Command
        {
            UserProfile = CreateExternalUserProfile(),
            LabelId = globalLabel.Id
        };

        Should.Throw<BusinessRuleValidationException>(async () =>
            await _handler.Handle(command, CancellationToken.None))
            .Message.ShouldContain("You do not have permission to perform this action");
    }

    [Test]
    public async Task Handle_AsInternalUserOnGlobalLabel_ShouldSucceed()
    {
        var globalLabel = Label.Create(
            "Global",
            "Description",
            AppColour.Primary,
            AppVariant.Filled,
            AppIcon.Label,
            null,
            new TestLabelCounter());

        _repository.SetExistingLabel(globalLabel);
        _labelCounter.SetParticipantCount(0);

        var command = new DeleteLabel.Command
        {
            UserProfile = CreateInternalUserProfile(),
            LabelId = globalLabel.Id
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
    }

    [Test]
    public void Handle_WithLinkedParticipants_ShouldThrowBusinessRuleException()
    {
        var label = Label.Create(
            "LinkedLabel",
            "Description",
            AppColour.Primary,
            AppVariant.Filled,
            AppIcon.Label,
            "CONTRACT-001",
            new TestLabelCounter());

        _repository.SetExistingLabel(label);
        _labelCounter.SetParticipantCount(5);

        var command = new DeleteLabel.Command
        {
            UserProfile = CreateInternalUserProfile(),
            LabelId = label.Id
        };

        Should.Throw<BusinessRuleValidationException>(async () =>
            await _handler.Handle(command, CancellationToken.None))
            .Message.ShouldContain("Label cannot be deleted because there are participants linked to it");
    }

    [Test]
    public async Task Handle_AsExternalUserOnContractLabel_ShouldSucceed()
    {
        var contractLabel = Label.Create(
            "Contract",
            "Description",
            AppColour.Primary,
            AppVariant.Filled,
            AppIcon.Label,
            "CONTRACT-001",
            new TestLabelCounter());

        _repository.SetExistingLabel(contractLabel);
        _labelCounter.SetParticipantCount(0);

        var command = new DeleteLabel.Command
        {
            UserProfile = CreateExternalUserProfile(),
            LabelId = contractLabel.Id
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
    }

    private static UserProfile CreateInternalUserProfile()
    {
        return new UserProfile
        {
            UserId = Guid.NewGuid().ToString(),
            UserName = "internal.user",
            Email = "internal.user@justice.gov.uk",
            TenantId = "1."
        };
    }

    private static UserProfile CreateExternalUserProfile()
    {
        return new UserProfile
        {
            UserId = Guid.NewGuid().ToString(),
            UserName = "external.user",
            Email = "external.user@example.com",
            TenantId = "1."
        };
    }

    private class TestLabelRepository : ILabelRepository
    {
        private Label? _existingLabel;

        public void SetExistingLabel(Label label) => _existingLabel = label;

        public Task AddAsync(Label label) => Task.CompletedTask;

        public Task<Label?> GetByIdAsync(LabelId labelId)
        {
            return Task.FromResult(_existingLabel != null && _existingLabel.Id == labelId ? _existingLabel : null);
        }

        public Task<int> CountParticipants(LabelId labelId) => Task.FromResult(0);
    }

    private class TestLabelCounter : ILabelCounter
    {
        private int _participantCount;

        public void SetParticipantCount(int count) => _participantCount = count;

        public int CountVisibleLabels(string name, string? contractId) => 0;
        public int CountParticipants(LabelId labelId) => _participantCount;
    }
}
