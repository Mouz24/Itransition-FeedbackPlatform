﻿using AutoMapper;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.IService;

namespace FeedbackPlatform.Controllers
{
    [Route("api/reviews")]
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
        public IActionResult GetReviews([FromQuery] List<int> tagIds, [FromQuery] RequestParameters requestParameters)
        {
            var reviews = _serviceManager.Review.GetAllReviews(tagIds, requestParameters, true);

            return Ok(reviews);
        }

        [HttpGet("highest-marked")]
        public IActionResult GetPopularReviews([FromQuery] List<int> tagIds,[FromQuery] RequestParameters requestParameters)
        {
            var reviews = _serviceManager.Review.GetHighestMarkedReviews(tagIds, requestParameters, true);

            return Ok(reviews);
        }
    }
}
