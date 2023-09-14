using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Repository
{
    public class ReviewRepository : RepositoryBase<Review>, IReviewRepository
    {
        private readonly IMapper _mapper;
        public ReviewRepository(ApplicationContext applicationContext, IMapper mapper) :
            base(applicationContext) 
        {
            _mapper = mapper;
        }

        public Review AddReview(Review review, Guid artworkId)
        {
            review.DateCreated = DateTime.UtcNow.AddHours(3);
            review.ArtworkId = artworkId;

            Create(review);

            return review;
        }

        public IEnumerable<ReviewDTO> GetAllReviews(IEnumerable<int> tagList, RequestParameters requestParameters, bool trackChanges)
        {
            var query = FindAll(trackChanges);

            if (tagList.Any())
            {
                // Filter reviews based on the specified tag IDs
                query = query.Where(review =>
                    review.Tags.Any(tag => tagList.Contains(tag.TagId))
                );
            }

            var reviews = query.OrderBy(review => review.DateCreated)
                .Skip((requestParameters.PageNumber - 1) * requestParameters.PageSize)
                .Take(requestParameters.PageSize)
                .ToList();

            var reviewDTOs = _mapper.Map<List<ReviewDTO>>(reviews);

            return reviewDTOs;
        }


        public IEnumerable<ReviewDTO> GetConnectedReviews(Guid reviewId, bool trackChanges)
        {
            var targetArtworkId = FindByCondition(review => review.Id.Equals(reviewId), trackChanges).Select(review => review.ArtworkId)
                .FirstOrDefault();

            var reviews = FindByCondition(review => review.ArtworkId.Equals(targetArtworkId) && review.Id != reviewId, trackChanges).ToList();

            var reviewDTOs = _mapper.Map<List<ReviewDTO>>(reviews);

            return reviewDTOs;
        }

        public IEnumerable<ReviewDTO> GetHighestMarkedReviews(IEnumerable<int> tagList, RequestParameters requestParameters, bool trackChanges)
        {
            var query = FindAll(trackChanges).AsEnumerable();

            if (tagList.Any())
            {
                query = query.Where(review => tagList.Any(tagId => review.Tags.Any(reviewTag => reviewTag.TagId == tagId)));
            }

            var reviews = query.OrderBy(review => review.Mark)
                .Skip((requestParameters.PageNumber - 1) * requestParameters.PageSize)
                .Take(requestParameters.PageSize).ToList();

            var reviewDTOs = _mapper.Map<List<ReviewDTO>>(reviews);

            return reviewDTOs;
        }

        public ReviewDTO GetReview(Guid id, bool trackChanges)
        {
            var review = FindByCondition(r => r.Id.Equals(id), trackChanges)
                .Include(r => r.Artwork)  
                .Include(r => r.User)
                .Include(r => r.Group)    
                .Include(r => r.ReviewImages)   
                .Include(r => r.Comments) 
                .Include(r => r.Tags)
                .FirstOrDefault();

            var reviewDTO = _mapper.Map<ReviewDTO>(review);

            return reviewDTO;
        }

        public Review GetReviewForLike(Guid id, bool trackChanges)
        {
            var review = FindByCondition(r => r.Id.Equals(id), trackChanges).FirstOrDefault();

            return review;
        }

        public IEnumerable<ReviewDTO> GetUserReviews(Guid userId, bool trackChanges)
        {
            var reviews = FindByCondition(review => review.UserId.Equals(userId), trackChanges)
            .OrderBy(review => review.DateCreated)
            .ToList();

            var reviewDTOs = _mapper.Map<List<ReviewDTO>>(reviews);

            return reviewDTOs;
        }
        
        public void LikeReview(Guid id)
        {
            var review = GetReviewForLike(id, true);

            review.Likes++;
            review.IsLikedByUser = true;

            Update(review);
        }

        public void DislikeReview(Guid id)
        {
            var review = GetReviewForLike(id, true);

            review.Likes--;
            review.IsLikedByUser = false;

            Update(review);
        }

        public void RemoveReview(Guid id)
        {
            var review = FindByCondition(r => r.Id.Equals(id), false).FirstOrDefault();

            Delete(review);
        }

        public void RemoveReviews(IEnumerable<Review> reviews)
        {
            DeleteAll(reviews);
        }
    }
}
