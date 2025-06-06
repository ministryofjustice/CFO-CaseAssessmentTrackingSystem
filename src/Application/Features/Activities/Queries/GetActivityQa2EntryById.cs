﻿using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Activities.Queries
{
    public static class GetActivityQa2EntryById
    {
        [RequestAuthorize(Policy = SecurityPolicies.Qa2)]
        public class Query : IRequest<Result<ActivityQueueEntryDto>>
        {
            public Guid Id { get; set; }
            public UserProfile? CurrentUser { get; set; }
        }

        public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<ActivityQueueEntryDto>>
        {
            public async Task<Result<ActivityQueueEntryDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var entry = await unitOfWork.DbContext.ActivityQa2Queue
                    .Where(a => a.Id == request.Id && a.TenantId.StartsWith(request.CurrentUser!.TenantId!)) 
                    .ProjectTo<ActivityQueueEntryDto>(mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(cancellationToken);

                if (entry == null)
                {
                    //question: do we return a specific error if the entry exists but is at a different tenant?
                    return Result<ActivityQueueEntryDto>.Failure("Not found");
                }

                return entry;
            }
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(r => r.Id)
                    .NotEmpty()
                    .WithMessage(string.Format(ValidationConstants.GuidMessage, "Id"));
            }
        }
    }
}