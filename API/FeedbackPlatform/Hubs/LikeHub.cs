using AutoMapper;
using Entities;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Service;
using Service.IService;
using System.Data;

namespace FeedbackPlatform.Hubs
{
    public class LikeHub : Hub
    {
        private readonly IServiceManager _serviceManager;
        private readonly IAuthenticationManager _authenticationManager;
        private object locker = new();

        public LikeHub(IServiceManager serviceManager, IAuthenticationManager authenticationManager)
        {
            _serviceManager = serviceManager;
            _authenticationManager = authenticationManager;
        }

        public async Task LikeReview(Guid reviewId, Guid userId, Guid loggedInUserId)
        {
            if (_serviceManager.LikedReview.IsReviewLikedByUser(loggedInUserId, reviewId))
            {
                await RemoveLike(reviewId, userId, loggedInUserId);
            }
            else
            {
                _serviceManager.LikedReview.AddLikedReview(loggedInUserId, reviewId);

                var review = _serviceManager.Review.GetReviewEntity(reviewId, true);

                _serviceManager.Review.LikeReview(review.Id);

                await _authenticationManager.AddLikeToUser(userId);

                await _serviceManager.SaveAsync();
            }

            await Clients.All.SendAsync("LikedReview");
        }

        public async Task RemoveLike(Guid reviewId, Guid userId, Guid loggedInUserId)
        {   
            _serviceManager.LikedReview.RemoveUserLike(loggedInUserId, reviewId);

            var review = _serviceManager.Review.GetReviewEntity(reviewId, true);

            _serviceManager.Review.DislikeReview(review.Id);

            await _authenticationManager.RemoveLikeFromUser(userId);

            await _serviceManager.SaveAsync();
        }
    }
}
