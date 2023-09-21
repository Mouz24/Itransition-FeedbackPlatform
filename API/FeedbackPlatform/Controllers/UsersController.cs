using AutoMapper;
using Castle.Core.Internal;
using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest;
using Service;
using Service.IService;

namespace FeedbackPlatform.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuthenticationManager _authManager;
        private readonly IServiceManager _serviceManager;
        private readonly IElasticClient _client;
        private readonly IMapper _mapper;

        public UsersController(
            UserManager<User> userManager, 
            IAuthenticationManager authManager, 
            IServiceManager serviceManager, 
            IElasticClient client,
            IMapper mapper
            )
        {
            _userManager = userManager;
            _authManager = authManager;
            _serviceManager = serviceManager;
            _client = client;
            _mapper = mapper;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString("D"));

            var userDTO = _mapper.Map<UserDTO>(user);

            return Ok(userDTO);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] Guid loggedInUserId)
        {
            var users = await _userManager.Users
                .Where(user => user.Id != loggedInUserId)
                .ToListAsync();

            var userViewModels = users.Select(user => new
            {
                Id = user.Id,
                Username = user.UserName,
                AvatarUrl = user.Avatar,
                Likes = user.Likes,
                Email = user.Email,
                Role = _userManager.GetRolesAsync(user).Result.SingleOrDefault(),
                RegistrationDate = user.RegistrationDate,
                LastLoginDate = user.LastLoginDate,
                isBlocked = user.isBlocked,
            })
            .ToList();

            return Ok(userViewModels);
        }



        [Authorize(Roles = "Administrator")]
        [HttpPut("block")]
        public async Task<IActionResult> BlockUser([FromBody] Guid id)
        {
            await _authManager.BlockUser(id);

            await _serviceManager.SaveAsync();

            return NoContent();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("unblock")]
        public async Task<IActionResult> UnblockUser([FromBody] Guid id)
        {
            await _authManager.UnblockUser(id);

            await _serviceManager.SaveAsync();

            return NoContent();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("make-admin")]
        public async Task<IActionResult> MakeUserAdmin([FromBody] Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString("D"));
            if (user == null)
            {
                return NotFound("User doesn't exist");
            }

            var removeFromUserRoleResult = await _userManager.RemoveFromRoleAsync(user, "User");
            if (!removeFromUserRoleResult.Succeeded)
            {
                return BadRequest("Failed to remove user from 'User' role");
            }

            var addToAdminRoleResult = await _userManager.AddToRoleAsync(user, "Administrator");
            if (!addToAdminRoleResult.Succeeded)
            {
                return BadRequest("Failed to add user to 'Administrator' role");
            }

            return NoContent();
        }

        [Authorize(Roles = "User, Administrator")]
        [HttpPost("{userId}/avatar")]
        public async Task<IActionResult> UploadAvatar(Guid userId, [FromForm] IFormFile image)
        {
            var imageUrl = await _serviceManager.ImageCloud.UploadImageAsync(image);

            await _authManager.SetAvatar(userId, imageUrl);

            await _serviceManager.SaveAsync();

            return Ok(imageUrl);
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUser([FromBody] IEnumerable<Guid> ids)
        {
            foreach (var id in ids)
            {
                var user = await _userManager.FindByIdAsync(id.ToString("D"));
                if (user == null)
                {
                    return NotFound("User doesn't exist");
                }

                var userReviews = _serviceManager.Review.GetUserReviews(user.Id, true);
                foreach (var review in userReviews)
                {
                    var reviewImages = _serviceManager.ReviewImage.GetReviewImagesUrls(review.Id, false);

                    if (reviewImages.Any())
                    {
                        await _serviceManager.ImageCloud.DeleteImagesAsync(reviewImages);
                    }

                    await _client.DeleteAsync<ReviewDTO>(review.Id.ToString());
                }

                var userComments = _serviceManager.Comment.GetUserComments(user.Id, true);

                _serviceManager.Comment.RemoveComments(userComments);

                if (!user.Avatar.IsNullOrEmpty())
                {
                    await _serviceManager.ImageCloud.DeleteImagesAsync(new List<string> { user.Avatar });
                }

                await _userManager.DeleteAsync(user);
            }

            await _serviceManager.SaveAsync();

            return NoContent();
        }
    }
}
