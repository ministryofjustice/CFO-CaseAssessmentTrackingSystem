using Cfo.Cats.Domain.Entities.Documents;

namespace Cfo.Cats.Application.Common.Interfaces;

public interface IUploadService
{
    Task<Result<string>> UploadAsync(string folder, UploadRequest request);
    Task<Result<Stream>> DownloadAsync(Document document);
}
