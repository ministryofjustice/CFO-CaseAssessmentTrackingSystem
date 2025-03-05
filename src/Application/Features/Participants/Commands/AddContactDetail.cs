using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public static class AddContactDetail
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Command : IRequest<Result>
    {
        public required string ParticipantId { get; set; }
        public string Description { get; set; } = string.Empty;
        public ParticipantAddressDto Address = new();
        public string? MobileNumber { get; set; }
        public string? EmailAddress { get; set; }
        public bool Primary { get; set; }
    }

    class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var contactDetail = ParticipantContactDetail.Create(
                request.ParticipantId, 
                request.Description, 
                request.Address.Address,
                request.Address.PostCode,
                request.Address.UPRN,
                request.MobileNumber,
                request.EmailAddress);

            if(request.Primary)
            {
                await unitOfWork.DbContext.ParticipantContactDetails
                    .Where(pcd => pcd.ParticipantId == request.ParticipantId)
                    .ExecuteUpdateAsync(prop => prop.SetProperty(pcd => pcd.Primary, false), cancellationToken);

                contactDetail.SetPrimary(request.Primary);
            }

            await unitOfWork.DbContext.ParticipantContactDetails.AddAsync(contactDetail, cancellationToken);

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private IUnitOfWork _unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.ParticipantId)
                .Must(Exist);

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
            RuleFor(c => c.Address)
                .Must(dto => string.IsNullOrEmpty(dto?.Address) is false)
                .WithMessage("You must choose a valid address");
        }

        bool Exist(string identifier) => _unitOfWork.DbContext.Participants.Any(e => e.Id == identifier);
    }

}
