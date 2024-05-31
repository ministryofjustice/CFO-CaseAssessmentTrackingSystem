using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Cfo.Cats.Server.UI.Components.Shared.FileUpload;

public partial class CatsFileUpload : ComponentBase
{
    [Parameter]
    public string UploadText { get; set; } = "Upload";

    [Parameter, EditorRequired]
    public required string Accept { get; set; }

    [Parameter]
    public EventCallback<int> FilesChanged { get; set; }

    public bool Loading { get; private set; }

    public Dictionary<Guid, IBrowserFile> Files { get; private set; } = [];

    private async Task OnFilesChanged(IReadOnlyList<IBrowserFile> files)
    {
        Loading = true;

        foreach(var file in files)
        {
            throw new NotImplementedException();
            //var result = await DocumentsService!.UploadFileAsync(file);
            //Files.Add(result, file);
            //await FilesChanged.InvokeAsync(Files.Count);
        }

        Loading = false;

        StateHasChanged();
    }

    public async Task OnFileDelete(MudChip chip)
    {
        Loading = true;

        var fileId = GetChipValue(chip);

        if(fileId != Guid.Empty)
        {
            throw new NotImplementedException();
            /*await DocumentsService!.DeleteFileAsync(fileId);
            Files.Remove(fileId);
            await FilesChanged.InvokeAsync(Files.Count);*/
        }

        Loading = false;
    }

    private static Guid GetChipValue(MudChip chip)
    {
        string? value = chip.Value.ToString();

        if (value is not null)
        {
            return Guid.Parse(value);
        }

        return Guid.Empty;
    }

}