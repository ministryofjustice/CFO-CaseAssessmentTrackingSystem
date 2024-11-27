using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.Features.Payables.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Payables.Commands;

public static class AddActivity
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result<bool>>
    {
        public required string ParticipantId { get; set; }
        public LocationDto? Location { get; set; }
        public ActivityDto? Activity { get; set; }
    }

    class Handler : IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            // TODO: record activity
            return await Task.FromResult(true);
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

            // Require an induction if the activity took place in a hub
            RuleFor(c => c.Location)
                .MustAsync(async (command, location, token) => await HaveAHubInduction(command.ParticipantId, location!, token))
                .When(c => c.Location is not null && c.Location.LocationType.IsHub)
                .WithMessage("A hub induction is required for the selected location");

            When(c => c.Location is not null, () =>
            {
                RuleFor(c => c.Activity)
                    .NotNull()
                    .WithMessage("You must choose an Activity/ETE");
            });
        }

        async Task<bool> HaveAHubInduction(string participantId, LocationDto location, CancellationToken cancellationToken)
        {
            return await unitOfWork.DbContext.HubInductions.AnyAsync(induction => 
                induction.ParticipantId == participantId && induction.LocationId == location.Id, cancellationToken);
        }
    }
}
