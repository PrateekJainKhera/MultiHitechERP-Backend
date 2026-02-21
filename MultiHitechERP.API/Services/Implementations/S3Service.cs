using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private readonly string _region;

        public S3Service(IConfiguration configuration)
        {
            var accessKey  = configuration["AWS:AccessKey"];
            var secretKey  = configuration["AWS:SecretKey"];
            _bucketName    = configuration["AWS:BucketName"]!;
            _region        = configuration["AWS:Region"]!;

            _s3Client = new AmazonS3Client(
                accessKey,
                secretKey,
                RegionEndpoint.GetBySystemName(_region));
        }

        public async Task<string> UploadAsync(Stream fileStream, string s3Key, string contentType)
        {
            var request = new PutObjectRequest
            {
                BucketName  = _bucketName,
                Key         = s3Key,
                InputStream = fileStream,
                ContentType = contentType
            };

            await _s3Client.PutObjectAsync(request);

            return $"https://{_bucketName}.s3.{_region}.amazonaws.com/{s3Key}";
        }

        public async Task DeleteAsync(string s3Key)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key        = s3Key
            };

            await _s3Client.DeleteObjectAsync(request);
        }
    }
}
