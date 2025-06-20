using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.Features.PRIs.DTOs;
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
                .Create(request.ParticipantId, DateOnly.FromDateTime(request.Release.ExpectedOn!.Value), request.Release.ExpectedRegion!.Id, currentUserService.UserId!, request.Release.CustodyLocation!.Id)
                .WithMeeting(DateOnly.FromDateTime(request.Meeting.AttendedOn!.Value), 
                    reasonCustodyDidNotAttendInPerson: request.Meeting.ReasonCustodyDidNotAttendInPerson, 
                    reasonCommunityDidNotAttendInPerson: request.Meeting.ReasonCommunityDidNotAttendInPerson,
                    reasonParticipantDidNotAttendInPerson: request.Meeting.ReasonParticipantDidNotAttendInPerson,
                    postReleaseCommunitySupportInformation: request.Meeting.PostReleaseCommunitySupportInformation);

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
        readonly IUnitOfWork unitOfWork;
        readonly ICurrentUserService currentUserService;
        readonly ILocationService locationService;

        public Validator(
            IUnitOfWork unitOfWork, 
            ICurrentUserService currentUserService,
            ILocationService locationService,
            IValidator<PriCodeDto> priCodeValidator, 
            IValidator<PriReleaseDto> priReleaseValidator, 
            IValidator<PriMeetingDto> priMeetingValidator)
        {
            this.unitOfWork = unitOfWork;
            this.currentUserService = currentUserService;
            this.locationService = locationService;

            RuleFor(c => c.ParticipantId)
                .MustAsync(NotAlreadyHavePRI)
                .WithMessage("A PRI has already been created for this participant")
                .DependentRules(() =>
                {
                    RuleFor(c => c.Code).SetValidator(priCodeValidator);
                    RuleFor(c => c.Release).SetValidator(priReleaseValidator);
                    RuleFor(c => c.Meeting).SetValidator(priMeetingValidator);

                    RuleFor(c => c)
                        .MustAsync(BeAuthorised)
                        .WithMessage("Community Support Worker does not have access to the expected release region");
                })
                .MustAsync(MustNotBeArchived)
                .WithMessage("Participant is archived");
            
        }

        private async Task<bool> NotAlreadyHavePRI(string participantId, CancellationToken cancellationToken)
            => await unitOfWork.DbContext.PRIs.AnyAsync(p => p.ParticipantId == participantId, cancellationToken) is false;

        private async Task<bool> BeAuthorised(Command command, CancellationToken cancellationToken)
        {
            string? assigneeTenantId;

            if(command.Code.SelfAssign)
            {
                assigneeTenantId = currentUserService.TenantId;
            }
            else
            {
                var createdBy = await unitOfWork.DbContext.PriCodes
                    .Where(p => p.ParticipantId == command.ParticipantId && p.Code == command.Code.Value)
                    .Select(p => p.CreatedBy)
                    .SingleAsync(cancellationToken);

                var tenantId = await unitOfWork.DbContext.Users.Where(u => u.Id == createdBy)
                    .Select(u => u.TenantId)
                    .FirstOrDefaultAsync(cancellationToken);

                assigneeTenantId = tenantId;
            }

            return locationService
                .GetVisibleLocations(assigneeTenantId!)
                .Select(l => l.Id)
                .Contains(command.Release.ExpectedRegion!.Id);
        }

        private async Task<bool> MustNotBeArchived(string participantId, CancellationToken cancellationToken)
            => await unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == participantId && e.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value, cancellationToken);
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
        public LocationDto? CustodyLocation { get; set; }
        public LocationDto? ExpectedRegion { get; set; }
        public DateTime? ExpectedOn { get; set; }

        public class Validator : AbstractValidator<PriReleaseDto>
        {
            public Validator()
            {
                RuleFor(c => c.CustodyLocation)
                    .NotNull()
                    .WithMessage("You must choose a discharge location");

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

        private class Mapper : Profile
        {
            public Mapper()
            {
                CreateMap<PRIDto, PriReleaseDto>(MemberList.None)
                 .ForMember(target => target.ExpectedRegion,
                    options => options.MapFrom(source => source.ExpectedReleaseRegion))
                 .ForMember(target => target.ExpectedOn,
                    options => options.MapFrom(source => source.ExpectedReleaseDate.ToDateTime(TimeOnly.MinValue)));
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
        public string? PostReleaseCommunitySupportInformation { get; set; }

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

        private class Mapper : Profile
        {
            public Mapper()
            {
                CreateMap<PRIDto, PriMeetingDto>(MemberList.None)
                 .ForMember(target => target.AttendedOn,
                    options => options.MapFrom(source => source.MeetingAttendedOn.ToDateTime(TimeOnly.MinValue)))
                 .ForMember(target => target.CustodyAttendedInPerson,
                    options => options.MapFrom(source => (source.CustodyAttendedInPerson ? ConfirmationStatus.Yes : ConfirmationStatus.No)))
                 .ForMember(target => target.CommunityAttendedInPerson,
                    options => options.MapFrom(source => (source.CommunityAttendedInPerson ? ConfirmationStatus.Yes : ConfirmationStatus.No)))
                 .ForMember(target => target.ParticipantAttendedInPerson,
                    options => options.MapFrom(source => (source.ParticipantAttendedInPerson ? ConfirmationStatus.Yes : ConfirmationStatus.No)));
            }
        }
    }
}