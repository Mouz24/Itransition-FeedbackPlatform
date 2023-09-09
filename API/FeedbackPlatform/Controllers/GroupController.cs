using AutoMapper;
using Entities.DTOs;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.IService;

namespace FeedbackPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        private readonly IMapper _mapper;

        public GroupController(IServiceManager serviceManager, IMapper mapper)
        {
            _serviceManager = serviceManager;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetGroups()
        {
            var groups = _serviceManager.Group.GetAllGroups(false);

            return Ok(groups);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost(Name = "AddGroup")]
        public async Task<IActionResult> AddGroup([FromBody]GroupToAddDTO groupToAdd)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }
                
            var group = _mapper.Map<Group>(groupToAdd);

            _serviceManager.Group.AddGroup(group);

            await _serviceManager.SaveAsync();

            return CreatedAtRoute("AddGroup", group);
        }
    }
}
