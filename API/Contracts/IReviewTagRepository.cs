using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IReviewTagRepository
    {
        void AddReviewTag(Guid reviewId, int tagId);
        void RemoveTagFromReview(Guid reviewId, int tagId);
        void RemoveTagsFromReview(Guid reviewId);
        IEnumerable<string> GetNewTags(Guid reviewId, IEnumerable<string> tags);
        int CountTagUsage(int tagId);
        IEnumerable<string> GetRemovedTags(Guid reviewId, IEnumerable<string> tags);
    }
}
