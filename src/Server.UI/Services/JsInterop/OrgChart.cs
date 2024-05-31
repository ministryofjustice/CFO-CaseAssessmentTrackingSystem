using Microsoft.JSInterop;

namespace Cfo.Cats.Server.UI.Services.JsInterop;

public class OrgChart
{
    private readonly IJSRuntime jsRuntime;

    public OrgChart(IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime;
    }

    public async Task<ValueTask> Create(List<OrgItem> data)
    {
        var jsmodule = await jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/orgchart.js");
        return jsmodule.InvokeVoidAsync("createOrgChart", data);
    }
}
