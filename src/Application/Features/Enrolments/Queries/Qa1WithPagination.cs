﻿using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Enrolments.DTOs;
using Cfo.Cats.Application.Features.Enrolments.Specifications;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Enrolments.Queries;

public static class Qa1WithPagination
{
    [RequestAuthorize(Roles = $"{RoleNames.QAOfficer}, {RoleNames.QAManager}, {RoleNames.QASupportManager}, {RoleNames.SMT}, {RoleNames.SystemSupport}")]
    public class Query : QueueEntryFilter, IRequest<PaginatedData<EnrolmentQueueEntryDto>>
    {
        public Query()
        {
            SortDirection = "Desc";
            OrderBy = "Created";
        }
        public EnrolmentQa1QueueEntrySpecification Specification => new(this);
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, PaginatedData<EnrolmentQueueEntryDto>>
    {
        public async Task<PaginatedData<EnrolmentQueueEntryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var data = await unitOfWork.DbContext.EnrolmentQa1Queue.OrderBy($"{request.OrderBy} {request.SortDirection}")
                .ProjectToPaginatedDataAsync<EnrolmentQa1QueueEntry, EnrolmentQueueEntryDto>(request.Specification, request.PageNumber, request.PageSize, mapper.ConfigurationProvider, cancellationToken);

            return data;
        }
    }
    
}