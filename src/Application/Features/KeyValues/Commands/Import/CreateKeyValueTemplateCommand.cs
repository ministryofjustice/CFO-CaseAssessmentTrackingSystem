using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Application.Features.KeyValues.Commands.Import;

[RequestAuthorize(Roles = "Admin, Basic")]
public record CreateKeyValueTemplateCommand : IRequest<byte[]>
{
}