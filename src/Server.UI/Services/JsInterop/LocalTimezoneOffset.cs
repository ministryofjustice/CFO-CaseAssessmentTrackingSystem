using Microsoft.JSInterop;

namespace Cfo.Cats.Server.UI.Services.JsInterop;

public class LocalTimezoneOffset
{
    private readonly IJSRuntime jsRuntime;

    public LocalTimezoneOffset(IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime;
    }

    public async ValueTask<int> Hours()
    {
        var jsmodule = await jsRuntime.InvokeAsync<IJSObjectReference>(
            "import",
            "/js/timezoneoffset.js"
        );
        return await jsmodule.InvokeAsync<int>(JsInteropConstants.GetTimezoneOffset);
    }
}
