namespace Cfo.Cats.Application.Features.Assessments.PostAssessmentCommand;

public class PostAssessmentCommandHandler() : IRequestHandler<PostAssessmentCommand, bool>
{
    public async Task<bool> Handle(PostAssessmentCommand request, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        foreach (var section in request.assessment.Pathways)
        {

        }

        return true;
    }
}
