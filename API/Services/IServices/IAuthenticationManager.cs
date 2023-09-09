using Entities.DTOs;
using Entities.Models;
using Entities.Models.ExternalLogin;
using Entities.Models.Facebook;
using Google.Apis.Auth;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IAuthenticationManager
    {
        Task<bool> ValidateUser(UserForAuthenticationDTO user);
        Task<bool> ValidateUserForExternalLogin(string userName);
        Task<bool> ValidateExpiredTokenClaims(string userName, string refreshToken);
        Task<string> CreateAccessToken();
        string CreateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        Task ReassignRefreshToken(string userName, string refreshToken);
        Task AddRefreshToken(string userName, string refreshToken);
        Task RevokeRefreshToken(string userName);
        Task UpdateLoginDate(string userName);
        Task AddUserRegistrationDate(string userName);
        Task<User> GetUser(string userName);
        Task<ExternalLoginPayload> VerifyExternalToken(ExternalAuthDTO externalAuth);
        Task</*GoogleJsonWebSignature.Payload*/ExternalLoginPayload> VerifyGoogleToken(ExternalAuthDTO externalAuth);
        Task</*FacebookPayload*/ExternalLoginPayload> VerifyFacebookToken(ExternalAuthDTO externalAuth);
        Task AddLikeToUser(Guid id);
        Task RemoveLikeFromUser(Guid id);
        Task RemoveLikesFromUser(Guid id, int likesNumber);
        Task BlockUser(Guid id);
        Task UnblockUser(Guid id);
        Task SetAvatar(Guid id, string imageUrl);
    }
}
