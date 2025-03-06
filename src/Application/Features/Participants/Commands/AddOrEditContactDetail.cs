using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public static class AddOrEditContactDetail
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Command : IRequest<Result>
    {
        public Guid? Id { get; set; }
        public required string ParticipantId { get; set; }
        public string Description { get; set; } = string.Empty;
        public ParticipantAddressDto AddressDetails { get; set; } = new();
        public string? MobileNumber { get; set; }
        public string? EmailAddress { get; set; }
        public bool Primary { get; set; }
    }

    class Mapping : Profile 
    { 
        public Mapping()
        {
            CreateMap<Command, ParticipantContactDetail>(MemberList.None)
                .ConstructUsing(command => ParticipantContactDetail.Create(
                    command.ParticipantId,
                    command.Description,
                    command.AddressDetails.Address,
                    command.AddressDetails.PostCode,
                    command.AddressDetails.UPRN,
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
                //.ForAllMembers(opt => opt.Ignore());

            CreateMap<ParticipantContactDetailDto, Command>()
                .ForMember(opt => opt.AddressDetails, dest => dest.MapFrom(src => new ParticipantAddressDto 
                { 
                    Address = src.Address,
                    PostCode = src.PostCode,
                    UPRN = src.UPRN
                }));
        }
    }


    class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Command, Result>
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
                .WithMessage("Maximum length of 100 characters exceeded");

            RuleFor(c => c.EmailAddress)
                .EmailAddress()
                .MaximumLength(256)
                .WithMessage("Maximum length of 100 characters exceeded");

            RuleFor(c => c.MobileNumber)
                .MaximumLength(16)
                .WithMessage("Maximum length of 16 characters exceeded");

            // Populated fields
            RuleFor(c => c.AddressDetails)
                .Must(dto => string.IsNullOrEmpty(dto?.Address) is false)
                .WithMessage("You must choose a valid address");
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

}
