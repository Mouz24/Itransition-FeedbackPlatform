using Entities.DTOs;
using Entities.Models;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Entities.Models.Facebook;
using System.Net.Http.Json;
using Entities.Models.ExternalLogin;

namespace Service
{
    public class AuthenticationManager : IAuthenticationManager
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private User _user;

        public AuthenticationManager(UserManager<User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<string> CreateAccessToken()
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims();
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        public string CreateRefreshToken()
        {
            var randomNumber = new byte[32];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);

                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task<bool> ValidateUser(UserForAuthenticationDTO user)
        {
            _user = await GetUser(user.UserName);

            return (_user != null && await _userManager.CheckPasswordAsync(_user, user.Password) && !_user.isBlocked);
        }

        public async Task<bool> ValidateUserForExternalLogin(string userName)
        {
            _user = await GetUser(userName);

            return (_user != null && !_user.isBlocked);
        }

        public async Task UpdateLoginDate(string userName)
        {
            _user = await GetUser(userName);
            _user.LastLoginDate = DateTime.UtcNow.AddHours(3);
        }

        public async Task AddRefreshToken(string userName, string refreshToken)
        {
            _user = await GetUser(userName);
            _user.RefreshToken = refreshToken;
            _user.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(3).AddDays(7);
        }

        public async Task ReassignRefreshToken(string userName, string refreshToken)
        {
            _user = await GetUser(userName);
            _user.RefreshToken = refreshToken;
        }

        public async Task RevokeRefreshToken(string userName)
        {
            _user = await GetUser(userName);
            _user.RefreshToken = null;
        }

        public async Task<User> GetUser(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task AddLikeToUser(Guid id)
        {
            _user = await _userManager.FindByIdAsync(id.ToString("D"));

            _user.Likes++;

            await _userManager.UpdateAsync(_user);
        }

        public async Task RemoveLikeFromUser(Guid id)
        {
            _user = await _userManager.FindByIdAsync(id.ToString("D"));

            _user.Likes--;

            await _userManager.UpdateAsync(_user);
        }

        public async Task RemoveLikesFromUser(Guid id, int likesNumber)
        {
            _user = await _userManager.FindByIdAsync(id.ToString("D"));

            _user.Likes -= likesNumber;
        }

        public async Task BlockUser(Guid id)
        {
            _user = await _userManager.FindByIdAsync(id.ToString("D"));
            _user.isBlocked = true;
            _user.RefreshToken = null;
        }

        public async Task UnblockUser(Guid id)
        {
            _user = await _userManager.FindByIdAsync(id.ToString("D"));
            _user.isBlocked = false;
        }

        public async Task AddUserRegistrationDate(string userName)
        {
            _user = await GetUser(userName);
            _user.RegistrationDate = DateTime.UtcNow.AddHours(3);
        }

        public async Task<bool> ValidateExpiredTokenClaims(string userName, string refreshToken)
        {
            _user = await GetUser(userName);

            return (_user != null || _user.RefreshToken == refreshToken || !_user.isBlocked || _user.RefreshTokenExpiryTime > DateTime.UtcNow.AddHours(3));
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET"));

            Console.Write(key);

            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, _user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(_user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var tokenOptions = new JwtSecurityToken
            (
            issuer: jwtSettings.GetSection("validIssuer").Value,
            audience: jwtSettings.GetSection("validAudience").Value,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings.GetSection("expires").Value)),
            signingCredentials: signingCredentials
            );

            return tokenOptions;
        }

        public async Task<ExternalLoginPayload> VerifyExternalToken(ExternalAuthDTO externalAuth)
        {
            if (externalAuth.Provider == "Google")
            {
                return await VerifyGoogleToken(externalAuth);
            }
            else if (externalAuth.Provider == "Facebook")
            {
                return await VerifyFacebookToken(externalAuth);
            }
            else
            {
                return null;
            }
        }

        public async Task<ExternalLoginPayload> VerifyGoogleToken(ExternalAuthDTO externalAuth)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>() { _configuration.GetSection("GoogleAuthSettings").GetSection("clientId").Value }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(externalAuth.IdToken, settings);
            
            return new ExternalLoginPayload
            {
                Email = payload.Email,
                Avatar = payload.Picture
            };
        }

        public async Task<ExternalLoginPayload> VerifyFacebookToken(ExternalAuthDTO externalAuth)
        {
            var facebookSetiings = _configuration.GetSection("FacebookAuthSettings");
            var httpClient = new HttpClient();

            var appAccessTokenResponse = await httpClient.GetFromJsonAsync<FacebookAppAccessTokenResponse>(
                $"https://graph.facebook.com/oauth/access_token?client_id={facebookSetiings.GetSection("clientId").Value}&client_secret={facebookSetiings.GetSection("clientSecret").Value}&grant_type=client_credentials");

            var response = await httpClient.GetFromJsonAsync<FacebookTokenValidationResult>(
                $"https://graph.facebook.com/debug_token?input_token={externalAuth.IdToken}&access_token={appAccessTokenResponse!.AccessToken}");

            if (response is null || !response.Data.IsValid)
            {
                return null;
            }

            return new ExternalLoginPayload
            {
                Email = externalAuth.Email,
                Avatar = externalAuth.Avatar
            };
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET")))
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken securityToken;

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        public async Task SetAvatar(Guid id, string imageUrl)
        {
            _user = await _userManager.FindByIdAsync(id.ToString("D"));
            _user.Avatar = imageUrl;
        }
    }
}