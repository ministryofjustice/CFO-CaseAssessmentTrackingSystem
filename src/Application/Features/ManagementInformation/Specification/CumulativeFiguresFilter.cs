using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Application.Features.ManagementInformation.Specification;

public record CumulativeFiguresFilter(DateOnly EndDate, string? ContractId, UserProfile CurrentUser);
