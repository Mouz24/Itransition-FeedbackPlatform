using AutoMapper;
using Entities.DTOs;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Nest;
using Service.IService;

namespace FeedbackPlatform.Hubs
{
    public class CommentHub : Hub
    {
        private readonly IMapper _mapper;
        private readonly IServiceManager _serviceManager;
        private readonly IElasticClient _client;

        public CommentHub(IMapper mapper, IServiceManager serviceManager, IElasticClient client)
        {
            _mapper = mapper;
            _serviceManager = serviceManager;
            _client = client;
        }

        public async Task LeaveComment(CommentToLeaveDTO commentDTO)
        {
            if (commentDTO.Text.IsNullOrEmpty() || commentDTO.UserId == Guid.Empty || commentDTO.ReviewId == Guid.Empty)
            {
                return;
            }

            var comment = _mapper.Map<Comment>(commentDTO);

            _serviceManager.Comment.AddComment(comment);

            await _serviceManager.SaveAsync();

            var reviewDTO = _serviceManager.Review.GetReview(commentDTO.ReviewId, true);

            await _client.DeleteAsync<ReviewDTO>(reviewDTO.Id.ToString());
            await _client.IndexDocumentAsync(reviewDTO);

            await Clients.All.SendAsync("ReceiveComment");
        }

        public async Task RemoveComment(Guid id, Guid reviewId)
        {
            _serviceManager.Comment.RemoveComment(id);

            await _serviceManager.SaveAsync();

            var reviewDTO = _serviceManager.Review.GetReview(reviewId, true);

            await _client.DeleteAsync<ReviewDTO>(reviewDTO.Id.ToString());
            await _client.IndexDocumentAsync(reviewDTO);

            await Clients.All.SendAsync("RemoveComment");
        }
    }
}
