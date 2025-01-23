using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.PRIs;

namespace Cfo.Cats.Application.Features.PRIs.Commands;

public static class AddPRI
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result>
    {
        public required string ParticipantId { get; set; }
        public PriCodeDto Code { get; set; } = new();
        public PriReleaseDto Release { get; set; } = new();
        public PriMeetingDto Meeting { get; set; } = new();
    }

    class Handler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var pri = PRI
                .Create(request.ParticipantId, DateOnly.FromDateTime(request.Release.ExpectedOn!.Value), request.Release.ExpectedRegion!.Id)
                .WithMeeting(DateOnly.FromDateTime(request.Meeting.AttendedOn!.Value), request.Meeting.AttendedInPerson, request.Meeting.NotAttendedInPersonJustification);

            if(request.Code.Value is { Length: > 0 })
            {
                //await unitOfWork.DbContext.PRICodes.FirstOrDefaultAsync(p => p.Code == request.Code && p.ParticipantId == request.PRI.ParticipantID);
                //pri.AssignTo(...);
            }
            else if(request.Code.SelfAssign)
            {
                pri.AssignTo(currentUserService.UserId!);
            }

            await unitOfWork.DbContext.PRIs.AddAsync(pri, cancellationToken);

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Code).SetValidator(new PriCodeDto.Validator());
            RuleFor(c => c.Release).SetValidator(new PriReleaseDto.Validator());
            RuleFor(c => c.Meeting).SetValidator(new PriMeetingDto.Validator());
        }
    }

    public class PriCodeDto
    {
        public string? Value { get; set; }
        public bool SelfAssign { get; set; }

        public class Validator : AbstractValidator<PriCodeDto>
        {
            public Validator()
            {
                When(c => c.Value is { Length: > 0 }, () =>
                {
                    RuleFor(c => c.Value)
                        .Length(6)
                        .WithMessage("Invalid format for code");
                })
                .Otherwise(() =>
                {
                    RuleFor(c => c.SelfAssign)
                        .Equal(true)
                        .WithMessage("You must self-assign when no PRI code is provided");
                });
            }
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
                    .WithMessage(ValidationConstants.DateMustBeInFuture);
            }
        }
    }

    public class PriMeetingDto
    {
        public DateTime? AttendedOn { get; set; }
        public bool AttendedInPerson { get; set; } = true;
        public string? NotAttendedInPersonJustification { get; set; }

        public class Validator : AbstractValidator<PriMeetingDto>
        {
            public Validator()
            {
                RuleFor(c => c.AttendedOn)
                    .NotNull()
                    .WithMessage("You must provide the meeting date")
                    .LessThanOrEqualTo(DateTime.Today)
                    .WithMessage(ValidationConstants.DateMustBeInPast);

                RuleFor(c => c.AttendedInPerson)
                    .NotNull()
                    .WithMessage("You must choose");

                RuleFor(c => c.NotAttendedInPersonJustification)
                    .NotEmpty()
                    .When(c => c.AttendedInPerson is false)
                    .WithMessage("You must provide a justification");
            }
        }
    }
}
