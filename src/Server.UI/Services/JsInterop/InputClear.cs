using Microsoft.JSInterop;

namespace Cfo.Cats.Server.UI.Services.JsInterop;

public class InputClear
{
    private readonly IJSRuntime jsRuntime;

    public InputClear(IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime;
    }

    public async Task<ValueTask> Clear(string targetId)
    {
        var jsmodule = await jsRuntime.InvokeAsync<IJSObjectReference>(
            "import",
            "/js/clearinput.js"
        );
        return jsmodule.InvokeVoidAsync(JsInteropConstants.ClearInput, targetId);
    }
}
