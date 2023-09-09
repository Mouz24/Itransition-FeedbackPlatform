using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IImageCloudService
    {
        Task<string> UploadImageAsync(IFormFile uploadedFile);
        Task DeleteImagesAsync(IEnumerable<string> imageUrls);
    }
}
