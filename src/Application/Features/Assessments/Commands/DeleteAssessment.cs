using Cfo.Cats.Application.Common.Interfaces;
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
            var assessment = await unitOfWork.DbContext.ParticipantAssessments.Where(x => x.Id == request.AssessmentId).FirstOrDefaultAsync(cancellationToken);
            int result = 0;
            if (assessment is not null) {
                unitOfWork.DbContext.ParticipantAssessments.Remove(assessment);
                result = await unitOfWork.SaveChangesAsync(cancellationToken);
            }
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
                .Must(ExistAndIncomplete);

        }

        bool ExistAndIncomplete(Guid identifier) => _unitOfWork.DbContext.ParticipantAssessments.Any(asmt => asmt.Id == identifier && asmt.Completed.HasValue==false);
    }

}
