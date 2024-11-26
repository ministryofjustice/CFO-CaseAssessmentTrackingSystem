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
        public LocationDto? Location { get; set; }
        public ActivityDto? Activity { get; set; }
    }

    class Handler : IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(true);
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Location)
                .NotNull()
                .WithMessage("You must choose a location");

            When(c => c.Location is not null, () =>
            {
                RuleFor(c => c.Activity)
                    .NotNull()
                    .WithMessage("You must choose an Activity/ETE");
            });
        }
    }
}
