using AutoMapper;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Service;
using Service.IService;
using System.Data;

namespace FeedbackPlatform.Hubs
{
    [Authorize(Roles = "Administrator, User")]
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

        public async Task LikeReview(Guid reviewId, Guid userId)
        {
            lock(locker)
            {
                if (_serviceManager.LikedReview.IsReviewLikedByUser(userId, reviewId))
                {
                    RemoveLike(reviewId, userId).Wait();

                    return;
                }

                _serviceManager.LikedReview.AddLikedReview(userId, reviewId);
                _serviceManager.Review.LikeReview(reviewId);
                _authenticationManager.AddLikeToUser(userId);
            }

            await _serviceManager.SaveAsync();

            await Clients.All.SendAsync("LikedReview");
        }

        public async Task RemoveLike(Guid reviewId, Guid userId)
        {
            lock(locker)
            {
                _serviceManager.LikedReview.RemoveUserLike(userId, reviewId);
                _serviceManager.Review.DislikeReview(reviewId);
                _authenticationManager.RemoveLikeFromUser(userId);
            }

            await _serviceManager.SaveAsync();

            await Clients.All.SendAsync("DislikedReview");
        }
    }
}
