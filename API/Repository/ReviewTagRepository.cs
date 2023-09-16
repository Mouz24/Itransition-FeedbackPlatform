using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class ReviewTagRepository : RepositoryBase<ReviewTag>, IReviewTagRepository
    {
        public ReviewTagRepository(ApplicationContext applicationContext) :
            base(applicationContext)
        { }

        public void AddReviewTag(Guid reviewId, int tagId)
        {
            var reviewTag = new ReviewTag
            {
                Id = Guid.NewGuid(),
                ReviewId = reviewId,
                TagId = tagId
            };

            Create(reviewTag);
        }

        public IEnumerable<string> GetNewTags(Guid reviewId, IEnumerable<string> tags)
        {
            var reviewTags =  FindByCondition(reviewTags => reviewTags.ReviewId.Equals(reviewId), true)
            .Select(r => r.Tag.Text)
            .ToList();

            var newTags = tags.Except(reviewTags);

            return newTags;
        }

        public void RemoveTagFromReview(Guid reviewId, int tagId)
        {
            var reviewTag = FindByCondition(reviewTags => reviewTags.ReviewId.Equals(reviewId) && reviewTags.TagId == tagId, false)
                .FirstOrDefault();

            Delete(reviewTag);
        }

        public void RemoveTagsFromReview(Guid reviewId)
        {
            var reviewTags = FindByCondition(reviewTags => reviewTags.ReviewId.Equals(reviewId), true)
                .ToList();

            DeleteAll(reviewTags);
        }
    }
}
