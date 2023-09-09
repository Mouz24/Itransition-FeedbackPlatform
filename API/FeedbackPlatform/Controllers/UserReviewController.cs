﻿using AutoMapper;
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

        public UserReviewController(IServiceManager serviceManager, IMapper mapper, IElasticClient client, IAuthenticationManager authmanager)
        {
            _serviceManager = serviceManager;
            _mapper = mapper;
            _client = client;
            _authManager = authmanager;
        }

        [HttpGet("{reviewId}")]
        public IActionResult GetReview([FromQuery] Guid reviewId)
        {
            var review = _serviceManager.Review.GetReview(reviewId, true);

            return Ok(review);
        }

        [HttpGet("{reviewId}/connected-reviews")]
        public IActionResult GetConnectedReviews([FromQuery] Guid id)
        {
            var conncectedReviews = _serviceManager.Review.GetConnectedReviews(id, true);

            return Ok(conncectedReviews);
        }

        [HttpGet]
        public IActionResult GetUserReviews([FromQuery] Guid userId)
        {
            var reviews = _serviceManager.Review.GetUserReviews(userId, true);

            return Ok(reviews);
        }

        [Authorize(Roles = "Administrator, User")]
        [HttpPost(Name = "AddReview")]
        public async Task<IActionResult> AddReview([FromBody] ReviewToAddDTO reviewToAdd, IEnumerable<IFormFile> imageFiles)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var review = _mapper.Map<Review>(reviewToAdd);

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

            await _serviceManager.SaveAsync();

            var reviewDTO = _serviceManager.Review.GetReview(review.Id, true);

            await _client.IndexDocumentAsync(reviewDTO);

            return CreatedAtRoute("AddReview", reviewDTO);
        }

        [Authorize(Roles = "Administrator, User")]
        [HttpPut("{reviewId}")]
        public async Task<IActionResult> ChangeReview(Guid reviewId, [FromBody] ReviewForManipulationDTO reviewForManipulation, IEnumerable<IFormFile> imageFiles)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var review = _serviceManager.Review.GetReview(reviewId, true);

            _mapper.Map(reviewForManipulation, review);

            if (imageFiles.Any())
            {
                var reviewImages = _serviceManager.ReviewImage.GetReviewImagesUrls(reviewId, false);

                await _serviceManager.ImageCloud.DeleteImagesAsync(reviewImages);
                _serviceManager.ReviewImage.RemoveReviewImages(reviewId);

                foreach (var imageFile in imageFiles)
                {
                    string imageUrl = await _serviceManager.ImageCloud.UploadImageAsync(imageFile);

                    _serviceManager.ReviewImage.AddReviewImage(reviewId, imageUrl);
                }
            }

            await _serviceManager.SaveAsync();

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
            _serviceManager.Review.RemoveReview(reviewId);

            await _serviceManager.SaveAsync();

            return NoContent();
        }
    }
}