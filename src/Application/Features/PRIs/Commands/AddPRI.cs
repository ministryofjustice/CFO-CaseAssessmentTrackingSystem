using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.PRIs;

namespace Cfo.Cats.Application.Features.PRIs.Commands;

public static class AddPRI
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public record Command(string ParticipantId) : IRequest<Result>
    {
        public PriCodeDto Code { get; set; } = new() { ParticipantId = ParticipantId };
        public PriReleaseDto Release { get; set; } = new();
        public PriMeetingDto Meeting { get; set; } = new();
    }

    class Handler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var pri = PRI
                .Create(request.ParticipantId, DateOnly.FromDateTime(request.Release.ExpectedOn!.Value), request.Release.ExpectedRegion!.Id)
                .WithMeeting(DateOnly.FromDateTime(request.Meeting.AttendedOn!.Value), 
                    reasonCustodyDidNotAttendInPerson: request.Meeting.ReasonCustodyDidNotAttendInPerson, 
                    reasonCommunityDidNotAttendInPerson: request.Meeting.ReasonCommunityDidNotAttendInPerson,
                    reasonParticipantDidNotAttendInPerson: request.Meeting.ReasonParticipantDidNotAttendInPerson);

            string? assignee = null;

            if(request.Code.Value is not null)
            {
                assignee = await unitOfWork.DbContext.PriCodes
                    .Where(p => p.Code == request.Code.Value && p.ParticipantId == request.ParticipantId)
                    .Select(p => p.CreatedBy)
                    .SingleAsync(cancellationToken);
            }
            else if(request.Code.SelfAssign)
            {
                assignee = currentUserService.UserId;
            }

            pri.AssignTo(assignee);

            await unitOfWork.DbContext.PRIs.AddAsync(pri, cancellationToken);

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator(IValidator<PriCodeDto> priCodeValidator, IValidator<PriReleaseDto> priReleaseValidator, IValidator<PriMeetingDto> priMeetingValidator)
        {
            RuleFor(c => c.Code).SetValidator(priCodeValidator);
            RuleFor(c => c.Release).SetValidator(priReleaseValidator);
            RuleFor(c => c.Meeting).SetValidator(priMeetingValidator);
        }
    }

    public class PriCodeDto
    {
        public int? Value { get; set; }
        public bool SelfAssign { get; set; }
        public bool IsSelfAssignmentAllowed { get; set; }
        public required string ParticipantId { get; set; }

        public class Validator : AbstractValidator<PriCodeDto>
        {
            readonly IUnitOfWork unitOfWork;

            public Validator(IUnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;

                RuleFor(c => c.ParticipantId)
                    .NotNull();

                When(c => c.Value is not null, () =>
                {
                    RuleFor(c => c.Value)
                        .InclusiveBetween(100_000, 999_999)
                        .WithMessage("Invalid format for code")
                        .DependentRules(() =>
                        {
                            RuleFor(c => c.Value)
                                .MustAsync(async (command, code, context, canc) => await BeValid(command.ParticipantId, code!.Value, canc))
                                .WithMessage("Invalid code");
                        });
                });

                When(c => c.IsSelfAssignmentAllowed, () =>
                {
                    RuleFor(c => c.SelfAssign)
                        .Equal(true)
                        .When(c => c.Value is not not null)
                        .WithMessage("You must self-assign when no PRI code is provided");
                })
                .Otherwise(() =>
                {
                    // Self assignment is not allowed
                    RuleFor(c => c.Value)
                        .NotEmpty()
                        .WithMessage("You must provide a code");
                });
            }

            async Task<bool> BeValid(string participantId, int code, CancellationToken cancellationToken)
                => await unitOfWork.DbContext.PriCodes.AnyAsync(pc => pc.Code == code && pc.ParticipantId == participantId, cancellationToken);
        }
    }

    public class PriReleaseDto
    {
        public LocationDto? ExpectedRegion { get; set; }
        public DateTime? ExpectedOn { get; set; }

        public class Validator : AbstractValidator<PriReleaseDto>
        {
            public Validator()
            {
                RuleFor(c => c.ExpectedRegion)
                    .NotNull()
                    .WithMessage("You must choose an expected release region");

                RuleFor(c => c.ExpectedOn)
                    .NotNull()
                    .WithMessage("You must provide an expected date of release")
                    .GreaterThanOrEqualTo(DateTime.Today)
                    .WithMessage(ValidationConstants.DateMustBeInFuture)
                    .LessThanOrEqualTo(DateTime.Today.AddMonths(3))
                    .WithMessage("Expected date of release must not be more than three months in the future");
            }
        }
    }

    public class PriMeetingDto
    {
        public DateTime? AttendedOn { get; set; }
        public ConfirmationStatus? CustodyAttendedInPerson { get; set; }
        public ConfirmationStatus? CommunityAttendedInPerson { get; set; }
        public ConfirmationStatus? ParticipantAttendedInPerson { get; set; }
        public string? ReasonCustodyDidNotAttendInPerson { get; set; }
        public string? ReasonCommunityDidNotAttendInPerson { get; set; }
        public string? ReasonParticipantDidNotAttendInPerson { get; set; }

        public class Validator : AbstractValidator<PriMeetingDto>
        {
            public Validator()
            {
                RuleFor(c => c.AttendedOn)
                    .NotNull()
                    .WithMessage("You must provide the meeting date")
                    .LessThanOrEqualTo(DateTime.Today)
                    .WithMessage(ValidationConstants.DateMustBeInPast);

                RuleFor(c => c.CustodyAttendedInPerson)
                    .NotNull()
                    .WithMessage("You must choose");

                RuleFor(c => c.CommunityAttendedInPerson)
                    .NotNull()
                    .WithMessage("You must choose");

                RuleFor(c => c.ParticipantAttendedInPerson)
                    .NotNull()
                    .WithMessage("You must choose");

                RuleFor(c => c.ReasonCustodyDidNotAttendInPerson)
                    .NotEmpty()
                    .When(c => c.CustodyAttendedInPerson == ConfirmationStatus.No)
                    .WithMessage("You must provide a reason");

                RuleFor(c => c.ReasonCommunityDidNotAttendInPerson)
                    .NotEmpty()
                    .When(c => c.CommunityAttendedInPerson == ConfirmationStatus.No)
                    .WithMessage("You must provide a reason");

                RuleFor(c => c.ReasonParticipantDidNotAttendInPerson)
                    .NotEmpty()
                    .When(c => c.ParticipantAttendedInPerson == ConfirmationStatus.No)
                    .WithMessage("You must provide a reason");
            }
        }
    }
}
