using AutoMapper;
using Entities.DTOs;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Service.IService;

namespace FeedbackPlatform.Hubs
{
    public class CommentHub : Hub
    {
        private readonly IMapper _mapper;
        private readonly IServiceManager _serviceManager;

        public CommentHub(IMapper mapper, IServiceManager serviceManager)
        {
            _mapper = mapper;
            _serviceManager = serviceManager;
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

            await Clients.All.SendAsync("ReceiveComment", comment);
        }

        public async Task RemoveComment(Guid id)
        {
            _serviceManager.Comment.RemoveComment(id);
            await _serviceManager.SaveAsync();

            await Clients.All.SendAsync("RemoveComment");
        }
    }
}
