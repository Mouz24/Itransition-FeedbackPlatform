using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class LikedReviewsRepository : RepositoryBase<LikedReview>, ILikedReviewRepository
    {
        public LikedReviewsRepository(ApplicationContext applicationContext) :
            base(applicationContext) { }

        public void AddLikedReview(Guid userId, Guid reviewId)
        {
            var likedReview = new LikedReview
            {
                UserId = userId,
                ReviewId = reviewId
            };

            Create(likedReview);
        }

        public bool IsReviewLikedByUser(Guid userId, Guid reviewId)
        {
            var likedReviews = FindAll(false).ToList();
            var isLikedByUser = likedReviews.Any(like => like.UserId == userId && like.ReviewId == reviewId);

            return isLikedByUser;
        }

        public void MarkLikedReview(Guid userId, ReviewDTO review)
        {    
            review.IsLikedByUser = IsReviewLikedByUser(userId, review.Id);   
        }

        public void RemoveUserLike(Guid userId, Guid reviewId)
        {
            var likedReview = FindByCondition(like => like.UserId == userId && like.ReviewId == reviewId, true).FirstOrDefault();

            Delete(likedReview);
        }
    }
}
