using Contracts;
using Entities;
using Entities.Models;
using Microsoft.AspNetCore.Http;
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

        public IEnumerable<IFormFile> GetNewImages(Review review, IEnumerable<IFormFile> imageFiles)
        {
            var existingImageUrls = review.ReviewImages.Select(ri => ri.ImageUrl).ToList();

            var newImages = imageFiles.Where(file =>
                !existingImageUrls.Contains(file.FileName, StringComparer.OrdinalIgnoreCase));

            return newImages;
        }

        public IEnumerable<string> GetRemovedImages(Review review, IEnumerable<IFormFile> imageFiles)
        {
            var existingImageUrls = review.ReviewImages.Select(ri => ri.ImageUrl).ToList();

            var removedImages = existingImageUrls
                .Where(url => !imageFiles.Any(file =>
                    string.Equals(file.FileName, url, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            return removedImages;
        }

        public IEnumerable<ReviewImage> GetReviewImages(Guid reviewId, bool trackChanges) =>
            FindByCondition(reviewImage => reviewImage.ReviewId == reviewId, trackChanges)
            .ToList();

        public IEnumerable<string> GetReviewImagesUrls(Guid reviewId, bool trackChanges) =>
            FindByCondition(reviewImage => reviewImage.ReviewId == reviewId, trackChanges).
            Select(reviewImage => reviewImage.ImageUrl)
            .ToList();

        public void RemoveReviewImage(Review review, string imageUrl)
        {
            var image = review.ReviewImages.Where(ri => ri.ImageUrl == imageUrl).SingleOrDefault();

            Delete(image);
        }

        public void RemoveReviewImages(Guid reviewId)
        {
            var reviewImages = GetReviewImages(reviewId, false);

            DeleteAll(reviewImages);
        }
    }
}
