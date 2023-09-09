using Contracts;
using Entities.Models;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ReviewImageService : IReviewImageService
    {
        private readonly IReviewImageRepository _reviewImageRepository;

        public ReviewImageService(IReviewImageRepository reviewImageRepository)
        {
            _reviewImageRepository = reviewImageRepository;
        }

        public void AddReviewImage(Guid reviewid, string imageUrl)
        {
            _reviewImageRepository.AddReviewImage(reviewid, imageUrl);
        }

        public IEnumerable<ReviewImage> GetReviewImages(Guid reviewid, bool trackChanges)
        {
            return _reviewImageRepository.GetReviewImages(reviewid, trackChanges);
        }

        public IEnumerable<string> GetReviewImagesUrls(Guid reviewid, bool trackChanges)
        {
            return _reviewImageRepository.GetReviewImagesUrls(reviewid, trackChanges);
        }

        public void RemoveReviewImage(ReviewImage reviewImage)
        {
            _reviewImageRepository.RemoveReviewImage(reviewImage);
        }

        public void RemoveReviewImages(Guid reviewId)
        {
            _reviewImageRepository.RemoveReviewImages(reviewId);
        }
    }
}
