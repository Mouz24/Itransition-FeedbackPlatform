using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IReviewImageService
    {
        void AddReviewImage(Guid reviewid, string imageUrl);
        IEnumerable<string> GetReviewImagesUrls(Guid reviewid, bool trackChanges);
        IEnumerable<ReviewImage> GetReviewImages(Guid reviewId, bool trackChanges);
        void RemoveReviewImage(ReviewImage reviewImage);
        void RemoveReviewImages(Guid reviewId);
    }
}
