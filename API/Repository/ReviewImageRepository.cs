using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class ReviewImageRepository : RepositoryBase<ReviewImage>, IReviewImageRepository
    {
        public ReviewImageRepository(ApplicationContext applicationContext) :
            base(applicationContext)
        { }

        public void AddReviewImage(Guid reviewid, string imageUrl)
        {
            var reviewImage = new ReviewImage
            {
                ReviewId = reviewid,
                ImageUrl = imageUrl
            };

            Create(reviewImage);
        }

        public IEnumerable<ReviewImage> GetReviewImages(Guid reviewId, bool trackChanges) =>
            FindByCondition(reviewImage => reviewImage.ReviewId == reviewId, trackChanges)
            .ToList();

        public IEnumerable<string> GetReviewImagesUrls(Guid reviewId, bool trackChanges) =>
            FindByCondition(reviewImage => reviewImage.ReviewId == reviewId, trackChanges).
            Select(reviewImage => reviewImage.ImageUrl)
            .ToList();

        public void RemoveReviewImage(ReviewImage reviewImage)
        {
            Delete(reviewImage);
        }

        public void RemoveReviewImages(Guid reviewId)
        {
            var reviewImages = GetReviewImages(reviewId, false);

            DeleteAll(reviewImages);
        }
    }
}
