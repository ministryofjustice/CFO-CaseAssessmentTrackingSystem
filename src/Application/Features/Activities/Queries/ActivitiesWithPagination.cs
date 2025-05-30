﻿using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.Features.Activities.Specifications;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Activities;

namespace Cfo.Cats.Application.Features.Activities.Queries;

public static class ActivitiesWithPagination
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Query : ActivitiesAdvancedFilter, IRequest<PaginatedData<ActivitySummaryDto>>
    {
        public ActivitiesAdvancedSpecification Specification => new(this);
    }

    class Handler(IUnitOfWork unitOfWork, IMapper mapper) 
        : IRequestHandler<Query, PaginatedData<ActivitySummaryDto>>
    {
        public async Task<PaginatedData<ActivitySummaryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await unitOfWork.DbContext.Activities
                .Include(a => a.TookPlaceAtLocation)
                .OrderByDescending(a => a.Status == ActivityStatus.PendingStatus.Value) // push "Pending" activities to top
                .ThenBy($"{request.OrderBy} {request.SortDirection}")
                .ProjectToPaginatedDataAsync<Activity, ActivitySummaryDto>(request.Specification, request.PageNumber, request.PageSize, mapper.ConfigurationProvider, cancellationToken);
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.ParticipantId)
                .NotNull();

            RuleFor(x => x.ParticipantId)
                .Length(9)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));
        }
    }
}