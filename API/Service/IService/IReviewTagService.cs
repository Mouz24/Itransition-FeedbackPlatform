using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IReviewTagService
    {
        void AddReviewTag(Guid reviewId, int tagId);
        void RemoveTagFromReview(Guid reviewId, int tagId);
    }
}
