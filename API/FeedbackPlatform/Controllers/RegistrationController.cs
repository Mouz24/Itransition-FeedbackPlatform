using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities.Models;
using FeedbackPlatform.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.IService;

namespace FeedbackPlatform.Controllers
{
    [Route("api")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IAuthenticationManager _authManager;
        private readonly IServiceManager _serviceManager;

        public RegistrationController(UserManager<User> userManager, IAuthenticationManager authManager, IServiceManager serviceManager, IMapper mapper)
        {
            _userManager = userManager;
            _authManager = authManager;
            _serviceManager = serviceManager;
            _mapper = mapper;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDTO userForRegistration)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var user = _mapper.Map<User>(userForRegistration);

            var result = await _userManager.CreateAsync(user, userForRegistration.Password);
            if (!result.Succeeded && result.Errors.Count() > 0)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }

                return UnprocessableEntity(ModelState);
            }

            await _authManager.AddUserRegistrationDate(user.UserName);
            await _userManager.AddToRolesAsync(user, userForRegistration.Role);

            await _serviceManager.SaveAsync();

            return StatusCode(201);
        }
    }
}
