namespace Cfo.Cats.Application.Common.Models;

public class UploadRequest
{
    public UploadRequest(string fileName, UploadType uploadType, byte[] data)
    {
        FileName = fileName;
        UploadType = uploadType;
        Data = data;
    }

    public string FileName { get; set; }
    public string? Extension { get; set; }
    public UploadType UploadType { get; set; }
    public byte[] Data { get; set; }
}
