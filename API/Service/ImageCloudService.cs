using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Service.IService;
using System;
using System.IO;
using System.Security.Principal;
using System.Threading.Tasks;

public class CloudinaryImageUploadService : IImageCloudService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryImageUploadService(string cloudName, string apiKey, string apiSecret)
    {
        Account cloudinaryAccount = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(cloudinaryAccount);
    }

    public async Task<string> UploadImageAsync(IFormFile imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
            throw new ArgumentException("Invalid image file");

        using (var stream = imageFile.OpenReadStream())
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(imageFile.FileName, stream),
                PublicId = Guid.NewGuid().ToString(),
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            return uploadResult.SecureUri.AbsoluteUri;
        }
    }

    public async Task DeleteImagesAsync(IEnumerable<string> imageUrls)
    {
        foreach (var imageUrl in imageUrls)
        {
            var publicId = Path.GetFileNameWithoutExtension(new Uri(imageUrl).Segments.Last());

            var deleteParams = new DeletionParams(publicId);
            await _cloudinary.DestroyAsync(deleteParams);
        }
    }
}