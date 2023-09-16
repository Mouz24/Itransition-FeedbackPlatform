using Amazon.S3.Model;
using AutoMapper;
using Entities;
using Entities.DTOs;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Service;
using Service.IService;
using System.Data;
using Tag = Entities.Models.Tag;

namespace FeedbackPlatform.Controllers
{
    [Route("api/review/{userId}")]
    [ApiController]
    public class UserReviewController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        private readonly IMapper _mapper;
        private readonly IElasticClient _client;
        private readonly IAuthenticationManager _authManager;
        private readonly ApplicationContext _context;

        public UserReviewController(
            IServiceManager serviceManager, 
            IMapper mapper, IElasticClient client, 
            IAuthenticationManager authmanager, 
            ApplicationContext context
        )
        {
            _serviceManager = serviceManager;
            _mapper = mapper;
            _client = client;
            _authManager = authmanager;
            _context = context;
        }

        [HttpGet("{reviewId}")]
        public IActionResult GetReview(Guid reviewId, [FromQuery] Guid loggedInUserId)
        {
            var review = _serviceManager.Review.GetReview(reviewId, true);

            _serviceManager.LikedReview.MarkLikedReview(loggedInUserId, review);

            return Ok(review);
        }

        [HttpGet("{reviewId}/connected-reviews")]
        public IActionResult GetConnectedReviews(Guid reviewId)
        {
            var conncectedReviews = _serviceManager.Review.GetConnectedReviews(reviewId, true);

            return Ok(conncectedReviews);
        }

        [HttpGet]
        public IActionResult GetUserReviews(Guid userId)
        {
            var reviews = _serviceManager.Review.GetUserReviews(userId, true);

            return Ok(reviews);
        }

        [Authorize(Roles = "Administrator, User")]
        [HttpPost]
        public async Task<IActionResult> AddReview([FromForm] ReviewToAddDTO reviewToAdd, [FromForm]IEnumerable<string> tags,
            [FromForm] IEnumerable<IFormFile> imageFiles)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var review = _mapper.Map<Review>(reviewToAdd);

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var duplicateArtwork = _serviceManager.Artwork.FindDuplicateArtwork(reviewToAdd.ArtworkName, false);
                if (duplicateArtwork == null)
                {
                    duplicateArtwork = _serviceManager.Artwork.AddArtwork(reviewToAdd.ArtworkName);
                }

                review = _serviceManager.Review.AddReview(review, duplicateArtwork.Id);

                if (imageFiles.Any())
                {
                    foreach (var imageFile in imageFiles)
                    {
                        string imageUrl = await _serviceManager.ImageCloud.UploadImageAsync(imageFile);

                        _serviceManager.ReviewImage.AddReviewImage(review.Id, imageUrl);
                    }
                }

                if (tags.Any())
                {
                    foreach (var tag in tags)
                    {
                        var duplicateTag = _serviceManager.Tag.FindDuplicateTag(tag, false);
                        if (duplicateTag == null)
                        {
                            var tagToAdd = _mapper.Map<Tag>(tag);

                            var addedTag = _serviceManager.Tag.AddTag(tagToAdd);

                            await _serviceManager.SaveAsync();
                            _serviceManager.ReviewTag.AddReviewTag(review.Id, addedTag.Id);
                        }
                        else
                        {
                            _serviceManager.ReviewTag.AddReviewTag(review.Id, duplicateTag.Id);
                        }
                    }
                }

                await _serviceManager.SaveAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            var reviewDTO = _serviceManager.Review.GetReview(review.Id, true);

            await _client.IndexDocumentAsync(reviewDTO);

            return CreatedAtRoute("AddReview", reviewDTO);
        }

        [Authorize(Roles = "Administrator, User")]
        [HttpPut("{reviewId}")]
        public async Task<IActionResult> ChangeReview(Guid reviewId, [FromForm] ReviewForManipulationDTO reviewForManipulation) 
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var reviewDTO = _serviceManager.Review.GetReview(reviewId, true);
                var review = _serviceManager.Review.GetReviewEntity(reviewId, true);

                _mapper.Map(reviewForManipulation, reviewDTO);
                _mapper.Map(reviewForManipulation, review);

                await _serviceManager.SaveAsync();

                if (reviewForManipulation.Tags != null)
                {
                    var newTags = _serviceManager.ReviewTag.GetNewTags(reviewId, reviewForManipulation.Tags);
                    foreach (var tag in newTags)
                    {
                        var duplicateTag = _serviceManager.Tag.FindDuplicateTag(tag, true);
                        if (duplicateTag == null)
                        {
                            var tagToAdd = _mapper.Map<Tag>(tag);

                            var addedTag = _serviceManager.Tag.AddTag(tagToAdd);

                            await _serviceManager.SaveAsync();
                            _serviceManager.ReviewTag.AddReviewTag(review.Id, addedTag.Id);
                        }
                        else
                        {
                            _serviceManager.ReviewTag.AddReviewTag(review.Id, duplicateTag.Id);
                        }
                    }
                }
                else
                {
                    _serviceManager.ReviewTag.RemoveTagsFromReview(reviewId);
                }

                await _serviceManager.SaveAsync();

                var reviewImages = _serviceManager.ReviewImage.GetReviewImagesUrls(reviewId, true);

                _serviceManager.ReviewImage.RemoveReviewImages(reviewId);

                await _serviceManager.SaveAsync();

                await _client.DeleteAsync<ReviewDTO>(reviewId.ToString());
                await _client.IndexDocumentAsync(reviewDTO);

                await transaction.CommitAsync();

                await _serviceManager.ImageCloud.DeleteImagesAsync(reviewImages);

                if (reviewForManipulation.ImageFiles != null)
                {
                    foreach (var imageFile in reviewForManipulation.ImageFiles)
                    {
                        string imageUrl = await _serviceManager.ImageCloud.UploadImageAsync(imageFile);

                        _serviceManager.ReviewImage.AddReviewImage(reviewId, imageUrl);
                    }
                }

                await _serviceManager.SaveAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return NoContent();
        }

        [Authorize(Roles = "Administrator, User")]
        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> RemoveReview(Guid userId, Guid reviewId)
        {
            string elasticSearchDocumentId = reviewId.ToString();
            var review = _serviceManager.Review.GetReview(reviewId, true);

            await _client.DeleteAsync<ReviewDTO>(elasticSearchDocumentId);
            await _authManager.RemoveLikesFromUser(userId, review.Likes);
            _serviceManager.Review.RemoveReview(review.Id);

            await _serviceManager.SaveAsync();

            return NoContent();
        }
    }
}
