using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ReviewImageService : IReviewImageService
    {
        private readonly IReviewImageRepository _reviewImageRepository;

        public ReviewImageService(IReviewImageRepository reviewImageRepository)
        {
            _reviewImageRepository = reviewImageRepository;
        }

        public void AddReviewImage(Guid reviewid, string imageUrl)
        {
            _reviewImageRepository.AddReviewImage(reviewid, imageUrl);
        }

        public IEnumerable<IFormFile> GetNewImages(Review review, IEnumerable<IFormFile> imageFiles)
        {
            return _reviewImageRepository.GetNewImages(review, imageFiles);
        }

        public IEnumerable<string> GetRemovedImages(Review review, IEnumerable<IFormFile> imageFiles)
        {
            return _reviewImageRepository.GetRemovedImages(review, imageFiles);
        }

        public IEnumerable<ReviewImage> GetReviewImages(Guid reviewid, bool trackChanges)
        {
            return _reviewImageRepository.GetReviewImages(reviewid, trackChanges);
        }

        public IEnumerable<string> GetReviewImagesUrls(Guid reviewid, bool trackChanges)
        {
            return _reviewImageRepository.GetReviewImagesUrls(reviewid, trackChanges);
        }

        public void RemoveReviewImage(Review review, string imageUrl)
        {
            _reviewImageRepository.RemoveReviewImage(review, imageUrl);
        }

        public void RemoveReviewImages(Guid reviewId)
        {
            _reviewImageRepository.RemoveReviewImages(reviewId);
        }
    }
}
