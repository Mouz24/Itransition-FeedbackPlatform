using AutoMapper;
using Entities;
using Entities.DTOs;
using Entities.Models;
using FeedbackPlatform.Controllers;
using FeedbackPlatform.Extensions.ModelsManipulationLogics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Nest;
using Service;
using Service.IService;
using System;
using Xunit;

namespace Tests
{
    public class UserReviewControllerTests
    {
        private UserReviewController _controller;
        private Mock<IServiceManager> _serviceManager;
        private Mock<IMapper> _mapper;
        private Mock<IElasticClient> _client;
        private Mock<IAuthenticationManager> _authManager;
        private ApplicationContext _context;
        private Mock<TagExtension> _tagExtension;
        private Mock<ILogger<UserReviewController>> _logger;

        public UserReviewControllerTests()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .AddEntityFrameworkProxies()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .UseInternalServiceProvider(serviceProvider)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _context = new ApplicationContext(options);
            _serviceManager = new Mock<IServiceManager>();
            _mapper = new Mock<IMapper>();
            _client = new Mock<IElasticClient>();
            _authManager = new Mock<IAuthenticationManager>();
            _tagExtension = new Mock<TagExtension>(_mapper.Object, _serviceManager.Object);
            _logger = new Mock<ILogger<UserReviewController>>();

            _controller = new UserReviewController(
                _serviceManager.Object,
                _mapper.Object,
                _client.Object,
                _authManager.Object,
                _context,
                _tagExtension.Object,
                _logger.Object
            );
        }

        [Fact]
        public void GetReview_ValidReviewId_ReturnsOk()
        {
            // Arrange
            Guid reviewId = Guid.NewGuid();
            Guid loggedInUserId = Guid.NewGuid();
            var review = new ReviewDTO();

            _serviceManager.Setup(s => s.Review.GetReview(reviewId, true)).Returns(review);
            _serviceManager.Setup(sm => sm.LikedReview.MarkLikedReview(loggedInUserId, review));

            // Act
            var result = _controller.GetReview(reviewId, loggedInUserId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(review, result.Value);
        }

        [Fact]
        public void GetConnectedReviews_ValidReviewId_ReturnsOk()
        {
            // Arrange
            Guid reviewId = Guid.NewGuid();
            var connectedReviews = new List<ReviewDTO>();

            _serviceManager.Setup(s => s.Review.GetConnectedReviews(reviewId, true)).Returns(connectedReviews);

            // Act
            var result = _controller.GetConnectedReviews(reviewId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(connectedReviews, result.Value);
        }

        [Fact]
        public void GetUserReviews_ValidUserId_ReturnsOk()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            var reviews = new List<ReviewDTO>();

            _serviceManager.Setup(s => s.Review.GetUserReviews(userId, true)).Returns(reviews);

            // Act
            var result = _controller.GetUserReviews(userId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(reviews, result.Value);
        }

        [Fact]
        public async Task AddReview_ValidData_ReturnsCreatedAtRoute()
        {
            // Arrange
            var reviewToAdd = new ReviewToAddDTO();
            var review = new Review();
            var reviewDTO = new ReviewDTO();
            var artwork = new Artwork();

            _mapper.Setup(m => m.Map<Review>(reviewToAdd)).Returns(review);
            _context.Database.BeginTransaction();
            _serviceManager.Setup(sm => sm.Artwork.FindDuplicateArtwork(reviewToAdd.ArtworkName, false)).Returns((Artwork)null);
            _serviceManager.Setup(sm => sm.Artwork.AddArtwork(reviewToAdd.ArtworkName)).Returns(artwork);
            _serviceManager.Setup(sm => sm.Review.AddReview(review, artwork.Id)).Returns(review);
            _tagExtension.Setup(te => te.AddTags(It.IsAny<List<string>>(), review)).Returns(Task.CompletedTask);
            _serviceManager.Setup(sm => sm.SaveAsync()).Returns(Task.CompletedTask);
            _context.Database.CommitTransaction();
            _serviceManager.Setup(sm => sm.Review.GetReview(review.Id, true)).Returns(reviewDTO);

            // Act
            var result = await _controller.AddReview(reviewToAdd) as CreatedAtRouteResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(201, result.StatusCode);
            Assert.Equal("AddReview", result.RouteName);
            Assert.Equal(reviewDTO.Id, result.RouteValues["Id"]);
        }

        [Fact]
        public async Task ChangeReview_ValidData_ReturnsNoContent()
        {
            // Arrange
            var reviewId = Guid.NewGuid();
            var reviewForManipulation = new ReviewForManipulationDTO();
            var review = new Review();
            var reviewDTO = new ReviewDTO();
            var reviewImages = new List<string>();
            var removedImages = new List<string>();
            var newImages = new List<IFormFile>();

            _context.Database.BeginTransaction();
            _serviceManager.Setup(sm => sm.Review.GetReviewEntity(reviewId, true)).Returns(review);
            _tagExtension.Setup(te => te.EditReviewTags(It.IsAny<List<string>>(), review.Id, _context)).Returns(Task.CompletedTask);
            _serviceManager.Setup(sm => sm.SaveAsync()).Returns(Task.CompletedTask);
            _serviceManager.Setup(sm => sm.ReviewImage.GetReviewImagesUrls(review.Id, false))
                .Callback((Guid id, bool trackChanges) => reviewImages = review.ReviewImages.Select(ri => ri.ImageUrl).ToList())
                .Returns(reviewImages);

            _serviceManager.Setup(sm => sm.ReviewImage.GetRemovedImages(review, reviewForManipulation.ImageFiles))
                .Callback((Review r, IEnumerable<IFormFile> files) => removedImages = reviewImages.Where(imageUrl => !files
                .Any(file => string.Equals(file.FileName, imageUrl, StringComparison.OrdinalIgnoreCase)))
                .ToList())
                .Returns(removedImages);

            _serviceManager.Setup(sm => sm.ReviewImage.GetNewImages(review, reviewForManipulation.ImageFiles)) 
                .Callback((Review r, IEnumerable<IFormFile> files) => newImages = files.Where(file => !reviewImages
                .Contains(file.FileName, StringComparer.OrdinalIgnoreCase))
                .ToList())
                .Returns(newImages);

            // Act
            var result = await _controller.ChangeReview(reviewId, reviewForManipulation) as NoContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(204, result.StatusCode);

            _tagExtension.Verify(te => te.EditReviewTags(reviewForManipulation.Tags, review.Id, _context), Times.Once);
            _serviceManager.Verify(ri => ri.ReviewImage.RemoveReviewImage(review, It.IsAny<string>()), Times.Exactly(removedImages.Count));
            _serviceManager.Verify(ri => ri.ReviewImage.AddReviewImage(review.Id, It.IsAny<string>()), Times.Exactly(newImages.Count));
        }

        [Fact]
        public async Task RemoveReview_ValidData_ReturnsNoContent()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            Guid reviewId = Guid.NewGuid();
            string reviewIdStringType = reviewId.ToString();
            var review = new Review();
            var reviewImages = new List<string>();

            _serviceManager.Setup(sm => sm.Review.GetReviewEntity(reviewId, false)).Returns(review);
            _serviceManager.Setup(sm => sm.ReviewImage.GetReviewImagesUrls(review.Id, false)).Returns(reviewImages);
            _authManager.Setup(auth => auth.RemoveLikesFromUser(userId, review.Likes)).Returns(Task.CompletedTask);
            _serviceManager.Setup(sm => sm.Review.RemoveReview(review.Id));
            _serviceManager.Setup(sm => sm.ImageCloud.DeleteImagesAsync(reviewImages));
            _serviceManager.Setup(sm => sm.SaveAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.RemoveReview(userId, reviewId) as NoContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(204, result.StatusCode);

            _serviceManager.Verify(sm => sm.Review.RemoveReview(review.Id), Times.Once);
            _serviceManager.Verify(sm => sm.ImageCloud.DeleteImagesAsync(reviewImages), Times.Once);
        }
    }

}