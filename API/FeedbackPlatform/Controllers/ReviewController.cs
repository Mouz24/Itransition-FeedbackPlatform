using AutoMapper;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.IService;

namespace FeedbackPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        private readonly IMapper _mapper;

        public ReviewController(IServiceManager serviceManager, IMapper mapper)
        {
            _serviceManager = serviceManager;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetReviews([FromQuery] Guid userId, [FromQuery] List<int> tagIds, [FromQuery] RequestParameters requestParameters)
        {
            var tagList = new List<string>();

            foreach (var tagId in tagIds)
            {
                var tag = _serviceManager.Tag.FindTagById(tagId, false);

                tagList.Add(tag.Text);
            }

            var reviews = _serviceManager.Review.GetAllReviews(tagList, requestParameters, true);

            _serviceManager.LikedReview.MarkLikedReviews(userId, reviews);

            return Ok(reviews);
        }

        [HttpGet("highest-marked")]
        public IActionResult GetPopularReviews([FromQuery] Guid userId, [FromQuery] List<int> tagIds,[FromQuery] RequestParameters requestParameters)
        {
            var tagList = new List<string>();

            foreach (var tagId in tagIds)
            {
                var tag = _serviceManager.Tag.FindTagById(tagId, false);

                tagList.Add(tag.Text);
            }

            var reviews = _serviceManager.Review.GetHighestMarkedReviews(tagList, requestParameters, true);

            _serviceManager.LikedReview.MarkLikedReviews(userId, reviews);

            return Ok(reviews);
        }
    }
}
