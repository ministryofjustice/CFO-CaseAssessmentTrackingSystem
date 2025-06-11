using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public static class AddOrUpdateContactDetail
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Command : IRequest<Result>
    {
        public Guid? Id { get; set; }
        public required string ParticipantId { get; set; }
        public string Description { get; set; } = string.Empty;
        public ParticipantAddressDto? AddressDetails { get; set; }
        public string? MobileNumber { get; set; }
        public string? EmailAddress { get; set; }
        public bool Primary { get; set; }
    }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Command, ParticipantContactDetail>(MemberList.None)
                .ConstructUsing(command => ParticipantContactDetail.Create(
                    command.ParticipantId,
                    command.Description,
                    command.AddressDetails != null ? command.AddressDetails.Address : null,
                    command.AddressDetails != null ? command.AddressDetails.PostCode : null,
                    command.AddressDetails != null ? command.AddressDetails.UPRN : null,
                    command.MobileNumber,
                    command.EmailAddress)
                )
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Id))
                .AfterMap((command, destination) =>
                {
                    // Clear domain events if entity exists
                    if (command.Id.HasValue)
                    {
                        destination.ClearDomainEvents();
                    }
                });

            CreateMap<ParticipantContactDetailDto, Command>()
                .ForMember(opt => opt.AddressDetails, dest => dest.MapFrom(src => new ParticipantAddressDto
                {
                    Address = src.Address,
                    PostCode = src.PostCode,
                    UPRN = src.UPRN
                }));
        }
    }
    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            bool update = request.Id.HasValue;

            var contactDetail = mapper.Map<ParticipantContactDetail>(request);

            if (request.Primary)
            {
                await unitOfWork.DbContext.ParticipantContactDetails
                    .Where(pcd => pcd.ParticipantId == request.ParticipantId)
                    .ExecuteUpdateAsync(prop => prop.SetProperty(pcd => pcd.Primary, false), cancellationToken);

                contactDetail.SetPrimary(request.Primary);
            }

            if (update)
            {
                unitOfWork.DbContext.ParticipantContactDetails.Update(contactDetail);
            }
            else
            {
                await unitOfWork.DbContext.ParticipantContactDetails.AddAsync(contactDetail, cancellationToken);
            }

            return Result.Success();
        }
    }

    public class A_BeValid : AbstractValidator<Command>
    {
        public A_BeValid()
        {
            RuleFor(c => c.Description)
                .NotEmpty()
                .WithMessage("Must not be empty")
                .MaximumLength(100)
                .WithMessage("Maximum length exceeded");

            RuleFor(c => c)
                .Must(model => model.EmailAddress is not null || model.MobileNumber is not null);

            When(c => string.IsNullOrEmpty(c.EmailAddress) is false, () =>
            {
                RuleFor(c => c.EmailAddress)
                    .EmailAddress()
                    .MaximumLength(256)
                    .WithMessage("Maximum length exceeded");
            });

            When(c => string.IsNullOrEmpty(c.MobileNumber) is false, () =>
            {
                RuleFor(c => c.MobileNumber)
                    .MaximumLength(16)
                    .WithMessage("Maximum length exceeded")
                    .Matches(ValidationConstants.Numbers)
                    .WithMessage(string.Format(ValidationConstants.NumbersMessage, "Mobile number"));
            });

            RuleFor(x => x)
                        .Custom((model, context) =>
                        {
                            if (string.IsNullOrWhiteSpace(model.AddressDetails?.Address) &&
                                string.IsNullOrWhiteSpace(model.EmailAddress) &&
                                string.IsNullOrWhiteSpace(model.MobileNumber))
                            {
                                context.AddFailure("You must provide at least one of the following: Address, Email Address, or Phone Number.");
                            }
                        });
        }
    }

    public class B_ParticipantMustExist : AbstractValidator<Command>
    {
        private IUnitOfWork _unitOfWork;

        public B_ParticipantMustExist(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.ParticipantId)
                .Must(Exist);
        }

        bool Exist(string identifier) => _unitOfWork.DbContext.Participants.Any(e => e.Id == identifier);
    }

    public class C_ContactDetailMustExist : AbstractValidator<Command>
    {
        private IUnitOfWork _unitOfWork;

        public C_ContactDetailMustExist(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            When(c => c.Id is not null, () =>
            {
                RuleFor(c => c.Id)
                    .Must(Exist);
            });
        }

        bool Exist(Guid? identifier) => _unitOfWork.DbContext.ParticipantContactDetails.Any(e => e.Id == identifier);
    }

    public class D_ParticipantMustBeActive : AbstractValidator<Command>
    {
        private IUnitOfWork _unitOfWork;

        public D_ParticipantMustBeActive(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.ParticipantId)
                .Must(MustNotBeArchived)
                 .WithMessage("Participant is archived"); 
        }

        bool MustNotBeArchived(string participantId)
                => _unitOfWork.DbContext.Participants.Any(e => e.Id == participantId && e.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value);
    }
}