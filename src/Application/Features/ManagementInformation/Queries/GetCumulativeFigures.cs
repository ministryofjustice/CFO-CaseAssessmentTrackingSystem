using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.ManagementInformation.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.ManagementInformation.Queries;

public static class GetCumulativeFigures
{
    [RequestAuthorize(Roles = $"{RoleNames.SystemSupport}, {RoleNames.Finance}")]
    public class Query : IRequest<Result<CumulativeFiguresDto[]>>
    {
        public required DateOnly EndDate { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<CumulativeFiguresDto[]>>
    {
        public async Task<Result<CumulativeFiguresDto[]>> Handle(Query request, CancellationToken cancellationToken)
        {
            DateOnly startDate = new DateOnly(2025, 1, 1);
            DateOnly endDate = request.EndDate;

            var results = await unitOfWork.DbContext.Database
                .SqlQuery<CumulativeFiguresDto>($"SELECT * FROM mi.GetCumulativeTotals({startDate}, {endDate})")
                .ToArrayAsync(cancellationToken);

            return results;
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(s => s.EndDate)
                .GreaterThan(new DateOnly(2024, 12, 31))
                .WithMessage("End date cannot before 01 January 2025");
        }
    }
}