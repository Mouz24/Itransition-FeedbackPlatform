using Contracts;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ReviewTagService : IReviewTagService
    {
        private readonly IReviewTagRepository _reviewTagRepository;

        public ReviewTagService(IReviewTagRepository reviewTagRepository)
        {
            _reviewTagRepository = reviewTagRepository;
        }

        public void AddReviewTag(Guid reviewId, int tagId)
        {
            _reviewTagRepository.AddReviewTag(reviewId, tagId);
        }

        public void RemoveTagFromReview(Guid reviewId, int tagId)
        {
            _reviewTagRepository.RemoveTagFromReview(reviewId, tagId);
        }
        public void RemoveTagsFromReview(Guid reviewId)
        {
            _reviewTagRepository.RemoveTagsFromReview(reviewId);
        }

        public IEnumerable<string> GetNewTags(Guid reviewId, IEnumerable<string> tags)
        {
            return _reviewTagRepository.GetNewTags(reviewId, tags);
        }

        public int CountTagUsage(int tagId)
        {
            return _reviewTagRepository.CountTagUsage(tagId);
        }

        public IEnumerable<string> GetRemovedTags(Guid reviewId, IEnumerable<string> tags)
        {
            return _reviewTagRepository.GetRemovedTags(reviewId, tags);
        }
    }
}
