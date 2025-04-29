using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cfo.Cats.Application.Features.Assessments.Commands;

public static class DeleteAssessment
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Command : IRequest<Result<int>>
    {
        public required Guid AssessmentId { get; set; }
    }

    class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result<int>>
    {
        public async Task<Result<int>> Handle(Command request, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.DbContext.ParticipantAssessments
                .Where(pcd => pcd.Id == request.AssessmentId)
                .ExecuteDeleteAsync(cancellationToken);

            return Result<int>.Success(result);
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private IUnitOfWork _unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.AssessmentId)
                .Must(Exist);
        }

        bool Exist(Guid identifier) => _unitOfWork.DbContext.ParticipantAssessments.Any(asmt => asmt.Id == identifier);
    }

}
