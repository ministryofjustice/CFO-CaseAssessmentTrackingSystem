using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Identity.Commands;

public static class SetHomePage
{
    private const int HomePageMaxLength = 50;

    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Command : IRequest<Result>
    {
        public required string HomePage { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = currentUserService.UserId;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result.Failure("User is not authenticated");
            }

            var user = await unitOfWork.DbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
            if (user is null)
            {
                return Result.Failure("User not found");
            }

            user.HomePage = request.HomePage;
            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.HomePage)
                .NotEmpty()
                .MaximumLength(HomePageMaxLength)
                .Must(x => x.StartsWith('/'))
                .WithMessage("Home page must start with '/'");
        }
    }
}