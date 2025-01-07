using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Inductions;

namespace Cfo.Cats.Application.Features.Inductions.Commands;

public static class AddWingInduction
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result>
    {
        [Description("The current user")]
        public UserProfile? CurrentUser { get; set; } = default!;

        [Description("Participant Id")]
        public string ParticipantId { get; set; } = default!;

        [Description("Date of Induction")]
        public DateTime? InductionDate { get;set; }

        [Description("Induction Location")]
        public LocationDto? Location { get; set; } 
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var induction = WingInduction.Create(request.ParticipantId, request.Location!.Id, request.InductionDate!.Value, request.CurrentUser!.UserId);
            await unitOfWork.DbContext.WingInductions.AddAsync(induction, cancellationToken);
            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocationService _locationService;

        public Validator(IUnitOfWork unitOfWork, ILocationService locationService)
        {
            this._unitOfWork = unitOfWork;
            this._locationService = locationService;

            RuleFor(x => x.ParticipantId)
               .NotNull()
               .MaximumLength(9)
               .MinimumLength(9)
               .Matches(ValidationConstants.AlphaNumeric)
               .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"))
               .MustAsync(MustExist)
               .WithMessage("Participant does not exist.");


            RuleFor(x => x.InductionDate)
                .NotNull()
                .GreaterThanOrEqualTo(DateTime.Today.AddMonths(-3))
                .WithMessage("Cannot backdate beyond 3 months")
                .LessThan(DateTime.Today.AddDays(1).Date)
                .WithMessage("Cannot be in the future");

            RuleFor(x => x)
                .MustAsync(BeOnOrAfterConsentDate)
                .WithMessage("Induction must be on or after the consent date");

            RuleFor(x => x.Location)
                .NotNull();

            RuleFor(x => x.CurrentUser)
                .NotNull();

        }

        private async Task<bool> MustExist(string identifier, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == identifier, cancellationToken);

        private async Task<bool> BeOnOrAfterConsentDate(Command command, CancellationToken cancellationToken)
        {
            var consentDate = await _unitOfWork.DbContext
                                .Participants.Where(x => x.Id == command.ParticipantId)
                                .Select(c => c.Consents.Max(d => d.Lifetime.StartDate))
                                .FirstAsync(cancellationToken);

            return command.InductionDate >= consentDate;

        }

    }
}