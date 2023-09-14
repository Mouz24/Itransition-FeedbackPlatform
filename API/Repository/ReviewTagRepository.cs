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

        public void RemoveTagFromReview(Guid reviewId, int tagId)
        {
            var reviewTag = FindByCondition(reviewTags => reviewTags.ReviewId.Equals(reviewId) && reviewTags.TagId == tagId, false)
                .FirstOrDefault();

            Delete(reviewTag);
        }
    }
}
