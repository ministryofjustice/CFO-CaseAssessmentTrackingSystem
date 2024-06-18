namespace Cfo.Cats.Application.Common.Interfaces;

public interface IUploadService
{
    Task<Result<string>> UploadAsync(string folder, UploadRequest request);
}
