using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Identity.Commands;

/// <summary>
/// Encapsulates the command and handlers for setting a user's home page preference.
/// This allows users to configure which page they navigate to upon login.
/// </summary>
public static class SetHomePage
{
    /// <summary>
    /// Maximum allowed length for the home page path.
    /// </summary>
    private const int HomePageMaxLength = 50;

    /// <summary>
    /// Command to update the current user's home page preference.
    /// Requires authorization via the AuthorizedUser policy.
    /// </summary>
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Command : ICommand<Result>
    {
        /// <summary>
        /// The home page path (must start with '/'). Null or empty to clear the preference.
        /// </summary>
        public string? HomePage { get; set; }
    }

    /// <summary>
    /// Handles the execution of the SetHomePage command.
    /// Updates the authenticated user's home page preference in the database.
    /// </summary>
    public class Handler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : ICommandHandler<Command, Result>
    {
        /// <summary>
        /// Processes the request to set the user's home page.
        /// Validates that the user is authenticated and exists before updating.
        /// </summary>
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

    /// <summary>
    /// Validates the SetHomePage command request.
    /// Ensures the home page conforms to path requirements when provided.
    /// </summary>
    public class Validator : AbstractValidator<Command>
    {
        /// <summary>
        /// Configures validation rules for the home page property.
        /// </summary>
        public Validator()
        {
            RuleFor(x => x.HomePage)
                .MaximumLength(HomePageMaxLength)
                .Must(x => string.IsNullOrWhiteSpace(x) || x.StartsWith('/'))
                .WithMessage("Home page must start with '/' when provided");
        }
    }
}