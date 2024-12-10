using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Payables;

namespace Cfo.Cats.Application.Features.Payables.Commands;

public static class AddActivity
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result>
    {
        public required string ParticipantId { get; set; }
        public required Guid TaskId { get; set; }
        public LocationDto? Location { get; set; }

        [Description("Activity/ETE")]
        public ActivityDefinition? ActivityDefinition { get; set; }

        [Description("Completed on")]
        public DateTime? Completed { get; set; }

        [Description("Additional Information")]
        public string? AdditionalInformation { get; set; }
    }

    class Handler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var participant = await unitOfWork.DbContext.Participants
                .Include(p => p.CurrentLocation)
                .ThenInclude(l => l.Contract)
                .FirstOrDefaultAsync(p => p.Id == request.ParticipantId, cancellationToken);

            if(participant is null)
            {
                return Result.Failure("Participant not found");
            }

            var location = await unitOfWork.DbContext.Locations
                .Include(l => l.Contract)
                .FirstOrDefaultAsync(l => l.Id == request.Location!.Id, cancellationToken);

            if(location is not { Contract: not null })
            {
                return Result.Failure("Activities cannot be recorded against the chosen location");
            }

            var task = await unitOfWork.DbContext.PathwayPlans
                .SelectMany(p => p.Objectives)
                .SelectMany(o => o.Tasks.Where(task => task.Id == request.TaskId))
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if(task is null)
            {
                return Result.Failure("Task not found");
            }

            var cxt = new Activity.ActivityContext(
                Definition: request.ActivityDefinition!,
                ParticipantId: participant.Id,
                Task: task,
                TookPlaceAtLocation: location,
                TookPlaceAtContract: location.Contract,
                ParticipantCurrentLocation: participant.CurrentLocation,
                ParticipantCurrentContract: participant.CurrentLocation.Contract,
                ParticipantStatus: participant.EnrolmentStatus!,
                Completed: request.Completed!.Value,
                currentUserService.TenantId!,
                AdditionalInformation: request.AdditionalInformation);

            var activity = NonISWActivity.Create(cxt);

            await unitOfWork.DbContext.NonISWActivities.AddAsync(activity, cancellationToken);

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        readonly IUnitOfWork unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;

            RuleFor(c => c.ParticipantId)
                .NotNull()
                .Length(9);

            RuleFor(c => c.Location)
                .NotNull()
                .WithMessage("You must choose a location");

            RuleFor(c => c.Location)
                .MustAsync(async (command, location, token) => await HaveAHubInduction(command.ParticipantId, location!, token))
                .When(c => c.Location is { LocationType.IsHub: true })
                .WithMessage("A hub induction is required for the selected location");

            When(c => c.Location is not null, () =>
            {
                RuleFor(c => c.ActivityDefinition)
                    .NotNull()
                    .WithMessage("You must choose an Activity/ETE");
            });

            RuleFor(c => c.Completed)
                .NotNull()
                .WithMessage("You must provide a date of completion")
                .Must(NotBeCompletedInTheFuture)
                .WithMessage("Completion date cannot be in the future");

            RuleFor(c => c.AdditionalInformation)
                .MaximumLength(ValidationConstants.NotesLength)
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Additional Information"));
        }

        async Task<bool> HaveAHubInduction(string participantId, LocationDto location, CancellationToken cancellationToken)
        {
            return await unitOfWork.DbContext.HubInductions.AnyAsync(induction => 
                induction.ParticipantId == participantId && induction.LocationId == location.Id, cancellationToken);
        }

        bool NotBeCompletedInTheFuture(DateTime? completed) => completed < DateTime.UtcNow;
    }
}
