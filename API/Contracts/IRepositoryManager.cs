using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IRepositoryManager
    {
        IArtworkRepository Artwork { get; }
        ICommentRepository Comment { get; }
        IGroupRepository Group { get; }
        IReviewRepository Review { get; }
        ITagRepository Tag { get; }
        ILikedReviewRepository LikedReview { get; }
        IRatedArtworkRepository RatedArtwork { get; }
        IReviewImageRepository ReviewImage { get; }
        Task SaveAsync();
    }
}
