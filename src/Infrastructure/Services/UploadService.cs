using Amazon.S3;
using Amazon.S3.Model;
using Cfo.Cats.Application.Common.Extensions;
using Microsoft.Extensions.Configuration;

namespace Cfo.Cats.Infrastructure.Services;

public class UploadService : IUploadService
{
    private readonly string _bucketName;
    private readonly IAmazonS3 _client;
    
    public UploadService(IConfiguration configuration, IAmazonS3 client)
    {
        _bucketName = configuration.GetValue<string>("AWS:Bucket") ?? throw new Exception("Missing configuration details");
        _client = client;
    }

    public async Task<string> UploadAsync(UploadRequest uploadRequest)
    {
        string key = $"{Guid.NewGuid()}";

        using var stream = new MemoryStream(uploadRequest.Data);

        var putRequest = new PutObjectRequest()
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = stream
        };

        await _client.PutObjectAsync(putRequest);
        return key;
    }

}