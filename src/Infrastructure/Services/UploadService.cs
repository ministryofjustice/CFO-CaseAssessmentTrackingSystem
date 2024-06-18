using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Cfo.Cats.Application.Common.Extensions;
using Microsoft.Extensions.Configuration;

namespace Cfo.Cats.Infrastructure.Services;

public class UploadService : IUploadService
{
    private readonly string _bucketName;
    private readonly IAmazonS3 _client;
    private readonly ILogger<UploadService> _logger;

    public UploadService(IConfiguration configuration, IAmazonS3 client, ILogger<UploadService> logger)
    {
        _bucketName = configuration.GetValue<string>("AWS:Bucket") ?? throw new Exception("Missing configuration details");
        _client = client;
        _logger = logger;
    }

    public async Task<Result<string>> UploadAsync(string folder, UploadRequest uploadRequest)
    {
        var scopeInfo = CreateScopeInformation(folder, uploadRequest);

        using (_logger.BeginScope(scopeInfo))
        {
            try
            {
                if (folder.EndsWith("/"))
                {
                    _logger.LogWarning("Attempt to upload a document with a forward slash in the folder");
                    return await Result<string>.FailureAsync("Folder should not end in forward slash");
                }
                
                string key = $"{folder}/{Guid.NewGuid()}";

                using var stream = new MemoryStream(uploadRequest.Data);
         
                var putRequest = new PutObjectRequest()
                {
                    BucketName = _bucketName,
                    Key = key,
                    InputStream = stream
                };

                _logger.LogDebug("Uploading to S3 bucket");
                var result =  await _client.PutObjectAsync(putRequest);
                if (result.HttpStatusCode == HttpStatusCode.OK)
                {
                    return putRequest.Key;
                }
                return await Result<string>.FailureAsync(result.HttpStatusCode.ToString());
            }
            catch (AmazonS3Exception s3Ex)
            {
                _logger.LogError(s3Ex, $"Error uploading file" );
                return await Result<string>.FailureAsync(s3Ex.Message);
            }
        }
    }

    private static Dictionary<string, object> CreateScopeInformation(string folder, UploadRequest uploadRequest)
    {
        var scopeInfo = new Dictionary<string, object>()
        {
            {
                "OperationId", Guid.NewGuid()
            },
            {
                "Folder", folder
            },
            {
                "FileName", uploadRequest.FileName
            }
        };
        return scopeInfo;
    }

}