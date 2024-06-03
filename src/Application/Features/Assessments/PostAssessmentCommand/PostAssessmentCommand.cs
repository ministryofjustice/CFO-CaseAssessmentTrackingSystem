using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Assessments.DTOs;

namespace Cfo.Cats.Application.Features.Assessments.PostAssessmentCommand;

[RequestAuthorize(Roles = "Admin, Basic")]
public record PostAssessmentCommand(AssessmentDto assessment) : IRequest<bool>;
