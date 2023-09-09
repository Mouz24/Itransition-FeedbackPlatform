using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.IService;

namespace FeedbackPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public SearchController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        public async Task<IActionResult> SearchForWord([FromQuery] string word)
        {
            var result = await _serviceManager.Search.Search(word);

            return Ok(result);
        }
    }
}
