using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Server.Common.Interfaces;
using Cfo.Cats.Server.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Cfo.Cats.Server.Services;

public class ServerHubWrapper : IApplicationHubWrapper
{
    private readonly IHubContext<ServerHub, ISignalRHub> hubContext;

    public ServerHubWrapper(IHubContext<ServerHub, ISignalRHub> hubContext)
    {
        this.hubContext = hubContext;
    }

    public async Task JobStarted(string message)
    {
        await hubContext.Clients.All.Start(message);
    }

    public async Task JobCompleted(string message)
    {
        await hubContext.Clients.All.Completed(message);
    }
}
