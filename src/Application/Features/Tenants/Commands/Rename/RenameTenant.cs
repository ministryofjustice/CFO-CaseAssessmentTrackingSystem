using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Tenants.Caching;
using Cfo.Cats.Application.Features.Tenants.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Tenants.Commands.Rename;

public static class RenameTenant
{
    [RequestAuthorize(Policy = SecurityPolicies.SystemFunctionsWrite)]
    public class Command  : IRequest<Result<string>>
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        
        private class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<TenantDto, Command>(MemberList.None);
            }
        }
        
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result<string>>
    {
        public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
        {
            var tenant = await unitOfWork.DbContext.Tenants.FindAsync(request.Id);
            if (tenant == null)
            {
                throw new NotFoundException("Tenant", request.Id);
            }
            tenant.Rename(request.Name);
            return tenant.Id;   
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(r => r.Id)
                .NotNull()
                .NotEmpty()
                .Matches(ValidationConstants.TenantId).WithMessage(ValidationConstants.TenantIdMessage);

            RuleFor(v => v.Name)
                .MaximumLength(50)
                .Matches(ValidationConstants.LettersSpacesUnderscores)
                .WithMessage(string.Format(ValidationConstants.LettersSpacesUnderscoresMessage, "Tenant"))
                .NotEmpty();
        }
    }

}
