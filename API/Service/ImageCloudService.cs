using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Service.IService;
using System;
using System.IO;
using System.Threading.Tasks;

public class S3ImageUploadService : IImageCloudService
{
    private readonly string _bucketName;
    private readonly IAmazonS3 _s3Client;

    public S3ImageUploadService(string bucketName, string awsAccessKey, string awsSecretKey)
    {
        _bucketName = bucketName;
        _s3Client = new AmazonS3Client(awsAccessKey, awsSecretKey, RegionEndpoint.USWest1);
    }

    public async Task<string> UploadImageAsync(IFormFile imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
            throw new ArgumentException("Invalid image file");

        string key = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);

        using (var memoryStream = new MemoryStream())
        {
            await imageFile.CopyToAsync(memoryStream);

            var putObjectRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = key,
                InputStream = memoryStream,
                ContentType = imageFile.ContentType,
                CannedACL = S3CannedACL.PublicRead
            };

            await _s3Client.PutObjectAsync(putObjectRequest);
        }

        string imageUrl = $"https://{_bucketName}.s3.amazonaws.com/{key}";

        return imageUrl;
    }

    public async Task DeleteImagesAsync(IEnumerable<string> imageUrls)
    {
        foreach (var imageUrl in imageUrls)
        {
            string key = imageUrl.Substring(imageUrl.LastIndexOf('/') + 1);

            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(deleteObjectRequest);
        }
    }
}