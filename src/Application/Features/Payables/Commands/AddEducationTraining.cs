using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Payables.Commands;

public static class AddEducationTraining
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result<bool>>
    {
        [Description("Course Title")]
        public string? CourseTitle { get; set; }

        [Description("Course Hyperlink")]
        public string? CourseHyperlink { get; set; }

        [Description("Course Level")]
        public string? CourseLevel { get; set; }

        [Description("Course Commenced Date")]
        public DateTime? CourseCommencedDate { get; set; }

        [Description("Course Completed Date")]
        public DateTime? CourseCompletedDate { get; set; }

        [Description("Passed")]
        public string? Passed { get; set; }
    }

    class Handler: IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            // TODO: record activity
            return await Task.FromResult(true);
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        readonly IUnitOfWork unitOfWork;
        public Validator(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;

            RuleFor(c => c.CourseTitle)
                .NotNull()
                .MaximumLength(100)
                .WithMessage("You must enter a Course Title");
            
            RuleFor(c => c.CourseLevel)
                .NotNull()
                .WithMessage("You must choose a Course Level");

            RuleFor(c => c.CourseCommencedDate)
                .NotNull()
                .WithMessage("You must enter Course Commenced Date"); 
            
            RuleFor(c => c.Passed)
                .NotNull()
                .WithMessage("You must choose a value for Passed");

        }

    }
}