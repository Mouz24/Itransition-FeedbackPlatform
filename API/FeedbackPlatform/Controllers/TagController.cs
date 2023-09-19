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
    public class TagController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        private readonly IMapper _mapper;

        public TagController(IServiceManager serviceManager, IMapper mapper)
        {
            _serviceManager = serviceManager;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetTags()
        {
            var tags = _serviceManager.Tag.GetTags(true);

            var tagsDTOs = _mapper.Map<List<ReviewTagDTO>>(tags);

            return Ok(tagsDTOs);
        }

        [Authorize(Roles = "Administrator, User")]
        [HttpPost(Name = "AddTag")]
        public async Task<IActionResult> AddTag([FromBody]TagToAddDTO tagToAdd)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var tag = _mapper.Map<Tag>(tagToAdd);

            var duplicateTag = _serviceManager.Tag.GetTag(tag.Value, false);
            if (duplicateTag != null)
            {
                return BadRequest("The tag already exists");
            }

            tag = _serviceManager.Tag.AddTag(tag);

            await _serviceManager.SaveAsync();

            return CreatedAtRoute("AddTag", tag);
        }
    }
}
