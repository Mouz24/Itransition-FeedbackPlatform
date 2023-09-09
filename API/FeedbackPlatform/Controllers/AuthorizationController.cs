using Azure.Core;
using Contracts;
using Entities.DTOs;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.IService;

namespace FeedbackPlatform.Controllers
{
    [ApiController]
    [Route("api")]
    public class AuthorizationController : ControllerBase
    {
        private readonly IAuthenticationManager _authManager;
        private readonly IServiceManager _servicemanager;
        private readonly UserManager<User> _userManager;

        public AuthorizationController(IAuthenticationManager authManager, IServiceManager servicemanager, UserManager<User> userManager)
        {
            _authManager = authManager;
            _servicemanager = servicemanager;
            _userManager = userManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDTO user)
        {
            if (!await _authManager.ValidateUser(user))
            {
                return Unauthorized();
            }

            var userData = await _authManager.GetUser(user.UserName);
            var userRole = await _userManager.GetRolesAsync(userData);
            var accessToken = await _authManager.CreateAccessToken();
            var refreshToken = _authManager.CreateRefreshToken();

            await _authManager.AddRefreshToken(user.UserName, refreshToken);
            await _authManager.UpdateLoginDate(user.UserName);

            await _servicemanager.SaveAsync();

            return Ok(new AuthenticatedResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserId = userData.Id,
                UserName = user.UserName,
                Role = userRole.SingleOrDefault(),
                Avatar = userData.AvatarUrl
            });
        }

        [HttpPost("external-login")]
        public async Task<IActionResult> ExternalLogin([FromBody] ExternalAuthDTO externalAuth)
        {
            var payload = await _authManager.VerifyExternalToken(externalAuth);
            if (payload == null)
            {
                return BadRequest("Invalid External Authentication.");
            }

            var user = await _userManager.FindByEmailAsync(payload.Email);
            if (user == null)
            {
                user = new User { Email = payload.Email, UserName = payload.Email, AvatarUrl = payload.Avatar};

                await _userManager.CreateAsync(user);
                await _userManager.AddToRoleAsync(user, "User");
            }

            if (!await _authManager.ValidateUserForExternalLogin(user.UserName))
            {
                return BadRequest("Can not find such user or the user is blocked");
            }

            var accessToken = await _authManager.CreateAccessToken();
            var refreshToken = _authManager.CreateRefreshToken();

            await _authManager.AddRefreshToken(user.UserName, refreshToken);
            await _authManager.UpdateLoginDate(user.UserName);
            await _authManager.AddUserRegistrationDate(user.UserName);

            var userRole = await _userManager.GetRolesAsync(user);

            await _servicemanager.SaveAsync();

            return Ok(new AuthenticatedResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserId = user.Id,
                UserName = user.UserName,
                Role = userRole.SingleOrDefault(),
                Avatar = user.AvatarUrl
            });
        }
    }
}
