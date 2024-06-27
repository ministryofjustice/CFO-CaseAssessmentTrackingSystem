using Cfo.Cats.Application.Common.Interfaces.Identity;

namespace Cfo.Cats.Server.UI.Fluxor;

public class Effects
{
    private readonly IIdentityService identityService;

    public Effects(IIdentityService identityService)
    {
        this.identityService = identityService;
    }

    [EffectMethod]
    public async Task HandleFetchDataAction(FetchUserDtoAction action, IDispatcher dispatcher)
    {
        var result = await identityService.GetApplicationUserDto(action.UserName);
        dispatcher.Dispatch(new FetchUserDtoResultAction(result!));
    }
}
