﻿using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.IService;

namespace FeedbackPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IAuthenticationManager _authManager;
        private readonly IMapper _mapper;
        private readonly IServiceManager _serviceManager;

        public TokenController(IAuthenticationManager authManager, IMapper mapper, IServiceManager serviceManager)
        {
            _authManager = authManager;
            _mapper = mapper;
            _serviceManager = serviceManager;
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(TokensDTO tokensDTO)
        {
            if (tokensDTO is null)
            {
                return BadRequest("TokensDTO object is null");
            }

            var accessToken = tokensDTO.AccessToken;
            var refreshToken = tokensDTO.RefreshToken;

            var username = _authManager.GetPrincipalFromExpiredToken(accessToken).Identity.Name;
            if (!await _authManager.ValidateExpiredTokenClaims(username, refreshToken))
            {
                return BadRequest("Invalid client request");
            }

            string newAccessToken = await _authManager.CreateAccessToken();
            string newRefreshToken = _authManager.CreateRefreshToken();

            await _authManager.ReassignRefreshToken(username, newRefreshToken);

            await _serviceManager.SaveAsync();

            return Ok(new AuthenticatedResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke(AccessTokenDTO Token)
        {
            var username = _authManager.GetPrincipalFromExpiredToken(Token.AccessToken).Identity.Name;

            await _authManager.RevokeRefreshToken(username);

            await _serviceManager.SaveAsync();

            return NoContent();
        }
    }
}
