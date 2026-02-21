namespace MultiHitechERP.API.Services.Interfaces
{
    public interface IS3Service
    {
        /// <summary>Upload a file stream to S3 and return the public URL.</summary>
        Task<string> UploadAsync(Stream fileStream, string s3Key, string contentType);

        /// <summary>Delete a file from S3 by its key.</summary>
        Task DeleteAsync(string s3Key);
    }
}
