using Contracts;
using Entities.DTOs;
using Entities.Models;
using Entities.RequestFeatures;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public Review AddReview(Review review, Guid artworkId)
        {
            return _reviewRepository.AddReview(review, artworkId);
        }

        public IEnumerable<ReviewDTO> GetAllReviews(IEnumerable<string> tagList, RequestParameters requestParameters, bool trackChanges)
        {
            return _reviewRepository.GetAllReviews(tagList, requestParameters, trackChanges);
        }

        public IEnumerable<ReviewDTO> GetConnectedReviews(Guid reviewId, bool trackChanges)
        {
            return _reviewRepository.GetConnectedReviews(reviewId, trackChanges);
        }

        public IEnumerable<ReviewDTO> GetHighestMarkedReviews(IEnumerable<string> tagList, RequestParameters requestParameters, bool trackChanges)
        {
            return _reviewRepository.GetHighestMarkedReviews(tagList, requestParameters, trackChanges);
        }

        public ReviewDTO GetReview(Guid id, bool trackChanges)
        {
            return _reviewRepository.GetReview(id, trackChanges);
        }

        public IEnumerable<ReviewDTO> GetUserReviews(Guid userId, bool trackChanges)
        {
            return _reviewRepository.GetUserReviews(userId, trackChanges);
        }

        public void LikeReview(Guid id)
        {
            _reviewRepository.LikeReview(id);
        }

        public void DislikeReview(Guid id)
        {
            _reviewRepository.DislikeReview(id);
        }

        public void RemoveReview(Guid id)
        {
            _reviewRepository.RemoveReview(id);
        }

        public void RemoveReviews(IEnumerable<Review> reviews)
        {
            _reviewRepository.RemoveReviews(reviews);
        }

        public Review GetReviewForLike(Guid id, bool trackChanges)
        {
            return _reviewRepository.GetReviewForLike(id, trackChanges);
        }
    }
}
