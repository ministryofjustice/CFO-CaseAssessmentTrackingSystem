using Cfo.Cats.Application.Features.Assessments.DTOs;

namespace Cfo.Cats.Application.Features.Assessments.PostAssessmentCommand;

public record PostAssessmentCommand(AssessmentDto assessment) : IRequest<bool>;
