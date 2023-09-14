using Entities.DTOs;
using Entities.Models;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IReviewService
    {
        Review AddReview(Review review, Guid artworkId);
        Review GetReviewForLike(Guid id, bool trackChanges);
        void RemoveReview(Guid id);
        void RemoveReviews(IEnumerable<Review> reviews);
        ReviewDTO GetReview(Guid id, bool trackChanges);
        IEnumerable<ReviewDTO> GetAllReviews(IEnumerable<string> tagList, RequestParameters requestParameters, bool trackChanges);
        IEnumerable<ReviewDTO> GetHighestMarkedReviews(IEnumerable<string> tagList, RequestParameters requestParameters, bool trackChanges);
        IEnumerable<ReviewDTO> GetConnectedReviews(Guid reviewId, bool trackChanges);
        void LikeReview(Guid id);
        void DislikeReview(Guid id);
        IEnumerable<ReviewDTO> GetUserReviews(Guid userId, bool trackChanges);
    }
}
