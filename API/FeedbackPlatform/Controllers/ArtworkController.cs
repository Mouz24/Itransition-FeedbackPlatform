using AutoMapper;
using Entities.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.IService;

namespace FeedbackPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtworkController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        private readonly IMapper _mapper;

        public ArtworkController(IServiceManager serviceManager, IMapper mapper)
        {
            _serviceManager = serviceManager;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAllArtworks()
        {
            var artworks = _serviceManager.Artwork.GetAllArtworks(false);

            var artworkDTOs = _mapper.Map<List<ArtworkDTO>>(artworks);

            return Ok(artworkDTOs);
        }
    }
}
