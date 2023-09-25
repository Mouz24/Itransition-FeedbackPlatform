using Contracts;
using Entities.DTOs;
using Entities.Models;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class LikedReviewsService : ILikedReviewService
    {
        private readonly ILikedReviewRepository _likesRepository;

        public LikedReviewsService(ILikedReviewRepository likesRepository)
        {
            _likesRepository = likesRepository;
        }

        public void AddLikedReview(Guid userId, Guid reviewId)
        {
            _likesRepository.AddLikedReview(userId, reviewId);
        }

        public bool IsReviewLikedByUser(Guid userId, Guid reviewId)
        {
            return _likesRepository.IsReviewLikedByUser(userId, reviewId);
        }

        public void MarkLikedReview(Guid userId, ReviewDTO review)
        {
            _likesRepository.MarkLikedReview(userId, review);
        }

        public void RemoveUserLike(Guid userId, Guid reviewId)
        {
            _likesRepository.RemoveUserLike(userId, reviewId);
        }

        public void RemoveUserLikedReviews(User user)
        {
            _likesRepository.RemoveUserLikedReviews(user);
        }
    }
}
