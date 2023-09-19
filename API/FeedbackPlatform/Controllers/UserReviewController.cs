using Amazon.S3.Model;
using AutoMapper;
using Entities;
using Entities.DTOs;
using Entities.Models;
using FeedbackPlatform.Extensions.ModelsManipulationLogics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly TagExtension _tagExtension;

        public UserReviewController(
            IServiceManager serviceManager, 
            IMapper mapper, IElasticClient client, 
            IAuthenticationManager authmanager, 
            ApplicationContext context,
            TagExtension tagExtension
        )
        {
            _serviceManager = serviceManager;
            _mapper = mapper;
            _client = client;
            _authManager = authmanager;
            _context = context;
            _tagExtension = tagExtension;
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
        [HttpPost(Name = "AddReview")]
        public async Task<IActionResult> AddReview([FromForm] ReviewToAddDTO reviewToAdd)
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

                await _tagExtension.AddTags(reviewToAdd.Tags, review);

                await _serviceManager.SaveAsync();

                await transaction.CommitAsync();

                //if (reviewToAdd.ImageFiles != null)
                //{
                //    foreach (var imageFile in reviewToAdd.ImageFiles)
                //    {
                //        string imageUrl = await _serviceManager.ImageCloud.UploadImageAsync(imageFile);

                //        _serviceManager.ReviewImage.AddReviewImage(review.Id, imageUrl);
                //    }
                //}

                await _serviceManager.SaveAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            var reviewDTO = _serviceManager.Review.GetReview(review.Id, true);

            await _client.IndexDocumentAsync(reviewDTO);

            return CreatedAtRoute("AddReview", new { Id = reviewDTO.Id}, reviewDTO);
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
                var review = _serviceManager.Review.GetReviewEntity(reviewId, true);

                _mapper.Map(reviewForManipulation, review);

                await _serviceManager.SaveAsync();

                await _tagExtension.EditReviewTags(reviewForManipulation.Tags, review.Id, _context);

                await _serviceManager.SaveAsync();

                var reviewImages = _serviceManager.ReviewImage.GetReviewImagesUrls(review.Id, false);

                //_context.Entry(review).Collection(r => r.ReviewImages).Query().ToList().ForEach(image => _context.Entry(image).State = EntityState.Detached);

                //_serviceManager.ReviewImage.RemoveReviewImages(review.Id);

                await _serviceManager.SaveAsync();

                var reviewDTO = _serviceManager.Review.GetReview(review.Id, true);

                await _client.DeleteAsync<ReviewDTO>(review.Id.ToString()); 
                await _client.IndexDocumentAsync(reviewDTO);

                await transaction.CommitAsync();

                //await _serviceManager.ImageCloud.DeleteImagesAsync(reviewImages);

                //if (reviewForManipulation.ImageFiles != null)
                //{
                //    foreach (var imageFile in reviewForManipulation.ImageFiles)
                //    {
                //        string imageUrl = await _serviceManager.ImageCloud.UploadImageAsync(imageFile);

                //        _serviceManager.ReviewImage.AddReviewImage(review.Id, imageUrl);
                //    }
                //}

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
